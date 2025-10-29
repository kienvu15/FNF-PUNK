using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiImageSwitcher : MonoBehaviour
{
    [Header("References")]
    public GameObject defaultImage;      // Ảnh gốc
    public List<GameObject> altImages;   // Danh sách ảnh thay thế

    [Header("Settings")]
    public float displayDuration = 0.5f; // Thời gian hiển thị ảnh random

    private Coroutine switchRoutine;

    void Start()
    {
        // Ẩn toàn bộ ảnh thay thế khi bắt đầu
        foreach (var img in altImages)
            img.SetActive(false);

        defaultImage.SetActive(true);
    }

    void Update()
    {
        // Kiểm tra nhấn phím mũi tên lên
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnArrowPressed();
        }
    }

    public void OnArrowPressed()
    {
        // Nếu đang hiển thị ảnh khác thì bỏ qua để tránh lỗi chồng
        if (switchRoutine == null)
            switchRoutine = StartCoroutine(SwitchRandomImage());
    }

    IEnumerator SwitchRandomImage()
    {
        // Ẩn ảnh gốc
        defaultImage.SetActive(false);

        // Chọn ảnh random
        int randIndex = Random.Range(0, altImages.Count);
        GameObject chosenImage = altImages[randIndex];
        chosenImage.SetActive(true);

        // Đợi 0.5s
        yield return new WaitForSeconds(displayDuration);

        // Ẩn ảnh random và bật lại ảnh gốc
        chosenImage.SetActive(false);
        defaultImage.SetActive(true);

        // Cho phép lần sau
        switchRoutine = null;
    }
}
