using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Text textToBlink;
    public float blinkTime = 0.5f;
    
    public static char currentScene;
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
 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
