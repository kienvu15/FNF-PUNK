using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private double timeInstantiated;          // thời điểm note được spawn
    public float assignedTime;                // thời điểm note phải được hit

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        if (t > 1f)
        {
            Destroy(gameObject); // Xóa note khi đã đi qua vùng despawn
        }
        else
        {
            // Di chuyển note từ vị trí spawn xuống despawn theo thời gian
            transform.localPosition = Vector3.Lerp(
                Vector3.up * SongManager.Instance.noteSpawnY,
                Vector3.up * SongManager.Instance.noteDespawnY,
                t
            );

            // Đảm bảo sprite luôn bật
            if (!spriteRenderer.enabled)
                spriteRenderer.enabled = true;
        }



    }
}
