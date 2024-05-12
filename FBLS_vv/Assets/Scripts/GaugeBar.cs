using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GaugeBar : MonoBehaviour
{
    public Slider gaugeBar; // 게이지
    private int myBlock = 0; // 내 누적블럭
    //private int myBlock = 20;
    public float limitTime; // 제한 시간
    public float goal; // 타이머 시작 개수
    private int enemyBlock = 0; // 상대 누적 블럭
    private int difference; // 누적 블럭 차이
    public int pivot = 50; // 기준(게이지 중간)
    public Text textTime;
    private float timer;
    public float timeRange; // 싱글에서 시간마다 줄어드는 게이지 범위
    public float timerLimit; // 타이머 제한 시간 감소치

    private GameObject stage;
    void InitializedGaugeBar(float time)
    {
        this.timer = time;
    }
    IEnumerator GaugeTimer() // 30초마다 게이지 타이머 제한 시간 2초씩 감소
    {
        
        while (true)
        {
            yield return new WaitForSeconds(30f);
            limitTime -= timerLimit;
            gaugeBar.maxValue -= timerLimit;
        }
    }
    void Start()
    {

        gaugeBar.value = gaugeBar.maxValue;
        StartCoroutine(GaugeTimer());
        InitializedGaugeBar(limitTime);
        
    
    }

    void pan(){
        stage = GameObject.Find("Stage");
        stage.GetComponent<Stage>().doPanalty();
    }
    void Update()
    {
        myBlock = Stage.blockCount;


        // 멀티
        if (SceneManager.GetActiveScene().name == "Computer")
        {
            difference = myBlock - enemyBlock;
            gaugeBar.value = difference + pivot;
            if (difference >= goal)
            { // 타이머 시작
                textTime.gameObject.SetActive(true);

                timer -= Time.deltaTime;
                textTime.text = ((int)timer).ToString();
            }
            if (timer <= 0) // 게이지 초기화
            {
                Stage.blockCount = 0;
                enemyBlock = 0;
                textTime.gameObject.SetActive(false);
                InitializedGaugeBar(limitTime);

            }
        }
        // 싱글
        else if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            gaugeBar.value -= Time.deltaTime;
            timer -= Time.deltaTime;
            textTime.text = ((int)timer).ToString();

            if (gaugeBar.value < gaugeBar.maxValue)
            {
                gaugeBar.value += myBlock;
            }
            if (myBlock >= goal)
            {
                gaugeBar.value = gaugeBar.maxValue;
                InitializedGaugeBar(limitTime);
                Stage.blockCount = 0;
            }
            if (gaugeBar.value <= 0)
            {
                InitializedGaugeBar(limitTime);
                Stage.blockCount = 0;
                gaugeBar.value = gaugeBar.maxValue;
                if (myBlock >= goal)
                {
                    Debug.Log("성공");
                }
                else
                {
                    Debug.Log("실패");
                    pan();
                    // 천장 제거
                }
            }
        }
    }
}