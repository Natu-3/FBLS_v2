using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Clone : MonoBehaviour
{
    [Header("Editor Objects")]
    public GameObject tilePrefab; //타일프리펩 불러옴
    public Transform backgroundNode; // 백그라운드 
    public Transform boardNode; //게임판(각 열 y0 - y19까지의 노드)
    public Transform tetrominoNode; //테트리미노
    public GameObject gameoverPanel; //게임오버

    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 10;
    [Range(5, 20)]
    public int boardHeight = 20;
    public float fallCycle = 1.0f;
    public int testp1 = -10;
    private int halfWidth; // 좌표 (가로)중앙값
    private int halfHeight; //좌표 (세로) 중앙값

    private float nextFallTime;






    private void Start()
    {
       // gameoverPanel.SetActive(false);

        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); //(5)
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); //(10)

   

        CreateBackground(); //백그라운드 생성 메소드

        for (int i = 0; i < boardHeight; ++i)  //보드 높이까지
        {
            var col = new GameObject("y_" + (boardHeight - i - 1).ToString());     //보드의 세로줄을 동적으로 생성하는중
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            col.transform.parent = boardNode;
        }

    }

    void Update()
    {
        
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
                CreateTile(backgroundNode, new Vector2(x - testp1, y), color, 0);// 배경 타일 생성하는 노드, 여기에 x,y 절편 먹이는걸로 위치 조정 가능?
            }
        }

        // 좌우 테두리
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode, new Vector2(-halfWidth - 1 - testp1, y), color, 0);
            CreateTile(backgroundNode, new Vector2(halfWidth - testp1, y), color, 0);
        }

        // 아래 테두리
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            CreateTile(backgroundNode, new Vector2(x - testp1, -halfHeight), color, 0);
        }
    }

}




