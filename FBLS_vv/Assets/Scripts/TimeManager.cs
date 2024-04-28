using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public Text[] text_time;
    public float limitTime = 30f; // 제한 시간(초)
    public GameObject gameover;
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        if (limitTime > 0)
        {
            limitTime -= Time.deltaTime;

            int minutes = (int)limitTime / 60;
            int seconds = (int)limitTime % 60;

            if (minutes < 10 || seconds < 10)
            {
                text_time[0].text = minutes.ToString("00"); //분
                text_time[1].text = seconds.ToString("00"); //초
            }
            else
            {
                text_time[0].text = minutes.ToString(); //분
                text_time[1].text = seconds.ToString(); //초
            }

        }
        else {
            SceneManager.LoadScene("GameOver");
        }

    }
    
}
