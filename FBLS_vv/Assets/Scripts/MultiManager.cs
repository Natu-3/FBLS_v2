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
    [Header("2p")]
    public GameObject warningPanel2; // 경고판
    public GameObject warningRed2; //텍스트
    public GameObject warningBlue2;
    public GameObject warningYellow2;
    public GameObject cancelSkill2;
    public GameObject warningImage2;
    private float skillTimer;
    private bool isSkillActive = false;
 

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
    }

    public void AtkRed1()
    {
        SetAttack(1, "R");
        ActivePanel(warningRed2);
    }

    public void AtkRed2()
    {
        SetAttack(2, "R");
        UnityEngine.Debug.Log("Red skill on");
        ActivePanel(warningRed1);
    }

    public void AtkBlue1()
    {
        SetAttack(1, "B");
        ActivePanel(warningBlue2);
    }

    public void AtkBlue2()
    {
        UnityEngine.Debug.Log("Blue skill on");
        SetAttack(2, "B");
        ActivePanel(warningBlue1);
    }

    public void AtkYellow1()
    {
        SetAttack(1, "Y");
        ActivePanel(warningYellow2);
    }

    public void AtkYellow2()
    {
        SetAttack(2, "Y");
        UnityEngine.Debug.Log("Yellow skill on");
        ActivePanel(warningYellow1);
    }

    public void Green1()
    {
        green1p = true;
        CancelCurrentSkill();
        ActivePanel(cancelSkill2);
        warningImage2.SetActive(false);
    }

    public void Green2()
    {
        green2p = true;
        CancelCurrentSkill();
        UnityEngine.Debug.Log("green skill on");
        ActivePanel(cancelSkill1);
        warningImage1.SetActive(false);
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
        if (atkPlayer == 1)
        {
            // player1의 스킬 수행 로직
        }
        else if (atkPlayer == 2)
        {
            if (!green2p)
            {
                StageMulti multi = player2.GetComponent<StageMulti>();
                switch (skillStatus)
                {
                    case "R":
                        multi.redSkill();
                        break;
                    case "B":
                        multi.blueSkill();
                        break;
                    case "Y":
                        multi.yellowSkill();
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
}