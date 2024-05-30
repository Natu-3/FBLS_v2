using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public int penaltyBlock; // �г�Ƽ ������ ����� ���� �� ����
    public Image penaltyZone; // �г�Ƽ ��
    private GameObject stage;

    public GameObject player1;
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

        // ��Ƽ
        if (SceneManager.GetActiveScene().name == "MultiScene")
        {

            //2p����
            myBlock = StageMulti.blockCount;
            enemyBlock = Stage1.blockCount;
            difference = enemyBlock - myBlock;
            gaugeBar.value = -difference + pivot;
            if (difference >= penaltyBlock) // Ÿ�̸� ����
            {
                
                textTime.gameObject.SetActive(true);
                penaltyZone.gameObject.SetActive(true);
                timer -= Time.deltaTime;
                textTime.text = ((int)timer).ToString();

            }

            if (timer <= 0) // Ÿ�̸� ���� �� �г�Ƽ
            {
                StageMulti.blockCount = 0;
                Stage1.blockCount = 0;
                textTime.gameObject.SetActive(false);
                InitializedGaugeBar(limitTime);
                pan();
            }
            if (timer >= 0 && difference <= penaltyBlock) // �ð� �ȿ� ����Ƽ ���� �Ѱ��� ��
            {
                textTime.gameObject.SetActive(false);
                InitializedGaugeBar(limitTime);
            }

        }
        // �̱�
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
                    Debug.Log("����");
                }
                else
                {
                    Debug.Log("����");
                    pan();
                    // õ�� ����
                }
            }
        }
    }
}