using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject _canvas;
    private GameObject _mainMenuPanel;
    private GameObject _optionsPanel;
    private GameObject _quitPanel;

    // Start is called before the first frame update
    void Start()
    {
        _mainMenuPanel = _canvas
.transform.GetChild(1).gameObject;
        _optionsPanel = _canvas
.transform.GetChild(2).gameObject;
        _quitPanel = _canvas
.transform.GetChild(3).gameObject;
    }

    void StartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void EnableMainMenuPanel()
    {
        _mainMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        _quitPanel.SetActive(false);
    }

    public void EnableOptionsPanel()
    {
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
    }

    public void EnableConfirmQuit(){
        _mainMenuPanel.SetActive(false);
        _quitPanel.SetActive(true);
    }

    public void Quit()
    {
        Debug.Log("Quitted");
        Application.Quit();
    }
}
