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


public class StageMulti : MonoBehaviour
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
   

    public Transform preview; // ���� ����
    public GameObject start;
    private Skill skill;
    private SkillManager skillManager;

    public BlockPosition blockPos; // ���� ����ü
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

    public int offset2p = 14;

    private float nextFallTime;
    public static int scoreVal = 0;
    private int indexVal = -1;
    private int arrIndexVal = -1;
    public static int redVal = 0; // ����� ���� ����
    public static int greenVal = 0; // ����� ���� ����
    public static int blueVal = 0;   // ����� ���� ����
    public static int yellowVal = 0; // ����� ���� ����
    public static int blockCount = 0;
   
    private bool isPaused = true;
    private void Start()
    {

        //gameoverPanel.SetActive(false);
        blockPos = new BlockPosition();
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); //(5)
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); //(10)

        nextFallTime = Time.time + fallCycle; //�����ֱ� ����
        //blockArray = new BlockArray(); //���� ������ ����ü ����
        //CreateBackground(); //��׶��� ���� �޼ҵ�

        for (int i = 0; i < boardHeight; ++i)  //���� ���̱���
        {
            GameObject col = Instantiate(nodePrefab);
            
            col.name = "y_" + (boardHeight - i - 1).ToString();
            col.transform.position = new Vector3(offset2p*2, halfHeight - i, 0);
           
            //* var col = new GameObject("y_" + (boardHeight - i - 1).ToString());     //������ �������� �������� �����ϴ���
           
            col.transform.parent = boardNode;
        }

        /*�ؾ��� ��
         1. ��Ʈ���� ���� 7�� ���� �ҷ����� + ����
         
         2. 7�鿡�� ��Ʈ���̳� �޾ƿ� �����ϱ�

         3. �ι�° ��Ͽ� �ִ� ��Ʈ���� ���� �̸������ �ű��

         4. (2~3) �տ� �������� -> ���� 7�� �����Ѱ� �� ��ٸ� ? �ٽ� 1�� ����
            
         �ʿ��Ѱ�, CreateTile�� void�� �����°� �ƴ� GameObject�� �迭�� return����!
        */


        create7Bag();
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

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveDir.x = -1;

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDir.x = 1;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isRotate = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveDir.y = -1;
            }

            if (Input.GetKeyDown(KeyCode.Space))
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
            
            if (Input.GetKeyDown(KeyCode.M) && MultiManager.Instance.redButton2.activeSelf){
                MultiManager.Instance.AtkRed2();
                updateBlock();
        
                UnityEngine.Debug.Log("Red skill!");
            }
            if (Input.GetKeyDown(KeyCode.Comma) && MultiManager.Instance.blueButton2.activeSelf)
            {
                MultiManager.Instance.AtkBlue2();
                UnityEngine.Debug.Log("Blue skill!");
                updateBlock();
            }
            if (Input.GetKeyDown(KeyCode.Period) && MultiManager.Instance.yellowButton2.activeSelf)
            {
                MultiManager.Instance.AtkYellow2();
                UnityEngine.Debug.Log("Yellow skill!");
                updateBlock();
            }
            if (Input.GetKeyDown(KeyCode.Slash) && MultiManager.Instance.greenButton2.activeSelf)
            {
                UnityEngine.Debug.Log("Green skill!");
                MultiManager.Instance.Green2();
                updateBlock();
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

        indexVal = UnityEngine.Random.Range(0, 7);
        arrIndexVal = UnityEngine.Random.Range(0, 24);

        preview.position = new Vector2(halfWidth + 3.3f+ 2f*offset2p , halfHeight - 2.5f); // �̸����� 

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
        int col1;
        int col2;
        int col3;
        int col4;
        //List<Transform> list7Bag = new List<Transform>();

        GameObject gameObject0 = new GameObject(); // ���ο� ���� ������Ʈ ����
        GameObject gameObject1 = new GameObject();
        GameObject gameObject2 = new GameObject();
        GameObject gameObject3 = new GameObject();
        GameObject gameObject4 = new GameObject();
        GameObject gameObject5 = new GameObject();
        GameObject gameObject6 = new GameObject();

        Transform node0 = gameObject0.transform;
        node0.transform.name = "node0";
        node0.transform.position = new Vector2(-20, 0);

        Transform node1 = gameObject1.transform;
        node1.transform.name = "node1";
        node1.transform.position = new Vector2(-24, 0);

        Transform node2 = gameObject2.transform;
        node2.transform.name = "node2";
        node2.transform.position = new Vector2(-28, 0);

        Transform node3 = gameObject3.transform;
        node3.transform.name = "node3";
        node3.transform.position = new Vector2(-32, 0);

        Transform node4 = gameObject4.transform;
        node4.transform.name = "node4";
        node4.transform.position = new Vector2(-36, 0);

        Transform node5 = gameObject5.transform;
        node5.transform.name = "node5";
        node5.transform.position = new Vector2(-40, 0);

        Transform node6 = gameObject6.transform;
        node6.transform.name = "node6";
        node6.transform.position = new Vector2(-44, 0);

        for (int i = 0; i < 7; i++)
        {
            arrIndexVal = UnityEngine.Random.Range(0, 24);
            col1 = colorArray[arrIndexVal, 0];
            col2 = colorArray[arrIndexVal, 1];
            col3 = colorArray[arrIndexVal, 2];
            col4 = colorArray[arrIndexVal, 3];

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
                SoundManager.Instance.playSfx(SfxType.Fall);
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
        //SoundManager.Instance.playSfx(SfxType.Destroy);
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

                        Destroy(tile.transform.GetChild(0).gameObject);
                        //currentTile.color = currentTile.preColor;
                        // UnityEngine.Debug.Log("�����");

                        currentTile.color = currentTile.preColor;
                        // UnityEngine.Debug.Log("�����");
                        SoundManager.Instance.playSfx(SfxType.Uniced);

                    }
                    else
                    {

                        tilesToRemove.Add(currentTile); // �Ⱦ������ ���� ����Ʈ �߰�
                        //UnityEngine.Debug.Log("�Ⱦ����");
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
                        UnityEngine.Debug.Log("countGreen");
                    }
                    else if (tile.color == Color.yellow)
                    {
                        yellowVal++;
                    }
                    updateBlock(); // ���� ������Ʈ
                    Destroy(tile.gameObject);
                    EffectManager.instance.Effect(tile.gameObject);
                    List<Transform> fallList2 = GetEx(tile.transform);
                    tilesFall.Add(fallList2);
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
        tetrominoNode.position = new Vector2(offset_x + 2*offset2p , halfHeight - panalty + offset_y); // ���� ���� ��ġ

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



    
    private void CheckTileGroups() // 4�� ������ ������ ������ Ž��/�����ϴ� �޼ҵ�
    {
        //SoundManager.Instance.playSfx(SfxType.Destroy);
        List<List<Transform>> tilesFall = new List<List<Transform>>();
        // ���� ������ ��� ���� ��ȸ�մϴ�.
        for (int y = 0; y < boardHeight; y++)
        {
            // �ش� �࿡�� ���ӵ� ���� �׷��� Ž���մϴ�.
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
                    if (!tile.isIced) { // �� ����� ��

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
                        tilesFall.Add(fallList2);


                        Destroy(blockTransform.gameObject);
                        EffectManager.instance.Effect(blockTransform.gameObject);
                        updateBlock();
                        UnityEngine.Debug.Log("���� ������: " + blockName);
                       
                    }
                    else
                    {
                        tile.isIced = false;

                        Destroy(tile.transform.GetChild(0).gameObject);
                        SoundManager.Instance.playSfx(SfxType.Uniced);

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
        List<Vector2Int> continuousBlocks = new List<Vector2Int>(); //���ӵ� ���� �� ��ü�� �����ϴ� ����Ʈ

        

        // ù ��° ������ ������ �����ɴϴ�.

        //UnityEngine.Debug.Log("ù ���� ����");
        string previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

        int currentStart = 0; // ������ ���� ��ǥ

        continuousBlocks.Add(new Vector2Int(currentStart, row));

        // �������� ��� ������ Ȯ���ϸ� ���ӵ� ���� �׷��� ã���ϴ�.
        for (int x = 1; x < 11; x++)
        {

            // ���� ������ ������ �����ɴϴ�.
            string currentColor = GetTileColorAtPosition(new Vector2Int(x, row));
            

            continuousBlocks = upStair(row, continuousBlocks);


            if (currentColor.Equals(previousColor) && currentColor != "null")
            {
                // ���� ���ϰ� ���� ������ ������ ������ ���ӵ� ���� �׷��Դϴ�.
                //currentGroupColors.Add(currentColor);
                continuousBlocks.Add(new Vector2Int(x, row));
               // UnityEngine.Debug.Log("������! \n");
            }
            else
            {
                // ���� ���ϰ� ���� ������ ������ �ٸ��� ���ӵ� ���� �׷��� �������ϴ�.
                // ���� ���ӵ� ���� �׷��� ����ġ�� Ȯ���ϰ�, 4 �̻��� ��쿡�� ����Ʈ�� �߰��մϴ�.
                if (continuousBlocks.Count >= 4)
                {
                    for (int i = x - continuousBlocks.Count; i < x; i++)
                    {
                       // UnityEngine.Debug.Log("���� �߰�!! \n");
                    }
                    return continuousBlocks;
                }

                // ���� ���ӵ� ���� �׷� �ʱ�ȭ
                continuousBlocks.Clear();
                continuousBlocks.Add(new Vector2Int(x, row));
            }

            // ���� ������ ������ ������ �������� �����մϴ�.
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

            // �ش� �࿡ �ִ� ��� ������ �����ɴϴ�.

            if (rowObject != null)
            {
                // �� ������Ʈ�� �߰ߵǸ� �ش� ���� �ڽ� ������Ʈ �߿��� x ��ǥ�� ���� �̸��� ���� ������ ã���ϴ�.
                string blockName = "x_" + position.x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);

                if (blockTransform != null)
                {
                    // ������ ã�ҽ��ϴ�
                    Tile tile = blockTransform.GetComponent<Tile>();
                    string coll = tile.getColor();

                    return coll;
                }
                else
                {
                 //UnityEngine.Debug.Log("�ش� ��ǥ ���� ��ã��!!:" +blockName );       
                // �ش� x ��ǥ�� ���� ������ �����ϴ�.
                }
            }
            else
            {
                // �ش� y ��ǥ�� ���� ���� �����ϴ�.
            }



        }

        // ��ġ�� ��ȿ���� �ʰų� �ش� ��ġ�� ������ ���� ��� �⺻������ �������� ��ȯ�մϴ�.
        string a = "null";
        return a;
    }

    /*void StructureBlock()
    {
       public Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
       

    }*/
    public void doPanalty()
    { // �г�Ƽ�ο� + �� �پ��
        //int buff = 19 - panalty;
        //for (int i = 0; i < 12; i++)
        //{
           // Transform backRow = backgroundNode.transform.Find("back" + buff.ToString());
            // backRow.transform.position = new Vector3Int(-50, 0,0);
            //backRow.transform.name = "delete";//�̸��� �ٲ���� ������ ���� ������ ������, destroy�� ��� ������ �ƴ϶� �����̰� �����ϹǷ�, �ݺ��� �ð����� �Ȱɸ��°� ����
            //Destroy(backRow);
        //}
        panalty++;
        panaltyVal++;
        SoundManager.Instance.playSfx(SfxType.Panalty);
    }

    //��ų ����
    [Header("Skill")]
    public int redSkillNum; // �����ų ���� ����
    public int yellowSkillNum; // �����ų ���� ����
    public int blueSkillNum; // �����ų ���� ����
    public int maxBlock;
    private Color fire = Color.black;
    private Color ice = Color.gray;
    public GameObject ice_1;
    public UnityEngine.UI.Image lightening;
    public float fadeInImage = 0.1f; // �̹��� ��Ÿ���� �ð�
    public float fadeOutImage = 1.01f; // �̹��� ������� �ð�
    public TextMeshProUGUI red; // ����� ����
    public TextMeshProUGUI green; // ����� ����
    public TextMeshProUGUI blue; // ����� ����
    public TextMeshProUGUI yellow; // ����� ����


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


    //IEnumerator yellowSkillActive()
    //{
        
    //    image.SetActive(true);
    //    float timer = 0;
    //    while (timer <= fadeInImage) // fade in
    //    {
    //        float alpha = Mathf.Lerp(0, 1, timer / fadeInImage);
    //        lightening.color = new Color(1, 1, 1, alpha);
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }
    //    lightening.color = Color.white;
        
        
            

    //    timer = 0;
    //    while (timer <= fadeOutImage) // fade out
    //    {
    //        float alpha = Mathf.Lerp(1, 0, timer / fadeOutImage);
    //        lightening.color = new Color(1, 1, 1, alpha);
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }

    //    lightening.color = new Color(1, 1, 1, 0); // ȭ�� �����

    //}
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
                    //tile.color = ice;
                    GameObject Ice = Instantiate(ice_1) as GameObject;
                    Ice.transform.position = tile.gameObject.transform.position;
                    Ice.transform.parent = tile.gameObject.transform;
                    i++;
                }
            }
        }
    }
    public void updateBlock()
    {
        red.text = $"{redVal}"; //���� ���� ���
        green.text = $"{greenVal}"; // ���� ���� ���
        blue.text = $"{blueVal}"; // ���� ���� ���
        yellow.text = $"{yellowVal}"; // ���� ���� ���
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





