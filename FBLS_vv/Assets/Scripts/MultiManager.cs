using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MultiManager : MonoBehaviour
{
    public static MultiManager Instance = null;
    public GameObject player1;
    public GameObject player2;

    public float limitTime = 5f;
    public string skillStatus = "";
    public bool green1p = false;
    public bool green2p = false;
    public int atkPlayer;
    
    [Header("1p")]
    public GameObject warningPanel1; // 경고판
    public GameObject warningRed1; //텍스트
    public GameObject warningBlue1;
    public GameObject warningYellow1;
    public GameObject cancelSkill1;
    public GameObject warningImage1;
    public GameObject redButton1; //버튼
    public GameObject blueButton1;
    public GameObject yellowButton1;
    public GameObject greenButton1;
    public GameObject sun_1;
    public GameObject rain_1;
    public GameObject snow_1;
    public SpriteRenderer backGround_1;
    public Transform lightningTransform_1;

    [Header("2p")]
    public GameObject warningPanel2; // 경고판
    public GameObject warningRed2; //텍스트
    public GameObject warningBlue2;
    public GameObject warningYellow2;
    public GameObject cancelSkill2;
    public GameObject warningImage2;
    public GameObject redButton2; //버튼
    public GameObject blueButton2;
    public GameObject yellowButton2;
    public GameObject greenButton2;
    public GameObject sun_2;
    public GameObject rain_2;
    public GameObject snow_2;
    public SpriteRenderer backGround_2;
    public Transform lightningTransform_2;

    private float skillTimer;
    private bool isSkillActive = false;
    Coroutine abc_1;
    Coroutine abc_2;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (isSkillActive)
        {
            skillTimer -= Time.deltaTime;
            if (skillTimer <= 0)
            {
                PerformSkill();
                isSkillActive = false;
            }
        }
        ActiveButton();
    }

    public void AtkRed1()
    {
        SetAttack(1, "R");
        ActivePanel(warningRed2);
        redButton1.SetActive(false);
        StartCoroutine(EffectManager.instance.WeatherEffect(sun_2));
        abc_2 = StartCoroutine(SkillBackGround.Instance.Transparency(backGround_2));
    }

    public void AtkRed2()
    {
        SetAttack(2, "R");
        UnityEngine.Debug.Log("Red skill on");
        ActivePanel(warningRed1);
        redButton2.SetActive(false);
        StartCoroutine(EffectManager.instance.WeatherEffect(sun_1));
        abc_1 = StartCoroutine(SkillBackGround.Instance.Transparency(backGround_1));
    }

    public void AtkBlue1()
    {
        SetAttack(1, "B");
        ActivePanel(warningBlue2);
        blueButton1.SetActive(false);
        StartCoroutine(EffectManager.instance.WeatherEffect(rain_2));
        StartCoroutine(EffectManager.instance.Lightning(lightningTransform_2));
    }

    public void AtkBlue2()
    {
        UnityEngine.Debug.Log("Blue skill on");
        SetAttack(2, "B");
        ActivePanel(warningBlue1);
        blueButton2.SetActive(false);
        StartCoroutine(EffectManager.instance.WeatherEffect(rain_1));
        StartCoroutine(EffectManager.instance.Lightning(lightningTransform_1));

    }

    public void AtkYellow1()
    {
        SetAttack(1, "Y");
        ActivePanel(warningYellow2);
        yellowButton1.SetActive(false);
        StartCoroutine(EffectManager.instance.WeatherEffect(snow_2));
    }

    public void AtkYellow2()
    {
        SetAttack(2, "Y");
        UnityEngine.Debug.Log("Yellow skill on");
        ActivePanel(warningYellow1);
        yellowButton2.SetActive(false);
        StartCoroutine(EffectManager.instance.WeatherEffect(snow_1));
    }

    public void Green1()
    {
        green1p = true;
        CancelCurrentSkill();
        ActivePanel(cancelSkill2);
        warningImage2.SetActive(false);
        greenButton1.SetActive(false);
        snow_1.gameObject.SetActive(false);
        sun_1.gameObject.SetActive(false);
        rain_1.gameObject.SetActive(false);
        StopCoroutine(abc_1);
        backGround_1.color = Color.white;
        backGround_1 = backGround_1.GetComponent<SpriteRenderer>();
    }

    public void Green2()
    {
        green2p = true;
        CancelCurrentSkill();
        UnityEngine.Debug.Log("green skill on");
        ActivePanel(cancelSkill1);
        warningImage1.SetActive(false);
        greenButton2.SetActive(false);
        snow_2.gameObject.SetActive(false);
        sun_2.gameObject.SetActive(false);
        rain_2.gameObject.SetActive(false);
        StopCoroutine(abc_2);
        backGround_2.color = Color.white;
        backGround_2 = backGround_2.GetComponent<SpriteRenderer>();
    }

    private void SetAttack(int player, string skill)
    {
        atkPlayer = player;
        skillStatus = skill;
        skillTimer = limitTime;
        isSkillActive = true;
    }

    private void PerformSkill()
    {
        UnityEngine.Debug.Log("스킬적용!!!");
        if (atkPlayer == 2)
        {
            if(!green1p){
                Stage1 p1 = player1.GetComponent<Stage1>();
  
                switch (skillStatus)
                    {
                        case "R":
                            p1.redSkill(5);
                            break;
                        case "B":
                            p1.blueSkill(5);
                            break;
                        case "Y":
                            p1.yellowSkill(5);
                            break;
                        default:
                            break;
                    }
                green2p = false;
            }
            atkPlayer = 0;
        skillTimer = limitTime;
        }
        else if (atkPlayer == 1)
        {
            if (!green2p)
            {
                StageMulti multi = player2.GetComponent<StageMulti>();
                switch (skillStatus)
                {
                    case "R":
                        multi.redSkill(5);
                        break;
                    case "B":
                        multi.blueSkill(5);
                        break;
                    case "Y":
                        multi.yellowSkill(5);
                        break;
                    default:
                        break;
                }
            }
            green2p = false;
        }
        atkPlayer = 0;
        skillTimer = limitTime;
    }

    private void CancelCurrentSkill()
    {
        isSkillActive = false;
        UnityEngine.Debug.Log("Current skill action canceled due to green state.");

        currentSkill.SetActive(false); // 막은 스킬 경고패널 비활성화
    }

    //경고판
    public void ActivePanel(GameObject skillText)
    {
        warningPanel1.SetActive(true);
        skillText.SetActive(true);
        currentSkill = skillText;
        Invoke("HidePanel", 5f);
    }
    private GameObject currentSkill;
    public void HidePanel()
    {
        warningPanel1.SetActive(false);
        currentSkill.SetActive(false);
    }

    //버튼
    public void ActiveButton()
    {
        //1p
        if (Stage1.redVal >= 0 && !redButton1.activeSelf)
        {
            redButton1.SetActive(true);
            Stage1.redVal = 0;
        }
        if (Stage1.blueVal >= 0 && !blueButton1.activeSelf)
        {
            blueButton1.SetActive(true);
            Stage1.blueVal = 0;
        }
        if (Stage1.yellowVal >= 0 && !yellowButton1.activeSelf)
        {
            yellowButton1.SetActive(true);
            Stage1.yellowVal = 0;
        }
        if (Stage1.greenVal >= 0 && !greenButton1.activeSelf)
        {
            greenButton1.SetActive(true);
            Stage1.greenVal = 0;
        }
        //2p
        if (StageMulti.redVal >= 0 && !redButton2.activeSelf)
        {
            redButton2.SetActive(true);
            StageMulti.redVal = 0;
            UnityEngine.Debug.Log(StageMulti.redVal.ToString());
        }
        if (StageMulti.blueVal >= 0 && !blueButton2.activeSelf)
        {
            blueButton2.SetActive(true);
            StageMulti.blueVal = 0;
            UnityEngine.Debug.Log(StageMulti.blueVal.ToString());
        }
        if (StageMulti.yellowVal >= 0 && !yellowButton2.activeSelf)
        {
            yellowButton2.SetActive(true);
            StageMulti.yellowVal = 0;
            UnityEngine.Debug.Log(StageMulti.yellowVal.ToString());
        }
        if (StageMulti.greenVal >= 0 && !greenButton2.activeSelf)
        {
            greenButton2.SetActive(true);
            StageMulti.greenVal = 0;
            UnityEngine.Debug.Log(StageMulti.greenVal.ToString());
        }
    }
}