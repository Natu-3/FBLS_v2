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
    //���ú���� �̹� �ش� ��ũ��Ʈ���� keycode�� �����Ƿ� ���� ������ �ʿ����
    /*public void UpdateLocalBoard(float moveHorizontal, float moveVertical, bool rotate, bool drop, bool action1, bool action2, bool action3, bool action4)
    {
        localBoard.Move(moveHorizontal, moveVertical); 
        if (rotate) localBoard.Rotate();
        if (drop) localBoard.Drop();
        // �߰����� �׼� ó��
        if (action1) localBoard.Action1();
        if (action2) localBoard.Action2();
        if (action3) localBoard.Action3();
        if (action4) localBoard.Action4();
    }*/

    //������ �ֵ鸸 ó��
    public void UpdateRemoteBoard(float moveHorizontal, float moveVertical, bool hold, bool drop, bool action1, bool action2, bool action3, bool action4)
    {
        remoteBoard.Move(moveHorizontal, moveVertical);
        if (hold) remoteBoard.Hold();
        if (drop) remoteBoard.Drop();
        // �߰����� �׼� ó��
        if (action1) remoteBoard.Action1();
        if (action2) remoteBoard.Action2();
        if (action3) remoteBoard.Action3();
        if (action4) remoteBoard.Action4();

        //remoteBoard.SetBlockIndices(currentBlockIndex, currentColorIndex, previewBlockIndex, previewColorIndex);
    }
}