using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.QuickMath
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private QuickMathConfigSO _gameConfig;
        [SerializeField] private LayerMask _planeMask;
        [SerializeField] private Transform _body;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rb;
        
        private bool _isMoving;
        private Vector3 _wayPoint;
        private InputAction _pointAction;
        private InputAction _pressAction;
        private bool _isDespawned;


        public override void OnStartClient()
        {
            base.OnStartClient();
            if(!IsOwner) return;
            _collider.enabled = true;
            _rb.useGravity = true;
            _wayPoint = transform.position;
            _pointAction = InputSystem.actions.FindAction("Point");
            _pressAction = InputSystem.actions.FindAction("Click");
        }

        private void Update()
        {
            if(!IsOwner || _isDespawned) return;
            CheckForDespawn();
            GetInput();
        }

        private void FixedUpdate()
        {
            if(!IsOwner || _isDespawned || !_isMoving) return;
            MovePlayerOnPlane();
        }

        private void CheckForDespawn()
        {
            if (transform.position.y > -5) return;
            _isDespawned = true;
            DespawnOnServer();
        }

        [ServerRpc]
        private void DespawnOnServer()
        {
            QuickMathManager.Instance.PlayerDied(OwnerId);
            Despawn(gameObject);
        }

        private void GetInput()
        {
            if (!_pressAction.WasPressedThisFrame()) return;
        
            _isMoving = true;
            _wayPoint = GetWayPoint(_pointAction.ReadValue<Vector2>());
            _wayPoint.y = 0;
        }

        private Vector3 GetWayPoint(Vector2 pointerPos)
        {
            if (!PointerConversionUtil.Instance.ScreenPointToWorldWithRaycast(pointerPos, _planeMask, 1000f,
                    out Vector3 hitPoint)) return transform.position;
            return hitPoint;

        }

        private void MovePlayerOnPlane()
        {
            var planarWayPoint = new Vector2(_wayPoint.x, _wayPoint.z);
            Vector2 planarDirection = planarWayPoint - new Vector2(_rb.position.x, _rb.position.z);
            Vector3 direction = _wayPoint - _rb.position;
            if (direction.sqrMagnitude < Mathf.Pow(_gameConfig.Speed * Time.fixedDeltaTime, 2))
            {
                _rb.position = _wayPoint;
                _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
                _isMoving = false;
                return;
            }
            if (_rb.position.y < -0.1f) { _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f); _isMoving = false; return; }
            _body.LookAt(new Vector3(planarWayPoint.x, _body.position.y, planarWayPoint.y));
            var planarVelocity = planarDirection.normalized * _gameConfig.Speed;
            _rb.linearVelocity = new Vector3(planarVelocity.x, _rb.linearVelocity.y, planarVelocity.y);
        }
    }
}