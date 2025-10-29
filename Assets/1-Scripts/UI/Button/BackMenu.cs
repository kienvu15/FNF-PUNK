using UnityEngine;

public class BackMenu : MonoBehaviour
{
    public GameObject panelContainer;

    public void OnBackButtonPressed()
    {

        if (panelContainer.transform.childCount > 0)
        {
            // Tìm panel đang bật cuối cùng
            for (int i = panelContainer.transform.childCount - 1; i >= 0; i--)
            {
                Transform panel = panelContainer.transform.GetChild(i);
                if (panel.gameObject.activeSelf)
                {
                    panel.gameObject.SetActive(false);
                    break;
                }
            }
        }

        // Sau khi tắt, nếu không còn panel nào bật -> ẩn nút Back
        bool anyActive = false;
        for (int i = 0; i < panelContainer.transform.childCount; i++)
        {
            if (panelContainer.transform.GetChild(i).gameObject.activeSelf)
            {
                anyActive = true;
                break;
            }
        }

        gameObject.SetActive(anyActive); // nếu không còn panel nào bật thì tự ẩn

    }



}
