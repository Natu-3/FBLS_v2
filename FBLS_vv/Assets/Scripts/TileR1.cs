using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileR1 : Tile
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Rigidbody�� ���� ��ȣ�ۿ��� ��Ȱ��ȭ
        }
    }

    
}
