using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class rowNode : MonoBehaviour
{
    public bool coli = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        // �浹�� ������Ʈ�� Rigidbody2D�� �ְ�, isKinematic�� false�� ��쿡�� ����
        if (rb != null && !rb.isKinematic)
        {
            other.transform.parent = transform;
            //UnityEngine.Debug.Log("�߰�");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // �浹�� ���� ������Ʈ�� �ڽĿ��� ����
      
            if (other.transform.parent == transform)
            {
                other.transform.parent = null;
               // UnityEngine.Debug.Log("����");
            }
        
    }

}
