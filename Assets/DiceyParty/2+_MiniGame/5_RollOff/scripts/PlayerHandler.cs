using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.RollOff
{
    public class PlayerHandler : NetworkBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private RollOffConfigSO _gameConfig;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Transform _camTransform;
        [SerializeField] private PlayerScoreHandler _scoreHandler;
        private Transform _spawnPoint;
        private InputAction _moveAction;
        private float _moveDirection = 0;
        private float _currentSpeed;
        private float _accelerationStartTime;
        private bool _isMovementEnabled;
        private int _longestRun;
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner)
            {
                _collider.enabled = false;
                _rb.isKinematic = true;
                return;
            }

            _collider.enabled = true;
            _rb.useGravity = true;

            _spawnPoint = RollOffManager.Instance.Spawnpoint;
            _moveAction = InputSystem.actions.FindAction("Move");
            Camera cam = Camera.main;
            cam.transform.position = _camTransform.position;
            cam.transform.SetParent(_camTransform, true);

            RollOffManager.OnTogglePlayerControls += ToggleMovement;
        }

        private void OnDestroy()
        {
            RollOffManager.OnTogglePlayerControls -= ToggleMovement;
        }

        private void ToggleMovement(bool toggle)
        {
            _isMovementEnabled = toggle;
            if(toggle) return;
            _rb.Sleep();
            if (_longestRun < Mathf.FloorToInt(_rb.position.z))
                _longestRun = Mathf.FloorToInt(_rb.position.z);
            RollOffManager.Instance.ClientFinishedGamePhase(OwnerId, _longestRun);
        }

        private void FixedUpdate()
        {
            if (!_isMovementEnabled)
                return;
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            if (moveInput.x < -0.05)
                _moveDirection = -1;
            else if (moveInput.x > 0.05)
                _moveDirection = 1;
            
            if(_moveDirection == 0) return;

            if(_accelerationStartTime < 0.05f)
                _accelerationStartTime = Time.time;
            
            if(_rb.position.z > _longestRun)
                _scoreHandler.SetLongestRun(Mathf.FloorToInt(_rb.position.z));

            float timeAccelerating = Time.time - _accelerationStartTime;
            _currentSpeed  = _gameConfig.SpeedFunctionMultiplyer * Mathf.Log(timeAccelerating + _gameConfig.AccelerationSecondsOffset, _gameConfig.MovementAccelerationLogBase);
            _rb.linearVelocity = new Vector3(_moveDirection * _currentSpeed, _rb.linearVelocity.y, _currentSpeed);
        }

        public void RespawnPlayer()
        {
            if (_longestRun < Mathf.FloorToInt(_rb.position.z))
            {
                _longestRun = Mathf.FloorToInt(_rb.position.z);
                Debug.Log(_longestRun);
            }

            _rb.Sleep();
            _rb.position = _spawnPoint.position;
            _accelerationStartTime = 0;
            _moveDirection = 0;
            _rb.WakeUp();
        }
    }
}