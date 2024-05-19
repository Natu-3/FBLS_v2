using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public int redSkillNum; // 적용시킬 블럭 개수
    public int blueSkillNum; // 적용시킬 블럭 개수
    public int yellowSkillNum; // 적용시킬 블럭 개수
    public string[,] grid;
    private StageMulti stage;
    private string black = Color.black.ToString();
    private int maxFilledWidth = 0;
    private int maxFilledHeight = 0;
    public bool[,] checkBlue = new bool[10,20]; // 얼려있는 블럭
    public Image lightening;
    public float fadeInImage = 0.1f; // 이미지 나타나는 시간
    public float fadeOutImage = 1.01f; // 이미지 사라지는 시간
    private SkillManager manager;
    public float warningTime = 2f; // 경고 제한 시간
    private static BlockPosition block;

    private void checkBlock()
    {
        grid = block.grid;
        for (int y = 0; y < stage.boardHeight; y++)
        {
            for (int x = 0; x < stage.boardWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    maxFilledWidth = System.Math.Max(maxFilledWidth, x);
                    maxFilledHeight = System.Math.Max(maxFilledHeight, y);
                }
            }
        }
    }
    public void redSkill()
    {
        checkBlock();
        int i = 0;
        while (i < redSkillNum)
        {
            int x = Random.Range(0, maxFilledWidth);
            int y = Random.Range(0, maxFilledHeight);

            if (grid[x, y] != null && grid[x, y] != black)
            {
                grid[x, y] = black;
                i++;
            }
        }
    }

    public void blueSkill()
    {
        checkBlock();
        int i = 0;
        while (i < blueSkillNum)
        {
            int x = Random.Range(0, maxFilledWidth);
            int y = Random.Range(0, maxFilledHeight);

            if (grid[x, y] != null && !checkBlue[x, y])
            {
                checkBlue[x, y] = true;
                i++;
            }
        }
    }
    public void yellowSkill()
    {
        StartCoroutine(yellowSkillActive());
    }
    IEnumerator yellowSkillActive()
    {
        float timer = 0;
        while (timer <= fadeInImage) // fade in
        {
            float alpha = Mathf.Lerp(0, 1, timer / fadeInImage);
            lightening.color = new Color(1, 1, 1, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        lightening.color = Color.white;
        checkBlock();
        int i = 0;
        while (i < yellowSkillNum) // 랜덤 블럭 삭제
        {
            int x = Random.Range(0, maxFilledWidth);
            int y = Random.Range(0, maxFilledHeight);

            if (grid[x, stage.boardHeight-y] != null)
            {
                GameObject row = GameObject.Find("y_" + (stage.boardHeight-y).ToString());
                string blockName = "x_" + x.ToString();
                Transform blockTransform = row.transform.Find(blockName);
                Destroy(blockTransform.gameObject);
                i++;
            }
        }
        timer = 0;
        while (timer <= fadeOutImage) // fade out
        {
            float alpha = Mathf.Lerp(1, 0, timer / fadeOutImage);
            lightening.color = new Color(1, 1, 1, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        lightening.color = new Color(1, 1, 1, 0); // 화면 사라짐

    }
    public void greenSkill()
    {
        Coroutine attackCoroutine = StartCoroutine(attackDelay(warningTime));
        float attackTime = manager.attackTime;
        if(attackCoroutine != null)
        {
            manager.isAttackCanceled = true;
            StopCoroutine(attackCoroutine);
        }

    }
    IEnumerator attackDelay(float warningTime)
    {
        yield return new WaitForSeconds(warningTime);
        if (!manager.isAttackCanceled)
        {
            manager.attack();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
