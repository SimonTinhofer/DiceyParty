using UnityEngine;

namespace DiceyParty
{
    public class PointerConversionUtil : MonoBehaviour
    {
        public static PointerConversionUtil Instance { get; private set; }

        [SerializeField] private Camera _gameplayCamera;
        [SerializeField] private RectTransform _gameplayViewRect;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool ScreenPointToWorldWithRaycast(
            Vector2 screenPos,
            LayerMask planeMask,
            float maxDistance,
            out Vector3 hitPoint)
        {
            hitPoint = default;

            if (_gameplayCamera == null || _gameplayViewRect == null)
                return false;

            if (!RectTransformUtility.RectangleContainsScreenPoint(_gameplayViewRect, screenPos, null))
                return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _gameplayViewRect,
                    screenPos,
                    null,
                    out Vector2 localPoint))
            {
                return false;
            }

            Rect rect = _gameplayViewRect.rect;

            float u = (localPoint.x / rect.width) + _gameplayViewRect.pivot.x;
            float v = (localPoint.y / rect.height) + _gameplayViewRect.pivot.y;

            if (u < 0f || u > 1f || v < 0f || v > 1f)
                return false;

            Ray ray = _gameplayCamera.ViewportPointToRay(new Vector3(u, v, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, planeMask))
            {
                hitPoint = hit.point;
                return true;
            }

            return false;
        }
    }
}
