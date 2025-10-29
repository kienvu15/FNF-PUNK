using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoMultiImageSwitcher : MonoBehaviour
{
    [Header("References")]
    public GameObject defaultImage;      // Ảnh gốc
    public List<GameObject> altImages;   // Danh sách ảnh thay thế

    [Header("Settings")]
    public float displayDuration = 0.5f; // Thời gian hiển thị mỗi ảnh random
    public Vector2 restTimeRange = new Vector2(1.5f, 3f); // Thời gian nghỉ giữa các chu kỳ
    public Vector2 burstCountRange = new Vector2(2, 3);   // Số ảnh hiển thị liên tục (2–3)

    private Coroutine autoRoutine;

    void Start()
    {
        // Ẩn toàn bộ ảnh thay thế khi bắt đầu
        foreach (var img in altImages)
            img.SetActive(false);

        defaultImage.SetActive(true);

        // Bắt đầu vòng lặp tự động
        autoRoutine = StartCoroutine(AutoSwitchLoop());
    }

    IEnumerator AutoSwitchLoop()
    {
        while (true)
        {
            // --- Pha "burst" (hiển thị 2-3 ảnh liên tục) ---
            int burstCount = Random.Range((int)burstCountRange.x, (int)burstCountRange.y + 1);

            for (int i = 0; i < burstCount; i++)
            {
                yield return StartCoroutine(SwitchRandomImage());
            }

            // --- Nghỉ ngẫu nhiên ---
            float rest = Random.Range(restTimeRange.x, restTimeRange.y);
            yield return new WaitForSeconds(rest);
        }
    }

    IEnumerator SwitchRandomImage()
    {
        // Ẩn ảnh gốc
        defaultImage.SetActive(false);

        // Chọn ảnh random
        int randIndex = Random.Range(0, altImages.Count);
        GameObject chosenImage = altImages[randIndex];
        chosenImage.SetActive(true);

        // Đợi trong thời gian hiển thị
        yield return new WaitForSeconds(displayDuration);

        // Ẩn ảnh random và bật lại ảnh gốc
        chosenImage.SetActive(false);
        defaultImage.SetActive(true);

        // Nghỉ ngắn giữa 2 ảnh liên tiếp trong burst (tùy chọn)
        yield return new WaitForSeconds(0.1f);
    }
}
