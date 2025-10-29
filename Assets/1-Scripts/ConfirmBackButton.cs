using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ConfirmBackButton : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI text;
    public UnityEvent onConfirmClick;

    [Header("Style Settings")]
    public Color normalColor = Color.black;
    public Color highlightColor = Color.white;
    public Color confirmFlashColor = new Color(1f, 0.5f, 0f);
    public Color normalOutlineColor = Color.white;
    public Color highlightOutlineColor = Color.black;

    [Header("Animation")]
    public float scaleUpSize = 1.2f;
    public float scaleTime = 0.2f;

    private bool isExpanded = false;
    private bool isConfirming = false;
    private Vector3 originalScale;
    private Tween scaleTween;

    void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();

        originalScale = transform.localScale;
    }

    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn)
            btn.onClick.AddListener(OnClick);

        // Setup ban đầu
        ResetVisualImmediate();
    }

    private void OnClick()
    {
        if (isConfirming) return;

        if (!isExpanded)
        {
            // 🔹 Lần đầu bấm: phóng to + highlight
            isExpanded = true;
            ExpandVisual();
        }
        else
        {
            // 🔹 Lần hai bấm: xác nhận
            isConfirming = true;
            ConfirmEffect(() =>
            {
                onConfirmClick?.Invoke();
                ResetButton(); // Tự reset về stage đầu
            });
        }
    }

    private void ExpandVisual(bool immediate = false)
    {
        scaleTween?.Kill();
        float duration = immediate ? 0f : scaleTime;

        scaleTween = transform.DOScale(originalScale * scaleUpSize, duration)
            .SetEase(Ease.OutBack);

        SetTextColor(highlightColor, highlightOutlineColor);
    }

    private void ConfirmEffect(System.Action onComplete)
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => SetTextColor(confirmFlashColor, highlightOutlineColor));
        seq.Append(transform.DOScale(originalScale * (scaleUpSize + 0.15f), 0.15f).SetEase(Ease.OutBack));
        seq.AppendCallback(() => SetTextColor(highlightColor, highlightOutlineColor));
        seq.Append(transform.DOScale(originalScale * scaleUpSize, 0.15f).SetEase(Ease.OutBack));

        seq.OnComplete(() =>
        {
            isConfirming = false;
            onComplete?.Invoke();
        });
    }

    private void SetTextColor(Color fill, Color outline)
    {
        if (!text) return;
        text.color = fill;
        text.outlineColor = outline;
        text.ForceMeshUpdate();

        if (text.fontMaterial != null && text.fontMaterial.HasProperty("_OutlineColor"))
            text.fontMaterial.SetColor("_OutlineColor", outline);
    }

    public void ResetButton()
    {
        if (!isExpanded && !isConfirming) return;

        isExpanded = false;
        isConfirming = false;

        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale, scaleTime).SetEase(Ease.InBack);
        SetTextColor(normalColor, normalOutlineColor);
    }

    private void ResetVisualImmediate()
    {
        transform.localScale = originalScale;
        SetTextColor(normalColor, normalOutlineColor);
        isExpanded = false;
        isConfirming = false;
    }

    private void OnDisable()
    {
        // Khi bị disable thì reset lại ngay
        ResetVisualImmediate();
    }
}
