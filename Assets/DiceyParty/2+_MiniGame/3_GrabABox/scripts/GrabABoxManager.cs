using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.GrabABox
{
    public class GrabABoxManager : NetworkBehaviour
    {
        [SerializeField] private GrabABoxConfigSO _gameConfig;
        [SerializeField] private SpawningManager _gameSpawner;

        public static Action OnStartRound;

        private static GrabABoxManager _instance;

        private List<int> _alivePlayers = new();
        private List<int> _survivedPlayers = new();
        private Dictionary<int, int> _placements = new();


        private void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Instance should be null");
            }
            _instance = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            MiniGameManager.OnStartGamePhase += StartGamePhase;
        }

        private void OnDestroy()
        {
            MiniGameManager.OnStartGamePhase -= StartGamePhase;
        }

        private void StartGamePhase()
        {
            var playerData =  SessionDataSystem.Instance.GetPlayerData();
            _alivePlayers = playerData.Select(a => a.Value.ClientId).ToList();
            PlayRound();
        }

        private async void PlayRound()
        {
            await Awaitable.WaitForSecondsAsync(_gameConfig.WaitForPlayerSpawnDuration);
            _gameSpawner.SpawnPlayers(_alivePlayers);
            await Awaitable.WaitForSecondsAsync(_gameConfig.WaitForSesselSpawnDuration);
            _gameSpawner.SpawnSessel(_alivePlayers.Count - 1);
        }


        public static void PlayerClaimedSessel(int clientID) => _instance.HandlePlayerClaimedSessel(clientID);

        private void HandlePlayerClaimedSessel(int clientID)
        {
            CheckIfServer();

            _survivedPlayers.Add(clientID);
            _alivePlayers.Remove(clientID);
            _gameSpawner.DespawnPlayer(clientID);
            if(_alivePlayers.Count == 1)
            {
                CleanUpRound();
            }
        }

        private async void CleanUpRound()
        {
            await Awaitable.WaitForSecondsAsync(1);
            _gameSpawner.DespawnPlayer(_alivePlayers[0]);
            _placements.Add(_alivePlayers[0], _survivedPlayers.Count);
            await Awaitable.WaitForSecondsAsync(1);
            _gameSpawner.DespawnSessel();

            _alivePlayers = new List<int>(_survivedPlayers);
            if (_alivePlayers.Count > 1)
            {
                _survivedPlayers.Clear();
                PlayRound();
            }
            else
            {
                _placements.Add(_survivedPlayers[0], 0);
                MiniGameManager.FinishedGamePhase(_placements);
            }
        }


        private void CheckIfServer()
        {
            if (!IsServerInitialized)
                throw new Exception("ServerGameManager must be called with ServerRPC");
        }

        public static bool IsPlayerFinished(int clientID) => _instance.HandleIsPlayerFinished(clientID);

        private bool HandleIsPlayerFinished(int clientID)
        {
            CheckIfServer();
            return _survivedPlayers.Contains(clientID);
        }
    }
}