using FishNet.Object;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class ShootingControls : NetworkBehaviour
    {
        [SerializeField] private PaintTheBallConfigSO _gameConfig;
        [SerializeField] private GlobalConfigSO _globalConfig;
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
            try
            {
                while (_shootingEnabled)
                {
                    await Awaitable.WaitForSecondsAsync(_gameConfig.ShootingCooldown, destroyCancellationToken);
                    ShootProjectile();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"StartShooting failed: {e.Message}");
            }
        }



        private void ShootProjectile()
        {
            if (!IsClientStarted || !IsOwner) return;
            
            float force =  _gameConfig.ShootingForce;
            Vector3 direction = _projectileSpawnPoint.forward;
            GameObject proj = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);
            int clientId = ClientManager.Connection.ClientId;
            int colorIndex = SessionDataSystem.Instance.GetPlayerData()[clientId].ColorIndex;
            Color color = _globalConfig.Colors[colorIndex];
            var projectileLogic = proj.GetComponent<ProjectileLogic>();
            projectileLogic.PassColor(color);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.linearVelocity = direction * force;
            }
        }
    }
}
