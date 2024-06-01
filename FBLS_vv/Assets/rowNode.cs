using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rowNode : MonoBehaviour
{
    public bool coli = false;
    private Dictionary<Collider2D, Coroutine> coroutines = new Dictionary<Collider2D, Coroutine>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        // 충돌한 오브젝트의 Rigidbody2D가 있고, isKinematic이 false인 경우에만 실행
        if (rb != null && !rb.isKinematic)
        {
            if (coroutines.ContainsKey(other))
            {
                StopCoroutine(coroutines[other]);
                coroutines.Remove(other);
            }

            Coroutine coroutine = StartCoroutine(WaitAndAddChild(other));
            coroutines.Add(other, coroutine);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 충돌이 끝난 오브젝트를 자식에서 제거
        if (coroutines.ContainsKey(other))
        {
            StopCoroutine(coroutines[other]);
            coroutines.Remove(other);
        }

        if (other.transform.parent == transform)
        {
            other.transform.parent = null;
            //UnityEngine.Debug.Log("빠이");
        }
    }

    private IEnumerator WaitAndAddChild(Collider2D other)
    {
        yield return new WaitForSeconds(0.5f);

        if (other != null && other.transform.parent != transform)
        {
            other.transform.parent = transform;
            //UnityEngine.Debug.Log("추가");
        }

        coroutines.Remove(other);
    }
}