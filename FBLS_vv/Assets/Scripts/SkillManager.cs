using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [HideInInspector] public float attackTime;
    [HideInInspector] public bool isAttackCanceled;
    public BlockPosition block;
    public Text red; // 사라진 블럭
    public Text green; // 사라진 블럭
    public Text blue; // 사라진 블럭
    public Text yellow; // 사라진 블럭
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
        red.text = $"{block.redVal}/{maxBlock}"; //블럭 개수 출력
        green.text = $"{block.greenVal}/{maxBlock}"; // 블럭 개수 출력
        blue.text = $"{block.blueVal}/{maxBlock}"; // 블럭 개수 출력
        yellow.text = $"{block.yellowVal}/{maxBlock}"; // 블럭 개수 출력
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
            redActive.interactable = true; // 버튼 나타나게
            block.redVal = 0; // 개수 초기화
        }
        if (block.blueVal == maxBlock)
        {
            blueActive.interactable = true; // 버튼 나타나게
            block.blueVal = 0;
        }
        if (block.greenVal == maxBlock)
        {
            greenActive.interactable = true; // 버튼 나타나게
            block.greenVal = 0;
        }
        if (block.yellowVal == maxBlock)
        {
            yellowActive.interactable = true; // 버튼 나타나게
            block.yellowVal = 0;
        }
        */
    }
}
