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
    public int penaltyBlock; // 패널티 존에서 벗어나기 위한 블럭 개수
    public Image penaltyZone; // 패널티 존
    private GameObject stage;

    public GameObject player1;
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
        textTime.gameObject.SetActive(false);
        gaugeBar.value = gaugeBar.maxValue;
        StartCoroutine(GaugeTimer());
        InitializedGaugeBar(limitTime);
    }

    void pan(){
        
        //var panaltys = player1.GetComponent<Stage>();
        //panaltys.doPanalty();
    }
    void Update()
    {

        // 멀티
        if (SceneManager.GetActiveScene().name == "MultiScene")
        {

            //2p기준
            myBlock = StageMulti.blockCount;
            enemyBlock = Stage1.blockCount;
            difference = enemyBlock - myBlock;
            gaugeBar.value = -difference + pivot;
            if (difference >= penaltyBlock) // 타이머 시작
            {
                
                textTime.gameObject.SetActive(true);
                penaltyZone.gameObject.SetActive(true);
                timer -= Time.deltaTime;
                textTime.text = ((int)timer).ToString();

            }

            if (timer <= 0) // 타이머 종료 후 패널티
            {
                StageMulti.blockCount = 0;
                Stage1.blockCount = 0;
                textTime.gameObject.SetActive(false);
                InitializedGaugeBar(limitTime);
                pan();
            }
            if (timer >= 0 && difference <= penaltyBlock) // 시간 안에 패털티 구간 넘겼을 때
            {
                textTime.gameObject.SetActive(false);
                InitializedGaugeBar(limitTime);
            }

        }
        // 싱글
        else if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            myBlock = Stage.blockCount;
            gaugeBar.value -= Time.deltaTime;
            timer -= Time.deltaTime;
            //textTime.text = ((int)timer).ToString();

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