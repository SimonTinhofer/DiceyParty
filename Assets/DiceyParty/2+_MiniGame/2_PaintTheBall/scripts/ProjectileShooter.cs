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
        private bool _controlsEnabled;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            _fireAction = InputSystem.actions.FindAction("Attack");

            PaintTheBallManager.TogglePlayerControls += ToggleControls;
        }
        
        private void OnDestroy()
        {
            PaintTheBallManager.TogglePlayerControls -= ToggleControls;
        }

        private void ToggleControls(bool toggle)
        {
            _controlsEnabled = toggle;
        }


        private void Update()
        {
            if (!_controlsEnabled) return;
            
            if (_fireAction.IsPressed() & !_onCooldown)
            {
                _onCooldown = true;
                ShootProjectile();
                ResetCooldown();
            }
        }

        private async void ResetCooldown()
        {
            await Awaitable.WaitForSecondsAsync(_gameConfig.ShootingCooldown);
            _onCooldown = false;
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
