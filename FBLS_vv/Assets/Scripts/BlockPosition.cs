using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

//using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class BlockPosition : MonoBehaviour
{

    private string[,] grid; // ����� ������ �����ϴ� 2���� �迭
    private Dictionary<string, List<(int, int)>> addedBlocks; //�ѹ��� �߰��� ����


    private bool beforeR;
    private bool beforeL;
    private bool beforeU;
    private bool beforeD;

    // Start is called before the first frame update
    void Start()
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
        addedBlocks = new Dictionary<string, List<(int, int)>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void deleteBlock(int x, int y)
    {
        grid[x, y] = "";
    }

    // ����� Ư�� ��ġ�� ��ġ�ϴ� �޼���
    public void insertBlock(int x, int y, string color, string key)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = color;


            // ����Ʈ�� ȣ���ؼ� ���� ������ �״�� �ְ�, �ƴϸ� �ش� ����Ʈ�� ���κп� x,y�� �߰��ϱ�
            List<(int, int)> toIns = new List<(int,int)>() { (x, y) };
            List<(int, int)> existingList;
            if (addedBlocks.TryGetValue(key, out existingList))
            {
                // ����Ʈ�� ���� ��� ���� �߰��Ϸ��� ���Ҹ� ����Ʈ�� ���� �߰�
                existingList.AddRange(toIns);
            }
            else
            {
                // ����Ʈ�� ���� ��� ���� ���� ����Ʈ�� ��ü
                addedBlocks[key] = toIns;
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for block placement!");
        }
    }

    public void insertBlock(int x, int y,string color)
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


    public List<(int, int)> fallReady(string key, int findX, int findY)
    {
        List<(int, int)> result = new List<(int, int)>();

        // key�� �ش��ϴ� ����Ʈ�� �ִ��� Ȯ��
        if (addedBlocks.ContainsKey(key))
        {
            var blockList = addedBlocks[key];

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

    /*public List<(int, int)> RowCheck(int y, string color)
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
            
            int startColumn = 0;

            for (int x = 1; x < 10; x++)
            {
                if (row[x] == color)
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
    }*/

    public string[] fallBlock(int x, int y, string key)
    {
        // ������ ��ġ�� ��ǥ ����Ʈ�� ������
        List<(int, int)> fallList = fallReady(key,x, y);

        // ������ ��ġ�� y ��ǥ�� ������ �迭 + �ʱ�ȭ
        string[] falltoY = new string[fallList.Count];

        for (int i = 0; i < falltoY.Length; i++)
        {
            falltoY[i] = "";
        }

        int yBuffer;
        string colBuffer;
        // ���� fallList�� ������� �ʴٸ� �Ʒ��� ��ɵ� ����
        if (fallList.Count > 0)
        {
            int index = 0;

            // fallList�� �ִ� �� Ʃ���� ���ҵ��� ��ȸ
            foreach ((int fallX, int fallY) in fallList)
            {
                colBuffer = grid[fallX, fallY];
                yBuffer = fallY;
                deleteBlock(fallX, fallY); 
                // �ڱ⺸�� y ��ǥ�� �Ʒ��� ���� ��ĭ�� �ִٸ�
                while (fallY > 0 && grid[fallX, fallY - 1] == "")
                {
                    // y ��ǥ�� 1�� ���ҽ�Ŵ
                    yBuffer--;
                }

                // ������ ��ġ�� y ��ǥ�� falltoY �迭�� ����
                falltoY[index] = yBuffer.ToString();
                index++;

                insertBlock(fallX, yBuffer,colBuffer);
            }

        }

        return falltoY;
    }

    public List<(int, int)> ContinousBlock()
    {
        List<(int, int)> result = new List<(int, int)>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != "") // ���� ��ġ�� �� ���ڿ��� �ƴ� ��쿡�� ���ӵ� ����� ã���ϴ�.
                {
                    Loop(x, y, result); // loop �޼��带 �����Ͽ� ���ӵ� ����� ã���ϴ�.
                }
            }
        }

        // ��� ����Ʈ�� ���Ұ� 4�� �̻��� ��쿡�� �״�� ��ȯ�ϰ�, �׷��� ���� ��쿡�� ����� ��ȯ�մϴ�.
        if (result.Count >= 4)
        {
            return result;
        }
        else
        {
            result.Clear();
            return result;
        }
    }


    public void Loop(int x, int y, List<(int, int)> res)
    {
        //res�� �ڽ��� x,y �� �߰�


        // �����¿츦 �˻��Ͽ� ���� ��ġ�� ���� ���̸鼭 ���� �湮���� ���� ��ġ�� ã���ϴ�.
        // ���� �̵�
        if (y + 1 < grid.GetLength(1) && grid[x, y] == grid[x, y + 1] && !res.Contains((x, y + 1)))
        {
            res.Add((x, y + 1));
            Loop(x, y + 1, res);
        }
        // �Ʒ��� �̵�
        if (y - 1 >= 0 && grid[x, y] == grid[x, y - 1] && !res.Contains((x, y - 1)))
        {
            res.Add((x, y - 1));
            Loop(x, y - 1, res);
        }
        // �������� �̵�
        if (x - 1 >= 0 && grid[x, y] == grid[x - 1, y] && !res.Contains((x - 1, y)))
        {
            res.Add((x - 1, y));
            Loop(x - 1, y, res);
        }
        // ���������� �̵�
        if (x + 1 < grid.GetLength(0) && grid[x, y] == grid[x + 1, y] && !res.Contains((x + 1, y)))
        {
            res.Add((x + 1, y));
            Loop(x + 1, y, res);
        }
    }

}