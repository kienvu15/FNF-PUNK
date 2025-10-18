using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VirtualScroll : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    public RectTransform content;
    public GameObject itemPrefab;

    [Header("Settings")]
    public int totalItemCount = 30;
    public int visibleCount = 5;
    public float itemHeight = 200f;
    public float spacing = 30f;
    public float snapDuration = 0.25f;
    public float selectedOffsetX = 60f; // 👉 Dịch sang phải khi được chọn

    private RectTransform[] items;
    private int topIndex = 0;
    private bool isDragging = false;
    private bool isSnapping = false;
    private int selectedIndex = -1;

    void Start()
    {
        items = new RectTransform[visibleCount];

        float totalContentHeight = totalItemCount * (itemHeight + spacing);
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalContentHeight);

        for (int i = 0; i < visibleCount; i++)
        {
            GameObject obj = Instantiate(itemPrefab, content);
            RectTransform rt = obj.GetComponent<RectTransform>();
            items[i] = rt;

            float y = -i * (itemHeight + spacing);
            rt.anchoredPosition = new Vector2(0, y);

            obj.GetComponentInChildren<TextMeshProUGUI>().text = "Item " + i;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    void Update()
    {
        if (!Input.GetMouseButton(0) && isDragging && !isSnapping)
        {
            isDragging = false;
            scrollRect.velocity = Vector2.zero;
            StartCoroutine(SnapToNearestItem());
        }
    }

    public void OnBeginDrag()
    {
        isDragging = true;
        isSnapping = false;
    }

    void OnScroll(Vector2 pos)
    {
        if (isSnapping) return;

        float scrollY = content.anchoredPosition.y;
        int newTop = Mathf.FloorToInt(scrollY / (itemHeight + spacing));

        if (newTop != topIndex)
            UpdateItems(newTop);
    }

    void UpdateItems(int newTop)
    {
        if (newTop < 0 || newTop + visibleCount > totalItemCount)
            return;

        topIndex = newTop;

        for (int i = 0; i < visibleCount; i++)
        {
            int dataIndex = topIndex + i;
            if (dataIndex < 0 || dataIndex >= totalItemCount) continue;

            RectTransform rt = items[i];
            rt.anchoredPosition = new Vector2(0, -dataIndex * (itemHeight + spacing));
            rt.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Item " + dataIndex;
        }
    }

    IEnumerator SnapToNearestItem()
    {
        isSnapping = true;
        scrollRect.enabled = false;

        float scrollY = content.anchoredPosition.y;
        float totalHeight = totalItemCount * (itemHeight + spacing);
        float viewportCenterY = scrollRect.viewport.rect.height / 2f;

        float minDist = float.MaxValue;
        float targetPos = scrollY;
        int nearestIndex = 0;

        // Tìm item có tâm gần giữa màn hình nhất
        for (int i = 0; i < totalItemCount; i++)
        {
            float itemCenterY = i * (itemHeight + spacing) + itemHeight / 2f;
            float dist = Mathf.Abs(itemCenterY - (scrollY + viewportCenterY));
            if (dist < minDist)
            {
                minDist = dist;
                targetPos = itemCenterY - viewportCenterY;
                nearestIndex = i;
            }
        }

        selectedIndex = nearestIndex;
        targetPos = Mathf.Clamp(targetPos, 0, totalHeight - scrollRect.viewport.rect.height);

        float elapsed = 0f;
        float start = content.anchoredPosition.y;

        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / snapDuration);
            float newY = Mathf.Lerp(start, targetPos, t);
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);
            yield return null;
        }

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetPos);

        HighlightSelectedItem();
        scrollRect.enabled = true;
        isSnapping = false;
    }

    void HighlightSelectedItem()
    {
        // Reset toàn bộ item
        foreach (RectTransform rt in items)
        {
            if (rt == null) continue;
            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
        }

        // Tìm item đang hiển thị tương ứng với selectedIndex
        for (int i = 0; i < visibleCount; i++)
        {
            int dataIndex = topIndex + i;
            if (dataIndex == selectedIndex)
            {
                // Dịch sang phải
                RectTransform rt = items[i];
                rt.anchoredPosition = new Vector2(selectedOffsetX, rt.anchoredPosition.y);
                break;
            }
        }
    }
}
