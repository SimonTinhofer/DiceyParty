using FishNet.Managing;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.Menu
{
    public class SessionSystemSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject _sessionSystemPrefab;
        [SerializeField] private Button _spawnButton;
        [SerializeField] private bool _isTester;

        public override void OnStartServer()
        {
            if (_isTester)
            {
                _spawnButton.interactable = true;
                _spawnButton.onClick.AddListener(SpawnSessionSystem);
            }
            else
            {
                SpawnSessionSystem();
            }
        }

        private void SpawnSessionSystem()
        {
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_sessionSystemPrefab, true);
            NetworkManager.ServerManager.Spawn(nob);
        }
    }
}