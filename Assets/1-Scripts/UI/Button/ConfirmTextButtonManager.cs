using UnityEngine;

public class ConfirmTextButtonManager : MonoBehaviour
{
    private ConfirmTextButton currentButton;
    public System.Action<ConfirmTextButton> OnButtonExpanded;

    public void RegisterExpand(ConfirmTextButton btn, bool silent = false)
    {
        if (currentButton != null && currentButton != btn)
            currentButton.ResetButton();

        currentButton = btn;

        if (!silent)
            OnButtonExpanded?.Invoke(btn);
    }

    public void Unregister(ConfirmTextButton btn)
    {
        if (currentButton == btn)
            currentButton = null;
    }

    public bool IsAnotherButtonActive(ConfirmTextButton btn)
    {
        return currentButton != null && currentButton != btn;
    }
}
