using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ModeButtonManager : MonoBehaviour
{
    [Header("Target Display")]
    public Image targetImage;
    public TextMeshProUGUI targetText;

    [Header("All Mode Buttons")]
    public List<ModeButton> modeButtons = new List<ModeButton>();

    private ConfirmTextButtonManager manager;

    private void Awake()
    {
        manager = GetComponentInChildren<ConfirmTextButtonManager>();
    }

    private void OnEnable()
    {
        if (manager != null)
            manager.OnButtonExpanded += HandleButtonExpanded;

        // Hiển thị đúng mode đang expanded sẵn
        foreach (var mode in modeButtons)
        {
            if (mode.confirmButton != null && mode.confirmButton.IsExpanded)
            {
                DisplayMode(mode);
                break;
            }
        }
    }

    private void OnDisable()
    {
        if (manager != null)
            manager.OnButtonExpanded -= HandleButtonExpanded;
    }

    private void HandleButtonExpanded(ConfirmTextButton expandedButton)
    {
        foreach (var mode in modeButtons)
        {
            if (mode.confirmButton == expandedButton)
            {
                DisplayMode(mode);
                return;
            }
        }
    }

    private void DisplayMode(ModeButton mode)
    {
        if (targetImage) targetImage.sprite = mode.displaySprite;
        if (targetText) targetText.text = mode.displayText;
    }

    public void ClosePanel()
    {
        Destroy(gameObject);
    }
}
