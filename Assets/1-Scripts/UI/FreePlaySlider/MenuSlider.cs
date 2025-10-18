using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(ScrollRect))]
public class HorizontalScrollController : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform viewport;
    public RectTransform itemPrefab;
    public Button leftButton;
    public Button rightButton;

    [Header("Settings")]
    public float scrollSpeed = 8f;
    private float scrollStep;
    private Coroutine scrollRoutine;

    private void Start()
    {
        StartCoroutine(InitAfterLayout());
    }

    private IEnumerator InitAfterLayout()
    {
        // Chờ 1 frame để layout tính toán xong
        yield return null;

        // Lấy spacing (nếu có HorizontalLayoutGroup)
        float spacing = 0f;
        var layout = content.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
            spacing = layout.spacing;

        float itemWidth = itemPrefab.rect.width + spacing;
        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;

        // Tránh chia cho 0
        if (contentWidth <= viewportWidth)
            scrollStep = 1f;
        else
            scrollStep = itemWidth / (contentWidth - viewportWidth);

        leftButton.onClick.AddListener(() => Scroll(-scrollStep));
        rightButton.onClick.AddListener(() => Scroll(scrollStep));
    }

    private void Scroll(float amount)
    {
        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        float target = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition + amount);
        scrollRoutine = StartCoroutine(SmoothScroll(target));
    }

    private IEnumerator SmoothScroll(float target)
    {
        float start = scrollRect.horizontalNormalizedPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scrollSpeed;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = target;
    }

    private void Update()
    {
        leftButton.interactable = scrollRect.horizontalNormalizedPosition > 0.01f;
        rightButton.interactable = scrollRect.horizontalNormalizedPosition < 0.99f;
    }
}
