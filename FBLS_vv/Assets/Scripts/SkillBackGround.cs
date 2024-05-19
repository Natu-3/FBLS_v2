using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBackGround : MonoBehaviour
{
    public static SkillBackGround Instance;
    Color cr;
    public float colorA = 145; // 최종 투명도

    private void Awake()
    {
        SkillBackGround.Instance = this;
    }

    public IEnumerator Transparency(Renderer render) //투명도 변경
    {
        cr = render.GetComponent<Renderer>().material.color;
        for (int i = 0; i < colorA; i++)
        {
            yield return new WaitForSeconds(0.01f);
            cr.a += 1f;
            render.GetComponent<Renderer>().material.color = cr;
        }
        yield return new WaitForSeconds(4f);
        for (int i = 0; i < colorA; i++)
        {
            yield return new WaitForSeconds(0.01f);
            cr.a -= 1f;
            render.GetComponent<Renderer>().material.color = cr;
        }

    }

}
