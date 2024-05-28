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

        // 충돌한 오브젝트의 Rigidbody2D가 있고, isKinematic이 false인 경우에만 실행
        if (rb != null && !rb.isKinematic)
        {
            other.transform.parent = transform;
            //UnityEngine.Debug.Log("추가");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 충돌이 끝난 오브젝트를 자식에서 제거
      
            if (other.transform.parent == transform)
            {
                other.transform.parent = null;
               // UnityEngine.Debug.Log("빠이");
            }
        
    }

}
