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
                { // ���ھ� ���ų� õ�� ��ġ ����
                    draw.SetActive(true);
                }
            }
            else if ((Stage1.panaltyVal < StageMulti.panaltyVal) || StageMulti.lose)
            { // 1p õ�� ��ġ �� ����, 2p�� õ�� ����� ��
                win1p.SetActive(true);
            }
            else if ((Stage1.panaltyVal > StageMulti.panaltyVal) || Stage1.lose)
            { //2p õ�� ��ġ �� ����, 1p�� õ�� ����� ��
                win2p.SetActive(true);
            }
        }
    }
}
