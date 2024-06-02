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
    public GameObject nodePrefab; //노드(가로줄) 프리펩
    public GameObject tilePrefab; //타일프리펩 불러옴
    public GameObject tileRed;
    public GameObject tileBlue;
    public GameObject tileGreen;
    public GameObject tileYellow;
    public GameObject sevenBag;

    public Transform backgroundNode; // 백그라운드 
    public Transform boardNode; //게임판(각 열 y0 - y19까지의 노드)
   

    public Transform tetrominoNode; //테트리미노
                                    // public GameObject gameoverPanel; //게임오버
    public TextMeshProUGUI score; // 점수
   

    public Transform preview; // 다음 블럭
    public GameObject start;
    private Skill skill;
    private SkillManager skillManager;

    public BlockPosition blockPos; // 블럭 구조체
    public static bool lose = false; //천장에 닿았는지
    public Transform ghostNode;

    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 10;
    [Range(5, 20)]
    public int boardHeight = 20;
    public float fallCycle = 1.0f;

    private int halfWidth; // 좌표 (가로)중앙값
    private int halfHeight; //좌표 (세로) 중앙값
    public int lineWeight; // 지워진 줄 점수
    public int colorWeight; // 지워진 색 점수
    public int panalty = 0; // 패널티시 생기는 가중치
    public int indexback = 0;
    public static int panaltyVal = 0;



    public float offset_x = 0f;
    public float offset_y = 0f;

    public int offset2p = 14;

    private float nextFallTime;
    public static int scoreVal = 0;
    private int indexVal = -1;
    private int arrIndexVal = -1;
    public static int redVal = 0; // 사라진 블럭 개수
    public static int greenVal = 0; // 사라진 블럭 개수
    public static int blueVal = 0;   // 사라진 블럭 개수
    public static int yellowVal = 0; // 사라진 블랙 개수
    public static int blockCount = 0;
   
    private bool isPaused = true;

    private void Start()
    {

        //gameoverPanel.SetActive(false);
        blockPos = new BlockPosition();
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); //(5)
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); //(10)

        nextFallTime = Time.time + fallCycle; //낙하주기 설정
        //blockArray = new BlockArray(); //블럭 저장할 구조체 선언
        //CreateBackground(); //백그라운드 생성 메소드

        for (int i = 0; i < boardHeight; ++i)  //보드 높이까지
        {
            GameObject col = Instantiate(nodePrefab);
            
            col.name = "y_" + (boardHeight - i - 1).ToString();
            col.transform.position = new Vector3(offset2p*2, halfHeight - i, 0);
           
            //* var col = new GameObject("y_" + (boardHeight - i - 1).ToString());     //보드의 가로줄을 동적으로 생성하는중
           
            col.transform.parent = boardNode;
        }

        /*해야할 일
         1. 테트리스 블록 7백 노드로 불러오기 + 섞기
         
         2. 7백에서 테트리미노 받아와 생성하기

         3. 두번째 목록에 있는 테트리스 블록 미리보기로 옮기기

         4. (2~3) 앞에 조건으로 -> 만약 7백 생성한걸 다 썼다면 ? 다시 1번 실행
            
         필요한것, CreateTile을 void로 끝나는게 아닌 GameObject의 배열로 return받자!
        */


        create7Bag();
        CreateTetromino();  //테트리미노 생성 메소드 실행
        CreatePreview(); // 미리보기
        updateScore(); // 점수 출력
        PlayerPrefs.SetInt("score", scoreVal); // 점수 넘겨주기


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
        // 이미 있는 미리보기 삭제하기
        foreach (Transform tile in preview)
        {
            Destroy(tile.gameObject);
        }
        preview.DetachChildren();

        indexVal = UnityEngine.Random.Range(0, 7);
        arrIndexVal = UnityEngine.Random.Range(0, 24);

        preview.position = new Vector2(halfWidth + 3.3f+ 2f*offset2p , halfHeight - 2.5f); // 미리보기 

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

        GameObject gameObject0 = new GameObject(); // 새로운 게임 오브젝트 생성
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
  
    // 테트로미노를 보드에 추가
    void AddToBoard(Transform root)
    {
        if (root == null)
        {
            UnityEngine.Debug.LogError("Root transform is null.");
            return;
        }
        int randomKey = UnityEngine.Random.Range(100000, 999999); // 100000부터 999999까지의 랜덤한 값
        string keyTime = randomKey.ToString();
        //UnityEngine.Debug.Log(keyTime);
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);

            int x = Mathf.RoundToInt(node.transform.position.x + -2*offset2p + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            node.parent = boardNode.Find("y_" + y.ToString());
            node.name = "x_" + x.ToString();
            // 추가파트
            Tile tileComponent = node.GetComponent<Tile>(); // 프리팹에서 Tile 컴포넌트 가져오기
            if (tileComponent != null)
            {
                // Tile 컴포넌트에서 색상 정보 가져오기
                Color tileColor = tileComponent.color;

                string sendcolor = tileColor.ToString();    //Color32ToRGBString(tileColor);
                //UnityEngine.Debug.Log(sendcolor);
                // insertBlock 메서드에 x, y 위치와 색상 정보를 전달
                //UnityEngine.Debug.Log("x: " + x + ", y: " + y + ", key: " + keyTime);
                //blockPos.insertBlock(x, y, sendcolor, keyTime);
                 rememberTile(node, keyTime);
            }
            else
            {
               // UnityEngine.Debug.Log("오브젝트가 null!");
            }
            //blockPos.insertBlock(x , y, )
            //node.tag = keyTime; <<< 못써먹음2
            //UnityEngine.Debug.Log(keyTime + "생성됨");
        }
    }

    // 보드에 완성된 행이 있으면 삭제
    
    void CheckBoardColumn()
    {
        
         List<List<Transform>> tilesFall = new List<List<Transform>>();
        foreach (Transform column in boardNode)
        {
            List<Tile> tilesToRemove = new List<Tile>(); // 제거할 타일 리스트
            if (column.childCount == boardWidth)// 완성된 행 == 행의 자식 갯수가 가로 크기
            {
                foreach (Transform tile in column)
                {
                    
                    Tile currentTile = tile.GetComponent<Tile>();

                    if (currentTile.isIced) // 얼었을 때
                    {
                        currentTile.isIced = false; // 풀기
                        currentTile.color = currentTile.preColor;
                       // UnityEngine.Debug.Log("얼었다");
                    }
                    else
                    {

                        tilesToRemove.Add(currentTile); // 안얼었으면 제거 리스트 추가
                        //UnityEngine.Debug.Log("안얼었다");
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
                    updateBlock(); // 개수 업데이트
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
                        //UnityEngine.Debug.Log("낙하에 추가함");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("문제있음");
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
            int x = Mathf.RoundToInt(node.transform.position.x -2*offset2p+ halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1 + offset_y);
            //UnityEngine.Debug.Log("x 노드:" + x);
            if (x < 0  || x > boardWidth - 1 ) // x좌표가 보드 이내
                return false;

            if (y < 0 + offset_y) //y가 음수
                return false;

            var column = boardNode.Find("y_" + y.ToString()); //y스트링 찾기

            if (column != null && column.Find("x_" + x.ToString()) != null)
                return false;
        }

        return true;
    }




    // 타일 생성
    Tile CreateTile(Transform parent, Vector2 position, int color, int order = 1)
    {


        //여기서 var go 를 기억해둬야 사용가능함

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
        //UnityEngine.Debug.Log(parentName); 디버그용
        go.transform.parent = backgroundNode;
        //go.transform.parent = backgroundNode;
        go.transform.localPosition = position;

        var tile = go.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;
        tile.transform.name = "back" + convertY;
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
                Createback(backgroundNode, new Vector2(x+3/2f*offset2p, y + offset_y), color, 0);//



            }
        }

        // 좌우 테두리
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            Createback(backgroundNode, new Vector2(-halfWidth - 1 + 3/2f*offset2p, y + offset_y), color, 0);
            Createback(backgroundNode, new Vector2(halfWidth + 3/2f*offset2p, y + offset_y), color, 0);
        }

        // 아래 테두리
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            Createback(backgroundNode, new Vector2(x + 3/2f*offset2p, -halfHeight + offset_y), color, 0);
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
        if (arrIndexVal == -1)
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

        int col1;
        int col2;
        int col3;
        int col4;
        col1 = colorArray[arrIndex, 0];
        col2 = colorArray[arrIndex, 1];
        col3 = colorArray[arrIndex, 2];
        col4 = colorArray[arrIndex, 3];

        tetrominoNode.rotation = Quaternion.identity;
        tetrominoNode.position = new Vector2(offset_x + 2*offset2p , halfHeight - panalty + offset_y); // 블럭 생성 위치

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



    
    private void CheckTileGroups() // 4개 조건을 만족한 블럭들 탐지/삭제하는 메소드
    {
        List<List<Transform>> tilesFall = new List<List<Transform>>();
        // 게임 보드의 모든 행을 순회합니다.
        for (int y = 0; y < boardHeight; y++)
        {
            // 해당 행에서 연속된 블록 그룹을 탐색합니다.
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
                    // 게임 오브젝트를 찾았으므로 삭제합니다.
                    if (!tile.isIced) { // 안 얼었을 때

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
                        UnityEngine.Debug.Log("블록 삭제됨: " + blockName);
                       
                    }
                    else
                    {
                        tile.isIced = false;
                    }
                    

                    scoreVal += colorWeight;
                    blockCount++;
                    updateScore();
                    PlayerPrefs.SetInt("score", scoreVal);

                }
                else
                {
                    // 게임 오브젝트를 찾지 못했음을 알립니다.
                    //UnityEngine.Debug.LogWarning("게임 오브젝트를 찾을 수 없습니다: " + blockName);
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
                        //UnityEngine.Debug.Log("낙하에 추가함");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("문제있음");
                    }
                }
            }
            
        }



    }
   

    List<Vector2Int> FindContinuousBlocksInRow(int row)
    {
        List<Vector2Int> continuousBlocks = new List<Vector2Int>(); //연속된 블록 그 자체를 저장하는 리스트

        

        // 첫 번째 블록의 색상을 가져옵니다.

        //UnityEngine.Debug.Log("첫 블록 색상");
        string previousColor = GetTileColorAtPosition(new Vector2Int(0, row));

        int currentStart = 0; // 지금의 시작 좌표

        continuousBlocks.Add(new Vector2Int(currentStart, row));

        // 좌측부터 모든 블록을 확인하며 연속된 블록 그룹을 찾습니다.
        for (int x = 1; x < 11; x++)
        {

            // 현재 블록의 색상을 가져옵니다.
            string currentColor = GetTileColorAtPosition(new Vector2Int(x, row));
            

            continuousBlocks = upStair(row, continuousBlocks);


            if (currentColor.Equals(previousColor) && currentColor != "null")
            {
                // 이전 블록과 현재 블록의 색상이 같으면 연속된 블록 그룹입니다.
                //currentGroupColors.Add(currentColor);
                continuousBlocks.Add(new Vector2Int(x, row));
               // UnityEngine.Debug.Log("연속임! \n");
            }
            else
            {
                // 이전 블록과 현재 블록의 색상이 다르면 연속된 블록 그룹이 끝났습니다.
                // 현재 연속된 블록 그룹의 가중치를 확인하고, 4 이상인 경우에만 리스트에 추가합니다.
                if (continuousBlocks.Count >= 4)
                {
                    for (int i = x - continuousBlocks.Count; i < x; i++)
                    {
                       // UnityEngine.Debug.Log("블럭 추가!! \n");
                    }
                    return continuousBlocks;
                }

                // 현재 연속된 블록 그룹 초기화
                continuousBlocks.Clear();
                continuousBlocks.Add(new Vector2Int(x, row));
            }

            // 이전 블록의 색상을 지금의 색상으로 설정합니다.
            previousColor = currentColor;



        }
        //UnityEngine.Debug.Log("연속계산중..");
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
             
                UnityEngine.Debug.Log("남은열삭제");
                CheckBoardColumn();
                break;
            }
        }

        for (int y = 0; y < 20; y++)
        {
            List<Vector2Int> cont = FindContinuousBlocksInRow(y);
            if(cont.Count >= 1)
            {
                //UnityEngine.Debug.Log("남은연속 삭제!!!");
            
                CheckTileGroups();
                break;
            }
        }

    }



    string GetTileColorAtPosition(Vector2Int position)
    {
        // 유효한 위치인지 확인합니다.
        if (position.x >= 0 && position.x < boardWidth &&
            position.y >= 0 && position.y < boardHeight)
        {
            // 해당 행의 게임 오브젝트를 가져옵니다.
            Transform rowObject = boardNode.transform.Find("y_" + position.y.ToString());

            // 해당 행에 있는 모든 블럭을 가져옵니다.

            if (rowObject != null)
            {
                // 행 오브젝트가 발견되면 해당 행의 자식 오브젝트 중에서 x 좌표와 같은 이름을 가진 블록을 찾습니다.
                string blockName = "x_" + position.x.ToString();
                Transform blockTransform = rowObject.transform.Find(blockName);

                if (blockTransform != null)
                {
                    // 블록을 찾았습니다
                    Tile tile = blockTransform.GetComponent<Tile>();
                    string coll = tile.getColor();

                    return coll;
                }
                else
                {
                 //UnityEngine.Debug.Log("해당 좌표 블럭 못찾음!!:" +blockName );       
                // 해당 x 좌표를 가진 블록이 없습니다.
                }
            }
            else
            {
                // 해당 y 좌표를 가진 행이 없습니다.
            }



        }

        // 위치가 유효하지 않거나 해당 위치에 블럭이 없는 경우 기본값으로 투명색을 반환합니다.
        string a = "null";
        return a;
    }

    /*void StructureBlock()
    {
       public Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
       

    }*/
    public void doPanalty()
    { // 패널티부여 + 줄 줄어듦
        //int buff = 19 - panalty;
        //for (int i = 0; i < 12; i++)
        //{
           // Transform backRow = backgroundNode.transform.Find("back" + buff.ToString());
            // backRow.transform.position = new Vector3Int(-50, 0,0);
            //backRow.transform.name = "delete";//이름을 바꿔줘야 딜레이 없이 삭제가 가능함, destroy는 즉시 삭제가 아니라 딜레이가 존재하므로, 반복문 시간동안 안걸리는것 같음
            //Destroy(backRow);
        //}
        panalty++;
        panaltyVal++;

    }

    //스킬 구현
    [Header("Skill")]
    public int redSkillNum; // 적용시킬 블럭 개수
    public int yellowSkillNum; // 적용시킬 블럭 개수
    public int blueSkillNum; // 적용시킬 블럭 개수
    public int maxBlock;
    private Color fire = Color.black;
    private Color ice = Color.gray;
    public UnityEngine.UI.Image lightening;
    public float fadeInImage = 0.1f; // 이미지 나타나는 시간
    public float fadeOutImage = 1.01f; // 이미지 사라지는 시간
    public TextMeshProUGUI red; // 사라진 블럭
    public TextMeshProUGUI green; // 사라진 블럭
    public TextMeshProUGUI blue; // 사라진 블럭
    public TextMeshProUGUI yellow; // 사라진 블럭


    public List<Tile> randomTile(int maxCount = 5)
    {
        List<Tile> selectedTiles = new List<Tile>();
        List<int> availableYIndices = new List<int>();

        // 각 y 노드에 대해 자식이 있는지 확인하고, 인덱스를 리스트에 추가
        for (int y = 0; y < 19; y++)
        {
            var ynode = boardNode.Find("y_" + y.ToString());
            if (ynode != null && ynode.childCount > 0)
            {
                availableYIndices.Add(y);
            }
        }

        // 사용 가능한 y 노드가 없으면 빈 리스트 반환
        if (availableYIndices.Count == 0)
        {
            return selectedTiles;
        }

        // 무작위로 5개의 y 인덱스 선택 (중복 없음)
        List<int> randomYIndices = new List<int>();
        while (randomYIndices.Count < maxCount && randomYIndices.Count < availableYIndices.Count)
        {
            int randomYIndex = availableYIndices[UnityEngine.Random.Range(0, availableYIndices.Count)];
            if (!randomYIndices.Contains(randomYIndex))
            {
                randomYIndices.Add(randomYIndex);
            }
        }

        // 선택된 각 y 노드에서 무작위로 x 노드 선택하여 타일 리스트에 추가
       
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
    public void redSkill(int num) // 빨강이
    {
        
        UnityEngine.Debug.Log("빨강스킬");
        List<Tile> tiles = randomTile(num);
       
        foreach (Tile tile in tiles)
        {
            if (!tile.isred())
            {
                tile.isFired = true;
                tile.color = fire;
            }else{
                    redSkill(1);// 랜덤 걸릴시 재사용
            }
        }
        
    }



    public void yellowSkill(int num) // 노랑이
    {
        UnityEngine.Debug.Log("노랑스킬");
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

        lightening.color = new Color(1, 1, 1, 0); // 화면 사라짐

    }
    public void blueSkill(int num) // 파랑이
    {
        UnityEngine.Debug.Log("파랑스킬");
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
        red.text = $"{redVal}"; //블럭 개수 출력
        green.text = $"{greenVal}"; // 블럭 개수 출력
        blue.text = $"{blueVal}"; // 블럭 개수 출력
        yellow.text = $"{yellowVal}"; // 블럭 개수 출력
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
            //UnityEngine.Debug.Log("나중걸로추가아");
        }
        else
        {
            addedTiles[key] = new List<Transform>() { tile };
           // UnityEngine.Debug.Log("새롭게추가");
        }
    }



    
 public List<Transform> GetEx(Transform tile)
    {
        List<Transform> result = new List<Transform>();
        string keyToRemove = null;
        // key에 해당하는 리스트가 있는지 확인
        foreach (var entry in addedTiles)
        {
            var blockList = entry.Value;

            // 현재 key에 대한 value에서 입력한 (x, y) 값을 제외하고 나머지 값을 결과에 추가
            foreach (Transform t in blockList)
            {
                if (t == tile)
                {
                    keyToRemove = entry.Key; // 투입한 값과 동일한 (x, y)를 가진 key를 저장
            
                }
                else
                {
                    result.Add(t); // (x, y)와 다른 값은 결과에 추가
                }
            }
        }

        if (keyToRemove != null)
        {
            addedTiles.Remove(keyToRemove);

        }
        else
        {
            UnityEngine.Debug.Log("이미 찾아서 존재하지않음");
        }

        return result;
    }







}





