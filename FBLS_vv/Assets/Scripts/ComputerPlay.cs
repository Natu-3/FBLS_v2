using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlay : MonoBehaviour
{
	/*
	//���� �����Ǿ��� �� �޼ҵ� �����(���� ���� I , O, T , S , Z , L ,J �� �μ��� �޾ƿ� : status )
	calRotationBlock(status)
	{

		//����ִ� ����Ʈ(rotationBlock) �ϳ� �Լ� ���ο� ����
		List<Vector2[]> rotationBlock = new List<Vector2[]>;

		//�޾ƿ�status�� switch-case�� �Ἥ rotationBlock ����Ʈ�� ȸ�������� ���� ä��

		switch (status)
		{
			case 'I':
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);

				break;
			case 'O':
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				break;
			case 'T':
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);

				break;
			case 'S':
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);

				break;
			case 'Z':
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);

				break;
			case 'L':
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);

				break;
			case 'J':
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);
				rotationBlock.Add(Vector2[]);

				break;
			default:
				break;
		}
	
		float totalScore = 0;
		foreach(rotationBlock){
		Verctor2Int[] currentBlock = rotationBlock[]; //�� ��ǥ ��������
		if(IsWall(x, rotationBlock[])){ // ���� �곪
			y = dictxkey[x]�� �ִ밪 + 1;
			if(dictxkey[x] == null){
				y = 0;
			}
			rotationScore = countBlock(y, rotationBlock);
			foreach(currentBlock){
				if(currentBlock[].x in dictykey[y]){ //�ش� ��ǥ�� ���� �̹� ������
					rotationScore = 0;
				}
			}		
			if(rotationScore > totalScore){
				totalScore = rotationScore;
				Vector2[] totalRotation = rotationBlock[];	//���� ȸ�� ����
			}
		}
	}
	}
	���� ���
	IsWall(int x, rotationBlock){
		foreach(rotationBlock[]){
			move = (���� ��ġ) - x;
			if(move > 0){
				if((���� ���� ���� x��ǥ) - move >= ���� ���� �� x��ǥ){
					return true;
				}
			else{
				if((���� ���� ū x��ǥ) - move <= ���� ������ �� x��ǥ){
					return true;
				}
			}
			return false;
		}
	}	

	����ġ
	rotationBlock[]���� ��� �� ���
	(����ġ ���� x����)
	countBlock(int y, rotationBlock){
		int count = 0;
		Vector2[] currentBlock= rotationBlock[];
		foreach(currentBlock){	
			if(currentBlock[].(x-1) in dictykey[y+1] || currentBlock.(x+1) in dicykey[y+1]){
				count++;		
			}
			else if(currentBlock.(x-1) in dictykey[y+1] && currentBlock.(x+1) in dictykey[y+1]){ // �翷 Ȯ��
				count += 2;
			}
			if(currentBlock[].(y-1) in dictxkey[x]){	//�Ʒ� Ȯ��
				count++;
			}
			if(dictkey[x] == null){ //�ٴڿ� �����ϸ�
				count++;
			}
		}
	}


	*/


}
