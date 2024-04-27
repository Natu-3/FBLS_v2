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

        // �־��� Ű���� �����ϴ� ����Ʈ���� ����Ʈ�� �����ϴ��� Ȯ��
        if (dictionary.ContainsKey(key))
        {
            // �־��� Ű���� �����ϴ� ����Ʈ���� ����Ʈ�� ������
            result = dictionary[key];
        }
        else
        {
            // �־��� Ű���� �����ϴ� ����Ʈ���� ����Ʈ�� �������� ���� ���� ó��
            //Console.WriteLine("�־��� Ű�� �����ϴ� ����Ʈ���� ����Ʈ�� �������� �ʽ��ϴ�.");
        }

        // ��� ����Ʈ���� ����Ʈ ��ȯ
        return result;
    }

    public int searchKeyToList(List<int> list)
    {
        public int searchKeyToList(List<int> list)
        {
            // ��ųʸ��� ��ȸ�ϸ� �־��� ����Ʈ�� ��ġ�ϴ� Ű���� ã��
            foreach (var pair in dictionary)
            {
                // ���� Ű�� �����ϴ� ����Ʈ�� ������
                List<int> currentList = pair.Value;

                // ���� ����Ʈ�� �־��� ����Ʈ�� ���Ͽ� ��ġ�ϴ��� Ȯ��
                if (Enumerable.SequenceEqual(currentList, list))
                {
                    // ��ġ�ϴ� ��� ���� Ű�� ��ȯ
                    return pair.Key;
                }
            }

            // ��ġ�ϴ� ����Ʈ�� ã�� ���� ��� -1 ��ȯ �Ǵ� ���� ó��
            return -1;
        }
    }

    public void deleteValue(List<int> list)
    {
        // ������ Ű������ ������ �ӽ� ����Ʈ
        List<int> keysToRemove = new List<int>();

        // ��ųʸ��� ��ȸ�ϸ� �־��� ����Ʈ�� ���� ���� ���� �׸��� Ű���� ã��
        foreach (var pair in dictionary)
        {
            // ���� Ű�� �����ϴ� ����Ʈ�� ������
            List<int> currentValue = pair.Value;

            // ���� ����Ʈ�� �־��� ����Ʈ�� ���Ͽ� ��ġ�ϴ��� Ȯ��
            if (Enumerable.SequenceEqual(currentValue, list))
            {
                // ��ġ�ϴ� ��� Ű���� ������ ����Ʈ�� �߰�
                keysToRemove.Add(pair.Key);
            }
        }

        // keysToRemove ����Ʈ�� ����� ��� Ű���� ����
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
