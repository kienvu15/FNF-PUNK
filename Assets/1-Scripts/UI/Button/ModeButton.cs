using UnityEngine;

public class ModeButton : MonoBehaviour
{
    [Header("References")]
    public ConfirmTextButton confirmButton;

    [Header("Display Info")]
    public Sprite displaySprite;
    [TextArea]
    public string displayText;

    private void Awake()
    {
        if (confirmButton == null)
            confirmButton = GetComponent<ConfirmTextButton>();
    }

    public void ShowSotryPanel() 
    { 
    
    }

    public void ShowFreePlayPanel()
    {

    }

    public void ShowZenPanel()
    {

    }

}
