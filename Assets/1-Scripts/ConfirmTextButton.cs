using UnityEngine;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class ConfirmTextButton : MonoBehaviour
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

    [Header("State")]
    [SerializeField] private bool isExpanded = false; // ✅ Serialize private để prefab giữ đúng
    public bool IsExpanded => isExpanded;

    private bool isConfirming = false;
    private Vector3 originalScale;
    private Tween scaleTween;
    private ConfirmTextButtonManager manager;
    private bool hasInitialized = false;

    void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        originalScale = transform.localScale;

        // Instance font material để đổi outline an toàn
        if (text != null && text.fontMaterial != null && !text.fontMaterial.name.EndsWith("(Instance)"))
            text.fontMaterial = new Material(text.fontMaterial);

        manager = GetComponentInParent<ConfirmTextButtonManager>();
    }

    void Start()
    {
        var button = GetComponent<Button>();
        if (button)
            button.onClick.AddListener(OnClick);

        // ✅ Đợi panel bật hoàn toàn rồi apply expand
        StartCoroutine(ApplyInitialExpand());
    }

    private IEnumerator ApplyInitialExpand()
    {
        yield return null; // chờ 1 frame cho layout ổn định

        if (isExpanded)
        {
            manager?.RegisterExpand(this, true);
            ExpandVisual(true);
        }
        else
        {
            SetTextColor(normalColor, normalOutlineColor);
            transform.localScale = originalScale;
        }

        hasInitialized = true;
    }

    public void OnClick()
    {
        if (isConfirming) return;

        if (manager == null)
            manager = GetComponentInParent<ConfirmTextButtonManager>();
        if (manager == null)
            return;

        // Nếu đang mở nút khác, đóng nó trước
        if (manager.IsAnotherButtonActive(this))
            manager.RegisterExpand(this);

        if (!isExpanded)
        {
            isExpanded = true;
            manager.RegisterExpand(this);
            ExpandVisual();
        }
        else
        {
            isConfirming = true;
            ConfirmEffect(() =>
            {
                onConfirmClick?.Invoke();
                isConfirming = false;
            });
        }
    }

    private void ExpandVisual(bool immediate = false)
    {
        scaleTween?.Kill();
        float duration = immediate ? 0f : scaleTime;
        scaleTween = transform.DOScale(originalScale * scaleUpSize, duration).SetEase(Ease.OutBack);
        SetTextColor(highlightColor, highlightOutlineColor);
    }

    private void ConfirmEffect(System.Action onComplete)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => SetTextColor(confirmFlashColor, highlightOutlineColor));
        seq.Append(transform.DOScale(originalScale * (scaleUpSize + 0.15f), 0.15f).SetEase(Ease.OutBack));
        seq.AppendCallback(() => SetTextColor(highlightColor, highlightOutlineColor));
        seq.Append(transform.DOScale(originalScale * scaleUpSize, 0.15f).SetEase(Ease.OutBack));
        seq.OnComplete(() => onComplete?.Invoke());
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
        if (!isExpanded) return;
        isExpanded = false;

        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale, scaleTime).SetEase(Ease.InBack);
        SetTextColor(normalColor, normalOutlineColor);

        manager?.Unregister(this);
    }

    //private void OnDisable()
    //{
    //    scaleTween?.Kill();
    //    transform.localScale = originalScale;
    //    isConfirming = false;

    //    // ❗Không reset isExpanded ở đây
    //    // để panel bật lại vẫn giữ đúng trạng thái prefab
    //    SetTextColor(normalColor, normalOutlineColor);
    //    manager?.Unregister(this);
    //}
}
