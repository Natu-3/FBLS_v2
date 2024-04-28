using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;




public class BlockMap : MonoBehaviour
{
    private string[,] grid; // 블록의 색상을 저장하는 2차원 배열
    private Dictionary<DateTime, List<Vector2Int>> addedBlocks;

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
        addedBlocks = new Dictionary<DateTime, List<Vector2Int>>();
    }

    // 블록을 특정 위치에 배치하는 메서드
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
    public void RemoveRow(int y)
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


    // 특정 열의 값을 추출하여 배열로 반환하는 메서드
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


}
