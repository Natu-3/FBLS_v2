using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;



public class BlockMap : MonoBehaviour
{
    private string[,] grid; // 블록의 색상을 저장하는 2차원 배열
    private Dictionary<string, List<(int,int)>> addedBlocks; //한번에 추가된 블럭들

    // 생성시 실행
    private void Start()
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
        
    }
    /*
    // 블록을 특정 위치에 배치하는 메서드
    public void insertBlock(int x, int y, string color, string key)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = color;


            // 리스트를 호출해서 값이 없으면 그대로 넣고, 아니면 해당 리스트의 끝부분에 x,y값 추가하기
            List<int> toIns = new List<int>() { (x, y) };
            List<int> existingList;
            if (addedBlocks.TryGetValue(key, out existingList))
            {
                // 리스트가 있을 경우 내가 추가하려는 원소를 리스트의 끝에 추가
                existingList.AddRange(tolns);
            }
            else
            {
                // 리스트가 없을 경우 내가 넣은 리스트로 대체
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

        // key에 해당하는 리스트가 있는지 확인
        if (addedBlocks.ContainsKey(key))
        {
            var blockList = addedBlock[key];

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

    

// 특정 위치의 블록의 색상을 가져오는 메서드
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

            // 연속된 동일 값 추출
            string currentValue = row[0];
            int startColumn = 0;

            for (int x = 1; x < 10; x++)
            {
                if (row[x] == currentValue)
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
    }

    public /*string[][] void fallBlock()
    {

    }

    // 특정 열에서 특정 원소를 제거하는 메서드
    public void RemoveElementInColumn(int x, int y)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = ""; // 원소 제거
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

    // 추가된 블록들의 추가된 시간과 위치를 반환하는 메서드
    public Dictionary<DateTime, List<Vector2Int>> GetAddedBlocks()
    {
        return addedBlocks;
    }


    public void RemoveBlock(int x, int y) //특정 위치의 블럭을 제거하는 메소드
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            grid[x, y] = ""; // 블록 제거

            // 동시에 생성된 블록들의 좌표값 확인
            foreach (var addedBlockEntry in addedBlocks)
            {
                List<Vector2Int> blockPositions = addedBlockEntry.Value;
                foreach (Vector2Int blockPosition in blockPositions)
                {
                    if (blockPosition.x == x && blockPosition.y == y)
                    {
                        // 해당 위치에 동시에 생성된 블록의 좌표값 출력
                        UnityEngine.Debug.Log("동시에 생성된 블록의 좌표값: (" + blockPosition.x + ", " + blockPosition.y + ")");
                    }
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for block removal!");
        }
    }

    // 특정 위치에서 블록을 아래로 이동시키고 이동한 후의 위치를 반환하는 메서드
    public Vector2Int MoveBlockDownAndGetNewPosition(int x, int y)
    {
        if (x >= 0 && x < 10 && y >= 0 && y < 20)
        {
            int newY = y - 1; // 아래로 이동할 위치
                              // 아래로 이동할 위치가 범위 내에 있고, 해당 위치가 비어있거나 바닥이 아닐 때까지 블록을 이동
            while (newY >= 0 && (grid[x, newY] == "" || newY == 0))
            {
                // 해당 위치로 블록 이동
                grid[x, newY + 1] = grid[x, newY];
                grid[x, newY] = "";
                // 다음 아래로 이동할 위치 설정
                newY--;
            }
            // 이동한 후의 위치 반환
            return new Vector2Int(x, newY + 1);
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid position for block movement!");
            return new Vector2Int(x, y);
        }
    }



    //추가된 시간과 위치를 반환하고 해당 블록을 제거하는 메서드
    public List<Vector2Int> GetAndRemoveAddedBlocks(Vector2Int targetBlock)
    {
        foreach (var entry in addedBlocks)
        {
            DateTime key = entry.Key;
            List<Vector2Int> blockList = entry.Value;

            // 주어진 좌표값과 일치하는 키를 가진 항목을 찾음
            if (blockList.Exists(block => block == targetBlock))
            {
                // 주어진 좌표값을 제외한 나머지 값을 구함
                List<Vector2Int> remainingBlocks = new List<Vector2Int>(blockList);
                remainingBlocks.Remove(targetBlock);

                // 해당 항목을 삭제
                addedBlocks.Remove(key);

                return remainingBlocks;
            }
        }

        // 주어진 좌표값과 일치하는 키가 없는 경우
        UnityEngine.Debug.LogError("Block at position (" + targetBlock.x + ", " + targetBlock.y + ") not found in added blocks!");
        return null;
    }

*/
}
