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

        // �浹�� ������Ʈ�� Rigidbody2D�� �ְ�, isKinematic�� false�� ��쿡�� ����
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
        // �浹�� ���� ������Ʈ�� �ڽĿ��� ����
        if (coroutines.ContainsKey(other))
        {
            StopCoroutine(coroutines[other]);
            coroutines.Remove(other);
        }

        if (other.transform.parent == transform)
        {
            other.transform.parent = null;
            //UnityEngine.Debug.Log("����");
        }
    }

    private IEnumerator WaitAndAddChild(Collider2D other)
    {
        yield return new WaitForSeconds(0.5f);

        if (other != null && other.transform.parent != transform)
        {
            other.transform.parent = transform;
            //UnityEngine.Debug.Log("�߰�");
        }

        coroutines.Remove(other);
    }
}