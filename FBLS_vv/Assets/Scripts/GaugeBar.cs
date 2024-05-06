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
    public float timerLimit; // Ÿ�̸� ���� �ð� ����ġ

    void InitializedGaugeBar(float time)
    {
        this.timer = time;
    }
    IEnumerator GaugeTimer() // 30�ʸ��� ������ Ÿ�̸� ���� �ð� 2�ʾ� ����
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            limitTime -= timerLimit;
        }
    }
    void Start()
    {
        InitializedGaugeBar(limitTime);
        //�̱�
        gaugeBar.value = gaugeBar.maxValue;
        StartCoroutine(GaugeTimer());
    }


    void Update()
    {
        myBlock = Stage.blockCount;
        gaugeBar.value -= Time.deltaTime * timeRange;

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
        if (timer <= 0)
        {
            InitializedGaugeBar(limitTime);
            Stage.blockCount = 0;
            gaugeBar.value = gaugeBar.maxValue;
            if (myBlock >= goal)
            {
                Debug.Log("����");
            }
            else
            {
                Debug.Log("����");
            }
        }
    }
}