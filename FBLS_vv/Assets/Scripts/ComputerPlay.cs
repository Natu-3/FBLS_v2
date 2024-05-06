using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlay : MonoBehaviour
{
	/*
	//블럭이 생성되었을 때 메소드 실행됨(블럭의 상태 I , O, T , S , Z , L ,J 값 인수로 받아옴 : status )
	calRotationBlock(status)
	{

		//비어있는 리스트(rotationBlock) 하나 함수 내부에 생성
		List<Vector2[]> rotationBlock = new List<Vector2[]>;

		//받아온status로 switch-case문 써서 rotationBlock 리스트에 회전가능한 상태 채움

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
		Verctor2Int[] currentBlock = rotationBlock[]; //블럭 좌표 가져오기
		if(IsWall(x, rotationBlock[])){ // 벽에 닿나
			y = dictxkey[x]의 최대값 + 1;
			if(dictxkey[x] == null){
				y = 0;
			}
			rotationScore = countBlock(y, rotationBlock);
			foreach(currentBlock){
				if(currentBlock[].x in dictykey[y]){ //해당 좌표에 블럭이 이미 있으면
					rotationScore = 0;
				}
			}		
			if(rotationScore > totalScore){
				totalScore = rotationScore;
				Vector2[] totalRotation = rotationBlock[];	//최종 회전 상태
			}
		}
	}
	}
	벽낌 계산
	IsWall(int x, rotationBlock){
		foreach(rotationBlock[]){
			move = (생성 위치) - x;
			if(move > 0){
				if((블럭의 제일 작은 x좌표) - move >= 보드 왼쪽 끝 x좌표){
					return true;
				}
			else{
				if((블럭의 제일 큰 x좌표) - move <= 보드 오른쪽 끝 x좌표){
					return true;
				}
			}
			return false;
		}
	}	

	가중치
	rotationBlock[]에서 닿는 면 계산
	(가중치 높은 x값만)
	countBlock(int y, rotationBlock){
		int count = 0;
		Vector2[] currentBlock= rotationBlock[];
		foreach(currentBlock){	
			if(currentBlock[].(x-1) in dictykey[y+1] || currentBlock.(x+1) in dicykey[y+1]){
				count++;		
			}
			else if(currentBlock.(x-1) in dictykey[y+1] && currentBlock.(x+1) in dictykey[y+1]){ // 양옆 확인
				count += 2;
			}
			if(currentBlock[].(y-1) in dictxkey[x]){	//아래 확인
				count++;
			}
			if(dictkey[x] == null){ //바닥에 착지하면
				count++;
			}
		}
	}


	*/


}
