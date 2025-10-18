using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip firstClickSound;
    public AudioClip confirmClickSound;

    [Header("Audio Source Settings")]
    public AudioSource audioSource;

    private ConfirmTextButton confirmButton;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        confirmButton = GetComponent<ConfirmTextButton>();

        if (audioSource == null)
        {
            // tạo AudioSource tạm nếu chưa có
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Gắn listener
        button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (confirmButton == null)
        {
            // nếu không có ConfirmTextButton -> phát âm mặc định
            Play(firstClickSound);
        }
        else
        {
            // có ConfirmTextButton -> phân biệt lần 1 / lần 2
            if (!confirmButton.IsExpanded)
                Play(firstClickSound);
            else
                Play(confirmClickSound);
        }
    }

    void Play(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
