using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class SkillBackGround : MonoBehaviour
{
    public static SkillBackGround Instance;

    Color cr = Color.white;
    public float cl = 120; // 최종 색
    MultiManager MultiManager;

    private void Awake()
    {
        SkillBackGround.Instance = this;
        MultiManager = new MultiManager();
    }

    public IEnumerator SunBackGround(SpriteRenderer render) //태양 배경 색 변경
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

    public IEnumerator SnowBackGround(SpriteRenderer render) //눈 배경 색 변경
    {
        cr = render.color;
        for (int i = 0; i < cl; i++)
        {
            cr.g -= 1f / 255f;
            cr.r -= 1f / 255f;
            render.color = cr;
            render = render.GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(0.04f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < cl; i++)
        {
            cr.g += 1f / 255f;
            cr.r += 1f / 255f;
            render.color = cr;
            render = render.GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(0.04f);
        }
    }

    public IEnumerator Twinkle(SpriteRenderer render) //번개 화면 반짝
    {
        render.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.3f);
        cr = render.color;

        for (int i = 0; i < 2; i++)
        {
            float timer = 0;
            while (timer <= 0.1f) // fade in
            {
                float alpha = Mathf.Lerp(0, 1, timer / 0.1f);
                render.color = new Color(1, 1, 1, alpha);
                timer += Time.deltaTime;
                yield return null;
            }
            render.color = Color.white;

            timer = 0;
            while (timer <= 1.01f) // fade out
            {
                float alpha = Mathf.Lerp(1, 0, timer / 1.01f);
                render.color = new Color(1, 1, 1, alpha);
                timer += Time.deltaTime;
                yield return null;
            }
            render.color = new Color(1, 1, 1, 0); // 화면 사라짐
        }

        yield return new WaitForSeconds(0.5f);
        render.color = Color.black;

        yield return new WaitForSeconds(1f);

        cr = render.color;

        while (render.color.a > 0f)
        {
            cr.a -= 7f / 255f;
            render.color = cr;
            render = render.GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
