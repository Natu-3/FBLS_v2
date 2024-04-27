using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Clone : MonoBehaviour
{
    [Header("Editor Objects")]
    public GameObject tilePrefab; //Ÿ�������� �ҷ���
    public Transform backgroundNode; // ��׶��� 
    public Transform boardNode; //������(�� �� y0 - y19������ ���)
    public Transform tetrominoNode; //��Ʈ���̳�
    public GameObject gameoverPanel; //���ӿ���

    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 10;
    [Range(5, 20)]
    public int boardHeight = 20;
    public float fallCycle = 1.0f;
    public int testp1 = -10;
    private int halfWidth; // ��ǥ (����)�߾Ӱ�
    private int halfHeight; //��ǥ (����) �߾Ӱ�

    private float nextFallTime;






    private void Start()
    {
       // gameoverPanel.SetActive(false);

        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); //(5)
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); //(10)

   

        CreateBackground(); //��׶��� ���� �޼ҵ�

        for (int i = 0; i < boardHeight; ++i)  //���� ���̱���
        {
            var col = new GameObject("y_" + (boardHeight - i - 1).ToString());     //������ �������� �������� �����ϴ���
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            col.transform.parent = boardNode;
        }

    }

    void Update()
    {
        
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
                CreateTile(backgroundNode, new Vector2(x - testp1, y), color, 0);// ��� Ÿ�� �����ϴ� ���, ���⿡ x,y ���� ���̴°ɷ� ��ġ ���� ����?
            }
        }

        // �¿� �׵θ�
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode, new Vector2(-halfWidth - 1 - testp1, y), color, 0);
            CreateTile(backgroundNode, new Vector2(halfWidth - testp1, y), color, 0);
        }

        // �Ʒ� �׵θ�
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            CreateTile(backgroundNode, new Vector2(x - testp1, -halfHeight), color, 0);
        }
    }

}




