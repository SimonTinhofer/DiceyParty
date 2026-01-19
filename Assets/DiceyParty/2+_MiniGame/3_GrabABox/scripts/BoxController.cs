using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.GrabABox
{
    public class BoxController : NetworkBehaviour
    {
        [SerializeField] private string _playerTag = "Player";
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Collider _collider;
        [SerializeField] private GlobalConfigSO _globalConfig;
        [SerializeField] private Rigidbody _rigidbody;

        private bool _triggered;
        private int _ownerID = -1;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_playerTag) && !_triggered)
            {
                _triggered = true;
                ClaimedByClient(ClientManager.Connection.ClientId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClaimedByClient(int clientID)
        {
            if(GrabABoxManager.IsPlayerFinished(clientID) || _ownerID != -1)
                return;

            _ownerID = clientID;
            GrabABoxManager.PlayerClaimedSessel(clientID);
            ObserverSesselClaimed(clientID);
        }

        [ObserversRpc]
        private void ObserverSesselClaimed(int clientID)
        {
            int colorIndex = SessionDataSystem.Instance.GetPlayerData()[clientID].ColorIndex;
            _renderer.material.color = _globalConfig.Colors[colorIndex];
            _collider.enabled = false;
            _rigidbody.Sleep();
        }
    }
}