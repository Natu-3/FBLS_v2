using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.CodeDom;
using System.Linq.Expressions;


using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel.Design;

using System.Security;
using System.Net.Sockets;
using System.Collections.Concurrent;
using static System.Net.Mime.MediaTypeNames;
using Text = UnityEngine.UI.Text;
using System.Security.Principal;
using TMPro;


public class StageMulti1 : MonoBehaviour
{
    [Header("Editor Objects")]
    public GameObject nodePrefab; //���(������) ������
    public GameObject tilePrefab; //Ÿ�������� �ҷ���
    public GameObject tileRed;
    public GameObject tileBlue;
    public GameObject tileGreen;
    public GameObject tileYellow;
    public GameObject sevenBag;

    public Transform backgroundNode; // ��׶��� 
    public Transform boardNode; //������(�� �� y0 - y19������ ���)
   

    public Transform tetrominoNode; //��Ʈ���̳�
                                    // public GameObject gameoverPanel; //���ӿ���
    public TextMeshProUGUI score; // ����
   

    public Transform preview; // ���� ��
    public GameObject start;
    private Skill skill;
    private SkillManager skillManager;

    public BlockPosition blockPos; // �� ����ü
    public static bool lose = false; //õ�忡 ��Ҵ���
    public Transform ghostNode;

    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 10;
    [Range(5, 20)]
    public int boardHeight = 20;
    public float fallCycle = 1.0f;

    private int halfWidth; // ��ǥ (����)�߾Ӱ�
    private int halfHeight; //��ǥ (����) �߾Ӱ�
    public int lineWeight; // ������ �� ����
    public int colorWeight; // ������ �� ����
    public int panalty = 0; // �г�Ƽ�� ����� ����ġ
    public int indexback = 0;
    public static int panaltyVal = 0;


    


    public float offset_x = 0f;
    public float offset_y = 0f;






    private float nextFallTime;
    public static int scoreVal = 0;
   
    public static int redVal = 0; // ����� �� ����
    public static int greenVal = 0; // ����� �� ����
    public static int blueVal = 0;   // ����� �� ����
    public static int yellowVal = 0; // ����� �� ����
    public static int blockCount = 0;




    [Header("Node")] // �̸����� Ȧ�� ������ ���� ���� ������
    public int offset2p = 14;


    public List<int> prevIndex;
    public List<int> prevarr;

    private int holdIndex = -1;
    private int holdArrIndex = -1;
    private bool hasHeld = false;
    public Transform holdNode;
    private int indexVal = -1;
    private int arrIndexVal = -1;


    public int currentBlockIndex = -1;
    public int currentColorIndex = -1;
    public int previewBlockIndex = -1;
    public int previewColorIndex = -1;

    private int previndexVal = -1;
    private int prevarrIndexVal = -1;

    public string garim = "Assets / Images / garim.png";

    private bool coderight= false;
    private bool codeleft = false;
    private bool codeup = false;
    private bool codedown = false;
    private bool codehold = false;
    private bool codedrop = false;
    private bool code1 = false;
    private bool code2 = false;
    private bool code3 = false;
    private bool code4 = false;

    public void Move(float moveHorizontal, float moveVertical)
    {
        if(moveHorizontal > 0) { codeleft = true; }
        else if(moveHorizontal < 0) { coderight = true; }
        
        if(moveVertical > 0) { codeup = true; }
        else if (moveVertical < 0) { codedown = true; }
        
    }

   

    public void SetBlockIndices(int newIndexVal, int newArrIndexVal)
    {
        indexVal = newIndexVal;
        arrIndexVal = newArrIndexVal;
    }





    public void Hold()
    {
        codehold = true;
    }

    public void Drop()
    {
        codedrop = true;
    }

    public void Action1()
    {
        code1 = true;
    }

    public void Action2()
    {
        code2 = true;
    }

    public void Action3()
    {
        code3 = true;
    }

    public void Action4()
    {
        code4 = true;
    }

