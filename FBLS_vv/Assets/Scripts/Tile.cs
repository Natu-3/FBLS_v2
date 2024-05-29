using System.Diagnostics;
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
    protected Rigidbody2D rb;

    public bool fall = false; // 낙하 구현용 변수

    public virtual void fallReady()
    {
        fall = true;
        rb.isKinematic = false;
        //UnityEngine.Debug.Log("낙하해라");
        rb.velocity = new Vector3(0, -1, 0);
    }

    public bool isItfall()
    {
        return fall;
    }




    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
           rb.isKinematic = true; // Rigidbody의 물리 상호작용을 비활성화
        }
        else
        {
            UnityEngine.Debug.Log("rb 왜없음?");
        }
        
    }

    protected virtual void Update()
    {
        if (fall)
        {
           Fall();
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (enableCollisionWithSubTiles)
        {
            if (collision.gameObject.CompareTag("TileR") ||
                collision.gameObject.CompareTag("TileG") ||
                collision.gameObject.CompareTag("TileB") ||
                collision.gameObject.CompareTag("TileY"))
            {
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
        if (rb != null && rb.isKinematic)
        {
            rb.isKinematic = false; // Rigidbody의 물리 상호작용을 활성화
            rb.velocity = new Vector3(0, -1, 0);
        }
        
    }

    public void setFall()
    {
        enableCollisionWithSubTiles = false;
    }

    public bool getFall()
    {
        return enableCollisionWithSubTiles;
    }

    public Color color
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

    public void setRed() { tileType = TileType.Red; }
    public void setBlue() { tileType = TileType.Blue; }
    public void setGreen() { tileType = TileType.Green; }
    public void setYellow() { tileType = TileType.Yellow; }
    public string getColor() { return tileType.ToString(); }

    public int sortingOrder
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

    SpriteRenderer spriteRenderer;
    public bool isFire = false;
    public bool isIce = false;

    protected virtual void Awake()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            UnityEngine.Debug.LogError("You need a SpriteRenderer for Block");
        }
    }

    public void EnableRigidbody()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Rigidbody의 물리 상호작용을 활성화
        }
    }
}
/*
public class TileR : Tile
{
    protected override void Start()
    {
        UnityEngine.Debug.Log("빨강생성");
        base.Start();
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Rigidbody의 물리 상호작용을 비활성화
        }
        else
        {
            UnityEngine.Debug.Log("못찾음");
        }
    }
}

public class TileG : Tile
{
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Rigidbody의 물리 상호작용을 비활성화
        }
        else
        {
            UnityEngine.Debug.Log("못찾음");
        }
    }

    protected override void HandleSubTileCollision(GameObject subTile)
    {
        // TileG와 충돌한 하위 블록의 처리
    }

    protected override void Update()
    {
        if (fall)
        {
            Fall();
        }
    }
}

public class TileB : Tile
{
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Rigidbody의 물리 상호작용을 비활성화
        }
        else
        {
            UnityEngine.Debug.Log("못찾음");
        }
    }

    protected override void HandleSubTileCollision(GameObject subTile)
    {
        // TileB와 충돌한 하위 블록의 처리
    }

    protected override void Update()
    {
        if (fall)
        {
            Fall();
        }
    }
}

public class TileY : Tile
{
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Rigidbody의 물리 상호작용을 비활성화
        }
        else
        {
            UnityEngine.Debug.Log("못찾음");
        }
    }

    protected override void HandleSubTileCollision(GameObject subTile)
    {
        // TileY와 충돌한 하위 블록의 처리
    }

    protected override void Update()
    {
        if (fall)
        {
            Fall();
        }
    }
}*/
