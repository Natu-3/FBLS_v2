using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blockstructure : MonoBehaviour
{
    public Dictionary<int,List<int>> dictionary = new Dictionary<int,List<int>>();
    
    public Blockstructure()
    {
                
    }

    public void getElement(int keys,List<int> values)
    {
        dictionary.Add(keys, values);
    }

    public List<List<int>> searchListToKey(int key)
    {
        List<List<int>> result = new List<List<int>>();

        // 주어진 키값에 대응하는 리스트들의 리스트가 존재하는지 확인
        if (dictionary.ContainsKey(key))
        {
            // 주어진 키값에 대응하는 리스트들의 리스트를 가져옴
            result = dictionary[key];
        }
        else
        {
            // 주어진 키값에 대응하는 리스트들의 리스트가 존재하지 않을 때의 처리
            //Console.WriteLine("주어진 키에 대응하는 리스트들의 리스트가 존재하지 않습니다.");
        }

        // 결과 리스트들의 리스트 반환
        return result;
    }

    public int searchKeyToList(List<int> list)
    {
        public int searchKeyToList(List<int> list)
        {
            // 딕셔너리를 순회하며 주어진 리스트와 일치하는 키값을 찾음
            foreach (var pair in dictionary)
            {
                // 현재 키에 대응하는 리스트를 가져옴
                List<int> currentList = pair.Value;

                // 현재 리스트와 주어진 리스트를 비교하여 일치하는지 확인
                if (Enumerable.SequenceEqual(currentList, list))
                {
                    // 일치하는 경우 현재 키값 반환
                    return pair.Key;
                }
            }

            // 일치하는 리스트를 찾지 못한 경우 -1 반환 또는 예외 처리
            return -1;
        }
    }

    public void deleteValue(List<int> list)
    {
        // 삭제할 키값들을 저장할 임시 리스트
        List<int> keysToRemove = new List<int>();

        // 딕셔너리를 순회하며 주어진 리스트와 같은 값을 가진 항목의 키값을 찾음
        foreach (var pair in dictionary)
        {
            // 현재 키에 대응하는 리스트를 가져옴
            List<int> currentValue = pair.Value;

            // 현재 리스트와 주어진 리스트를 비교하여 일치하는지 확인
            if (Enumerable.SequenceEqual(currentValue, list))
            {
                // 일치하는 경우 키값을 삭제할 리스트에 추가
                keysToRemove.Add(pair.Key);
            }
        }

        // keysToRemove 리스트에 저장된 모든 키값을 삭제
        foreach (var key in keysToRemove)
        {
            dictionary.Remove(key);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