    private bool isPaused = true;
    private void Start()
    {

        //gameoverPanel.SetActive(false);
        blockPos = new BlockPosition();
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); //(5)
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); //(10)

        nextFallTime = Time.time + fallCycle; //�����ֱ� ����
        //blockArray = new BlockArray(); //�� ������ ����ü ����
        //CreateBackground(); //��׶��� ���� �޼ҵ�

        for (int i = 0; i < boardHeight; ++i)  //���� ���̱���
        {
            GameObject col = Instantiate(nodePrefab);
            
            col.name = "y_" + (boardHeight - i - 1).ToString();
            col.transform.position = new Vector3(offset2p*2 - 0.5f, halfHeight - i, 0);
           
            //* var col = new GameObject("y_" + (boardHeight - i - 1).ToString());     //������ �������� �������� �����ϴ���
           
            col.transform.parent = boardNode;

            SpriteRenderer renderer = col.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        /*�ؾ��� ��
         1. ��Ʈ���� ��� 7�� ���� �ҷ����� + ����
         
         2. 7�鿡�� ��Ʈ���̳� �޾ƿ� �����ϱ�

         3. �ι�° ��Ͽ� �ִ� ��Ʈ���� ��� �̸������ �ű��

         4. (2~3) �տ� �������� -> ���� 7�� �����Ѱ� �� ��ٸ� ? �ٽ� 1�� ����
            
         �ʿ��Ѱ�, CreateTile�� void�� �����°� �ƴ� GameObject�� �迭�� return����!
        */


        //create7Bag();
        CreateTetromino();  //��Ʈ���̳� ���� �޼ҵ� ����
        CreatePreview(); // �̸�����
        updateScore(); // ���� ���
        PlayerPrefs.SetInt("score", scoreVal); // ���� �Ѱ��ֱ�


        Time.timeScale = 0f;
    }

    void Update()
    {
        if (isPaused)
        {
            if (Input.anyKeyDown)
            {
                isPaused = false;
                Time.timeScale = 1f;
                start.SetActive(false);
                return;
            }
        }
        else
        {
            CheckAgain();
            Vector3 moveDir = Vector3.zero;
            bool isRotate = false;

            if (coderight)//Input.GetKeyDown(//KeyCode.LeftArrow))
            {
                moveDir.x = -1;
                coderight = false;
            }
            else if (codeleft)////Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDir.x = 1;
                codeleft = false;
            }

            if (codeup)//Input.GetKeyDown(codeup//KeyCode.UpArrow))
            {

                isRotate = true;
                codeup = false;
            }
            else if (codedown)//Input.GetKeyDown(codedown//KeyCode.DownArrow))
            {
                moveDir.y = -1;
                codedown = false;
            }

            if (codedrop)//Input.GetKeyDown(codedrop//KeyCode.Space))
            {
                while (MoveTetromino(Vector3.down, false))
                {
                }
                codedrop = false;
            }

            // �Ʒ��� �������� ���� ������ �̵���ŵ�ϴ�.
            if (Time.time > nextFallTime)
            {
                nextFallTime = Time.time + fallCycle;
                moveDir = Vector3.down;
                isRotate = false;
            }

            if (moveDir != Vector3.zero || isRotate)
            {
                MoveTetromino(moveDir, isRotate);
            }
            
            if (/*Input.GetKeyDown(KeyCode.M)*/code1 && MultiManager.Instance.redButton2.activeSelf){
                MultiManager.Instance.AtkRed2();
                updateBlock();
                code1 = false;
                UnityEngine.Debug.Log("Red skill!");
            }
            if (/*Input.GetKeyDown(KeyCode.Comma)*/code2 && MultiManager.Instance.blueButton2.activeSelf)
            {
                MultiManager.Instance.AtkBlue2();
                UnityEngine.Debug.Log("Blue skill!");
                updateBlock();
                code2 = false;
            }
            if (/*Input.GetKeyDown(KeyCode.Period)*/code3 && MultiManager.Instance.yellowButton2.activeSelf)
            {

                MultiManager.Instance.AtkYellow2();
                UnityEngine.Debug.Log("Yellow skill!");
                updateBlock();
                code3 = false;
            }
            if (/*Input.GetKeyDown(KeyCode.Slash)*/code4 && MultiManager.Instance.greenButton2.activeSelf)
            {
                UnityEngine.Debug.Log("Green skill!");
                MultiManager.Instance.Green2();
                updateBlock();
                code4 = false;
            }
            if (codehold/*Input.GetKeyDown(KeyCode.RightControl)*/)
            {
                HoldTetromino();
                codehold = false;
            }





        }
        setGhostBlock();
    }


    void CreatePreview()
    {
        // �̹� �ִ� �̸����� �����ϱ�
        foreach (Transform tile in preview)
        {
            Destroy(tile.gameObject);
        }
        preview.DetachChildren();



       

        preview.position = new Vector2(halfWidth + 3.3f+ 2f*offset2p , halfHeight - 2.5f); // �̸����� 





        if (prevIndex.Count == 0)
        {
            prevIndex = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
            int n = prevIndex.Count;

            for (int i = 0; i < n; i++)
            {
                int r = UnityEngine.Random.Range(0, n);
                int temp = prevIndex[r];
                prevIndex[r] = prevIndex[i];
                prevIndex[i] = temp;
                prevarr.Add(UnityEngine.Random.Range(0, 24));
            }// 7 ���� �ε��� ����
        }



        indexVal = prevIndex[0];
        prevIndex.RemoveAt(0);
        arrIndexVal = prevarr[0];
        prevarr.RemoveAt(0);





        int[,] colorArray = new int[24, 4] {
        {1, 1, 2, 3}, {1, 1, 2, 4}, {1, 1, 3, 2},
        {1, 1, 3, 4}, {1, 1, 4, 2}, {1, 1, 4, 3},
        {2, 2, 1, 3}, {2, 2, 1, 4}, {2, 2, 3, 4},
        {2, 2, 3, 1}, {2, 2, 4, 1}, {2, 2, 4, 3},
        {3, 3, 1, 2}, {3, 3, 1, 4}, {3, 3, 2, 1},
        {3, 3, 2, 4}, {3, 3, 4, 1}, {3, 3, 4, 2},
        {4, 4, 1, 2}, {4, 4, 1, 3}, {4, 4, 2, 1},
        {4, 4, 2, 3}, {4, 4, 3, 1}, {4, 4, 3, 2}
        };

        int col1;
        int col2;
        int col3;
        int col4;
        col1 = colorArray[arrIndexVal, 0];
        col2 = colorArray[arrIndexVal, 1];
        col3 = colorArray[arrIndexVal, 2];
        col4 = colorArray[arrIndexVal, 3];

        switch (indexVal)
        {
            case 0: // I

                CreateTile(preview, new Vector2(0f, 1f), col1);
                CreateTile(preview, new Vector2(0f, 0f), col2);
                CreateTile(preview, new Vector2(0f, -1f), col3);
                CreateTile(preview, new Vector2(0f, -2f), col4);
                break;

            case 1: // J

                CreateTile(preview, new Vector2(-1f, 0.0f), col1);
                CreateTile(preview, new Vector2(0f, 0.0f), col2);
                CreateTile(preview, new Vector2(1f, 0.0f), col3);
                CreateTile(preview, new Vector2(-1f, 1.0f), col4);
                break;

            case 2: // L

                CreateTile(preview, new Vector2(-1f, 0.0f), col1);
                CreateTile(preview, new Vector2(0f, 0.0f), col2);
                CreateTile(preview, new Vector2(1f, 0.0f), col3);
                CreateTile(preview, new Vector2(1f, 1.0f), col4);
                break;

            case 3: // O 

                CreateTile(preview, new Vector2(0f, 0f), col1);
                CreateTile(preview, new Vector2(1f, 0f), col2);
                CreateTile(preview, new Vector2(0f, 1f), col3);
                CreateTile(preview, new Vector2(1f, 1f), col4);
                break;

            case 4: //  S

                CreateTile(preview, new Vector2(-1f, -1f), col1);
                CreateTile(preview, new Vector2(0f, -1f), col2);
                CreateTile(preview, new Vector2(0f, 0f), col3);
                CreateTile(preview, new Vector2(1f, 0f), col4);
                break;

            case 5: //  T

                CreateTile(preview, new Vector2(-1f, 0f), col1);
                CreateTile(preview, new Vector2(0f, 0f), col2);
                CreateTile(preview, new Vector2(1f, 0f), col3);
                CreateTile(preview, new Vector2(0f, 1f), col4);
                break;

            case 6: // Z

                CreateTile(preview, new Vector2(-1f, 1f), col1);
                CreateTile(preview, new Vector2(0f, 1f), col2);
                CreateTile(preview, new Vector2(0f, 0f), col3);
                CreateTile(preview, new Vector2(1f, 0f), col4);
                break;
        }
    }



    void CreateHoldPreview()
    {
        // ������ �̸����⸦ ����
        foreach (Transform tile in holdNode)
        {
            Destroy(tile.gameObject);
        }
        holdNode.DetachChildren();

        if (holdIndex == -1 || holdArrIndex == -1)
            return;

        int[,] colorArray = new int[24, 4] {
            {1, 1, 2, 3}, {1, 1, 2, 4}, {1, 1, 3, 2}, {1, 1, 3, 4}, {1, 1, 4, 2}, {1, 1, 4, 3},
            {2, 2, 1, 3}, {2, 2, 1, 4}, {2, 2, 3, 4}, {2, 2, 3, 1}, {2, 2, 4, 1}, {2, 2, 4, 3},
            {3, 3, 1, 2}, {3, 3, 1, 4}, {3, 3, 2, 1}, {3, 3, 2, 4}, {3, 3, 4, 1}, {3, 3, 4, 2},
            {4, 4, 1, 2}, {4, 4, 1, 3}, {4, 4, 2, 1}, {4, 4, 2, 3}, {4, 4, 3, 1}, {4, 4, 3, 2}
        };

        int col1 = colorArray[holdArrIndex, 0];
        int col2 = colorArray[holdArrIndex, 1];
        int col3 = colorArray[holdArrIndex, 2];
        int col4 = colorArray[holdArrIndex, 3];

        //holdNode.position = new Vector2(-20,-40); // Ȧ�� �̸����� ��ġ
        // preview.position = new Vector2(halfWidth + 3.3f + offset1p, halfHeight - 2.5f); // �̸����� 
        switch (holdIndex)
        {
            // I
            case 0:
                CreateTile(holdNode, new Vector2(0f, 0.3f), col1);
                CreateTile(holdNode, new Vector2(0f, 0f), col2);
                CreateTile(holdNode, new Vector2(0f, -0.3f), col3);
                CreateTile(holdNode, new Vector2(0f, -0.6f), col4);
                break;
            // J
            case 1:
                CreateTile(holdNode, new Vector2(-0.3f, 0.0f), col1);
                CreateTile(holdNode, new Vector2(0f, 0.0f), col2);
                CreateTile(holdNode, new Vector2(0.3f, 0.0f), col3);
                CreateTile(holdNode, new Vector2(-0.3f, 0.3f), col4);
                break;
            // L
            case 2:
                CreateTile(holdNode, new Vector2(-0.3f, 0.0f), col1);
                CreateTile(holdNode, new Vector2(0f, 0.0f), col2);
                CreateTile(holdNode, new Vector2(0.3f, 0.0f), col3);
                CreateTile(holdNode, new Vector2(0.3f, 0.3f), col4);
                break;
            // O
            case 3:
                CreateTile(holdNode, new Vector2(0f, 0f), col1);
                CreateTile(holdNode, new Vector2(0.3f, 0f), col2);
                CreateTile(holdNode, new Vector2(0f, 0.3f), col3);
                CreateTile(holdNode, new Vector2(0.3f, 0.3f), col4);
                break;
            // S
            case 4:
                CreateTile(holdNode, new Vector2(-0.3f, -0.3f), col1);
                CreateTile(holdNode, new Vector2(0f, -0.3f), col2);
                CreateTile(holdNode, new Vector2(0f, 0f), col3);
                CreateTile(holdNode, new Vector2(0.3f, 0f), col4);
                break;
            // T
            case 5:
                CreateTile(holdNode, new Vector2(-0.3f, 0f), col1);
                CreateTile(holdNode, new Vector2(0f, 0f), col2);
                CreateTile(holdNode, new Vector2(0.3f, 0f), col3);
                CreateTile(holdNode, new Vector2(0f, 0.3f), col4);
                break;
            // Z
            case 6:
                CreateTile(holdNode, new Vector2(-0.3f, 0.3f), col1);
                CreateTile(holdNode, new Vector2(0f, 0.3f), col2);
                CreateTile(holdNode, new Vector2(0f, 0f), col3);
                CreateTile(holdNode, new Vector2(0.3f, 0f), col4);
                break;
        }
    }




    void HoldTetromino()
    {
        if (hasHeld)
            return;

        if (holdIndex == -1)
        {
            holdIndex = previndexVal;
            holdArrIndex = prevarrIndexVal;

            DestroyCurrentTetromino();
            CreateTetromino();
            CreatePreview();

        }
        else
        {
            int tempIndex = previndexVal;
            int tempArrIndex = prevarrIndexVal;
            indexVal = holdIndex;
            arrIndexVal = holdArrIndex;
            holdIndex = tempIndex;
            holdArrIndex = tempArrIndex;
            DestroyCurrentTetromino();
            CreateTetromino();
            CreatePreview();

        }
        CreateHoldPreview();
        hasHeld = true; // Ȧ�� �Ŀ��� �ٽ� Ȧ�� �Ұ� ���·� ����

    }


    void DestroyCurrentTetromino()
    {
        foreach (Transform tile in tetrominoNode)
        {
            Destroy(tile.gameObject);
        }
        tetrominoNode.DetachChildren();

        foreach (Transform tile in ghostNode)
        {
            Destroy(tile.gameObject);
        }
        ghostNode.DetachChildren();
    }




    bool MoveTetromino(Vector3 moveDir, bool isRotate)
    {
        Vector3 oldPos = tetrominoNode.transform.position;
        Quaternion oldRot = tetrominoNode.transform.rotation;

        tetrominoNode.transform.position += moveDir;
        if (isRotate)
        {
            tetrominoNode.transform.rotation *= Quaternion.Euler(0, 0, 90);
        }

        if (!CanMoveTo(tetrominoNode))
        {
            tetrominoNode.transform.position = oldPos;
            tetrominoNode.transform.rotation = oldRot;

            if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && isRotate == false)
            {
                AddToBoard(tetrominoNode);
                for (int i = 0; i < ghostNode.childCount; i++)
                {
                    Transform child = ghostNode.GetChild(i);
                    Destroy(child.gameObject);
                }
                CheckBoardColumn();
                CreateTetromino();
                CreatePreview();
                CheckTileGroups();

                if (!CanMoveTo(tetrominoNode))
                {
                    //gameoverPanel.SetActive(true);
                    lose = true;
                    SceneManager.LoadScene("MultiGameOver");
                }
            }

            return false;
        }

        return true;
    }

    // ��Ʈ�ι̳븦 ���忡 �߰�
    void AddToBoard(Transform root)
    {
        if (root == null)
        {
            UnityEngine.Debug.LogError("Root transform is null.");
            return;
        }
        int randomKey = UnityEngine.Random.Range(100000, 999999); // 100000���� 999999������ ������ ��
        string keyTime = randomKey.ToString();
        //UnityEngine.Debug.Log(keyTime);
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);

            int x = Mathf.RoundToInt(node.transform.position.x + -2*offset2p + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            node.parent = boardNode.Find("y_" + y.ToString());
            node.name = "x_" + x.ToString();
            // �߰���Ʈ
            Tile tileComponent = node.GetComponent<Tile>(); // �����տ��� Tile ������Ʈ ��������
            if (tileComponent != null)
            {
                // Tile ������Ʈ���� ���� ���� ��������
                Color tileColor = tileComponent.color;

                string sendcolor = tileColor.ToString();    //Color32ToRGBString(tileColor);
                //UnityEngine.Debug.Log(sendcolor);
                // insertBlock �޼��忡 x, y ��ġ�� ���� ������ ����
                //UnityEngine.Debug.Log("x: " + x + ", y: " + y + ", key: " + keyTime);
                //blockPos.insertBlock(x, y, sendcolor, keyTime);
                 rememberTile(node, keyTime);
            }
            else
            {
               // UnityEngine.Debug.Log("������Ʈ�� null!");
            }
            //blockPos.insertBlock(x , y, )
            //node.tag = keyTime; <<< �������2
            //UnityEngine.Debug.Log(keyTime + "������");
        }
    }

    // ���忡 �ϼ��� ���� ������ ����
    
    void CheckBoardColumn()
    {
        
         List<List<Transform>> tilesFall = new List<List<Transform>>();
        foreach (Transform column in boardNode)
        {
            List<Tile> tilesToRemove = new List<Tile>(); // ������ Ÿ�� ����Ʈ
            if (column.childCount == boardWidth)// �ϼ��� �� == ���� �ڽ� ������ ���� ũ��
            {
                foreach (Transform tile in column)
                {
                    
                    Tile currentTile = tile.GetComponent<Tile>();

                    if (currentTile.isIced) // ����� ��
                    {
                        currentTile.isIced = false; // Ǯ��
                        currentTile.color = currentTile.preColor;
                       // UnityEngine.Debug.Log("�����");
                    }
                    else
                    {

                        tilesToRemove.Add(currentTile); // �Ⱦ������ ���� ����Ʈ �߰�
                        //UnityEngine.Debug.Log("�Ⱦ����");
                    }
                }
                foreach (var tile in tilesToRemove)
                {
                    if (tile.getColor() == "Red")
                    {
                        redVal++;
                    }
                    else if (tile.getColor() == "Blue")
                    {
                        blueVal++;
                    }
                    else if (tile.getColor() == "Green")
                    {
                        greenVal++;
                        UnityEngine.Debug.Log("countGreen");
                    }
                    else if (tile.getColor() == "Yellow")
                    {
                        yellowVal++;
                    }
                    updateBlock(); // ���� ������Ʈ

                    List<Transform> fallList2 = GetEx(tile.transform);
                    tilesFall.Add(fallList2);

                    Destroy(tile.gameObject);
                }


                // isCleared = true;
                scoreVal += tilesToRemove.Count * lineWeight;
                    updateScore();
                    PlayerPrefs.SetInt("score", scoreVal);
                    blockCount += tilesToRemove.Count;
                   // UnityEngine.Debug.Log("count");
                
            }
        }
        
        
        foreach (List<Transform> fall2 in tilesFall)
        {
            foreach(Transform tt in fall2) {
                if (tt != null)
                {
                    Tile tile = tt.GetComponent<Tile>();
                    if (tile != null)
                    {
                        tile.fallReady();
                        //UnityEngine.Debug.Log("���Ͽ� �߰���");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("��������");
                    }
                }
            }
            
        }

      
    }
    


    // �̵� �������� üũ
    bool CanMoveTo(Transform root)
    {
        for (int i = 0; i < root.childCount; ++i)
        {
            var node = root.GetChild(i);
            int x = Mathf.RoundToInt(node.transform.position.x -2*offset2p+ halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1 + offset_y);
            //UnityEngine.Debug.Log("x ���:" + x);
            if (x < 0  || x > boardWidth - 1 ) // x��ǥ�� ���� �̳�
                return false;

            if (y < 0 + offset_y) //y�� ����
                return false;

            var column = boardNode.Find("y_" + y.ToString()); //y��Ʈ�� ã��

            if (column != null && column.Find("x_" + x.ToString()) != null)
                return false;
        }

        return true;
    }




    // Ÿ�� ����
    Tile CreateTile(Transform parent, Vector2 position, int color, int order = 1)
    {


        //���⼭ var go �� ����ص־� ��밡����

        GameObject go;

        switch (color)
        {
            case 1:
                go = Instantiate(tileRed);
                go.transform.parent = parent;
                go.transform.localPosition = position;
                var tiler = go.GetComponent<Tile>();
                tiler.sortingOrder = order;
                tiler.setRed();
                //tiler.transform.localScale = new Vector3(scale1, scale2, 0);

                return tiler;

            case 2:
                go = Instantiate(tileGreen);
                go.transform.parent = parent;
                go.transform.localPosition = position;
                var tileg = go.GetComponent<Tile>();
                tileg.sortingOrder = order;
                tileg.setGreen();
                //tileg.transform.localScale = new Vector3(scale1, scale2, 0);
                return tileg;

            case 3:
                go = Instantiate(tileBlue);
                go.transform.parent = parent;
                go.transform.localPosition = position;
                var tileb = go.GetComponent<Tile>();
                tileb.sortingOrder = order;
                tileb.setBlue();
                //tileb.transform.localScale = new Vector3(scale1, scale2, 0);
                return tileb;

            case 4:
                go = Instantiate(tileYellow);
                go.transform.parent = parent;
                go.transform.localPosition = position;
                var tiley = go.GetComponent<Tile>();
                tiley.sortingOrder = order;
                tiley.setYellow();
                //tiley.transform.localScale = new Vector3(scale1, scale2, 0);
                return tiley;
            case 0:
                go = Instantiate(tilePrefab);
                go.transform.parent = parent;
                go.transform.localPosition = position;
                var tileGhost = go.GetComponent<Tile>();
                tileGhost.sortingOrder = order;
                tileGhost.color = new Color(186f / 255f, 186f / 255f, 186f / 255f, 186f / 255f);

                return tileGhost;
            default:
                go = Instantiate(tilePrefab);
                go.transform.parent = parent;
                go.transform.localPosition = position;
                var tile = go.GetComponent<Tile>();
                tile.sortingOrder = order;
                //tile.transform.localScale = new Vector3(scale1, scale2, 0);
                return tile;

        }
        



        // tile.transform.name = "tile" + position.x.ToString() + "_" + position.y.ToString();

    }
    public float scale1;
    public float scale2;


    Tile Createback(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab);
        int convertY = (int)position.y + 9;
        string parentName = "back" + convertY.ToString();

        Transform pback = backgroundNode.Find(parentName);
        //UnityEngine.Debug.Log(parentName); ����׿�
        go.transform.parent = backgroundNode;
        //go.transform.parent = backgroundNode;
        go.transform.localPosition = position;

        var tile = go.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;
        tile.transform.name = "back" + convertY;
        return tile;
    }


    // ��� Ÿ���� ����
    void CreateBackground()
    {
        Color color = Color.gray;

        // Ÿ�� ����
        color.a = 0.5f;
        for (int x = -halfWidth; x < halfWidth; ++x)
        {
            for (int y = halfHeight; y > -halfHeight; --y)
            {
                Createback(backgroundNode, new Vector2(x+3/2f*offset2p, y + offset_y), color, 0);//



            }
        }

        // �¿� �׵θ�
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            Createback(backgroundNode, new Vector2(-halfWidth - 1 + 3/2f*offset2p, y + offset_y), color, 0);
            Createback(backgroundNode, new Vector2(halfWidth + 3/2f*offset2p, y + offset_y), color, 0);
        }

        // �Ʒ� �׵θ�
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            Createback(backgroundNode, new Vector2(x + 3/2f*offset2p, -halfHeight + offset_y), color, 0);
        }
    }

    // ��Ʈ�ι̳� ����
    void CreateTetromino()
    {
        int index;
        if (indexVal == -1)
        {
            index = UnityEngine.Random.Range(0, 7);
            prevIndex.Remove(index);
        }
        else index = indexVal;
        int arrIndex;
        if (arrIndexVal == -1)
        {
            arrIndex = UnityEngine.Random.Range(0, 24);
        }
        else arrIndex = arrIndexVal;

        int[,] colorArray = new int[24, 4] {
        {1, 1, 2, 3}, {1, 1, 2, 4}, {1, 1, 3, 2}, //������ �����ϴ� Ű��
        {1, 1, 3, 4}, {1, 1, 4, 2}, {1, 1, 4, 3},
        {2, 2, 1, 3}, {2, 2, 1, 4}, {2, 2, 3, 4},
        {2, 2, 3, 1}, {2, 2, 4, 1}, {2, 2, 4, 3},
        {3, 3, 1, 2}, {3, 3, 1, 4}, {3, 3, 2, 1},
        {3, 3, 2, 4}, {3, 3, 4, 1}, {3, 3, 4, 2},
        {4, 4, 1, 2}, {4, 4, 1, 3}, {4, 4, 2, 1},
        {4, 4, 2, 3}, {4, 4, 3, 1}, {4, 4, 3, 2}
        };

        int col1;
        int col2;
        int col3;
        int col4;
        col1 = colorArray[arrIndex, 0];
        col2 = colorArray[arrIndex, 1];
        col3 = colorArray[arrIndex, 2];
        col4 = colorArray[arrIndex, 3];

        tetrominoNode.rotation = Quaternion.identity;
        tetrominoNode.position = new Vector2(offset_x + 2*offset2p , halfHeight - panalty + offset_y); // �� ���� ��ġ

        switch (index)
        {
            // I 
            case 0:

                CreateTile(tetrominoNode, new Vector2(0f, 1f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col2);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), col3);
                CreateTile(tetrominoNode, new Vector2(0f, -2f), col4);

                CreateTile(ghostNode, new Vector2(0f, 1f), 0);
                CreateTile(ghostNode, new Vector2(0f, 0f), 0);
                CreateTile(ghostNode, new Vector2(0f, -1f), 0);
                CreateTile(ghostNode, new Vector2(0f, -2f), 0);
                break;

            // J 
            case 1:

                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 0.0f), col2);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), col3);
                CreateTile(tetrominoNode, new Vector2(-1f, 1.0f), col4);

                CreateTile(ghostNode, new Vector2(-1f, 0.0f), 0);
                CreateTile(ghostNode, new Vector2(0f, 0.0f), 0);
                CreateTile(ghostNode, new Vector2(1f, 0.0f), 0);
                CreateTile(ghostNode, new Vector2(-1f, 1.0f), 0);
                break;

            // L 
            case 2:

                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 0.0f), col2);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), col3);
                CreateTile(tetrominoNode, new Vector2(1f, 1.0f), col4);

                CreateTile(ghostNode, new Vector2(-1f, 0.0f), 0);
                CreateTile(ghostNode, new Vector2(0f, 0.0f), 0);
                CreateTile(ghostNode, new Vector2(1f, 0.0f), 0);
                CreateTile(ghostNode, new Vector2(1f, 1.0f), 0);
                break;

            // O 
            case 3:

                CreateTile(tetrominoNode, new Vector2(0f, 0f), col1);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), col2);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), col3);
                CreateTile(tetrominoNode, new Vector2(1f, 1f), col4);

                CreateTile(ghostNode, new Vector2(0f, 0f), 0);
                CreateTile(ghostNode, new Vector2(1f, 0f), 0);
                CreateTile(ghostNode, new Vector2(0f, 1f), 0);
                CreateTile(ghostNode, new Vector2(1f, 1f), 0);
                break;

            // S 
            case 4:

                CreateTile(tetrominoNode, new Vector2(-1f, -1f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), col2);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col3);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), col4);

                CreateTile(ghostNode, new Vector2(-1f, -1f), 0);
                CreateTile(ghostNode, new Vector2(0f, -1f), 0);
                CreateTile(ghostNode, new Vector2(0f, 0f), 0);
                CreateTile(ghostNode, new Vector2(1f, 0f), 0);
                break;

            // T 
            case 5:

                CreateTile(tetrominoNode, new Vector2(-1f, 0f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col2);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), col3);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), col4);

                CreateTile(ghostNode, new Vector2(-1f, 0f), 0);
                CreateTile(ghostNode, new Vector2(0f, 0f), 0);
                CreateTile(ghostNode, new Vector2(1f, 0f), 0);
                CreateTile(ghostNode, new Vector2(0f, 1f), 0);
                break;

            // Z 
            case 6:

                CreateTile(tetrominoNode, new Vector2(-1f, 1f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), col2);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col3);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), col4);

                CreateTile(ghostNode, new Vector2(-1f, 1f), 0);
                CreateTile(ghostNode, new Vector2(0f, 1f), 0);
                CreateTile(ghostNode, new Vector2(0f, 0f), 0);
                CreateTile(ghostNode, new Vector2(1f, 0f), 0);
                break;
        }
        hasHeld = false;
        previndexVal = indexVal;
        prevarrIndexVal = arrIndexVal;
    }

    Color32 GetColor(int colorNum)
    {
        switch (colorNum)
        {
            case 1: // ������
                return new Color32(255, 0, 0, 255);
            case 2: // �Ķ���
                return new Color32(0, 0, 255, 255);
            case 3: // �����
                return new Color32(255, 255, 0, 255);
            case 4: // �ʷϻ�
                return new Color32(0, 255, 0, 255);
            default:
                return Color.white;
        }
    }



    
    private void CheckTileGroups() // 4�� ������ ������ ���� Ž��/�����ϴ� �޼ҵ�
    {
        List<List<Transform>> tilesFall = new List<List<Transform>>();
        // ���� ������ ��� ���� ��ȸ�մϴ�.
        for (int y = 0; y < boardHeight; y++)
        {
            // �ش� �࿡�� ���ӵ� ��� �׷��� Ž���մϴ�.
            List<Vector2Int> continuousBlocks = FindContinuousBlocksInRow(y);

            
            foreach (Vector2Int blockPosition in continuousBlocks)
            {
                Transform rowObject = boardNode.transform.Find("y_" + blockPosition.y.ToString());
                int xgrav = blockPosition.x;
                string blockName = "x_" + blockPosition.x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);
                
                 if(blockTransform == null){
                   //UnityEngine.Debug.Log("null");
                    
                }
               
                //UnityEngine.Debug.Log(blockPosition.x.ToString());
                if (blockTransform != null && blockPosition.x < 10)
                {
                    Tile tile = blockTransform.GetComponent<Tile>();
                    int ygrav = y;
                    // ���� ������Ʈ�� ã�����Ƿ� �����մϴ�.
                    if (!tile.isIced && tile.isFired == false ) { // �� ����� �� 

                        if(tile.getColor() == "Red")
                        {
                            redVal++;
                        }
                        else if (tile.getColor() == "Blue")
                        {
                            blueVal++;
                        }
                        else if (tile.getColor() == "Green")
                        {
                            greenVal++;
                        }
                        else if (tile.getColor() == "Yellow")
                        {
                            yellowVal++;
                        }

                        List<Transform> fallList2 = GetEx(blockTransform);
                        if (fallList2 != null)
                        {
                            tilesFall.Add(fallList2);
                        }

                        Destroy(blockTransform.gameObject);
                        updateBlock();
                        UnityEngine.Debug.Log("��� ������: " + blockName);
                       
                    }
                    else
                    {
                        if (tile.isFired == true)
                        {}
                        else
                        {
                            tile.isIced = false;
                        }
                    }
                    

                    scoreVal += colorWeight;
                    blockCount++;
                    updateScore();
                    PlayerPrefs.SetInt("score", scoreVal);

                }
                else
                {
                    // ���� ������Ʈ�� ã�� �������� �˸��ϴ�.
                    //UnityEngine.Debug.LogWarning("���� ������Ʈ�� ã�� �� �����ϴ�: " + blockName);
                }

            }


        }    
       
        List<Transform> allF = new List<Transform>();
        foreach (List<Transform> fall2 in tilesFall)
        {
            foreach(Transform tt in fall2) {
                if (tt != null)
                {
                    Tile tile = tt.GetComponent<Tile>();
                    if (tile != null)
                    {
                        tile.fallReady();
                        //UnityEngine.Debug.Log("���Ͽ� �߰���");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("��������");
                    }
                }
            }
            
        }



    }
   

    List<Vector2Int> FindContinuousBlocksInRow(int row)
    {
        List<Vector2Int> continuousBlocks = new List<Vector2Int>(); //���ӵ� ��� �� ��ü�� �����ϴ� ����Ʈ

        

        // ù ��° ����� ������ �����ɴϴ�.

        //UnityEngine.Debug.Log("ù ��� ����");
        string previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

        int currentStart = 0; // ������ ���� ��ǥ

        continuousBlocks.Add(new Vector2Int(currentStart, row));

        // �������� ��� ����� Ȯ���ϸ� ���ӵ� ��� �׷��� ã���ϴ�.
        for (int x = 1; x < 11; x++)
        {

            // ���� ����� ������ �����ɴϴ�.
            string currentColor = GetTileColorAtPosition(new Vector2Int(x, row));
            

            continuousBlocks = upStair(row, continuousBlocks);


            if (currentColor.Equals(previousColor) && currentColor != "null")
            {
                // ���� ��ϰ� ���� ����� ������ ������ ���ӵ� ��� �׷��Դϴ�.
                //currentGroupColors.Add(currentColor);
                continuousBlocks.Add(new Vector2Int(x, row));
               // UnityEngine.Debug.Log("������! \n");
            }
            else
            {
                // ���� ��ϰ� ���� ����� ������ �ٸ��� ���ӵ� ��� �׷��� �������ϴ�.
                // ���� ���ӵ� ��� �׷��� ����ġ�� Ȯ���ϰ�, 4 �̻��� ��쿡�� ����Ʈ�� �߰��մϴ�.
                if (continuousBlocks.Count >= 4)
                {
                    for (int i = x - continuousBlocks.Count; i < x; i++)
                    {
                       // UnityEngine.Debug.Log("�� �߰�!! \n");
                    }
                    return continuousBlocks;
                }

                // ���� ���ӵ� ��� �׷� �ʱ�ȭ
                continuousBlocks.Clear();
                continuousBlocks.Add(new Vector2Int(x, row));
            }

            // ���� ����� ������ ������ �������� �����մϴ�.
            previousColor = currentColor;



        }
        //UnityEngine.Debug.Log("���Ӱ����..");
        return continuousBlocks;
    }

    List<Vector2Int> upStair(int row, List<Vector2Int> downStair)
    {
        Vector2Int remember = new Vector2Int(-100, -100);

        string previousColor = GetTileColorAtPosition(downStair[0]);
        foreach (var block in downStair)
        {
            string currentColor = GetTileColorAtPosition(new Vector2Int(block.x, row + 1));
            if (currentColor.Equals(previousColor) && currentColor != "null" && !downStair.Contains(new Vector2Int(block.x, row + 1)))
            {
                remember = new Vector2Int(block.x, row + 1);
                downStair.Add(remember);
                break;
            }
        }


        if (remember.x != -100)
        {
            int middle = remember.x;
            int n = 1;
            string right;
            string left;
            do
            {
                left = GetTileColorAtPosition(new Vector2Int(remember.x - 1 * n, row + 1));
                if (left.Equals(previousColor))
                {
                    downStair.Add(new Vector2Int(remember.x - 1 * n, row + 1));
                }
                n++;
            } while (left.Equals(previousColor));
            n = 1;
            do
            {
                right = GetTileColorAtPosition(new Vector2Int(remember.x + 1 * n, row + 1));
                if (right.Equals(previousColor))
                {
                    downStair.Add(new Vector2Int(remember.x + 1 * n, row + 1));
                }
                n++;
            } while (right.Equals(previousColor));


            downStair = upStair(row + 1, downStair);
        }



        return downStair;
    }









      public void CheckAgain()
    {
     
        foreach (Transform column in boardNode)
        {
            if(column.childCount == boardWidth)
            {
             
                UnityEngine.Debug.Log("����������");
                CheckBoardColumn();
                break;
            }
        }

        for (int y = 0; y < 20; y++)
        {
            List<Vector2Int> cont = FindContinuousBlocksInRow(y);
            if(cont.Count >= 1)
            {
                //UnityEngine.Debug.Log("�������� ����!!!");
            
                CheckTileGroups();
                break;
            }
        }

    }



    string GetTileColorAtPosition(Vector2Int position)
    {
        // ��ȿ�� ��ġ���� Ȯ���մϴ�.
        if (position.x >= 0 && position.x < boardWidth &&
            position.y >= 0 && position.y < boardHeight)
        {
            // �ش� ���� ���� ������Ʈ�� �����ɴϴ�.
            Transform rowObject = boardNode.transform.Find("y_" + position.y.ToString());

            // �ش� �࿡ �ִ� ��� ���� �����ɴϴ�.

            if (rowObject != null)
            {
                // �� ������Ʈ�� �߰ߵǸ� �ش� ���� �ڽ� ������Ʈ �߿��� x ��ǥ�� ���� �̸��� ���� ����� ã���ϴ�.
                string blockName = "x_" + position.x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);

                if (blockTransform != null)
                {
                    // ����� ã�ҽ��ϴ�
                    Tile tile = blockTransform.GetComponent<Tile>();
                    string coll = tile.getColor();

                    return coll;
                }
                else
                {
                 //UnityEngine.Debug.Log("�ش� ��ǥ �� ��ã��!!:" +blockName );       
                // �ش� x ��ǥ�� ���� ����� �����ϴ�.
                }
            }
            else
            {
                // �ش� y ��ǥ�� ���� ���� �����ϴ�.
            }



        }

        // ��ġ�� ��ȿ���� �ʰų� �ش� ��ġ�� ���� ���� ��� �⺻������ ������� ��ȯ�մϴ�.
        string a = "null";
        return a;
    }

    /*void StructureBlock()
    {
       public Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
       

    }*/
    public void doPanalty()
    { // �г�Ƽ�ο� + �� �پ��
        int buff = 19 - panalty;
        Transform backRow = boardNode.Find("y_" + buff.ToString());
        SpriteRenderer renderer = backRow.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.enabled = true;
        }


        //for (int i = 0; i < 12; i++)
        //{
        // Transform backRow = backgroundNode.transform.Find("back" + buff.ToString());
        // backRow.transform.position = new Vector3Int(-50, 0,0);
        //backRow.transform.name = "delete";//�̸��� �ٲ���� ������ ���� ������ ������, destroy�� ��� ������ �ƴ϶� �����̰� �����ϹǷ�, �ݺ��� �ð����� �Ȱɸ��°� ����
        //Destroy(backRow);
        //}
        UnityEngine.Debug.Log("2P�г�Ƽ");
        panalty++;
        panaltyVal++;

    }

    //��ų ����
    [Header("Skill")]
    public int redSkillNum; // �����ų �� ����
    public int yellowSkillNum; // �����ų �� ����
    public int blueSkillNum; // �����ų �� ����
    public int maxBlock;
    private Color fire = Color.black;
    private Color ice = Color.gray;
    public UnityEngine.UI.Image lightening;
    public float fadeInImage = 0.1f; // �̹��� ��Ÿ���� �ð�
    public float fadeOutImage = 1.01f; // �̹��� ������� �ð�
    public Text red; // ����� ��
    public Text green; // ����� ��
    public Text blue; // ����� ��
    public Text yellow; // ����� ��


    public List<Tile> randomTile(int maxCount = 5)
    {
        List<Tile> selectedTiles = new List<Tile>();
        List<int> availableYIndices = new List<int>();

        // �� y ��忡 ���� �ڽ��� �ִ��� Ȯ���ϰ�, �ε����� ����Ʈ�� �߰�
        for (int y = 0; y < 19; y++)
        {
            var ynode = boardNode.Find("y_" + y.ToString());
            if (ynode != null && ynode.childCount > 0)
            {
                availableYIndices.Add(y);
            }
        }

        // ��� ������ y ��尡 ������ �� ����Ʈ ��ȯ
        if (availableYIndices.Count == 0)
        {
            return selectedTiles;
        }

        // �������� 5���� y �ε��� ���� (�ߺ� ����)
        List<int> randomYIndices = new List<int>();
        while (randomYIndices.Count < maxCount && randomYIndices.Count < availableYIndices.Count)
        {
            int randomYIndex = availableYIndices[UnityEngine.Random.Range(0, availableYIndices.Count)];
            if (!randomYIndices.Contains(randomYIndex))
            {
                randomYIndices.Add(randomYIndex);
            }
        }

        // ���õ� �� y ��忡�� �������� x ��� �����Ͽ� Ÿ�� ����Ʈ�� �߰�
       
        foreach (int yIndex in randomYIndices)
        {
            var ynode = boardNode.Find("y_" + yIndex.ToString());
            if (ynode != null)
            {
                int randomXIndex = UnityEngine.Random.Range(0, ynode.childCount);
                var xnode = ynode.GetChild(randomXIndex);
                Tile tile = xnode.GetComponent<Tile>();
                if (tile != null)
                {
                    selectedTiles.Add(tile);
                }
            }
        }

        return selectedTiles;
    }
    public void redSkill(int num) // ������
    {
        
        UnityEngine.Debug.Log("������ų");
        List<Tile> tiles = randomTile(num);
       
        foreach (Tile tile in tiles)
        {
            if (!tile.isred())
            {
                tile.isFired = true;
                tile.color = fire;
            }else{
                    redSkill(1);// ���� �ɸ��� ����
            }
        }
        
    }



    public void yellowSkill(int num) // �����
    {
        UnityEngine.Debug.Log("�����ų");
        StartCoroutine(yellowSkillActive());
        yellowDel(num);
    }
    public GameObject image;
    
    
    public void yellowDel(int value)
    {
        int count = 0;
        List<Tile> tiles = randomTile(value);
        foreach (Tile tile in tiles)
        {
            if (tile != null && (tile.color != Color.white || tile.color != Color.gray))
            {
                Destroy(tile.gameObject);
            }
            else if (tile.color != Color.white || tile.color != Color.gray)
            {
                count++;
            }
        }
        if (count > 0)
        {
            yellowDel(value - count);
        }

    }


    IEnumerator yellowSkillActive()
    {
        
        image.SetActive(true);
        float timer = 0;
        while (timer <= fadeInImage) // fade in
        {
            float alpha = Mathf.Lerp(0, 1, timer / fadeInImage);
            lightening.color = new Color(1, 1, 1, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        lightening.color = Color.white;
        
        
            

        timer = 0;
        while (timer <= fadeOutImage) // fade out
        {
            float alpha = Mathf.Lerp(1, 0, timer / fadeOutImage);
            lightening.color = new Color(1, 1, 1, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        lightening.color = new Color(1, 1, 1, 0); // ȭ�� �����

    }
    public void blueSkill(int num) // �Ķ���
    {
        UnityEngine.Debug.Log("�Ķ���ų");
        int i = 0;
        
        while (i < num)
        {
            List<Tile> tiles = randomTile(num-i);
            foreach (Tile tile in tiles)
            {
                if (!tile.isIced)
                {
                    tile.isIced = true;
                    tile.color = ice;
                    i++;
                }
            }
        }
    }
    public void updateBlock()
    {
        red.text = $"{redVal}"; //�� ���� ���
        green.text = $"{greenVal}"; // �� ���� ���
        blue.text = $"{blueVal}"; // �� ���� ���
        yellow.text = $"{yellowVal}"; // �� ���� ���
    }
    public void updateScore()
    {
        score.text = $"{scoreVal}";
    }

    bool DownGhostBlock(Vector3 moveDir, bool isRotate)
    {
        Vector3 oldPos = ghostNode.transform.position;
        Quaternion oldRot = ghostNode.transform.rotation;

        ghostNode.transform.position += moveDir;

        if (isRotate)
        {
            ghostNode.transform.rotation *= Quaternion.Euler(0, 0, 90);
        }
        if (!CanMoveTo(ghostNode))
        {
            ghostNode.transform.position = oldPos;
            ghostNode.transform.rotation = oldRot;

            return false;
        }

        return true;
    }

    private void setGhostBlock()
    {
        ghostNode.position = tetrominoNode.position;
        ghostNode.rotation = tetrominoNode.rotation;
        while (DownGhostBlock(Vector3.down, false))
        {

        }

    }




    
    private Dictionary<string, List<Transform>> addedTiles = new Dictionary<string, List<Transform>>();

     public void rememberTile(Transform tile, string key)
    {
        List<Transform> tetroList;
        if (addedTiles.TryGetValue(key, out tetroList))
        {
            tetroList.Add(tile);
            //UnityEngine.Debug.Log("���߰ɷ��߰���");
        }
        else
        {
            addedTiles[key] = new List<Transform>() { tile };
           // UnityEngine.Debug.Log("���Ӱ��߰�");
        }
    }

    /*
    public List<Transform> GetEx(Transform tile)
    {
        List<Transform> result = new List<Transform>();
        string keyToRemove = null;

        // addedTiles�� �� �׸��� �ݺ�
        foreach (var entry in addedTiles)
        {
            var blockList = entry.Value;

            // ���� blockList�� �Էµ� tile�� �ִ��� Ȯ��
            foreach (Transform t in blockList)
            {
                if (t.gameObject == tile.gameObject)
                {
                    keyToRemove = entry.Key; // �Էµ� tile�� ���Ե� blockList�� key�� ����
                    break; // �� �̻� Ȯ���� �ʿ� �����Ƿ� ���� ����
                }
            }

            if (keyToRemove != null)
            {
                // �Էµ� tile�� ������ ������ Transform�� ��� ����Ʈ�� �߰�
                foreach (Transform t in blockList)
                {
                    if (t.gameObject != tile.gameObject)
                    {
                        result.Add(t);
                    }
                }
                break; // �ش� ����Ʈ�� ã�����Ƿ� �ܺ� ���� ����
            }
        }

        // identified�� key�� addedTiles���� ����
        if (keyToRemove != null)
        {
            addedTiles.Remove(keyToRemove);
        }
        else
        {
            UnityEngine.Debug.Log("�ش��ϴ� Ÿ���� ã�� ���߽��ϴ�.");
        }

        return result;
    }
    */

    
    public List<Transform> GetEx(Transform tile)
    {
        List<Transform> result = new List<Transform>();
        string keyToRemove = null;
        // key�� �ش��ϴ� ����Ʈ�� �ִ��� Ȯ��
        foreach (var entry in addedTiles)
        {
            var blockList = entry.Value;

            // ���� key�� ���� value���� �Է��� (x, y) ���� �����ϰ� ������ ���� ����� �߰�
            foreach (Transform t in blockList)
            {
                if (t == tile)
                {
                    keyToRemove = entry.Key; // ������ ���� ������ (x, y)�� ���� key�� ����

                }
                else
                {
                    result.Add(t); // (x, y)�� �ٸ� ���� ����� �߰�
                }
            }
        }

        if (keyToRemove != null)
        {
            addedTiles.Remove(keyToRemove);

        }
        else
        {
            UnityEngine.Debug.Log("�̹� ã�Ƽ� ������������");
        }

        return result;
    }



}



