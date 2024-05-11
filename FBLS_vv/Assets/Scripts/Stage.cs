using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Stage : MonoBehaviour
{
    [Header("Editor Objects")]
    public GameObject tilePrefab; //타일프리펩 불러옴
    public Transform backgroundNode; // 백그라운드 
    public Transform boardNode; //게임판(각 열 y0 - y19까지의 노드)
    public Transform tetrominoNode; //테트리미노
   // public GameObject gameoverPanel; //게임오버
    public Text score; // 점수
    public Text red; // 사라진 블럭
    public Text green; // 사라진 블럭
    public Text blue; // 사라진 블럭
    public Text yellow; // 사라진 블럭
    public Transform preview; // 다음 블럭
    public GameObject start;

    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 10;
    [Range(5, 20)]
    public int boardHeight = 20;
    public float fallCycle = 1.0f;
    public int testp1 = 5;
    private int halfWidth; // 좌표 (가로)중앙값
    private int halfHeight; //좌표 (세로) 중앙값
    public int lineWeight; // 지워진 줄 점수
    public int colorWeight; // 지워진 색 점수
    
    private float nextFallTime;
    private int scoreVal = 0;
    private int indexVal = -1;
    private int arrIndexVal = -1;
    private int redVal = 0; // 사라진 블럭 개수
    private int greenVal = 0; // 사라진 블럭 개수
    private int blueVal = 0;   // 사라진 블럭 개수
    private int yellowVal = 0; // 사라진 블랙 개수
    public static int blockCount = 0;
    private bool isPaused = true;


    private void Start()
    {
  
        //gameoverPanel.SetActive(false);

        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); //(5)
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); //(10)

        nextFallTime = Time.time + fallCycle; //낙하주기 설정

        CreateBackground(); //백그라운드 생성 메소드

        for (int i = 0; i < boardHeight; ++i)  //보드 높이까지
        {
            var col = new GameObject("y_" + (boardHeight - i - 1).ToString());     //보드의 세로줄을 동적으로 생성하는중
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            col.transform.parent = boardNode;
        }
        create7Bag();
        CreateTetromino();  //테트리미노 생성 메소드 실행
        CreatePreview(); // 미리보기
        score.text = "Score: " + scoreVal; // 점수 출력
        PlayerPrefs.SetInt("score", scoreVal); // 점수 넘겨주기
        red.text = redVal.ToString(); //블럭 개수 출력
        green.text = greenVal.ToString(); // 블럭 개수 출력
        blue.text = blueVal.ToString(); // 블럭 개수 출력
        yellow.text = yellowVal.ToString(); // 블럭 개수 출력

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (isPaused)
        {
            if (Input.anyKeyDown)
            {
                Time.timeScale = 1f;
                start.SetActive(false);
                isPaused = false;
                return;
            }
        }
        else
        {

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

            // 아래로 떨어지는 경우는 강제로 이동시킵니다.
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
        // 이미 있는 미리보기 삭제하기
        foreach (Transform tile in preview)
        {
            Destroy(tile.gameObject);
        }
        preview.DetachChildren();

        indexVal = UnityEngine.Random.Range(0, 7);
        arrIndexVal = UnityEngine.Random.Range(0, 24);
        
        preview.position = new Vector2(halfWidth + 2.5f, halfHeight - 2.5f); // 미리보기 
        
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
                color = new Color32(235, 51, 35, 255);    // 빨간색
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
       
        GameObject gameObject0 = new GameObject(); // 새로운 게임 오브젝트 생성
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

    // 테트로미노를 보드에 추가
    void AddToBoard(Transform root)
    {
        // String keyTime = DateTime.Now.ToString("HHmmss"); //처음 생성될때 시분초값을 tag값으로 사용 <<<< 못써먹음 개가튼거
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);

            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            node.parent = boardNode.Find("y_" + y.ToString());
            node.name = "x_" + x.ToString();
            //node.tag = keyTime; <<< 못써먹음2
            //UnityEngine.Debug.Log(keyTime + "생성됨");
        }
    }

    // 보드에 완성된 행이 있으면 삭제
    void CheckBoardColumn()
    {
        bool isCleared = false;
      
        foreach (Transform column in boardNode)
        {
            if (column.childCount == boardWidth)// 완성된 행 == 행의 자식 갯수가 가로 크기
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




        // 비어 있는 행이 존재하면 아래로 당기기
        if (isCleared)
        {
            for (int i = 1; i < boardNode.childCount; ++i)
            {
                var column = boardNode.Find("y_" + i.ToString());

                // 이미 비어 있는 행은 무시
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
        //int x, y; //확인함
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
                    else floor = true; // 아래가 바닥
                }
            }
            Transform targetNode = boardNode.Find("x_" + blocksConnect[i][0]);
            targetNode.SetParent(boardNode.Find("y_" + (yBuffer - 1).ToString()));
        }
    }
    */
    void gravity(int startX, int startY)
    {
        for (int y = startY; y >= 0; y--) // 아래쪽부터 시작하여 위로 이동
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
                        // 현재 행의 위쪽 행에서 같은 열에 블록이 있는지 검사
                        var nextBlock = nextRowNode.transform.Find("x_" + x.ToString());
                        if (nextBlock == null)
                        {
                            // 위쪽에 블록이 없으면 블록을 아래로 이동
                            block.SetParent(nextRowNode.transform);
                            block.localPosition -= new Vector3(0, 1, 0); // 아래로 이동
                                                                         // 이동된 위치에 다른 블록이 있는지 확인
                            for (int i = y - 1; i >= 0; i--)
                            {
                                var tempRowNode = GameObject.Find("y_" + i.ToString());
                                var tempBlock = tempRowNode.transform.Find("x_" + x.ToString());
                                if (tempBlock != null)
                                {
                                    break; // 다음 블록을 확인하기 위해 반복문 탈출
                                }
                                else
                                {
                                    // 위쪽에 블록이 없으면 계속해서 이동
                                    block.SetParent(tempRowNode.transform);
                                    block.localPosition -= new Vector3(0, 1, 0); // 아래로 이동
                                    y = i; // y값 갱신
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    // 이동 가능한지 체크
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

    // 타일 생성
    Tile CreateTile(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab);
        go.transform.parent = parent;
        go.transform.localPosition = position;

        var tile = go.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;

        return tile;
    }

    // 배경 타일을 생성
    void CreateBackground()
    {
        Color color = Color.gray;

        // 타일 보드
        color.a = 0.5f;
        for (int x = -halfWidth; x < halfWidth; ++x)
        {
            for (int y = halfHeight; y > -halfHeight; --y)
            {
                CreateTile(backgroundNode, new Vector2(x, y), color, 0);// 배경 타일 생성하는 노드, 여기에 x,y 절편 먹이는걸로 위치 조정 가능?
            }
        }

        // 좌우 테두리
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode, new Vector2(-halfWidth - 1, y), color, 0);
            CreateTile(backgroundNode, new Vector2(halfWidth, y), color, 0);
        }

        // 아래 테두리
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            CreateTile(backgroundNode, new Vector2(x, -halfHeight), color, 0);
        }
    }

    // 테트로미노 생성
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
        {1, 1, 2, 3}, {1, 1, 2, 4}, {1, 1, 3, 2}, //색상을 결정하는 키값
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
        tetrominoNode.position = new Vector2(0, halfHeight);

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
            case 1: // 빨간색
                return new Color32(255, 0, 0, 255);
            case 2: // 파란색
                return new Color32(0, 0, 255, 255);
            case 3: // 노란색
                return new Color32(255, 255, 0, 255);
            case 4: // 초록색
                return new Color32(0, 255, 0, 255);
            default:
                return Color.white;
        }
    }




    void CheckTileGroups() // 4개 조건을 만족한 블럭들 탐지/삭제하는 메소드
    {
        // 게임 보드의 모든 행을 순회합니다.
        for (int y = 0; y < boardHeight; y++)
        {
            // 해당 행에서 연속된 블록 그룹을 탐색합니다.
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
                    // 게임 오브젝트를 찾았으므로 삭제합니다.
                    Destroy(blockTransform.gameObject);
                    UnityEngine.Debug.Log("블록 삭제됨: " + blockName);
                    gravity(xgrav, ygrav);
                    scoreVal += colorWeight;
                    blockCount++;
                    score.text = "Score: " + scoreVal;
                    PlayerPrefs.SetInt("score", scoreVal);
                    //gravity(blockName, y);
                }
                else
                {
                    // 게임 오브젝트를 찾지 못했음을 알립니다.
                    UnityEngine.Debug.LogWarning("게임 오브젝트를 찾을 수 없습니다: " + blockName);
                }
            }

        }
    }


    //List<Vector2Int> FindContinuousBlocksInRow(int row) //연속되는 블록 탐색 메소드
    //{
    //    List<Vector2Int> continuousBlocks = new List<Vector2Int>();


    //    List<Vector2Int> currentGroup = new List<Vector2Int>(); //연속 블럭그룹
    //                                                            // 첫 번째 블록의 색상을 가져옵니다.
    //    UnityEngine.Debug.Log(" 첫 블록 색상 ");
    //    Color32 previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

    //    // 현재 연속된 블록의 가중치를 초기화합니다.
    //    int currentWeight = 1;


    //    // 좌측부터 모든 블록을 확인하며 연속된 블록 그룹을 찾습니다.
    //    for (int x = 1 ; x < boardWidth ; x++)
    //    {
    //        // 현재 블록의 색상을 가져옵니다.
    //        Color32 currentColor = GetTileColorAtPosition(new Vector2Int(x, row));

    //        currentWeight += vertWeight(x, row, currentColor/*,continuousBlocks*/);


    //        // 현재 블록의 색상이 이전 블록의 색상과 같은지 확인합니다.
    //        if (currentColor.Equals(previousColor))
    //        {
    //            // 이전 블록과 현재 블록의 색상이 같으면 연속된 블록 그룹입니다.
    //            currentWeight++;
    //            UnityEngine.Debug.Log(" 연속임! \n ");
    //        }
    //        else
    //        {
    //            // 이전 블록과 현재 블록의 색상이 다르면 연속된 블록 그룹이 끝났습니다.
    //            // 현재 연속된 블록 그룹의 가중치를 확인하고, 3 이상인 경우에만 리스트에 추가합니다.
    //            if (currentWeight >= 4)
    //            {
    //                for (int i = x - currentWeight; i < x; i++)
    //                {
    //                    continuousBlocks.Add(new Vector2Int(i, row));
    //                    UnityEngine.Debug.Log(" 블럭 추가!! \n ");
    //                }
    //            }

    //            // 가중치를 초기화합니다.
    //            currentWeight = 1;
    //        }

    //        // 현재 블록의 색상을 이전 색상으로 설정합니다.
    //        previousColor = currentColor;
    //    }

    //    return continuousBlocks;
    //}
    List<Vector2Int> FindContinuousBlocksInRow(int row)
    {
        List<Vector2Int> continuousBlocks = new List<Vector2Int>();

        List<Color32> currentGroupColors = new List<Color32>(); // 연속된 블록 그룹의 색상을 저장하는 리스트

        // 첫 번째 블록의 색상을 가져옵니다.
        UnityEngine.Debug.Log("첫 블록 색상");
        Color32 previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

        // 좌측부터 모든 블록을 확인하며 연속된 블록 그룹을 찾습니다.
        for (int x = 0; x < boardWidth; x++)
        {
            // 현재 블록의 색상을 가져옵니다.
            Color32 currentColor = GetTileColorAtPosition(new Vector2Int(x, row));
            vertWeight(x, row, currentColor, continuousBlocks, currentGroupColors/*,continuousBlocks*/);
            // 현재 블록의 색상이 이전 블록의 색상과 같은지 확인합니다.
            if (currentColor.Equals(previousColor) && currentColor != Color.clear)
            {
                // 이전 블록과 현재 블록의 색상이 같으면 연속된 블록 그룹입니다.
                currentGroupColors.Add(currentColor);
                UnityEngine.Debug.Log("연속임! \n");
            }
            else
            {
                // 이전 블록과 현재 블록의 색상이 다르면 연속된 블록 그룹이 끝났습니다.
                // 현재 연속된 블록 그룹의 가중치를 확인하고, 4 이상인 경우에만 리스트에 추가합니다.
                if (currentGroupColors.Count >= 4)
                {
                    for (int i = x - currentGroupColors.Count; i < x; i++)
                    {
                        continuousBlocks.Add(new Vector2Int(i, row));
                        UnityEngine.Debug.Log("블럭 추가!! \n");
                    }
                }

                // 현재 연속된 블록 그룹 초기화
                currentGroupColors.Clear();
            }

            // 현재 블록의 색상을 이전 색상으로 설정합니다.
            previousColor = currentColor;
        }

        return continuousBlocks;
    }
    //연속에 대한 가중치에 수직을 추가하기 위함
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
                // 블록을 찾았습니다
                SpriteRenderer spriteRenderer = blockUp.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color32 col2 = spriteRenderer.color;
                    if (col2.Equals(col) && col2 != Color.clear)
                    {
                        // 같은 색상의 블록을 찾았으므로 continuousBlocks와 colorGroup 리스트에 정보를 추가합니다.
                        contBlocks.Add(new Vector2Int(x, yy));
                        colorGroup.Add(col2);

                        // 재귀 호출을 통해 위쪽 방향의 블록을 탐색합니다.
                        vertWeight(x, yy, col, contBlocks, colorGroup);
                    }
                }
            }
        }
    }



    Color32 GetTileColorAtPosition(Vector2Int position)
    {
        // 유효한 위치인지 확인합니다.
        if (position.x >= 0 && position.x < boardWidth &&
            position.y >= 0 && position.y < boardHeight)
        {
            // 해당 행의 게임 오브젝트를 가져옵니다.
            GameObject rowObject = GameObject.Find("y_" + position.y.ToString());

            // 해당 행에 있는 모든 블럭을 가져옵니다.

            if (rowObject != null)
            {
                // 행 오브젝트가 발견되면 해당 행의 자식 오브젝트 중에서 x 좌표와 같은 이름을 가진 블록을 찾습니다.
                string blockName = "x_" + position.x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);

                if (blockTransform != null)
                {
                    // 블록을 찾았습니다
                    SpriteRenderer spriteRenderer = blockTransform.GetComponent<SpriteRenderer>();

                    Color32 coll = spriteRenderer.color;

                    return coll;
                }
                else
                {
                    // 해당 x 좌표를 가진 블록이 없습니다.
                }
            }
            else
            {
                // 해당 y 좌표를 가진 행이 없습니다.
            }



        }

        // 위치가 유효하지 않거나 해당 위치에 블럭이 없는 경우 기본값으로 투명색을 반환합니다.
        return Color.clear;
    }

    /*void StructureBlock()
    {
       public Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
       

    }*/

}




