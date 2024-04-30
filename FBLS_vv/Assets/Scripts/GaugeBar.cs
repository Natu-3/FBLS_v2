using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    public Slider gaugeBar; // ������
    private int myBlock = 0; // �� ������
    //private int myBlock = 20;
    public float limitTime; // ���� �ð�
    public float goal; // Ÿ�̸� ���� ����
    private int enemyBlock = 0; // ��� ���� ��
    private int difference; // ���� �� ����
    public int pivot = 50; // ����(������ �߰�)
    public Text textTime;
    private float timer;
    public float timeRange; // �̱ۿ��� �ð����� �پ��� ������ ����

    void InitializedGaugeBar(float timer)
    {
        this.timer = timer;
    }

    // Start is called before the first frame update
    void Start()
    {
        textTime.gameObject.SetActive(false);
        InitializedGaugeBar(limitTime);
        //�̱�
        gaugeBar.value = gaugeBar.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        myBlock = Stage.blockCount;

        //��Ƽ
        /*
        difference = myBlock - enemyBlock;
        gaugeBar.value = difference + pivot;
        if (difference >= goal) { // Ÿ�̸� ����
            textTime.gameObject.SetActive(true);

            timer -= Time.deltaTime;
            textTime.text = ((int)timer).ToString();
        }
        if (timer <= 0) // ������ �ʱ�ȭ
        {
            Stage.blockCount = 0;
            enemyBlock = 0;
            textTime.gameObject.SetActive(false);
            InitializedGaugeBar(limitTime);

        }
        */
        //�̱�
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
            Debug.Log("����");
        }
        else if(timer <= 0)
        {
            textTime.gameObject.SetActive(false);
            InitializedGaugeBar(limitTime);
            Stage.blockCount = 0;
            gaugeBar.value = gaugeBar.maxValue;
            Debug.Log("����");
        }
        
        
        
       
       
    }
}