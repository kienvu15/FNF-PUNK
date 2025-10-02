using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public AudioSource audioSource;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (audioSource.isPlaying) audioSource.Pause();
            else audioSource.Play();
        }
    }

    // phương thức dùng để Start từ code
    public void PlayClip() => audioSource.Play();
}