using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [HideInInspector] public float attackTime;
    [HideInInspector] public bool isAttackCanceled;
    public BlockPosition block;
    public Text red; // ����� ��
    public Text green; // ����� ��
    public Text blue; // ����� ��
    public Text yellow; // ����� ��
    public Button redActive;
    public Button blueActive;
    public Button yellowActive;
    public Button greenActive;
    public int maxBlock;
    public void attack()
    {
        isAttackCanceled = false;
        attackTime = Time.time;
    }
    /*
    public void updateBlock()
    {
        red.text = $"{block.redVal}/{maxBlock}"; //�� ���� ���
        green.text = $"{block.greenVal}/{maxBlock}"; // �� ���� ���
        blue.text = $"{block.blueVal}/{maxBlock}"; // �� ���� ���
        yellow.text = $"{block.yellowVal}/{maxBlock}"; // �� ���� ���
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        //updateBlock();
    }

    // Update is called once per frame
    void Update()
    {

        /*
        if (block.redVal == maxBlock)
        {
            redActive.interactable = true; // ��ư ��Ÿ����
            block.redVal = 0; // ���� �ʱ�ȭ
        }
        if (block.blueVal == maxBlock)
        {
            blueActive.interactable = true; // ��ư ��Ÿ����
            block.blueVal = 0;
        }
        if (block.greenVal == maxBlock)
        {
            greenActive.interactable = true; // ��ư ��Ÿ����
            block.greenVal = 0;
        }
        if (block.yellowVal == maxBlock)
        {
            yellowActive.interactable = true; // ��ư ��Ÿ����
            block.yellowVal = 0;
        }
        */
    }
}
