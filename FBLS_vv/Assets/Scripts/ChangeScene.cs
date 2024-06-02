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
        SoundManager.Instance.playSfx(SfxType.Click);
    }
    public void changeComputer()
    {
        SceneManager.LoadScene("MultiScene");
        SoundManager.Instance.playSfx(SfxType.Click);
    }
    public void GameExit()
    {
        Application.Quit();
        Debug.Log("Á¾·á");
        SoundManager.Instance.playSfx(SfxType.Click);
    }
    public void goMain()
    {
        SceneManager.LoadScene("Main");
        SoundManager.Instance.playSfx(SfxType.Click);

    }    
    public void openSetting()
    {
        setting.SetActive(true);
        SoundManager.Instance.playSfx(SfxType.Click);
    }
    public void CloseSetting()
    {
        setting.SetActive(false);
        SoundManager.Instance.playSfx(SfxType.Click);
    }
}
