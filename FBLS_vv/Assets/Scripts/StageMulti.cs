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

public class StageMulti : MonoBehaviour
{
    [Header("Editor Objects")]
    public GameObject tilePrefab; //Ÿ�������� �ҷ���
    public Transform backgroundNode; // ��׶��� 
    public Transform boardNode; //������(�� �� y0 - y19������ ���)


    public Transform tetrominoNode; //��Ʈ���̳�
                                    // public GameObject gameoverPanel; //���ӿ���
    public Text score; // ����
    
    public Transform preview; // ���� ��
    public GameObject start;
    private Skill skill;
    private SkillManager skillManager;

    public BlockPosition blockPos; // �� ����ü



    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 10;
    [Range(5, 20)]
    public int boardHeight = 20;
    public float fallCycle = 1.0f;
    

    public float offset_x = 0f;
    public float offset_y = 0f;

    public int offset2p = 14;



    private int halfWidth; // ��ǥ (����)�߾Ӱ�
    private int halfHeight; //��ǥ (����) �߾Ӱ�
    public int lineWeight; // ������ �� ����
    public int colorWeight; // ������ �� ����
    public int panalty = 0; // �г�Ƽ�� ����� ����ġ
    public int indexback = 0;


    private float nextFallTime;
    private int scoreVal = 0;
    private int indexVal = -1;
    private int arrIndexVal = -1;
    [HideInInspector] public int redVal = 0; // ����� �� ����
    [HideInInspector] public int greenVal = 0; // ����� �� ����
    [HideInInspector] public int blueVal = 0;   // ����� �� ����
    [HideInInspector] public int yellowVal = 0; // ����� �� ����
    public static int blockCount = 0;
    public GameObject[] backs = new GameObject[200];
    public GameObject[] backgrid = new GameObject[40];
    private bool isPaused = true;
    private void Start()
    {
        
        //gameoverPanel.SetActive(false);
        blockPos = new BlockPosition();
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); //(5)
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); //(10)

        nextFallTime = Time.time + fallCycle; //�����ֱ� ����
        //blockArray = new BlockArray(); //�� ������ ����ü ����
        CreateBackground(); //��׶��� ���� �޼ҵ�

        for (int i = 0; i < boardHeight; ++i)  //���� ���̱���
        {
            var col = new GameObject("y_" + (boardHeight - i - 1).ToString());     //������ �������� �������� �����ϴ���
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            col.transform.parent = boardNode;
        }

        /* for (int i = 0; i < boardHeight; ++i)  //���� ���̱���
         {
             var col = new GameObject("back" + (boardHeight - i - 1).ToString());     //background�� �������� �������� �����ϴ���
             col.transform.position = new Vector3(0, halfHeight - i, 0);
             col.transform.parent = backgroundNode;
         }*/

        /*�ؾ��� ��
         1. ��Ʈ���� ��� 7�� ���� �ҷ����� + ����
         
         2. 7�鿡�� ��Ʈ���̳� �޾ƿ� �����ϱ�

         3. �ι�° ��Ͽ� �ִ� ��Ʈ���� ��� �̸������ �ű��

         4. (2~3) �տ� �������� -> ���� 7�� �����Ѱ� �� ��ٸ� ? �ٽ� 1�� ����
            
         �ʿ��Ѱ�, CreateTile�� void�� �����°� �ƴ� GameObject�� �迭�� return����!
        */



        create7Bag();
        CreateTetromino();  //��Ʈ���̳� ���� �޼ҵ� ����
        CreatePreview(); // �̸�����
        score.text = "Score: " + scoreVal; // ���� ���
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
            Vector3 moveDir = Vector3.zero;
            bool isRotate = false;

            if (Input.GetKeyDown(KeyCode.A))
            {
                moveDir.x = -1;

            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                moveDir.x = 1;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                isRotate = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                moveDir.y = -1;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                while (MoveTetromino(Vector3.down, false))
                {
                }
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
        }

    }
    void CreatePreview()
    {
        // �̹� �ִ� �̸����� �����ϱ�
        foreach (Transform tile in preview)
        {
            Destroy(tile.gameObject);
        }
        preview.DetachChildren();

        indexVal = UnityEngine.Random.Range(0, 7);
        arrIndexVal = UnityEngine.Random.Range(0, 24);

        preview.position = new Vector2(halfWidth + 2.5f + offset2p, halfHeight - 2.5f); // �̸����� 

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
        Color32 color = Color.white;
        Color32 col1;
        Color32 col2;
        Color32 col3;
        Color32 col4;
        col1 = GetColor(colorArray[arrIndexVal, 0]);
        col2 = GetColor(colorArray[arrIndexVal, 1]);
        col3 = GetColor(colorArray[arrIndexVal, 2]);
        col4 = GetColor(colorArray[arrIndexVal, 3]);

        switch (indexVal)
        {
            case 0: // I
                color = new Color32(115, 251, 253, 255);
                CreateTile(preview, new Vector2(0f, 1f), col1);
                CreateTile(preview, new Vector2(0f, 0f), col2);
                CreateTile(preview, new Vector2(0f, -1f), col3);
                CreateTile(preview, new Vector2(0f, -2f), col4);
                break;

            case 1: // J
                color = new Color32(0, 33, 245, 255);
                CreateTile(preview, new Vector2(-1f, 0.0f), col1);
                CreateTile(preview, new Vector2(0f, 0.0f), col2);
                CreateTile(preview, new Vector2(1f, 0.0f), col3);
                CreateTile(preview, new Vector2(-1f, 1.0f), col4);
                break;

            case 2: // L
                color = new Color32(243, 168, 59, 255);
                CreateTile(preview, new Vector2(-1f, 0.0f), col1);
                CreateTile(preview, new Vector2(0f, 0.0f), col2);
                CreateTile(preview, new Vector2(1f, 0.0f), col3);
                CreateTile(preview, new Vector2(1f, 1.0f), col4);
                break;

            case 3: // O 
                color = new Color32(255, 253, 84, 255);
                CreateTile(preview, new Vector2(0f, 0f), col1);
                CreateTile(preview, new Vector2(1f, 0f), col2);
                CreateTile(preview, new Vector2(0f, 1f), col3);
                CreateTile(preview, new Vector2(1f, 1f), col4);
                break;

            case 4: //  S
                color = new Color32(117, 250, 76, 255);
                CreateTile(preview, new Vector2(-1f, -1f), col1);
                CreateTile(preview, new Vector2(0f, -1f), col2);
                CreateTile(preview, new Vector2(0f, 0f), col3);
                CreateTile(preview, new Vector2(1f, 0f), col4);
                break;

            case 5: //  T
                color = new Color32(155, 47, 246, 255);
                CreateTile(preview, new Vector2(-1f, 0f), col1);
                CreateTile(preview, new Vector2(0f, 0f), col2);
                CreateTile(preview, new Vector2(1f, 0f), col3);
                CreateTile(preview, new Vector2(0f, 1f), col4);
                break;

            case 6: // Z
                color = new Color32(235, 51, 35, 255);    // ������
                CreateTile(preview, new Vector2(-1f, 1f), col1);
                CreateTile(preview, new Vector2(0f, 1f), col2);
                CreateTile(preview, new Vector2(0f, 0f), col3);
                CreateTile(preview, new Vector2(1f, 0f), col4);
                break;
        }
    }

    public void create7Bag()
    {

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
        Color32 col1;
        Color32 col2;
        Color32 col3;
        Color32 col4;
        //List<Transform> list7Bag = new List<Transform>();

        GameObject gameObject0 = new GameObject(); // ���ο� ���� ������Ʈ ����
        GameObject gameObject1 = new GameObject();
        GameObject gameObject2 = new GameObject();
        GameObject gameObject3 = new GameObject();
        GameObject gameObject4 = new GameObject();
        GameObject gameObject5 = new GameObject();
        GameObject gameObject6 = new GameObject();

        Transform node0 = gameObject0.transform;
        node0.transform.name = "2node0";
        node0.transform.position = new Vector2(-20, -50);

        Transform node1 = gameObject1.transform;
        node1.transform.name = "2node1";
        node1.transform.position = new Vector2(-24, -50);

        Transform node2 = gameObject2.transform;
        node2.transform.name = "2node2";
        node2.transform.position = new Vector2(-28, -50);

        Transform node3 = gameObject3.transform;
        node3.transform.name = "2node3";
        node3.transform.position = new Vector2(-32, -50);

        Transform node4 = gameObject4.transform;
        node4.transform.name = "2node4";
        node4.transform.position = new Vector2(-36, -50);

        Transform node5 = gameObject5.transform;
        node5.transform.name = "2node5";
        node5.transform.position = new Vector2(-40, 0);

        Transform node6 = gameObject6.transform;
        node6.transform.name = "2node6";
        node6.transform.position = new Vector2(-44, 0);

        for (int i = 0; i < 7; i++)
        {
            arrIndexVal = UnityEngine.Random.Range(0, 24);
            col1 = GetColor(colorArray[arrIndexVal, 0]);
            col2 = GetColor(colorArray[arrIndexVal, 1]);
            col3 = GetColor(colorArray[arrIndexVal, 2]);
            col4 = GetColor(colorArray[arrIndexVal, 3]);

            switch (i)
            {
                case 0: // I

                    CreateTile(node0, new Vector2(0f, 1f), col1);
                    CreateTile(node0, new Vector2(0f, 0f), col2);
                    CreateTile(node0, new Vector2(0f, -1f), col3);
                    CreateTile(node0, new Vector2(0f, -2f), col4);
                    break;

                case 1: // J

                    CreateTile(node1, new Vector2(-1f, 0.0f), col1);
                    CreateTile(node1, new Vector2(0f, 0.0f), col2);
                    CreateTile(node1, new Vector2(1f, 0.0f), col3);
                    CreateTile(node1, new Vector2(-1f, 1.0f), col4);
                    break;

                case 2: // L

                    CreateTile(node2, new Vector2(-1f, 0.0f), col1);
                    CreateTile(node2, new Vector2(0f, 0.0f), col2);
                    CreateTile(node2, new Vector2(1f, 0.0f), col3);
                    CreateTile(node2, new Vector2(1f, 1.0f), col4);
                    break;

                case 3: // O 

                    CreateTile(node3, new Vector2(0f, 0f), col1);
                    CreateTile(node3, new Vector2(1f, 0f), col2);
                    CreateTile(node3, new Vector2(0f, 1f), col3);
                    CreateTile(node3, new Vector2(1f, 1f), col4);
                    break;

                case 4: //  S

                    CreateTile(node4, new Vector2(-1f, -1f), col1);
                    CreateTile(node4, new Vector2(0f, -1f), col2);
                    CreateTile(node4, new Vector2(0f, 0f), col3);
                    CreateTile(node4, new Vector2(1f, 0f), col4);
                    break;

                case 5: //  T

                    CreateTile(node5, new Vector2(-1f, 0f), col1);
                    CreateTile(node5, new Vector2(0f, 0f), col2);
                    CreateTile(node5, new Vector2(1f, 0f), col3);
                    CreateTile(node5, new Vector2(0f, 1f), col4);
                    break;

                case 6: // Z

                    CreateTile(node6, new Vector2(-1f, 1f), col1);
                    CreateTile(node6, new Vector2(0f, 1f), col2);
                    CreateTile(node6, new Vector2(0f, 0f), col3);
                    CreateTile(node6, new Vector2(1f, 0f), col4);
                    break;
            }
        }
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
                CheckBoardColumn();
                CreateTetromino();
                CreatePreview();
                CheckTileGroups();

                if (!CanMoveTo(tetrominoNode))
                {
                    //gameoverPanel.SetActive(true);
                    SceneManager.LoadScene("SingleGameOver");
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

            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth) - offset2p;
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
                UnityEngine.Debug.Log("x: " + x + ", y: " + y + ", key: " + keyTime);
                blockPos.insertBlock(x, y, sendcolor, keyTime);
            }
            //blockPos.insertBlock(x , y, )
            //node.tag = keyTime; <<< �������2
            //UnityEngine.Debug.Log(keyTime + "������");
        }
    }

    // ++ RGB ���ڷ� ��ȯ��
    //    string Color32ToRGBString(Color color)
    //  {
    //    return string.Format("{0} {1} {2}", color.r*255, color.g*255, color.b*255);
    //}













    // ���忡 �ϼ��� ���� ������ ����
    
    void CheckBoardColumn()
    {
        bool isCleared = false;

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
                    }
                    else
                    {
                        tilesToRemove.Add(currentTile); // �Ⱦ������ ���� ����Ʈ �߰�
                    }
                }
                foreach (var tile in tilesToRemove)
                {
                    if (tile.color == Color.red)
                    {
                        redVal++;
                    }
                    else if (tile.color == Color.blue)
                    {
                        blueVal++;
                    }
                    else if (tile.color == Color.green)
                    {
                        greenVal++;
                    }
                    else if (tile.color == Color.yellow)
                    {
                        yellowVal++;
                    }
                    //skillManager.updateBlock(); // ���� ������Ʈ
                    Destroy(tile);
                }

                    column.DetachChildren();
                    isCleared = true;
                    scoreVal += tilesToRemove.Count * lineWeight;
                    score.text = "Score: " + scoreVal;
                    PlayerPrefs.SetInt("score", scoreVal);
                    blockCount += tilesToRemove.Count;
                    UnityEngine.Debug.Log("count");
                
            }
        }


        // ��� �ִ� ���� �����ϸ� �Ʒ��� ����
        if (isCleared)
        {
            for (int i = 1; i < boardNode.childCount; ++i)
            {
                var column = boardNode.Find("y_" + i.ToString());

                // �̹� ��� �ִ� ���� ����
                if (column.childCount == 0)
                    continue;

                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0)
                {
                    if (boardNode.Find("y_" + j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }
                    j--;
                }

                if (emptyCol > 0)
                {
                    var targetColumn = boardNode.Find("y_" + (i - emptyCol).ToString());

                    while (column.childCount > 0)
                    {
                        Transform tile = column.GetChild(0);
                        tile.parent = targetColumn;
                        tile.transform.position += new Vector3(0, -emptyCol, 0);
                    }
                    column.DetachChildren();
                }
            }
        }
    }
    /*
    void CheckBoardColumn()
    {
        bool isCleared = false;

        foreach (Transform column in boardNode)
        {
            if (column.childCount == boardWidth)// �ϼ��� �� == ���� �ڽ� ������ ���� ũ��
            {
                foreach (Transform tile in column)
                {
                    Destroy(tile.gameObject);

                }

                column.DetachChildren();
                isCleared = true;
                scoreVal += 10 * lineWeight;
                score.text = "Score: " + scoreVal;
                PlayerPrefs.SetInt("score", scoreVal);
                blockCount += 10;
            }
        }




        // ��� �ִ� ���� �����ϸ� �Ʒ��� ����
        if (isCleared)
        {
            for (int i = 1; i < boardNode.childCount; ++i)
            {
                var column = boardNode.Find("y_" + i.ToString());

                // �̹� ��� �ִ� ���� ����
                if (column.childCount == 0)
                    continue;

                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0)
                {
                    if (boardNode.Find("y_" + j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }
                    j--;
                }

                if (emptyCol > 0)
                {
                    var targetColumn = boardNode.Find("y_" + (i - emptyCol).ToString());

                    while (column.childCount > 0)
                    {
                        Transform tile = column.GetChild(0);
                        tile.parent = targetColumn;
                        tile.transform.position += new Vector3(0, -emptyCol, 0);
                    }
                    column.DetachChildren();
                }
            }
        }
    }
    */
    /*
    void gravity(string blockname, int y)
    {
        //int x, y; //Ȯ����
        List<string> blocksConnect = new List<string>();
        int xBuffer = int.Parse(blockname);
        int yBuffer = y;

        for (int i = 0; i < blocksConnect.Count; i++)
        {
            yBuffer = blocksConnect[i][1];
            GameObject row = GameObject.Find("y_" + yBuffer.ToString());
            if (row != null)
            {
                Transform target = boardNode.Find("x_" + blocksConnect[i][0]);
                if (target != null)
                {
                    xBuffer = blocksConnect[i][0];
                }

            }
            bool floor = false;
            while (!floor)
            {
                GameObject rowNode = GameObject.Find("y_" + yBuffer.ToString());

                if (rowNode != null)
                {
                    Transform rowNodeTransfrom = rowNode.transform.Find("x_" + xBuffer.ToString());
                    if (rowNodeTransfrom == null)
                    {
                        yBuffer--;
                    }
                    else floor = true; // �Ʒ��� �ٴ�
                }
            }
            Transform targetNode = boardNode.Find("x_" + blocksConnect[i][0]);
            targetNode.SetParent(boardNode.Find("y_" + (yBuffer - 1).ToString()));
        }
    }*/

    void gravity(int startX, int startY)
    {
        for (int y = startY; y >= 0; y--) // �Ʒ��ʺ��� �����Ͽ� ���� �̵�
        {
            var rowNode = GameObject.Find("y_" + y.ToString());
            var nextRowNode = GameObject.Find("y_" + (y - 1).ToString());

            if (rowNode != null && nextRowNode != null)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    var block = rowNode.transform.Find("x_" + x.ToString());
                    if (block != null)
                    {
                        // ���� ���� ����(�Ʒ�) �࿡�� ���� ���� ����� �ִ��� �˻�
                        var nextBlock = nextRowNode.transform.Find("x_" + x.ToString());
                        if (nextBlock == null)
                        {
                            // ����� ������ �Ʒ��� �̵�
                            block.SetParent(nextRowNode.transform);
                            block.localPosition -= new Vector3(0, 1, 0); // �Ʒ��� �̵�
                                                                         // �̵��� ��ġ�� �ٸ� ����� �ִ��� Ȯ��
                            for (int i = y - 1; i >= 0; i--)
                            {
                                var tempRowNode = GameObject.Find("y_" + i.ToString());
                                var tempBlock = tempRowNode.transform.Find("x_" + x.ToString());
                                if (tempBlock != null)
                                {
                                    break; // ���� ����� Ȯ���ϱ� ���� �ݺ��� Ż��
                                }
                                else
                                {
                                    // ���� ����� ������ ����ؼ� �̵�
                                    block.SetParent(tempRowNode.transform);
                                    block.localPosition -= new Vector3(0, 1, 0); // �Ʒ��� �̵�
                                    y = i; // y�� ����
                                }
                            }
                        }
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
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth)-offset2p;
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            if (x < 0 || x > boardWidth - 1) // x��ǥ�� ���� �̳�
                return false;

            if (y < 0) //y�� ����
                return false;

            var column = boardNode.Find("y_" + y.ToString()); //y��Ʈ�� ã��

            if (column != null && column.Find("x_" + x.ToString()) != null)
                return false;
        }

        return true;
    }




    // Ÿ�� ����
    Tile CreateTile(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab);
        go.transform.parent = parent;
        go.transform.localPosition = position;

        //���⼭ var go �� ����ص־� ��밡����


        var tile = go.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;
        // tile.transform.name = "tile" + position.x.ToString() + "_" + position.y.ToString();
        return tile;
    }



    Tile Createback(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab);
        int convertY = (int)position.y + 9;
        string parentName = "back" + convertY.ToString();

        Transform pback = backgroundNode.transform.Find(parentName);
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
                Createback(backgroundNode, new Vector2(x+offset2p/2, y), color, 0);//



            }
        }

        // �¿� �׵θ�
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            Createback(backgroundNode, new Vector2(-halfWidth - 1 + offset2p/2, y), color, 0);
            Createback(backgroundNode, new Vector2(halfWidth + offset2p/2, y), color, 0);
        }

        // �Ʒ� �׵θ�
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            CreateTile(backgroundNode, new Vector2(x + offset2p/2, -halfHeight), color, 0);
        }
    }

    // ��Ʈ�ι̳� ����
    void CreateTetromino()
    {
        int index;
        if (indexVal == -1)
        {
            index = UnityEngine.Random.Range(0, 7);
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

        Color32 color = Color.white;
        Color32 col1;
        Color32 col2;
        Color32 col3;
        Color32 col4;
        col1 = GetColor(colorArray[arrIndex, 0]);
        col2 = GetColor(colorArray[arrIndex, 1]);
        col3 = GetColor(colorArray[arrIndex, 2]);
        col4 = GetColor(colorArray[arrIndex, 3]);

        tetrominoNode.rotation = Quaternion.identity;
        tetrominoNode.position = new Vector2(offset_x + offset2p , halfHeight - panalty + offset_y);

        switch (index)
        {
            // I 
            case 0:
                color = new Color32(115, 251, 253, 255);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col2);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), col3);
                CreateTile(tetrominoNode, new Vector2(0f, -2f), col4);
                break;

            // J 
            case 1:
                color = new Color32(0, 33, 245, 255);
                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 0.0f), col2);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), col3);
                CreateTile(tetrominoNode, new Vector2(-1f, 1.0f), col4);
                break;

            // L 
            case 2:
                color = new Color32(243, 168, 59, 255);
                CreateTile(tetrominoNode, new Vector2(-1f, 0.0f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 0.0f), col2);
                CreateTile(tetrominoNode, new Vector2(1f, 0.0f), col3);
                CreateTile(tetrominoNode, new Vector2(1f, 1.0f), col4);
                break;

            // O 
            case 3:
                color = new Color32(255, 253, 84, 255);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col1);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), col2);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), col3);
                CreateTile(tetrominoNode, new Vector2(1f, 1f), col4);
                break;

            // S 
            case 4:
                color = new Color32(117, 250, 76, 255);
                CreateTile(tetrominoNode, new Vector2(-1f, -1f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, -1f), col2);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col3);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), col4);
                break;

            // T 
            case 5:
                color = new Color32(155, 47, 246, 255);
                CreateTile(tetrominoNode, new Vector2(-1f, 0f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col2);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), col3);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), col4);
                break;

            // Z 
            case 6:
                color = new Color32(235, 51, 35, 255);
                CreateTile(tetrominoNode, new Vector2(-1f, 1f), col1);
                CreateTile(tetrominoNode, new Vector2(0f, 1f), col2);
                CreateTile(tetrominoNode, new Vector2(0f, 0f), col3);
                CreateTile(tetrominoNode, new Vector2(1f, 0f), col4);
                break;
        }
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



    /*
    private void CheckTileGroups() // 4�� ������ ������ ���� Ž��/�����ϴ� �޼ҵ�
    {
        List<List<(int, int)>> allFall = new List<List<(int, int)>>();
        // ���� ������ ��� ���� ��ȸ�մϴ�.
        for (int y = 0; y < boardHeight; y++)
        {
            // �ش� �࿡�� ���ӵ� ��� �׷��� Ž���մϴ�.
            List<Vector2Int> continuousBlocks = FindContinuousBlocksInRow(y);


            foreach (Vector2Int blockPosition in continuousBlocks)
            {
                GameObject rowObject = GameObject.Find("y_" + blockPosition.y.ToString());
                int xgrav = blockPosition.x;
                string blockName = "x_" + blockPosition.x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);
                Tile tile = blockTransform.GetComponent<Tile>();
                //UnityEngine.Debug.Log(blockPosition.x.ToString());
                if (blockTransform != null && blockPosition.x < 10)
                {
                    int ygrav = y;
                    // ���� ������Ʈ�� ã�����Ƿ� �����մϴ�.
                    if (!tile.isIced) { // �� ����� ��

                        if(tile.color == Color.red)
                        {
                            redVal++;
                        }
                        else if (tile.color == Color.blue)
                        {
                            blueVal++;
                        }
                        else if (tile.color == Color.green)
                        {
                            greenVal++;
                        }
                        else if (tile.color == Color.yellow)
                        {
                            yellowVal++;
                        }
                        Destroy(blockTransform.gameObject);
                        skillManager.updateBlock();
                        UnityEngine.Debug.Log("��� ������: " + blockName);
                        List<(int, int)> fallList = blockPos.GetExcept(xgrav, ygrav);
                        allFall.Add(fallList);
                    }
                    else
                    {
                        tile.isIced = false;
                    }
                    CheckBoardColumn();

                    scoreVal += colorWeight;
                    blockCount++;
                    score.text = "Score: " + scoreVal;
                    PlayerPrefs.SetInt("score", scoreVal);

                }
                else
                {
                    // ���� ������Ʈ�� ã�� �������� �˸��ϴ�.
                    UnityEngine.Debug.LogWarning("���� ������Ʈ�� ã�� �� �����ϴ�: " + blockName);
                }
            }


        }
    */
    private void CheckTileGroups() // 4�� ������ ������ ���� Ž��/�����ϴ� �޼ҵ�
    {
        List<List<(int, int)>> allFall = new List<List<(int, int)>>();
        // ���� ������ ��� ���� ��ȸ�մϴ�.
        for (int y = 0; y < boardHeight; y++)
        {
            // �ش� �࿡�� ���ӵ� ��� �׷��� Ž���մϴ�.
            List<Vector2Int> continuousBlocks = FindContinuousBlocksInRow(y);


            foreach (Vector2Int blockPosition in continuousBlocks)
            {
                GameObject rowObject = GameObject.Find("y_" + blockPosition.y.ToString());
                int xgrav = blockPosition.x;
                string blockName = "x_" + blockPosition.x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);
                //UnityEngine.Debug.Log(blockPosition.x.ToString());
                if (blockTransform != null && blockPosition.x < 10)
                {
                    int ygrav = y;
                    // ���� ������Ʈ�� ã�����Ƿ� �����մϴ�.
                    Destroy(blockTransform.gameObject);
                    UnityEngine.Debug.Log("��� ������: " + blockName);
                    List<(int, int)> fallList = blockPos.GetExcept(xgrav, ygrav);
                    allFall.Add(fallList);

                    CheckBoardColumn();

                    scoreVal += colorWeight;
                    blockCount++;
                    score.text = "Score: " + scoreVal;
                    PlayerPrefs.SetInt("score", scoreVal);

                }
                else
                {
                    // ���� ������Ʈ�� ã�� �������� �˸��ϴ�.
                    UnityEngine.Debug.LogWarning("���� ������Ʈ�� ã�� �� �����ϴ�: " + blockName);
                }
            }


        }

        List<(int, int)> allTuples = new List<(int, int)>(); //y ���� �����ź��� �����Ҽ� �ְ� ��������,
        foreach (List<(int, int)> fall in allFall)
        {
            allTuples.AddRange(fall);
        }
        allTuples.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));

        foreach ((int xx, int yy) in allTuples)
        {
            gravity(xx, yy);
            //�̹� �ѹ� ������ ������ ���ķε� ��� �����ؾ���, prefab�� �μ��� isitFall - bool�� �����س���, ���������� �����ȸ�ؼ� isitfall �ִ¾ֵ� ���� ���Ͻ�Ų����,
            // list�� �ִ�(�����¸� ����, �ֵ� ���� + �������� ���Ͽ��� �������ֱ�
        }
        /*
        List<(int, int)> arrayToDelete = new List<(int, int)>();
        arrayToDelete.Clear();
        arrayToDelete = blockPos.ContinousBlock();
        
        foreach (var tuple in arrayToDelete)
        {
            UnityEngine.Debug.Log("test:"+tuple);
        }
        if (arrayToDelete.Count >= 4)
        {
           
            foreach (var tuple in arrayToDelete)
            {
                int x = tuple.Item1;
                int y = tuple.Item2;

                
                GameObject rowObject = GameObject.Find("y_" + y.ToString());
                string blockName = "x_" + x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);
                if (blockTransform != null) { 
                   // Destroy(blockTransform.gameObject);
                UnityEngine.Debug.Log("x ��ǥ: " + x + ", y ��ǥ: " + y + "�ν���!!");
                    }
            }
        }


        */

    }
    /*

    List<Vector2Int> FindContinuousBlocksInRow(int row) //���ӵǴ� ��� Ž�� �޼ҵ�
    {
        List<Vector2Int> continuousBlocks = new List<Vector2Int>();


        List<Vector2Int> currentGroup = new List<Vector2Int>(); //���� ���׷�
                                                               // ù ��° ����� ������ �����ɴϴ�.
        UnityEngine.Debug.Log(" ù ��� ���� ");
        Color32 previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

        // ���� ���ӵ� ����� ����ġ�� �ʱ�ȭ�մϴ�.
        //int currentWeight = weight;


        // �������� ��� ����� Ȯ���ϸ� ���ӵ� ��� �׷��� ã���ϴ�.
        for (int x = 1 ; x < boardWidth ; x++)
        {
            // ���� ����� ������ �����ɴϴ�.
            Color32 currentColor = GetTileColorAtPosition(new Vector2Int(x, row));

            // currentWeight += vertWeight(x, row, currentColor
            List<Vector2Int> verticalLine = onlyUp(x, row, currentColor);




            // ���� ����� ������ ���� ����� ����� ������ Ȯ���մϴ�.
            if (currentColor.Equals(previousColor))
            {
               // ���� ��ϰ� ���� ����� ������ ������ ���ӵ� ��� �׷��Դϴ�.
                
                currentWeight++;
                UnityEngine.Debug.Log(" ������! \n ");
            }
            else
            {
                // ���� ��ϰ� ���� ����� ������ �ٸ��� ���ӵ� ��� �׷��� �������ϴ�.
                // ���� ���ӵ� ��� �׷��� ����ġ�� Ȯ���ϰ�, 3 �̻��� ��쿡�� ����Ʈ�� �߰��մϴ�.
                if (currentWeight >= 4)
                {
                    for (int i = x - currentWeight; i < x; i++)
                    {
                        continuousBlocks.Add(new Vector2Int(i, row));
                        UnityEngine.Debug.Log(" �� �߰�!! \n ");
                    }
                }

                // ����ġ�� �ʱ�ȭ�մϴ�.
                currentWeight = 1;
            }

            // ���� ����� ������ ���� �������� �����մϴ�.
            previousColor = currentColor;
        }

        return continuousBlocks;
    }
    

    List<Vector2Int> onlyUp(int x, int row, Color32 previouisColor)
    {
        List<Vector2Int> semi = new List<Vector2Int>();
        Color32 curColor = GetTileColorAtPosition(new Vector2Int(x, row+1));

        if(curColor.Equals(previouisColor))
        {
            semi.Add(new Vector2Int(x, row + 1));
            List<Vector2Int> buf = FindContinuousBlocksInRow(row + 1);
        }

        return semi;
    }
    

    */

    List<Vector2Int> FindContinuousBlocksInRow(int row)
    {
        List<Vector2Int> continuousBlocks = new List<Vector2Int>(); //���ӵ� ��� �� ��ü�� �����ϴ� ����Ʈ

        List<Color32> currentGroupColors = new List<Color32>(); // ���ӵ� ��� �׷��� ������ �����ϴ� ����Ʈ

        // ù ��° ����� ������ �����ɴϴ�.

        UnityEngine.Debug.Log("ù ��� ����");
        Color32 previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

        int currentStart = 0; // ������ ���� ��ǥ

        continuousBlocks.Add(new Vector2Int(currentStart, row));

        // �������� ��� ����� Ȯ���ϸ� ���ӵ� ��� �׷��� ã���ϴ�.
        for (int x = 1; x < 11; x++)
        {

            // ���� ����� ������ �����ɴϴ�.
            Color32 currentColor = GetTileColorAtPosition(new Vector2Int(x, row));
            //vertWeight(x, row, currentColor, continuousBlocks, currentGroupColors/*,continuousBlocks);
            // ���� ����� ������ ���� ����� ����� ������ Ȯ���մϴ�.

            /*
            List<Vector2Int> buf = onlyUp(x, row + 1, previousColor);
            List<Vector2Int> merg = new List<Vector2Int>();
            merg.AddRange(continuousBlocks);
            merg.AddRange(buf);
            continuousBlocks = merg;
            */

            continuousBlocks = upStair(row, continuousBlocks);


            if (currentColor.Equals(previousColor) && currentColor != Color.clear)
            {
                // ���� ��ϰ� ���� ����� ������ ������ ���ӵ� ��� �׷��Դϴ�.
                //currentGroupColors.Add(currentColor);
                continuousBlocks.Add(new Vector2Int(x, row));
                UnityEngine.Debug.Log("������! \n");
            }
            else
            {
                // ���� ��ϰ� ���� ����� ������ �ٸ��� ���ӵ� ��� �׷��� �������ϴ�.
                // ���� ���ӵ� ��� �׷��� ����ġ�� Ȯ���ϰ�, 4 �̻��� ��쿡�� ����Ʈ�� �߰��մϴ�.
                if (continuousBlocks.Count >= 4)
                {
                    for (int i = x - continuousBlocks.Count; i < x; i++)
                    {
                        UnityEngine.Debug.Log("�� �߰�!! \n");
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

        return continuousBlocks;
    }

    List<Vector2Int> upStair(int row, List<Vector2Int> downStair)
    {
        Vector2Int remember = new Vector2Int(-100, -100);

        Color32 previousColor = GetTileColorAtPosition(downStair[0]);
        foreach (var block in downStair)
        {
            Color32 currentColor = GetTileColorAtPosition(new Vector2Int(block.x, row + 1));
            if (currentColor.Equals(previousColor) && currentColor != Color.clear && !downStair.Contains(new Vector2Int(block.x, row + 1)))
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
            Color32 right;
            Color32 left;
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








    // List<Vector2Int> onlyUp


    //   onlyDown



    //�÷�32 ��, 









    //���ӿ� ���� ����ġ�� ������ �߰��ϱ� ����
    void vertWeight(int x, int y, Color32 col, List<Vector2Int> contBlocks, List<Color32> colorGroup)
    {
        int yy = y + 1;
        GameObject rowObject = GameObject.Find("y_" + yy.ToString());
        if (rowObject != null)
        {
            string block = "x_" + x.ToString();
            Transform blockUp = rowObject.transform.Find(block);
            if (blockUp != null)
            {
                // ����� ã�ҽ��ϴ�
                SpriteRenderer spriteRenderer = blockUp.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color32 col2 = spriteRenderer.color;
                    if (col2.Equals(col) && col2 != Color.clear)
                    {
                        // ���� ������ ����� ã�����Ƿ� continuousBlocks�� colorGroup ����Ʈ�� ������ �߰��մϴ�.
                        contBlocks.Add(new Vector2Int(x, yy));
                        colorGroup.Add(col2);

                        // ��� ȣ���� ���� ���� ������ ����� Ž���մϴ�.
                        vertWeight(x, yy, col, contBlocks, colorGroup);
                    }
                }
            }
        }
    }



    Color32 GetTileColorAtPosition(Vector2Int position)
    {
        // ��ȿ�� ��ġ���� Ȯ���մϴ�.
        if (position.x >= 0 && position.x < boardWidth &&
            position.y >= 0 && position.y < boardHeight)
        {
            // �ش� ���� ���� ������Ʈ�� �����ɴϴ�.
            GameObject rowObject = GameObject.Find("y_" + position.y.ToString());

            // �ش� �࿡ �ִ� ��� ���� �����ɴϴ�.

            if (rowObject != null)
            {
                // �� ������Ʈ�� �߰ߵǸ� �ش� ���� �ڽ� ������Ʈ �߿��� x ��ǥ�� ���� �̸��� ���� ����� ã���ϴ�.
                string blockName = "x_" + position.x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);

                if (blockTransform != null)
                {
                    // ����� ã�ҽ��ϴ�
                    SpriteRenderer spriteRenderer = blockTransform.GetComponent<SpriteRenderer>();

                    Color32 coll = spriteRenderer.color;

                    return coll;
                }
                else
                {
                    // �ش� x ��ǥ�� ���� ����� �����ϴ�.
                }
            }
            else
            {
                // �ش� y ��ǥ�� ���� ���� �����ϴ�.
            }



        }

        // ��ġ�� ��ȿ���� �ʰų� �ش� ��ġ�� ���� ���� ��� �⺻������ ������� ��ȯ�մϴ�.
        return Color.clear;
    }

    /*void StructureBlock()
    {
       public Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
       

    }*/
    public void doPanalty()
    { // �г�Ƽ�ο� + �� �پ��
        int buff = 19 - panalty;
        for (int i = 0; i < 12; i++)
        {
            GameObject backRow = GameObject.Find("back" + buff.ToString());
            // backRow.transform.position = new Vector3Int(-50, 0,0);
            backRow.transform.name = "delete";//�̸��� �ٲ���� ������ ���� ������ ������, destroy�� ��� ������ �ƴ϶� �����̰� �����ϹǷ�, �ݺ��� �ð����� �Ȱɸ��°� ����
            Destroy(backRow);
        }
        panalty++;

    }

    //��ų ����

    public int redSkillNum; // �����ų �� ����
    public int yellowSkillNum; // �����ų �� ����
    public int blueSkillNum; // �����ų �� ����
    private Color fire = Color.black;
    private Color ice = Color.white;
    public UnityEngine.UI.Image lightening;
    public float fadeInImage = 0.1f; // �̹��� ��Ÿ���� �ð�
    public float fadeOutImage = 1.01f; // �̹��� ������� �ð�

    public Tile randomTile()
    {
        int y = UnityEngine.Random.Range(0, 20);
        var ynode = boardNode.Find("y_" + y.ToString());
        int x = UnityEngine.Random.Range(0, ynode.childCount);
        var xnode = ynode.GetChild(x);
        Tile tile = xnode.GetComponent<Tile>();
        return tile;
    }
    public void redSkill() // ������
    {
        Tile tile = randomTile();
        int i = 0;
        while(i < redSkillNum)
        {
            if (!tile.isFired)
            {
                tile.isFired = true;
                tile.color = fire;
                i++;
            }
        }
    }
    public void yellowSkill() // �����
    {
        StartCoroutine(yellowSkillActive());
    }
    IEnumerator yellowSkillActive()
    {
        float timer = 0;
        while (timer <= fadeInImage) // fade in
        {
            float alpha = Mathf.Lerp(0, 1, timer / fadeInImage);
            lightening.color = new Color(1, 1, 1, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        lightening.color = Color.white;
        int i = 0;
        while (i < yellowSkillNum) // ���� �� ����
        {
            Tile tile = randomTile();
            if(tile != null)
            {
                Destroy(tile);
            }
        }
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
    public void blueSkill() // �Ķ���
    {
        int i = 0;
        while (i < blueSkillNum)
        {
            Tile tile = randomTile();
            tile.preColor = tile.color; // ���� �� ����
            if (!tile.isIced)
            {
                tile.isIced = true;
                tile.color = ice;
                i++;
            }
        }
    }
}





