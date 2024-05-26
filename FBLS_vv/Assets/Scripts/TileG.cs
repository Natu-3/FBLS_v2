using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileG : Tile
{
    // Start is called before the first frame update



    protected override void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.isKinematic = true; // Rigidbody�� ���� ��ȣ�ۿ��� ��Ȱ��ȭ
            //UnityEngine.Debug.Log("TileG");
        }
        else
        {
            UnityEngine.Debug.Log("rb �־���?");
        }
    }

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void fallReady()
    {
        UnityEngine.Debug.Log("�����ض�G");
        rb.isKinematic = false;
    }
}
