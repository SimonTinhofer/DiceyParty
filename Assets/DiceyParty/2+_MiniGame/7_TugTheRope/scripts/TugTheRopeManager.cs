using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DiceyParty.MiniGame.TugTheRope
{
    public class TugTheRopeManager : NetworkBehaviour
    {
        public static TugTheRopeManager Instance;
        
        [SerializeField] private TugTheRopeConfig _gameConfig;
        [SerializeField] private GraphicsManager _graphicsManager;
        [SerializeField] private RopeController _ropeController;
        private Dictionary<int, Team> _playerTeams = new();
        private int _playerCountTeamLeft;
        private int _playerCountTeamRight;
        private int _clientTugs;

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
            DecideTeams();
            MiniGameManager.OnStartGamePhase += OnStartGamePhaseServer;
        }

        private void DecideTeams()
        {
            var clients = SessionDataSystem.Instance.GetClientIds();
            var shuffledClientIds  = clients.OrderBy(_ => Random.value).ToList();
            int a = Random.Range(0, 2);
            for(int i = 0; i < shuffledClientIds.Count; i++)
            {
                if ((i + a) % 2 == 0)
                {
                    _playerTeams.Add(shuffledClientIds[i], Team.LeftTeam);
                    _playerCountTeamLeft++;
                }
                else
                {
                    _playerTeams.Add(shuffledClientIds[i], Team.RightTeam);
                    _playerCountTeamRight++;
                }
            }
        }

        private void OnDestroy()
        {
            MiniGameManager.OnStartGamePhase -= OnStartGamePhaseServer;
            UIManager.OnTug -= ClientOnTug;
        }
        
        private void OnStartGamePhaseServer()
        {
            _graphicsManager.ShowPlayers(_playerTeams, _playerCountTeamLeft, _playerCountTeamRight);
            OnStartGamePhaseClient();
        }
        
        [ObserversRpc (BufferLast = true)]
        private void OnStartGamePhaseClient()
        {
            UIManager.Instance.TogglePullButton(true);
            UIManager.OnTug += ClientOnTug;
        }

        private void ClientOnTug()
        {
            _clientTugs++;
            ServerOnTug(LocalConnection.ClientId, _clientTugs);
        }

        [ServerRpc (RequireOwnership = false)]
        private void ServerOnTug(int clientId, int clientTugs)
        {
            int id = clientId;
            _graphicsManager.UpdateTugTextServer(clientId, clientTugs);
            Team team = _playerTeams[clientId];
            int teamSize = team == Team.LeftTeam ? _playerCountTeamLeft : _playerCountTeamRight;
            _ropeController.AddTug(team, teamSize);
        }
        
        public void TeamWon(Team winnerTeam)
        {
            Dictionary<int, int> placements = new();
            foreach (var entry in _playerTeams)
            {
                if(entry.Value == winnerTeam)
                    placements.Add(entry.Key, 0);
                else
                {
                    placements.Add(entry.Key, 1);
                }
            }
            MiniGameManager.FinishedGamePhase(placements);
        }
    }
    public enum Team {LeftTeam, RightTeam}
}