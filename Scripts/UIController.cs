using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{

    public GameObject MainPanel;
    public GameObject GamePlayPanel;

 
    public void StartTheGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        GamePlayPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void ExitTheGame()
    {
        Application.Quit();
    }
    
}
