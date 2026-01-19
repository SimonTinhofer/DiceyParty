using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class CanonMovement : NetworkBehaviour
    {
        [SerializeField] PaintTheBallConfigSO _paintTheBallConfig;
        [SerializeField] Transform _bodyTransform;
        [SerializeField] Transform _camTransform;
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

            int clientId = ClientManager.Connection.ClientId;
            var clientIds = SessionDataSystem.Instance.GetClientIds();
            for (int i = 0; i < clientIds.Count; i++)
            {
                if (clientId == clientIds[i])
                {
                    float angleDistance = 360f / clientIds.Count;
                    _ringAngleDeg = angleDistance * i;
                }
            }


            _moveAction = InputSystem.actions.FindAction("Move");
            
            Camera cam = Camera.main;
            cam.transform.position = _camTransform.position;
            cam.transform.rotation = _camTransform.rotation;
            cam.transform.parent = _camTransform;

            PaintTheBallManager.ToggleGamePhase += ToggleControls;
        }

        private void OnDestroy()
        {
            PaintTheBallManager.ToggleGamePhase -= ToggleControls;
        }

        private void ToggleControls(bool toggle)
        {
            _controlsEnabled = toggle;
        }

        private void Update()
        {
            if (!_controlsEnabled) return;
            
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            float deltaPitch = -_paintTheBallConfig.LookSpeed * Time.deltaTime * moveInput.y;
            _pitch += deltaPitch;
            _pitch = Mathf.Clamp(_pitch, -_paintTheBallConfig.MaxPitch, _paintTheBallConfig.MaxPitch);

            if(Mathf.Abs(moveInput.x) > 0.1f)
                _ringAngleDeg += moveInput.x * _paintTheBallConfig.AngleSpeed * Time.deltaTime;

            transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * _ringAngleDeg) * _paintTheBallConfig.Radius, 0f, Mathf.Sin(Mathf.Deg2Rad * _ringAngleDeg) * _paintTheBallConfig.Radius);
            _bodyTransform.localRotation = Quaternion.Euler(_pitch, _paintTheBallConfig.RotationOffset.y - _ringAngleDeg, 0f);
        }
    }
}

