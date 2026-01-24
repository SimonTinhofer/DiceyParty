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
        private float _teamLeftMultiplayer = 1;
        private float _teamRightMultiplayer = 1;
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
            int playerCountTeamLeft = new();
            int playerCountTeamRight = new();
            var clients = SessionDataSystem.Instance.GetClientIds();
            var shuffledClientIds  = clients.OrderBy(_ => Random.value).ToList();
            int a = Random.Range(0, 2);
            for(int i = 0; i < shuffledClientIds.Count; i++)
            {
                if ((i + a) % 2 == 0)
                {
                    _playerTeams.Add(shuffledClientIds[i], Team.LeftTeam);
                    playerCountTeamLeft++;
                }
                else
                {
                    _playerTeams.Add(shuffledClientIds[i], Team.RightTeam);
                    playerCountTeamRight++;
                }
            }

            if (playerCountTeamLeft != playerCountTeamRight)
            {
                if (_playerTeams.Count == 3)
                {
                    if (playerCountTeamLeft < playerCountTeamRight)
                        _teamLeftMultiplayer = _gameConfig.BalancingMultiplyers[2];
                    else
                        _teamRightMultiplayer = _gameConfig.BalancingMultiplyers[2];
                }
                else if (_playerTeams.Count == 5)
                {
                    if (playerCountTeamLeft < playerCountTeamRight)
                        _teamLeftMultiplayer = _gameConfig.BalancingMultiplyers[1];
                    else
                        _teamRightMultiplayer = _gameConfig.BalancingMultiplyers[1];
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
            _ropeController.Setup(_playerTeams.Count);
            _graphicsManager.ShowPlayers(_playerTeams, _teamLeftMultiplayer, _teamRightMultiplayer);
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
            if (_playerTeams[id] == Team.LeftTeam)
            {
                _ropeController.ApplyForce(Vector3.left, _teamLeftMultiplayer);
            }
            else if (_playerTeams[id] == Team.RightTeam)
            {
                _ropeController.ApplyForce(Vector3.right, _teamRightMultiplayer);
            }
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