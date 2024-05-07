using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

//using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class BlockPosition : MonoBehaviour
{

    private string[,] grid; // 블록의 색상을 저장하는 2차원 배열
    private Dictionary<string, List<(int, int)>> addedBlocks; //한번에 추가된 블럭들


    private bool beforeR;
    private bool beforeL;
    private bool beforeU;
    private bool beforeD;

    // Start is called before the first frame update
    void Start()
    {
        // 10x20 크기의 그리드를 생성하고 모든 위치를 빈 문자열("")로 초기화
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

    // 블록을 특정 위치에 배치하는 메서드
    public void insertBlock(int x, int y, string color, string key)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = color;


            // 리스트를 호출해서 값이 없으면 그대로 넣고, 아니면 해당 리스트의 끝부분에 x,y값 추가하기
            List<(int, int)> toIns = new List<(int,int)>() { (x, y) };
            List<(int, int)> existingList;
            if (addedBlocks.TryGetValue(key, out existingList))
            {
                // 리스트가 있을 경우 내가 추가하려는 원소를 리스트의 끝에 추가
                existingList.AddRange(toIns);
            }
            else
            {
                // 리스트가 없을 경우 내가 넣은 리스트로 대체
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

        // key에 해당하는 리스트가 있는지 확인
        if (addedBlocks.ContainsKey(key))
        {
            var blockList = addedBlocks[key];

            // 리스트에서 원하는 블럭 찾기
            int indexToRemove = blockList.FindIndex(block => block.Item1 == findX && block.Item2 == findY);
            if (indexToRemove != -1)
            {
                // 찾은 블럭 삭제
                blockList.RemoveAt(indexToRemove);
            }

            // 삭제된 블럭을 제외한 나머지 값 반환
            result.AddRange(blockList);

            // 해당 key에 대한 딕셔너리 값 제거
            addedBlocks.Remove(key);
        }

        return result;
    }

    // 특정 행을 삭제하고 위의 행을 한 칸씩 당기는 메서드
    public void RowBreak(int y)
    {
        if (y >= 0 && y < 20)
        {
            // 아래서부터 시작하여 해당 행을 삭제하고 위의 행을 한 칸씩 당김
            for (int i = y; i < 19; i++)
            {
                for (int x = 0; x < 10; x++)
                {
                    grid[x, i] = grid[x, i + 1];
                }
            }
            // 가장 윗 줄은 비움
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


    // 특정 열의 빈칸이 아닌 연속된 동일 값을 추출하여 그 좌표를 List로 반환하는 메서드
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

            // 연속된 동일 값 추출
            string currentValue = column[0];
            int startRow = 0;

            for (int y = 1; y < 20; y++)
            {
                if (column[y] == currentValue)
                {
                    // 현재 값이 연속됨
                    continue;
                }
                else
                {
                    // 현재 값과 다른 값이 나옴
                    if (y - startRow >= 2)
                    {
                        // 연속된 동일 값이 3개 이상인 경우 좌표를 결과에 추가
                        for (int i = startRow; i < y; i++)
                        {
                            result.Add((x, i));
                        }
                    }

                    // 시작 위치 갱신
                    currentValue = column[y];
                    startRow = y;
                }
            }

            // 마지막 값이 연속됨
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

            // 연속된 동일 값 추출
            
            int startColumn = 0;

            for (int x = 1; x < 10; x++)
            {
                if (row[x] == color)
                {
                    // 현재 값이 연속됨
                    continue;
                }
                else
                {
                    // 현재 값과 다른 값이 나옴
                    if (x - startColumn >= 2)
                    {
                        // 연속된 동일 값이 3개 이상인 경우 좌표를 결과에 추가
                        for (int i = startColumn; i < x; i++)
                        {
                            result.Add((i, y));
                        }
                    }

                    // 시작 위치 갱신
                    currentValue = row[x];
                    startColumn = x;
                }
            }

            // 마지막 값이 연속됨
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
        // 낙하할 위치의 좌표 리스트를 가져옴
        List<(int, int)> fallList = fallReady(key,x, y);

        // 낙하한 위치의 y 좌표를 저장할 배열 + 초기화
        string[] falltoY = new string[fallList.Count];

        for (int i = 0; i < falltoY.Length; i++)
        {
            falltoY[i] = "";
        }

        int yBuffer;
        string colBuffer;
        // 만약 fallList가 비어있지 않다면 아래의 명령들 실행
        if (fallList.Count > 0)
        {
            int index = 0;

            // fallList에 있는 각 튜플의 원소들을 순회
            foreach ((int fallX, int fallY) in fallList)
            {
                colBuffer = grid[fallX, fallY];
                yBuffer = fallY;
                deleteBlock(fallX, fallY); 
                // 자기보다 y 좌표가 아래인 곳에 빈칸이 있다면
                while (fallY > 0 && grid[fallX, fallY - 1] == "")
                {
                    // y 좌표를 1씩 감소시킴
                    yBuffer--;
                }

                // 낙하한 위치의 y 좌표를 falltoY 배열에 저장
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
                if (grid[x, y] != "") // 현재 위치가 빈 문자열이 아닌 경우에만 연속된 블록을 찾습니다.
                {
                    Loop(x, y, result); // loop 메서드를 실행하여 연속된 블록을 찾습니다.
                }
            }
        }

        // 결과 리스트에 원소가 4개 이상인 경우에는 그대로 반환하고, 그렇지 않은 경우에는 비워서 반환합니다.
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
        //res에 자신의 x,y 값 추가


        // 상하좌우를 검사하여 현재 위치와 같은 값이면서 아직 방문하지 않은 위치를 찾습니다.
        // 위로 이동
        if (y + 1 < grid.GetLength(1) && grid[x, y] == grid[x, y + 1] && !res.Contains((x, y + 1)))
        {
            res.Add((x, y + 1));
            Loop(x, y + 1, res);
        }
        // 아래로 이동
        if (y - 1 >= 0 && grid[x, y] == grid[x, y - 1] && !res.Contains((x, y - 1)))
        {
            res.Add((x, y - 1));
            Loop(x, y - 1, res);
        }
        // 왼쪽으로 이동
        if (x - 1 >= 0 && grid[x, y] == grid[x - 1, y] && !res.Contains((x - 1, y)))
        {
            res.Add((x - 1, y));
            Loop(x - 1, y, res);
        }
        // 오른쪽으로 이동
        if (x + 1 < grid.GetLength(0) && grid[x, y] == grid[x + 1, y] && !res.Contains((x + 1, y)))
        {
            res.Add((x + 1, y));
            Loop(x + 1, y, res);
        }
    }

}