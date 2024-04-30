using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void InitializedGaugeBar(float timer)
    {
        this.timer = timer;
    }

    // Start is called before the first frame update
    void Start()
    {
        textTime.gameObject.SetActive(false);
        InitializedGaugeBar(limitTime);
        //싱글
        gaugeBar.value = gaugeBar.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        myBlock = Stage.blockCount;

        //멀티
        /*
        difference = myBlock - enemyBlock;
        gaugeBar.value = difference + pivot;
        if (difference >= goal) { // 타이머 시작
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
        */
        //싱글
        if (gaugeBar.value < gaugeBar.maxValue)
        {
            gaugeBar.value += myBlock;
        }
        
        textTime.gameObject.SetActive(true);
        timer -= Time.deltaTime;
        textTime.text = ((int)timer).ToString();
        gaugeBar.value -= Time.deltaTime * timeRange;
        if (timer <= 0 && myBlock >= goal)
        {
            textTime.gameObject.SetActive(false);
            InitializedGaugeBar(limitTime);
            Stage.blockCount = 0;
            gaugeBar.value = gaugeBar.maxValue; 
            Debug.Log("성공");
        }
        else if(timer <= 0)
        {
            textTime.gameObject.SetActive(false);
            InitializedGaugeBar(limitTime);
            Stage.blockCount = 0;
            gaugeBar.value = gaugeBar.maxValue;
            Debug.Log("실패");
        }
        
        
        
       
       
    }
}