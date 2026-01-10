using FishNet.Object;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class ShootingControls : NetworkBehaviour
    {
        [SerializeField] private GameConfigSO _gameConfig;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _projectileSpawnPoint;       
        private bool _onCooldown;
        private InputAction _fireAction;
        private bool _shootingEnabled;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            _fireAction = InputSystem.actions.FindAction("Attack");

            PaintTheBallManager.TogglePlayerControls += ToggleShooting;
        }
        
        private void OnDestroy()
        {
            PaintTheBallManager.TogglePlayerControls -= ToggleShooting;
        }

        private void ToggleShooting(bool toggle)
        {
            _shootingEnabled = toggle;
            if(toggle)
                StartShooting();
        }


        private async void StartShooting()
        {
            while (_shootingEnabled)
            {
                ShootProjectile();
                await Awaitable.WaitForSecondsAsync(_gameConfig.ShootingCooldown);
            }
        }



        private void ShootProjectile()
        {
            float force =  _gameConfig.ShootingForce;
            Vector3 direction = _projectileSpawnPoint.forward;
            GameObject proj = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.linearVelocity = direction * force;
            }
        }
    }
}
