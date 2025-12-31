using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.TestGame
{
    public class TestGameManager : NetworkBehaviour
    {
        //server side
        private readonly Dictionary<int, int> _placements = new();
        private int _playerCount;
        
        //client side
        private int _clientId;

        public override void OnStartServer()
        {
            base.OnStartServer();
            _playerCount = SessionDataSystem.GetPlayerCount();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _clientId = ClientManager.Connection.ClientId;
            MiniGameManager.OnStartGamePhase += OnStartGamePhase;
        }

        private void OnDestroy()
        {
            MiniGameManager.OnStartGamePhase -= OnStartGamePhase;
        }

        private void OnStartGamePhase()
        {
            Debug.Log("GamePhase started");
            ClientFinishedGamePhase(_clientId);
        }
        
        [ServerRpc (RequireOwnership = false)] 
        private void ClientFinishedGamePhase(int clientId)
        {
            _placements.Add(clientId, _placements.Count);
            if (_placements.Count == _playerCount)
            {
                MiniGameManager.FinishedGamePhase(_placements);
            }
        }
    }
}