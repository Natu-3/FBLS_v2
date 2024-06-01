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
        // delayTime 만큼 대기
        yield return new WaitForSeconds(delayTime);

        // 대기 후에 수행할 행동
        PerformDelayedAction();
    }


    // 지연 후 수행할 특정 행동 정의
    private void PerformDelayedAction()
    {
        UnityEngine.Debug.Log("Action performed after delay");
        // 여기서 원하는 행동을 정의
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
