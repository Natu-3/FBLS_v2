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
    }

    public void AtkRed2()
    {
        SetAttack(2, "R");
        UnityEngine.Debug.Log("Red skill on");
    }

    public void AtkBlue1()
    {
        SetAttack(1, "B");
    }

    public void AtkBlue2()
    {
        UnityEngine.Debug.Log("Blue skill on");
        SetAttack(2, "B");
    }

    public void AtkYellow1()
    {
        SetAttack(1, "Y");
    }

    public void AtkYellow2()
    {
        SetAttack(2, "Y");
        UnityEngine.Debug.Log("Yellow skill on");
    }

    public void Green1()
    {
        green1p = true;
        CancelCurrentSkill();
    }

    public void Green2()
    {
        green2p = true;
        CancelCurrentSkill();
        UnityEngine.Debug.Log("green skill on");
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
    }
}