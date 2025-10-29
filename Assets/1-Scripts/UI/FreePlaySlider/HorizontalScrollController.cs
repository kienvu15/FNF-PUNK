using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Display")]
    public Image targetImage;          // Image bên ngoài để hiển thị
    public Sprite[] volumeSprites;     // Mảng các sprite tương ứng với level

    [Header("Selection")]
    public int currentIndex = 0;  // item hiện tại
    public System.Action<int> OnItemSelected;

    private List<RectTransform> items = new List<RectTransform>();

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private void Start()
    {
        StartCoroutine(InitAfterLayout());
    }

    private IEnumerator InitAfterLayout()
    {
        yield return null;

        float spacing = 0f;
        var layout = content.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
            spacing = layout.spacing;

        float itemWidth = itemPrefab.rect.width + spacing;
        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;

        if (contentWidth <= viewportWidth)
            scrollStep = 1f;
        else
            scrollStep = itemWidth / (contentWidth - viewportWidth);

        // Lưu danh sách các item
        foreach (Transform child in content)
        {
            if (child is RectTransform rect)
                items.Add(rect);
        }

        leftButton.onClick.AddListener(ScrollLeft);
        rightButton.onClick.AddListener(ScrollRight);

        UpdateButtons();
        NotifySelection();
    }

    private void ScrollLeft()
    {
        if (currentIndex <= 0) return;

        currentIndex--;
        audioSource.PlayOneShot(clickSound);
        ScrollToIndex(currentIndex);
    }

    private void ScrollRight()
    {
        if (currentIndex >= items.Count - 1) return;

        currentIndex++;
        audioSource.PlayOneShot(clickSound);
        ScrollToIndex(currentIndex);
    }

    private void ScrollToIndex(int index)
    {
        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        float target = 0f;
        if (items.Count > 1)
            target = Mathf.Clamp01((float)index / (items.Count - 1));

        scrollRoutine = StartCoroutine(SmoothScroll(target));
    }

    private IEnumerator SmoothScroll(float target)
    {
        float start = scrollRect.horizontalNormalizedPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * scrollSpeed;
            scrollRect.horizontalNormalizedPosition =
                Mathf.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = target;

        UpdateButtons();
        NotifySelection();
    }

    private void UpdateButtons()
    {
        leftButton.interactable = currentIndex > 0;
        rightButton.interactable = currentIndex < items.Count - 1;
    }

    private void NotifySelection()
    {
        OnItemSelected?.Invoke(currentIndex);
        // ---- Hiển thị ảnh theo dữ liệu của item hiện tại ----
        
            var data = items[currentIndex].GetComponentInChildren<TestButton>();
            if (data != null)
            {
                int level = Mathf.Clamp(data.volumeLevel, 1, volumeSprites.Length);
                targetImage.sprite = volumeSprites[level - 1];
            }
        

    }

}
