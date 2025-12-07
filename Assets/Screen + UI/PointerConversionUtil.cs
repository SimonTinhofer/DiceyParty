using UnityEngine;

public class PointerConversionUtil : MonoBehaviour
{
    public static PointerConversionUtil Instance { get; private set; }

    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private RectTransform gameplayViewRect;

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

        if (gameplayCamera == null || gameplayViewRect == null)
            return false;

        if (!RectTransformUtility.RectangleContainsScreenPoint(gameplayViewRect, screenPos, null))
            return false;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gameplayViewRect,
                screenPos,
                null,
                out Vector2 localPoint))
        {
            return false;
        }

        Rect rect = gameplayViewRect.rect;

        float u = (localPoint.x / rect.width) + gameplayViewRect.pivot.x;
        float v = (localPoint.y / rect.height) + gameplayViewRect.pivot.y;

        if (u < 0f || u > 1f || v < 0f || v > 1f)
            return false;

        Ray ray = gameplayCamera.ViewportPointToRay(new Vector3(u, v, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, planeMask))
        {
            hitPoint = hit.point;
            return true;
        }

        return false;
    }
}
