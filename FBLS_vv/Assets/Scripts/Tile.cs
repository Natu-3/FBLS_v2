using UnityEngine;

public class Tile : MonoBehaviour
{
    public Color color //색상 클래스
    {
        set
        {
            spriteRenderer.color = value;  //입력한 값으로 초기화
        }

        get
        {
            return spriteRenderer.color; // 색상값 돌려줌
        }
    }

    public int sortingOrder // 정렬순서
    {
        set
        {
            spriteRenderer.sortingOrder = value;
        }

        get
        {
            return spriteRenderer.sortingOrder;
        }
    }
    public bool isFired
    {
        set
        {
            isFire = value;
            Color fire = Color.black;
            spriteRenderer.color = fire;
        }
        get
        {
            return isFire;
        }
    }
    public bool isIced
    {
        set
        {
            isIce = value;
            Color ice = Color.white;
            spriteRenderer.color = ice;
            UnityEngine.Debug.Log("ice");
        }
        get
        {
            return isIce;
        }
    }
    public Color preColor
    {
        set
        {
            spriteRenderer.color = value;
        }
        get
        {
            return spriteRenderer.color;
        }
    }
    SpriteRenderer spriteRenderer;  //스프라이트 렌더러 선언
    bool isFire = false;
    bool isIce = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("You need to SpriteRenderer for Block");
        }
    }
}
