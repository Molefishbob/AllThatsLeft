using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject Canvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableMainMenuPanel()
    {
        Canvas.transform.GetChild(1).gameObject.SetActive(true); // Main Menu panel
        Canvas.transform.GetChild(2).gameObject.SetActive(false); // Options panel
    }

    public void EnableOptionsPanel()
    {
        Canvas.transform.GetChild(1).gameObject.SetActive(false); // Main Menu panel
        Canvas.transform.GetChild(2).gameObject.SetActive(true); // Options panel
    }
}
