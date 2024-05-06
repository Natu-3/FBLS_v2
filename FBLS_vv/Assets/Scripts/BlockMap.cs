using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;



public class BlockMap : MonoBehaviour
{
    private string[,] grid; // ����� ������ �����ϴ� 2���� �迭
    private Dictionary<string, List<(int,int)>> addedBlocks; //�ѹ��� �߰��� ����

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
        
    }
    /*
    // ����� Ư�� ��ġ�� ��ġ�ϴ� �޼���
    public void insertBlock(int x, int y, string color, string key)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = color;


            // ����Ʈ�� ȣ���ؼ� ���� ������ �״�� �ְ�, �ƴϸ� �ش� ����Ʈ�� ���κп� x,y�� �߰��ϱ�
            List<int> toIns = new List<int>() { (x, y) };
            List<int> existingList;
            if (addedBlocks.TryGetValue(key, out existingList))
            {
                // ����Ʈ�� ���� ��� ���� �߰��Ϸ��� ���Ҹ� ����Ʈ�� ���� �߰�
                existingList.AddRange(tolns);
            }
            else
            {
                // ����Ʈ�� ���� ��� ���� ���� ����Ʈ�� ��ü
                addedBlocks[key] = tolns;
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for block placement!");
        }
    }

    public List<(int, int)> fallReady(string key, int findX, int findY)
    {
        List<(int, int)> result = new List<(int, int)>();

        // key�� �ش��ϴ� ����Ʈ�� �ִ��� Ȯ��
        if (addedBlocks.ContainsKey(key))
        {
            var blockList = addedBlock[key];

            // ����Ʈ���� ���ϴ� �� ã��
            int indexToRemove = blockList.FindIndex(block => block.Item1 == findX && block.Item2 == findY);
            if (indexToRemove != -1)
            {
                // ã�� �� ����
                blockList.RemoveAt(indexToRemove);
            }

            // ������ ���� ������ ������ �� ��ȯ
            result.AddRange(blockList);

            // �ش� key�� ���� ��ųʸ� �� ����
            addedBlocks.Remove(key);
        }

        return result;
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
    public void RowBreak(int y)
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


    // Ư�� ���� ��ĭ�� �ƴ� ���ӵ� ���� ���� �����Ͽ� �� ��ǥ�� List�� ��ȯ�ϴ� �޼���
    public List<(int, int)> ColumnCheck(int x)
    {
        List<(int, int)> result = new List<(int, int)>();

        if (x >= 0 && x < 10)
        {
            string[] column = new string[20];
            for (int y = 0; y < 20; y++)
            {
                column[y] = grid[x, y];
            }

            // ���ӵ� ���� �� ����
            string currentValue = column[0];
            int startRow = 0;

            for (int y = 1; y < 20; y++)
            {
                if (column[y] == currentValue)
                {
                    // ���� ���� ���ӵ�
                    continue;
                }
                else
                {
                    // ���� ���� �ٸ� ���� ����
                    if (y - startRow >= 2)
                    {
                        // ���ӵ� ���� ���� 3�� �̻��� ��� ��ǥ�� ����� �߰�
                        for (int i = startRow; i < y; i++)
                        {
                            result.Add((x, i));
                        }
                    }

                    // ���� ��ġ ����
                    currentValue = column[y];
                    startRow = y;
                }
            }

            // ������ ���� ���ӵ�
            if (20 - startRow >= 2)
            {
                for (int i = startRow; i < 20; i++)
                {
                    result.Add((x, i));
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid column index for column extraction!");
        }

        return result;
    }


    public List<(int, int)> RowCheck(int y)
    {
        List<(int, int)> result = new List<(int, int)>();

        if (y >= 0 && y < 20)
        {
            string[] row = new string[10];
            for (int x = 0; x < 10; x++)
            {
                row[x] = grid[x, y];
            }

            // ���ӵ� ���� �� ����
            string currentValue = row[0];
            int startColumn = 0;

            for (int x = 1; x < 10; x++)
            {
                if (row[x] == currentValue)
                {
                    // ���� ���� ���ӵ�
                    continue;
                }
                else
                {
                    // ���� ���� �ٸ� ���� ����
                    if (x - startColumn >= 2)
                    {
                        // ���ӵ� ���� ���� 3�� �̻��� ��� ��ǥ�� ����� �߰�
                        for (int i = startColumn; i < x; i++)
                        {
                            result.Add((i, y));
                        }
                    }

                    // ���� ��ġ ����
                    currentValue = row[x];
                    startColumn = x;
                }
            }

            // ������ ���� ���ӵ�
            if (10 - startColumn >= 2)
            {
                for (int i = startColumn; i < 10; i++)
                {
                    result.Add((i, y));
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid row index for row extraction!");
        }

        return result;
    }

    public /*string[][] void fallBlock()
    {

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

*/
}
