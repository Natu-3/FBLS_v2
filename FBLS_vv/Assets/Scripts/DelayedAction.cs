using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DelayedAction : MonoBehaviour
{
    private Coroutine delayedCoroutine;


    public void ExecuteAction()
    {
        UnityEngine.Debug.Log("Action started");
        delayedCoroutine = StartCoroutine(DelayedActionCoroutine(5.0f));
    }

    private IEnumerator DelayedActionCoroutine(float delayTime)
    {
        // delayTime ��ŭ ���
        yield return new WaitForSeconds(delayTime);

        // ��� �Ŀ� ������ �ൿ
        PerformDelayedAction();
    }


    // ���� �� ������ Ư�� �ൿ ����
    private void PerformDelayedAction()
    {
        UnityEngine.Debug.Log("Action performed after delay");
        // ���⼭ ���ϴ� �ൿ�� ����
    }



    public void CancelDelayedAction()
    {
        if (delayedCoroutine != null)
        {
            StopCoroutine(delayedCoroutine);
            delayedCoroutine = null;
            UnityEngine.Debug.Log("Delayed action canceled");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
