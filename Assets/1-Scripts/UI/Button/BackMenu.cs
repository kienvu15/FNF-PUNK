using UnityEngine;

public class BackMenu : MonoBehaviour
{
    public GameObject panelContainer;

    public void OnBackButtonPressed()
    {

        if (panelContainer.transform.childCount > 0)
        {
            Transform lastPanel = panelContainer.transform.GetChild(panelContainer.transform.childCount - 1);
            if (lastPanel != null)
            { 
                lastPanel.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }

        if (panelContainer.transform.childCount == 0)
        {
            gameObject.SetActive(false);
        }
    }



}
