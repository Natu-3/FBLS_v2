using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public Text score;
    public GameObject draw;
    public GameObject win;
    public GameObject lose;
    // Start is called before the first frame update
    void Start()
    {
        score.text = "Score: " + PlayerPrefs.GetInt("score").ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "SingleGameOver")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("Main");
            }
        }
        else if(SceneManager.GetActiveScene().name == "Computer")
        {
            /*
            if(스코어 같거나 && 천장 위치 동일){
                draw.SetActive(true);
            }
            else if(천장 위치 더 높음){
                win.SetActive(true);
            }
            else
            {
                lose.SetActive(true);
            }
            */
        }
    }
}
