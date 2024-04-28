using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
   // public Transform preview;
    public void change()
    {
        SceneManager.LoadScene("SampleScene");
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
