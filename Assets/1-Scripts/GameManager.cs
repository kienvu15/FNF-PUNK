using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject scene1;
    //public GameObject scene2;

    void Awake()
    {
        
    }

    public void touchStart()
    {
        scene1.SetActive(false);
        //scene2.SetActive(true);
    }

    public void Back2Menu()
    {
        //scene2.SetActive(false);
        scene1.SetActive(true);
    }

    public void Pess2Start()
    {
        SceneManager.LoadScene("Press2Start");
    }

    public void GiveItToMe()
    {
        SceneManager.LoadScene("GiveItToMe");
    }
    
    public void KhuTaoSon()
    {
        SceneManager.LoadScene("KhuTaoSong");
    }
    public void PlasticBeach()
    {
        SceneManager.LoadScene("PlasticBeach");
    }

    public void showmethesky()
    {
        SceneManager.LoadScene("showmethesky"); 
    }

    public void Aloso()
    {
        SceneManager.LoadScene("Aloso");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
