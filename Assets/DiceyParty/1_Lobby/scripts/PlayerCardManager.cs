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
            RemoveOldPlayerCards(playerData);

            int localClientId = LocalConnection.ClientId;
            foreach (var pair in playerData)
            {
                int clientId = pair.Key;
                PlayerInfo p = pair.Value;
                if (_handlers.TryGetValue(p.ClientId, out PlayerCardHandler oldHandler))
                {
                    oldHandler.Setup(p, localClientId);
                }
                else
                {
                    var go = Instantiate(_prefab, _containerTransform);
                    var newHandler = go.gameObject.GetComponent<PlayerCardHandler>();
                    newHandler.Setup(p, localClientId);
                    _handlers.Add(p.ClientId, newHandler);  
                }
            }
        }

        private void RemoveOldPlayerCards(Dictionary<int, PlayerInfo> playerData)
        {
            var handlerCopy = new Dictionary<int, PlayerCardHandler>(_handlers);
            foreach (var pair in handlerCopy)
            {
                int clientId = pair.Key;
                if (!playerData.ContainsKey(clientId))
                {
                    var handler = pair.Value;
                    Destroy(handler.gameObject);
                    _handlers.Remove(clientId);
                }
            }
        }
    }
}