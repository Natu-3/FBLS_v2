using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

//using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class BlockPosition
{
    public string[,] grid; // ����� ������ �����ϴ� 2���� �迭
    private Dictionary<string, List<(int, int)>> addedBlocks; //�ѹ��� �߰��� ����
    private Dictionary<string, List<Transform>> addedTiles;
    private StageMulti stage;

    public int redVal = 0; // ����� �� ����
    public int greenVal = 0; // ����� �� ����
    public int blueVal = 0;   // ����� �� ����
    public int yellowVal = 0; // ����� �� ����

    private bool beforeR;
    private bool beforeL;
    private bool beforeU;
    private bool beforeD;
    public Text red; // ����� ��
    public Text green; // ����� ��
    public Text blue; // ����� ��
    public Text yellow; // ����� ��
    void Start()
    {
    }


    // Start is called before the first frame update
    public BlockPosition()
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
        addedTiles = new Dictionary<string, List<Transform>>();
}

 


    public void deleteBlock(int x, int y)
    {
        /*
        switch(grid[x, y])
        {
            case "RGBA(1.000, 0.000, 0.000, 1.000)":
                redVal++;
                PlayerPrefs.SetInt("red", redVal);
                break;
            case "RGBA(0.000, 0.000, 1.000, 1.000)":
                blueVal++;
                PlayerPrefs.SetInt("blue", blueVal);
                break;
            case "RGBA(1.000, 1.000, 0.000, 1.000)":
                yellowVal++;
                PlayerPrefs.SetInt("yellow", yellowVal);
                break;
            case "RGBA(0.000, 1.000, 0.000, 1.000)":
                greenVal++;
                PlayerPrefs.SetInt("green", greenVal);
                break;
        }   
        */
        
        grid[x, y] = "";
    }

    // ����� Ư�� ��ġ�� ��ġ�ϴ� �޼���
    public void insertBlock(int x, int y, string color, string key)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = color;

            UpdateBlockList(x, y, key);
            // ����Ʈ�� ȣ���ؼ� ���� ������ �״�� �ְ�, �ƴϸ� �ش� ����Ʈ�� ���κп� x,y�� �߰��ϱ�
            
           
        }
        else
        {
            UnityEngine.Debug.Log("Invalid position for block placement!");
        }
    }


    public void insertObject(Transform tile,string key) {
        List<Transform> tileList; //Ÿ�Ͽ�����Ʈ ���� ����Ʈ

        // ����Ʈ�� ���� �� �߰�
        if (addedTiles.TryGetValue(key, out tileList))
        {
           tileList.Add(tile); // ����Ʈ�� �̹� ������ ���� �߰��մϴ�.
        }
        else
        {
            addedTiles[key] = new List<Transform> {tile}; // ����Ʈ�� ������ ���ο� ����Ʈ�� ����� �߰��մϴ�.
        }

        // �ƴϸ� �� ����Ʈ ����� �߰�
    }


    private void UpdateBlockList(int x, int y, string key)
    {
        List<(int, int)> blockList;
        if (addedBlocks.TryGetValue(key, out blockList))
        {
            blockList.Add((x, y)); // ����Ʈ�� �̹� ������ ���� �߰��մϴ�.
        }
        else
        {
            addedBlocks[key] = new List<(int, int)> { (x, y) }; // ����Ʈ�� ������ ���ο� ����Ʈ�� ����� �߰��մϴ�.
        }
    }


    public List<Transform> GetExcept2(Transform tile)
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







    public List<(int, int)> GetExcept(int x, int y)
    {
        List<(int, int)> result = new List<(int, int)>();
        string keyToRemove = null;
        // key�� �ش��ϴ� ����Ʈ�� �ִ��� Ȯ��
        foreach (var entry in addedBlocks)
        {
            var blockList = entry.Value;

            // ���� key�� ���� value���� �Է��� (x, y) ���� �����ϰ� ������ ���� ����� �߰�
            foreach ((int blockX, int blockY) in blockList)
            {
                if (blockX == x && blockY == y)
                {
                    keyToRemove = entry.Key; // ������ ���� ������ (x, y)�� ���� key�� ����
                }
                else
                {
                    result.Add((blockX, blockY)); // (x, y)�� �ٸ� ���� ����� �߰�
                }
            }
        }
        
        if (keyToRemove != null)
        {
            addedBlocks.Remove(keyToRemove);

        }
        else
        {
            UnityEngine.Debug.Log("�̹� ã�Ƽ� ������������");
        }
        
        return result;
    }

    public void insertBlock(int x, int y, string color)
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
        List<(int, int)> fallList = fallReady(key, x, y);

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

                insertBlock(fallX, yBuffer, colBuffer);
            }

        }

        return falltoY;
    }


    /*
    public List<(int, int)> ContinousBlock()
    {
        List<(int, int)> result = new List<(int, int)>(); //x y ��ǥ ���� ����Ʈ

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != "" && grid[x, y] != null) // ���� ��ġ�� �� ���ڿ��� �ƴ� ��쿡�� ���ӵ� ����� ã���ϴ�.
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
        res.Add((x, y)); // ���� ��ġ�� ��� ����Ʈ�� �߰��մϴ�.

        // �����¿츦 �˻��Ͽ� ���� ��ġ�� ���� ���̸鼭 ���� �湮���� ���� ��ġ�� ã���ϴ�.
        // ���� �̵�
        if (y + 1 < grid.GetLength(1) && grid[x, y] == grid[x, y + 1] && !res.Contains((x, y + 1)))
        {
            UnityEngine.Debug.Log(grid[x, y] + "   ����?    " + grid[x, y + 1]);
            Loop(x, y + 1, res);
        }
        // �Ʒ��� �̵�
        if (y - 1 >= 0 && grid[x, y] == grid[x, y - 1] && !res.Contains((x, y - 1)))
        {
            UnityEngine.Debug.Log(grid[x, y] + "   ����?    " + grid[x, y - 1]);
            Loop(x, y - 1, res);
        }
        // �������� �̵�
        if (x - 1 >= 0 && grid[x, y] == grid[x - 1, y] && !res.Contains((x - 1, y)))
        {
            UnityEngine.Debug.Log(grid[x, y] + "   ����?    " + grid[x - 1, y]);
            Loop(x - 1, y, res);
        }
        // ���������� �̵�
        if (x + 1 < grid.GetLength(0) && grid[x, y] == grid[x + 1, y] && !res.Contains((x + 1, y)))
        {
            UnityEngine.Debug.Log(grid[x, y] + "   ����?    " + grid[x + 1, y]);
            Loop(x + 1, y, res);
        }
    }
    */




    public List<(int, int)> ContinousBlock()
    {
        List<(int, int)> result = new List<(int, int)>(); // x y ��ǥ ���� ����Ʈ
        HashSet<(int, int)> visited = new HashSet<(int, int)>(); // �湮�� ��ǥ�� ����� ����

        PrintGrid();
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (!string.IsNullOrEmpty(grid[x, y]) && !visited.Contains((x, y)))
                {
                    // BFS Ž�� ����
                    Queue<(int, int)> queue = new Queue<(int, int)>();
                    queue.Enqueue((x, y));
                    visited.Add((x, y));

                    // BFS�� �����¿츦 Ž���ϸ鼭 ���ӵ� ��� ã��
                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        result.Add(current);

                        // �����¿츦 Ž���ϸ鼭 ���ӵ� ����� ã��
                        foreach (var neighbor in GetNeighbors(current.Item1, current.Item2))
                        {
                            if (IsSameBlock(current.Item1, current.Item2, neighbor.Item1, neighbor.Item2)
                                && !visited.Contains(neighbor))
                            {
                                queue.Enqueue(neighbor);
                                visited.Add(neighbor);
                            }
                        }
                    }
                }
            }
        }
        UnityEngine.Debug.Log("����������: " + result);
        return result;
    }

    // ���� ��ġ�� �̿��� ��ġ�� ����� ������ Ȯ���ϴ� �޼���
    private bool IsSameBlock(int x1, int y1, int x2, int y2)
    {
        if (x1 < 0 || x1 >= grid.GetLength(0) || y1 < 0 || y1 >= grid.GetLength(1)
            || x2 < 0 || x2 >= grid.GetLength(0) || y2 < 0 || y2 >= grid.GetLength(1))
        {
            // �迭 ������ ����� ���
            return false;
        }
        // �� ��ġ�� ����� �������� Ȯ��
       // return grid[x1, y1]. == grid[x2, y2];
        return Equals(grid[x1, y1], grid[x2, y2]);
    }

    // Ư�� ��ġ�� �����¿� �̿��� ��ġ�� �������� �޼���
    private IEnumerable<(int, int)> GetNeighbors(int x, int y)
    {
        yield return (x, y + 1); // ��
        yield return (x, y - 1); // �Ʒ�
        yield return (x - 1, y); // ����
        yield return (x + 1, y); // ������
    }


    public void PrintGrid()
    {
        string gridString = "Grid Contents:\n";

        for (int y = 0; y < 20; y++)
        {
            string row = "";
            for (int x = 0; x < 10; x++)
            {
                row += grid[x, y] + " ";
            }
            gridString += row + "\n";
        }

        UnityEngine.Debug.Log(gridString);
    }

}