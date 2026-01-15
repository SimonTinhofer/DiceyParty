using UnityEngine;

namespace DiceyParty.MiniGame.RollOff
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [SerializeField] private PlayerHandler _playerHandler;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Death"))
            {
                _playerHandler.RespawnPlayer();
            }
        }
    }
}