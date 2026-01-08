using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class CanonMovement : NetworkBehaviour
    {
        [SerializeField] GameConfigSO _gameConfig;
        [SerializeField] Transform _bodyTransform;
        private float _ringAngleDeg;
        private float _pitch;
        private InputAction _moveAction;
        private bool _controlsEnabled;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner)
            {
                enabled = false;
                return;
            }

            _moveAction = InputSystem.actions.FindAction("Move");
            
            Camera cam = Camera.main;
            cam.transform.position = transform.position;
            cam.transform.rotation = transform.rotation;
            cam.transform.parent = _bodyTransform;

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
            
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            float deltaPitch = -_gameConfig.LookSpeed * Time.deltaTime * moveInput.y;
            _pitch += deltaPitch;
            _pitch = Mathf.Clamp(_pitch, -_gameConfig.MaxPitch, _gameConfig.MaxPitch);

            if(Mathf.Abs(moveInput.x) > 0.1)
                _ringAngleDeg += moveInput.x * _gameConfig.AngleSpeed * Time.deltaTime;

            transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * _ringAngleDeg) * _gameConfig.Radius, 1, Mathf.Sin(Mathf.Deg2Rad * _ringAngleDeg) * _gameConfig.Radius);
            _bodyTransform.localRotation = Quaternion.Euler(_pitch, _gameConfig.RotationOffset.y - _ringAngleDeg, 0f);
        }
    }
}

