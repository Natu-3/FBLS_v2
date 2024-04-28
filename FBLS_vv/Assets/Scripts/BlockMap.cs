using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;




public class BlockMap : MonoBehaviour
{
    private string[,] grid; // ����� ������ �����ϴ� 2���� �迭
    private Dictionary<DateTime, List<Vector2Int>> addedBlocks;

    // ������ ����
    private void Start()
    {
        // 10x20 ũ���� �׸��带 �����ϰ� ��� ��ġ�� �� ���ڿ�("")�� �ʱ�ȭ
        grid = new string[10, 20];
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                grid[x, y] = "";
            }
        }
        addedBlocks = new Dictionary<DateTime, List<Vector2Int>>();
    }

    // ����� Ư�� ��ġ�� ��ġ�ϴ� �޼���
    public void PlaceBlock(int x, int y, string color)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = color;
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for block placement!");
        }
    }

    // Ư�� ��ġ�� ����� ������ �������� �޼���
    public string GetBlockColorAt(int x, int y)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            return grid[x, y];
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for block checking!");
            return null;
        }
    }

    // Ư�� ���� �����ϰ� ���� ���� �� ĭ�� ���� �޼���
    public void RemoveRow(int y)
    {
        if (y >= 0 && y < 20)
        {
            // �Ʒ������� �����Ͽ� �ش� ���� �����ϰ� ���� ���� �� ĭ�� ���
            for (int i = y; i < 19; i++)
            {
                for (int x = 0; x < 10; x++)
                {
                    grid[x, i] = grid[x, i + 1];
                }
            }
            // ���� �� ���� ���
            for (int x = 0; x < 10; x++)
            {
                grid[x, 19] = "";
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid row index for row removal!");
        }
    }


    // Ư�� ���� ���� �����Ͽ� �迭�� ��ȯ�ϴ� �޼���
    public string[] GetColumn(int x)
    {
        if (x >= 0 && x < 10)
        {
            string[] column = new string[20];
            for (int y = 0; y < 20; y++)
            {
                column[y] = grid[x, y];
            }
            return column;
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid column index for column extraction!");
            return null;
        }
    }

    // Ư�� ������ Ư�� ���Ҹ� �����ϴ� �޼���
    public void RemoveElementInColumn(int x, int y)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = ""; // ���� ����
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for element removal in column!");
        }
    }

    public void AddBlocks(List<Vector2Int> blockPositions)
    {
        DateTime currentTime = DateTime.Now;
        addedBlocks[currentTime] = blockPositions;
    }

    // �߰��� ��ϵ��� �߰��� �ð��� ��ġ�� ��ȯ�ϴ� �޼���
    public Dictionary<DateTime, List<Vector2Int>> GetAddedBlocks()
    {
        return addedBlocks;
    }


    public void RemoveBlock(int x, int y) //Ư�� ��ġ�� ���� �����ϴ� �޼ҵ�
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = ""; // ��� ����

            // ���ÿ� ������ ��ϵ��� ��ǥ�� Ȯ��
            foreach (var addedBlockEntry in addedBlocks)
            {
                List<Vector2Int> blockPositions = addedBlockEntry.Value;
                foreach (Vector2Int blockPosition in blockPositions)
                {
                    if (blockPosition.x == x && blockPosition.y == y)
                    {
                        // �ش� ��ġ�� ���ÿ� ������ ����� ��ǥ�� ���
                        UnityEngine.Debug.Log("���ÿ� ������ ����� ��ǥ��: (" + blockPosition.x + ", " + blockPosition.y + ")");
                    }
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for block removal!");
        }
    }

    // Ư�� ��ġ���� ����� �Ʒ��� �̵���Ű�� �̵��� ���� ��ġ�� ��ȯ�ϴ� �޼���
    public Vector2Int MoveBlockDownAndGetNewPosition(int x, int y)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            int newY = y - 1; // �Ʒ��� �̵��� ��ġ
                              // �Ʒ��� �̵��� ��ġ�� ���� ���� �ְ�, �ش� ��ġ�� ����ְų� �ٴ��� �ƴ� ������ ����� �̵�
            while (newY >= 0 && (grid[x, newY] == "" || newY == 0))
            {
                // �ش� ��ġ�� ��� �̵�
                grid[x, newY + 1] = grid[x, newY];
                grid[x, newY] = "";
                // ���� �Ʒ��� �̵��� ��ġ ����
                newY--;
            }
            // �̵��� ���� ��ġ ��ȯ
            return new Vector2Int(x, newY + 1);
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for block movement!");
            return new Vector2Int(x, y);
        }
    }



    //�߰��� �ð��� ��ġ�� ��ȯ�ϰ� �ش� ����� �����ϴ� �޼���
    public List<Vector2Int> GetAndRemoveAddedBlocks(Vector2Int targetBlock)
    {
        foreach (var entry in addedBlocks)
        {
            DateTime key = entry.Key;
            List<Vector2Int> blockList = entry.Value;

            // �־��� ��ǥ���� ��ġ�ϴ� Ű�� ���� �׸��� ã��
            if (blockList.Exists(block => block == targetBlock))
            {
                // �־��� ��ǥ���� ������ ������ ���� ����
                List<Vector2Int> remainingBlocks = new List<Vector2Int>(blockList);
                remainingBlocks.Remove(targetBlock);

                // �ش� �׸��� ����
                addedBlocks.Remove(key);

                return remainingBlocks;
            }
        }

        // �־��� ��ǥ���� ��ġ�ϴ� Ű�� ���� ���
        UnityEngine.Debug.LogError("Block at position (" + targetBlock.x + ", " + targetBlock.y + ") not found in added blocks!");
        return null;
    }


}
