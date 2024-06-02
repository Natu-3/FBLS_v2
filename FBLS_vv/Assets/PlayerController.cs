using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    private PhotonView photonView;

    private float moveHorizontal;
    private float moveVertical;
    private bool hold;
    private bool drop;
    private bool action1;
    private bool action2;
    private bool action3;
    private bool action4;
    private int indexVal;
    private int arrIndexVal;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView != null)
        {
            if (photonView.ObservedComponents == null)
            {
                photonView.ObservedComponents = new List<UnityEngine.Component>();
            }
            photonView.ObservedComponents.Add(this);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            bool newMoveHorizontal = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A);
            bool newMoveVertical = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S);
            bool newHold = Input.GetKeyDown(KeyCode.R);
            bool newDrop = Input.GetKeyDown(KeyCode.F);
            bool newAction1 = Input.GetKeyDown(KeyCode.Alpha1);
            bool newAction2 = Input.GetKeyDown(KeyCode.Alpha2);
            bool newAction3 = Input.GetKeyDown(KeyCode.Alpha3);
            bool newAction4 = Input.GetKeyDown(KeyCode.Alpha4);

            if (newMoveHorizontal || newMoveVertical || newHold || newDrop || newAction1 || newAction2 || newAction3 || newAction4)
            {
                moveHorizontal = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
                moveVertical = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
                hold = newHold;
                drop = newDrop;
                action1 = newAction1;
                action2 = newAction2;
                action3 = newAction3;
                action4 = newAction4;

                // 인덱스 값 가져오기
                //indexVal = StageMulti.Instance.indexVal;
                //arrIndexVal = StageMulti.Instance.arrIndexVal;

                // 입력값을 네트워크로 전송
                photonView.RPC("SendInput", RpcTarget.Others, moveHorizontal, moveVertical, hold, drop, action1, action2, action3, action4);

                UnityEngine.Debug.Log($"Local Input: {moveHorizontal}, {moveVertical}, {hold}, {drop}, {action1}, {action2}, {action3}, {action4} ");
            }
        }
    }

    [PunRPC]
    void SendInput(float moveHorizontal, float moveVertical, bool hold, bool drop, bool action1, bool action2, bool action3, bool action4)
    {
        // 원격 플레이어의 입력 처리
        UnityEngine.Debug.Log($"Received Input: {moveHorizontal}, {moveVertical}, {hold}, {drop}, {action1}, {action2}, {action3}, {action4}");
        GameManager.Instance.UpdateRemoteBoard(moveHorizontal, moveVertical, hold, drop, action1, action2, action3, action4);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터를 보냅니다
            stream.SendNext(moveHorizontal);
            stream.SendNext(moveVertical);
            stream.SendNext(hold);
            stream.SendNext(drop);
            stream.SendNext(action1);
            stream.SendNext(action2);
            stream.SendNext(action3);
            stream.SendNext(action4);
            stream.SendNext(indexVal);
            stream.SendNext(arrIndexVal);
        }
        else
        {
            // 데이터를 받습니다
            moveHorizontal = (float)stream.ReceiveNext();
            moveVertical = (float)stream.ReceiveNext();
            hold = (bool)stream.ReceiveNext();
            drop = (bool)stream.ReceiveNext();
            action1 = (bool)stream.ReceiveNext();
            action2 = (bool)stream.ReceiveNext();
            action3 = (bool)stream.ReceiveNext();
            action4 = (bool)stream.ReceiveNext();
            indexVal = (int)stream.ReceiveNext();
            arrIndexVal = (int)stream.ReceiveNext();
        }
    }
}
