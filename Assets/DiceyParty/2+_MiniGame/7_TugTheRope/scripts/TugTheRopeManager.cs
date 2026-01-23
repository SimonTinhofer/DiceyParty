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
        private Dictionary<int, Team> _playerTeam = new();
        private Dictionary<int, int> _playerTugs = new();
        private int _leftTeamTugs;
        private int _rightTeamTugs;
        private int _clientTugs;

        public override void OnStartServer()
        {
            base.OnStartServer();
            var clients = SessionDataSystem.Instance.GetClientIds();
            var shuffledClientIds  = clients.OrderBy(_ => Random.value).ToList();
            for(int i = 0; i < shuffledClientIds.Count; i++)
            {
                if(i % 2 == 0)
                    _playerTeam.Add(shuffledClientIds[i], Team.LeftTeam);
                else
                {
                    _playerTeam.Add(shuffledClientIds[i], Team.RightTeam);
                }
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            MiniGameManager.OnStartGamePhase += OnStartGamePhaseClient;
            
        }

        private void OnDestroy()
        {
            MiniGameManager.OnStartGamePhase -= OnStartGamePhaseClient;
            UIManager.OnTug -= ClientOnTug;
        }

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
            _playerTugs[clientId] = clientTugs;
            if (_playerTeam[clientId] == Team.LeftTeam)
                _leftTeamTugs++;
            else if (_playerTeam[clientId] == Team.RightTeam)
                _rightTeamTugs++;
            ShowScoreObservers(_rightTeamTugs - _leftTeamTugs);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void ShowScoreObservers(int score)
        {
            UIManager.Instance.ShowScore(score);
        }

        private enum Team {LeftTeam, RightTeam}
    }
}