using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public StageMulti1 remoteBoard; // 2P

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //로컬보드는 이미 해당 스크립트에서 keycode를 받으므로 별도 조작이 필요없음
    /*public void UpdateLocalBoard(float moveHorizontal, float moveVertical, bool rotate, bool drop, bool action1, bool action2, bool action3, bool action4)
    {
        localBoard.Move(moveHorizontal, moveVertical); 
        if (rotate) localBoard.Rotate();
        if (drop) localBoard.Drop();
        // 추가적인 액션 처리
        if (action1) localBoard.Action1();
        if (action2) localBoard.Action2();
        if (action3) localBoard.Action3();
        if (action4) localBoard.Action4();
    }*/

    //보내줄 애들만 처리
    public void UpdateRemoteBoard(float moveHorizontal, float moveVertical, bool hold, bool drop, bool action1, bool action2, bool action3, bool action4)
    {
        remoteBoard.Move(moveHorizontal, moveVertical);
        if (hold) remoteBoard.Hold();
        if (drop) remoteBoard.Drop();
        // 추가적인 액션 처리
        if (action1) remoteBoard.Action1();
        if (action2) remoteBoard.Action2();
        if (action3) remoteBoard.Action3();
        if (action4) remoteBoard.Action4();

        //remoteBoard.SetBlockIndices(currentBlockIndex, currentColorIndex, previewBlockIndex, previewColorIndex);
    }
}