using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileY : Tile
{
    // Start is called before the first frame update

    protected override void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.isKinematic = true; // Rigidbody의 물리 상호작용을 비활성화
           // UnityEngine.Debug.Log("TileG");
        }
        else
        {
            UnityEngine.Debug.Log("rb 왜없음?");
        }
    }

    protected override void Start()
    {
        base.Start();
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; // Rigidbody의 물리 상호작용을 비활성화
            //UnityEngine.Debug.Log("TileY");
        }
        else
        {
            UnityEngine.Debug.Log("rb 왜없음?");
        }
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void fallReady()
    {
        UnityEngine.Debug.Log("낙하해라Y");
        rb.isKinematic = false;
    }
}
