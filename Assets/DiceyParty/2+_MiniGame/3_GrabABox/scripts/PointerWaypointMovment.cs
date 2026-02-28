using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.GrabABox
{
    public class PointerWaypointMovment : NetworkBehaviour
    {
        [SerializeField] private GrabABoxConfigSO _gameConfig;
        [SerializeField] private LayerMask _planeMask;
        [SerializeField] private Transform _body;
        [SerializeField] private Animator _animator;
    
        private bool _isMoving;
        private Vector3 _wayPoint;
        private Vector3 _movementDirection;
        private InputAction _pointAction;
        private InputAction _pressAction;

        private void Start()
        {
            _wayPoint = transform.position;
            _pointAction = InputSystem.actions.FindAction("Point");
            _pressAction = InputSystem.actions.FindAction("Click");
        }

        private void Update()
        {
            if(!IsOwner) return;
                
            GetInput();
            if(!_isMoving) return;
            MovePlayer();
        }

        private void GetInput()
        {
            if (!_pressAction.WasPressedThisFrame()) return;
        
            _isMoving = true;
            _wayPoint = GetWayPoint(_pointAction.ReadValue<Vector2>());
        }

        private Vector3 GetWayPoint(Vector2 pointerPos)
        {
            if (!PointerConversionUtil.Instance.ScreenPointToWorldWithRaycast(pointerPos, _planeMask, 1000f,
                    out Vector3 hitPoint)) return transform.position;
        
            _isMoving = true;
            UpdateAnimation(true);
            
            return hitPoint;

        }

        private void MovePlayer()
        {
            if(transform.position == _wayPoint) return;
            
            _movementDirection = _wayPoint - transform.position;
            if (_movementDirection.magnitude < _gameConfig.Speed * Time.deltaTime)
            {
                transform.position = _wayPoint;
                _isMoving = false;
                UpdateAnimation(false);
                return;
            }
            _body.LookAt(_wayPoint);
            transform.position += _movementDirection.normalized * (_gameConfig.Speed * Time.deltaTime);
        
        }

        private void UpdateAnimation(bool isRunning)
        {
            ToggleIsRunning(isRunning);
            UpdateAnimationServer(isRunning);
        }

        [ServerRpc]
        private void UpdateAnimationServer(bool isRunning)
        {
            ToggleIsRunning(isRunning);
            UpdateAnimationObservers(isRunning);
        }

        [ObserversRpc(ExcludeOwner =  true)]
        private void UpdateAnimationObservers(bool isRunning)
        {
            ToggleIsRunning(isRunning);
        }
        
        private void ToggleIsRunning(bool toggle)
        {
            _animator.SetBool("isRunning", toggle);
        }
    }
}