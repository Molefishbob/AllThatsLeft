using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject Canvas;
    private GameObject mainMenuPanel;
    private GameObject optionsPanel;
    private GameObject quitPanel;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuPanel = Canvas.transform.GetChild(1).gameObject;
        optionsPanel = Canvas.transform.GetChild(2).gameObject;
        quitPanel = Canvas.transform.GetChild(3).gameObject;
    }

    void StartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void EnableMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        quitPanel.SetActive(false);
    }

    public void EnableOptionsPanel()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void EnableConfirmQuit(){
        mainMenuPanel.SetActive(false);
        quitPanel.SetActive(true);
    }

    public void Quit()
    {
        Debug.Log("Quitted");
        Application.Quit();
    }
}
