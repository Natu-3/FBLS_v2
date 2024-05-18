
using UnityEngine;
public enum TileType
{
    Red,
    Blue,
    Green,
    Yellow
}
public class Tile : MonoBehaviour
{
    
    public TileType tileType;
    protected bool enableCollisionWithSubTiles = false;
    private Rigidbody rb;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        // enableCollisionAndFall가 true일 때만 충돌 판정 및 낙하 동작 실행
        if (enableCollisionWithSubTiles)
        {
            Fall();
        }
    }


    protected virtual void OnCollisionEnter(Collision collision)
    {
        // 하위 블록들과의 충돌 판정을 활성화한 경우에만 실행
        if (enableCollisionWithSubTiles)
        {
            // 충돌한 오브젝트가 TileR, TileG, TileB, TileY 중 하나인지 확인
            if (collision.gameObject.CompareTag("TileR") ||
                collision.gameObject.CompareTag("TileG") ||
                collision.gameObject.CompareTag("TileB") ||
                collision.gameObject.CompareTag("TileY"))
            {
                // 충돌한 블록 처리
                HandleSubTileCollision(collision.gameObject);
            }
        }
    }

    protected virtual void HandleSubTileCollision(GameObject subTile)
    {
        // 하위 클래스에서 구현
    }

    protected void Fall()
    {
        // Rigidbody를 이용하여 y 방향으로 낙하
        rb.velocity = new Vector3(0, -1, 0);
    }


    public void setFall()
    {
        enableCollisionWithSubTiles = false;
    }

    public bool getFall()
    {
        return enableCollisionWithSubTiles;
    }

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

    public void setRed() { tileType = TileType.Red; }
    public void setBlue() { tileType = TileType.Blue; }
    public void setGreen() { tileType = TileType.Green; }
    public void setYellow() { tileType = TileType.Yellow; }
    public string getColor() { return tileType.ToString();}
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
            isFire = false;
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


    public bool isred()
    {
        return isFire;
    }

    public bool getice()
    {
        return isIce;
    }

    SpriteRenderer spriteRenderer;  //스프라이트 렌더러 선언
    public bool isFire = false;
    public bool isIce = false;
    private void Awake()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("You need to SpriteRenderer for Block");
        }
    }
}



public class TileR : Tile
{
    protected override void Start()
    {
        base.Start();
        // 하위 블록들과의 충돌 판정을 활성화
        enableCollisionWithSubTiles = true;
    }

    protected override void HandleSubTileCollision(GameObject subTile)
    {
        // TileR과 충돌한 하위 블록의 처리
    }

    protected override void Update()
    {
        // enableCollisionAndFall가 true일 때만 충돌 판정 및 낙하 동작 실행
       
            Fall();
        
    }
}