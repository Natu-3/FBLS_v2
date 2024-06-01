using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public Text score;
    public GameObject draw;
    public GameObject win1p;
    public GameObject win2p;
    private Stage1 stage;
    private StageMulti multi;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "SingleGameOver")
        {
            score.text = "Score: " + PlayerPrefs.GetInt("score").ToString();
        }
        else if (SceneManager.GetActiveScene().name == "MultiGameOver")
        {
            draw.SetActive(false);
            win1p.SetActive(false);
            win2p.SetActive(false);
        }
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
        else if(SceneManager.GetActiveScene().name == "MultiGameOver")
        {
            if (!StageMulti.lose && !Stage1.lose)
            {
                if ((Stage1.scoreVal == StageMulti.scoreVal) || (Stage1.panaltyVal == StageMulti.panaltyVal))
                { // 스코어 같거나 천장 위치 동일
                    draw.SetActive(true);
                }
            }
            else if ((Stage1.panaltyVal < StageMulti.panaltyVal) || StageMulti.lose)
            { // 1p 천장 위치 더 높음, 2p가 천장 닿았을 때
                win1p.SetActive(true);
            }
            else if ((Stage1.panaltyVal > StageMulti.panaltyVal) || Stage1.lose)
            { //2p 천장 위치 더 높음, 1p가 천장 닿았을 때
                win2p.SetActive(true);
            }
        }
    }
}
