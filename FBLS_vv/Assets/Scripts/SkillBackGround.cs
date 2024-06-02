using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBackGround : MonoBehaviour
{
    public static SkillBackGround Instance;

    Color cr = Color.white;
    public float cl = 120; // 최종 색

    private void Awake()
    {
        SkillBackGround.Instance = this;
    }

    public IEnumerator Transparency(SpriteRenderer render) //색 변경
    {
        cr = render.color;
        for (int i = 0; i < cl; i++)
        {
            cr.g -= 1f / 255f;
            cr.b -= 1f / 255f;
            render.color = cr;
            render = render.GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(0.04f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < cl; i++)
        {
            cr.g += 1f / 255f;
            cr.b += 1f / 255f;
            render.color = cr;
            render = render.GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(0.04f);
        }

    }
}
