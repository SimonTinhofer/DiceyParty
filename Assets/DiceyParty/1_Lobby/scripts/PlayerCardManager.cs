using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.Lobby
{
    public class PlayerCardManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _containerTransform;
        private Dictionary<int, PlayerCardHandler> _handlers = new();

        [Server]
        public void SetupPlayerCardsServer()
        {
            var playerData = new Dictionary<int, PlayerInfo>(SessionDataSystem.Instance.GetPlayerData());
            SetupPlayerCardsObservers(playerData);
        }

        [ObserversRpc (RunLocally = true, BufferLast = true)]
        private void SetupPlayerCardsObservers(Dictionary<int, PlayerInfo> playerData)
        {
            foreach (var pair in playerData)
            {
                int clientId = pair.Key;
                PlayerInfo p = pair.Value;
                if (_handlers.TryGetValue(p.ClientId, out PlayerCardHandler oldHandler))
                {
                    oldHandler.Setup(p);
                }
                else
                {
                    var go = Instantiate(_prefab, _containerTransform);
                    var newHandler = go.gameObject.GetComponent<PlayerCardHandler>();
                    newHandler.Setup(p);
                    _handlers.Add(p.ClientId, newHandler);  
                }
            }
        }
        
        [Server]
        public void RemovePlayerCardServer(int clientId)
        {
            RemovePlayerCardObservers(clientId);
        }
        
        [ObserversRpc (RunLocally = true, BufferLast = true)]
        private void RemovePlayerCardObservers(int clientId)
        {
            if (_handlers.TryGetValue(clientId, out var handler))
            {
                Destroy(handler.gameObject);
                _handlers.Remove(clientId);
            }
            else
            {
                Debug.LogWarning("Key to Destroy was not present");
            }
            
        }
    }
}