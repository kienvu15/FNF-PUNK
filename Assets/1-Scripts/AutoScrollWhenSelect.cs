using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class AutoScrollWhenSelect : MonoBehaviour
{
    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        viewport = scrollRect.viewport;
        content = scrollRect.content;
    }

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null || !selected.transform.IsChildOf(content))
            return;

        RectTransform selectedRect = selected.GetComponent<RectTransform>();
        if (selectedRect == null)
            return;

        EnsureVisible(selectedRect);
    }

    private void EnsureVisible(RectTransform target)
    {
        // chuyển sang tọa độ local của viewport
        Vector3[] viewportCorners = new Vector3[4];
        Vector3[] targetCorners = new Vector3[4];
        viewport.GetWorldCorners(viewportCorners);
        target.GetWorldCorners(targetCorners);

        float viewportHeight = viewportCorners[2].y - viewportCorners[0].y;
        float contentHeight = content.rect.height;

        float diffTop = targetCorners[1].y - viewportCorners[1].y;
        float diffBottom = targetCorners[0].y - viewportCorners[0].y;

        float normalizedPos = scrollRect.verticalNormalizedPosition;

        if (diffTop > 0)
            normalizedPos -= diffTop / (contentHeight - viewportHeight);
        else if (diffBottom < 0)
            normalizedPos -= diffBottom / (contentHeight - viewportHeight);

        normalizedPos = Mathf.Clamp01(normalizedPos);
        scrollRect.verticalNormalizedPosition = Mathf.Lerp(
            scrollRect.verticalNormalizedPosition, normalizedPos, Time.deltaTime * 12f
        );
    }
}
