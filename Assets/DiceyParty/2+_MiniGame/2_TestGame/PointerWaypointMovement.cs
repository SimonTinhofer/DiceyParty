using DiceyParty; 
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.TestGame
{
    public class PointerWaypointMovement : MonoBehaviour
    {
        //hard coded config
        [SerializeField] private float _speed = 5f;
        [SerializeField] private LayerMask _planeMask;
        [SerializeField] private Transform _body;
    
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
            return hitPoint;

        }

        private void MovePlayer()
        {
            if(transform.position == _wayPoint) return;
            
            _movementDirection = _wayPoint - transform.position;
            if (_movementDirection.magnitude < _speed * Time.deltaTime)
            {
                transform.position = _wayPoint;
                _isMoving = false;
                return;
            }
            _body.LookAt(_wayPoint);
            transform.position += _movementDirection.normalized * (_speed * Time.deltaTime);
        
        }
    }
}

