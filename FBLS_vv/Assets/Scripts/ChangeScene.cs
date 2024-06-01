using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{ 
    public static char currentScene;
    public GameObject setting;
   // public Transform preview;
    public void changeSingle()
    {
        
        SceneManager.LoadScene("SampleScene");
    }
    public void changeComputer()
    {
        SceneManager.LoadScene("MultiScene");
    }
    public void GameExit()
    {
        Application.Quit();
        Debug.Log("Á¾·á");
    }
    public void goMain()
    {
        SceneManager.LoadScene("Main");
        
    }    
    public void openSetting()
    {
        setting.SetActive(true);
    }
    public void CloseSetting()
    {
        setting.SetActive(false);
    }
}
