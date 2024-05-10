using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StageMulti : MonoBehaviour
{
    [Header("Editor Objects")]
    public GameObject tilePrefab; //Ÿ�������� �ҷ���
    public Transform backgroundNode; // ��׶��� 
    public Transform boardNode; //������(�� �� y0 - y19������ ���)
    

    public Transform tetrominoNode; //��Ʈ���̳�
   // public GameObject gameoverPanel; //���ӿ���
    public Text score; // ����
    public Text red; // ����� ��
    public Text green; // ����� ��
    public Text blue; // ����� ��
    public Text yellow; // ����� ��
    public Transform preview; // ���� ��
    public GameObject start;
    


    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 10;
    [Range(5, 20)]
    public int boardHeight = 20;
    public float fallCycle = 1.0f;
    public int testp1 = 5;
    private int halfWidth; // ��ǥ (����)�߾Ӱ�
    private int halfHeight; //��ǥ (����) �߾Ӱ�
    public int lineWeight; // ������ �� ����
    public int colorWeight; // ������ �� ����
    public int panalty = 0; // �г�Ƽ�� ����� ����ġ
    public int indexback = 0;

    public float offsetX = 0f;
    public float offsetY = 0f;

    public int offset2p = 14;


    private float nextFallTime;
    private int scoreVal = 0;
    private int indexVal = -1;
    private int arrIndexVal = -1;
    private int redVal = 0; // ����� �� ����
    private int greenVal = 0; // ����� �� ����
    private int blueVal = 0;   // ����� �� ����
    private int yellowVal = 0; // ����� �� ����
    public static int blockCount = 0;
    public GameObject[] backs = new GameObject[200];
    public GameObject[] backgrid = new GameObject[40];


    private void Start()
    {
  
        //gameoverPanel.SetActive(false);

        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); //(5)
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); //(10)

        nextFallTime = Time.time + fallCycle; //�����ֱ� ����

        CreateBackground(); //��׶��� ���� �޼ҵ�

        for (int i = 0; i < boardHeight; ++i)  //���� ���̱���
        {
            var col = new GameObject("y_" + (boardHeight - i - 1).ToString());     //������ �������� �������� �����ϴ���
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            col.transform.parent = boardNode;
        }

       
        create7Bag();
        CreateTetromino();  //��Ʈ���̳� ���� �޼ҵ� ����
        CreatePreview(); // �̸�����
        score.text = "Score: " + scoreVal; // ���� ���
        PlayerPrefs.SetInt("score", scoreVal); // ���� �Ѱ��ֱ�
        red.text = redVal.ToString(); //�� ���� ���
        green.text = greenVal.ToString(); // �� ���� ���
        blue.text = blueVal.ToString(); // �� ���� ���
        yellow.text = yellowVal.ToString(); // �� ���� ���

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Time.timeScale = 1f;
            start.SetActive(false);
        }
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
        
        preview.position = new Vector2(halfWidth + 2.5f, halfHeight - 2.5f); // �̸����� 
        
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
        node0.transform.name = "node0";
        node0.transform.position = new Vector2(-20,0);

        Transform node1 = gameObject1.transform;
        node1.transform.name = "node1";
        node1.transform.position = new Vector2(-24,0);

        Transform node2 = gameObject2.transform;
        node2.transform.name = "node2";
        node2.transform.position = new Vector2(-28,0);

        Transform node3 = gameObject3.transform;
        node3.transform.name = "node3";
        node3.transform.position = new Vector2(-32,0);

        Transform node4 = gameObject4.transform;
        node4.transform.name = "node4";
        node4.transform.position = new Vector2(-36,0);

        Transform node5 = gameObject5.transform;
        node5.transform.name = "node5";
        node5.transform.position = new Vector2(-40,0);
        
        Transform node6 = gameObject6.transform;
        node6.transform.name = "node6";
        node6.transform.position = new Vector2(-44,0);

        for( int i = 0 ; i < 7 ; i++){
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
        // String keyTime = DateTime.Now.ToString("HHmmss"); //ó�� �����ɶ� �ú��ʰ��� tag������ ��� <<<< ������� ����ư��
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);

            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            node.parent = boardNode.Find("y_" + y.ToString());
            node.name = "x_" + x.ToString();
            //node.tag = keyTime; <<< �������2
            //UnityEngine.Debug.Log(keyTime + "������");
        }
    }

    // ���忡 �ϼ��� ���� ������ ����
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
    }
    */
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
                        // ���� ���� ���� �࿡�� ���� ���� ����� �ִ��� �˻�
                        var nextBlock = nextRowNode.transform.Find("x_" + x.ToString());
                        if (nextBlock == null)
                        {
                            // ���ʿ� ����� ������ ����� �Ʒ��� �̵�
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
                                    // ���ʿ� ����� ������ ����ؼ� �̵�
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
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            if (x < 0 || x > boardWidth - 1)
                return false;

            if (y < 0)
                return false;

            var column = boardNode.Find("y_" + y.ToString());

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

        var tile = go.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;
       // tile.transform.name = "tile" + position.x.ToString() + "_" + position.y.ToString();
        return tile;
    }

    Tile Createback(Transform parent, Vector2 position, Color color, int order = 1)
    {
        

    var go = Instantiate(tilePrefab);
        go.transform.parent = parent;
        go.transform.localPosition = position;

        var tile = go.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;
       // tile.transform.name = "tile" + position.x.ToString() + "_" + position.y.ToString();
       backs[indexback] = tile.gameObject;
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
                Createback(backgroundNode, new Vector2(x, y), color, 0);//
                

            }
        }

        // �¿� �׵θ�
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode, new Vector2(-halfWidth - 1, y), color, 0);
            CreateTile(backgroundNode, new Vector2(halfWidth, y), color, 0);
        }

        // �Ʒ� �׵θ�
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            CreateTile(backgroundNode, new Vector2(x, -halfHeight), color, 0);
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
        if(arrIndexVal == -1)
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
        tetrominoNode.position = new Vector2(0, halfHeight - panalty);

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




    void CheckTileGroups() // 4�� ������ ������ ���� Ž��/�����ϴ� �޼ҵ�
    {
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

                if (blockTransform != null)
                {
                    int ygrav = y;
                    // ���� ������Ʈ�� ã�����Ƿ� �����մϴ�.
                    Destroy(blockTransform.gameObject);
                    UnityEngine.Debug.Log("��� ������: " + blockName);
                    gravity(xgrav, ygrav);
                    scoreVal += colorWeight;
                    blockCount++;
                    score.text = "Score: " + scoreVal;
                    PlayerPrefs.SetInt("score", scoreVal);
                    //gravity(blockName, y);
                }
                else
                {
                    // ���� ������Ʈ�� ã�� �������� �˸��ϴ�.
                    UnityEngine.Debug.LogWarning("���� ������Ʈ�� ã�� �� �����ϴ�: " + blockName);
                }
            }

        }
    }


    //List<Vector2Int> FindContinuousBlocksInRow(int row) //���ӵǴ� ��� Ž�� �޼ҵ�
    //{
    //    List<Vector2Int> continuousBlocks = new List<Vector2Int>();


    //    List<Vector2Int> currentGroup = new List<Vector2Int>(); //���� ���׷�
    //                                                            // ù ��° ����� ������ �����ɴϴ�.
    //    UnityEngine.Debug.Log(" ù ��� ���� ");
    //    Color32 previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

    //    // ���� ���ӵ� ����� ����ġ�� �ʱ�ȭ�մϴ�.
    //    int currentWeight = 1;


    //    // �������� ��� ����� Ȯ���ϸ� ���ӵ� ��� �׷��� ã���ϴ�.
    //    for (int x = 1 ; x < boardWidth ; x++)
    //    {
    //        // ���� ����� ������ �����ɴϴ�.
    //        Color32 currentColor = GetTileColorAtPosition(new Vector2Int(x, row));

    //        currentWeight += vertWeight(x, row, currentColor/*,continuousBlocks*/);


    //        // ���� ����� ������ ���� ����� ����� ������ Ȯ���մϴ�.
    //        if (currentColor.Equals(previousColor))
    //        {
    //            // ���� ��ϰ� ���� ����� ������ ������ ���ӵ� ��� �׷��Դϴ�.
    //            currentWeight++;
    //            UnityEngine.Debug.Log(" ������! \n ");
    //        }
    //        else
    //        {
    //            // ���� ��ϰ� ���� ����� ������ �ٸ��� ���ӵ� ��� �׷��� �������ϴ�.
    //            // ���� ���ӵ� ��� �׷��� ����ġ�� Ȯ���ϰ�, 3 �̻��� ��쿡�� ����Ʈ�� �߰��մϴ�.
    //            if (currentWeight >= 4)
    //            {
    //                for (int i = x - currentWeight; i < x; i++)
    //                {
    //                    continuousBlocks.Add(new Vector2Int(i, row));
    //                    UnityEngine.Debug.Log(" �� �߰�!! \n ");
    //                }
    //            }

    //            // ����ġ�� �ʱ�ȭ�մϴ�.
    //            currentWeight = 1;
    //        }

    //        // ���� ����� ������ ���� �������� �����մϴ�.
    //        previousColor = currentColor;
    //    }

    //    return continuousBlocks;
    //}
    List<Vector2Int> FindContinuousBlocksInRow(int row)
    {
        List<Vector2Int> continuousBlocks = new List<Vector2Int>();

        List<Color32> currentGroupColors = new List<Color32>(); // ���ӵ� ��� �׷��� ������ �����ϴ� ����Ʈ

        // ù ��° ����� ������ �����ɴϴ�.
        UnityEngine.Debug.Log("ù ��� ����");
        Color32 previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

        // �������� ��� ����� Ȯ���ϸ� ���ӵ� ��� �׷��� ã���ϴ�.
        for (int x = 0; x < boardWidth; x++)
        {
            // ���� ����� ������ �����ɴϴ�.
            Color32 currentColor = GetTileColorAtPosition(new Vector2Int(x, row));
            vertWeight(x, row, currentColor, continuousBlocks, currentGroupColors/*,continuousBlocks*/);
            // ���� ����� ������ ���� ����� ����� ������ Ȯ���մϴ�.
            if (currentColor.Equals(previousColor) && currentColor != Color.clear)
            {
                // ���� ��ϰ� ���� ����� ������ ������ ���ӵ� ��� �׷��Դϴ�.
                currentGroupColors.Add(currentColor);
                UnityEngine.Debug.Log("������! \n");
            }
            else
            {
                // ���� ��ϰ� ���� ����� ������ �ٸ��� ���ӵ� ��� �׷��� �������ϴ�.
                // ���� ���ӵ� ��� �׷��� ����ġ�� Ȯ���ϰ�, 4 �̻��� ��쿡�� ����Ʈ�� �߰��մϴ�.
                if (currentGroupColors.Count >= 4)
                {
                    for (int i = x - currentGroupColors.Count; i < x; i++)
                    {
                        continuousBlocks.Add(new Vector2Int(i, row));
                        UnityEngine.Debug.Log("�� �߰�!! \n");
                    }
                }

                // ���� ���ӵ� ��� �׷� �ʱ�ȭ
                currentGroupColors.Clear();
            }

            // ���� ����� ������ ���� �������� �����մϴ�.
            previousColor = currentColor;
        }

        return continuousBlocks;
    }
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
    public void doPanalty(){
        for(int i = 0; i < 10; i++){
            Destroy(backs[indexback - i]);
            indexback--;
        }
         panalty++;
        
    }
}




