using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")]
    public Transform uiRoot;
   public GameObject backButton;
    public List<GameObject> panelPrefabs;

    private Dictionary<string, GameObject> panelPool = new();

    void Awake()
    {
        Instance = this;
    }

    // 🔹 Hàm gọi từ button
    public void ShowPanel(string panelName)
    {
        GameObject panel = GetOrCreatePanel(panelName);
        if (panel.activeSelf) return;

        panel.SetActive(true);
        AnimateOpen(panel);
       backButton.SetActive(true);
    }

    // 🔹 Hàm đóng panel
    public void HidePanel(string panelName)
    {
        if (!panelPool.TryGetValue(panelName, out var panel)) return;
        AnimateClose(panel);
      backButton.SetActive(false);
    }

    // 🔹 Tạo hoặc lấy panel trong pool
    private GameObject GetOrCreatePanel(string name)
    {
        if (panelPool.TryGetValue(name, out var panel)) return panel;

        var prefab = panelPrefabs.Find(p => p.name == name);
        if (prefab == null)
        {
            Debug.LogWarning($"❌ No prefab found for panel: {name}");
            return null;
        }

        panel = Instantiate(prefab, uiRoot);
        panel.name = name;
        panel.SetActive(false);
        panelPool[name] = panel;
        return panel;
    }

    // 🔹 Hiệu ứng mở panel (fade in)
    private void AnimateOpen(GameObject panel)
    {
        var cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        cg.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
    }

    private void AnimateClose(GameObject panel)
    {
        // ✅ Reset ConfirmTextButton NGAY LẬP TỨC để tránh giữ scale cũ
        foreach (var btn in panel.GetComponentsInChildren<ConfirmTextButton>(true))
            btn.ResetButton();

        var cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        cg.DOKill(); // tránh tween chồng
        cg.DOFade(0f, 0.3f)
          .SetEase(Ease.InQuad)
          .OnComplete(() =>
          {
              cg.alpha = 0f;
              panel.SetActive(false);
          });
    }


}
