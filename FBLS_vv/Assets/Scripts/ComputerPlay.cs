using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlay : MonoBehaviour
{
	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////	
	int comOutput; //컴퓨터가 getKey로 전달하는 입력값
		
	void comCheck(void){ //블록을 놓을 위치 및 path찾는 함수
			int comX[10]; //블록 path X좌표모음집(현재 [0]과 [1]만 사용)
			int comY[10]; //블록 path Y좌표모음집(현재 [0]과 [1]만 사용)
			int comR[10]; //블록 path 회전값모음집(현재 미사용)
			int comXYRP; //path순서(pointer) 저장
			int comGameValue[HEIGHT][WIDTH]; //현재 게임판의 좌표value를 저장               <<<<< 이거대신 computer의 Blockposition 클래스 사용하기
			int comGameUnder[HEIGHT][WIDTH]; //현재 게임판에서 블록아래 빈공간이 있는 곳을 저장  <<<<< 빈공간 찾는 알고리즘 구상해야함
			bool comCheck_on; //블록 놓을 위치 및 path 재계산이 필요한 경우 true
			int comCheckAroundValue(int x, int y, int rotation); //블록의 주변값을 더해 최종 값 계산
		}
	const int speed[11]={20,20,15,10,20,10,5,15,10,2,1}; //속도 저장 -- 안쓴다

	int comCheckAroundValue(int x, int y, int rotation){
		int sum=0;
		for(int i=0;i<4;i++){
			for(int j=0;j<4;j++){ 
				if(block.shape[block.type][rotation][i][j]==1){
					for(int k=0;k<4;k++){
						switch(k){
						case 0:
							if(((i==0)&&(comGameValue[y+i-1][x+j]>0)) || 
								((i!=0)&&(block.shape[block.type][rotation][i-1][j]!=1)&&(comGameValue[y+i-1][x+j]>0))) sum+=comGameValue[y+i-1][x+j];
							break;
						case 1:
							if((i==4)|| 
								(block.shape[block.type][rotation][i+1][j]!=1)) sum+=comGameValue[y+i+1][x+j];
							break;
						case 2:
							if(((j==0)&&(comGameValue[y+i][x+j-1]>0)) || 
								((j!=0)&&(block.shape[block.type][rotation][i][j-1]!=1)&&(comGameValue[y+i][x+j-1]>0))) sum+=comGameValue[y+i][x+j-1];
							break;
						case 3:
							if(((j==4)&&comGameValue[y+i][x+j+1]>0) || 
								((j!=4)&&(block.shape[block.type][rotation][i][j+1]!=1)&&(comGameValue[y+i][x+j+1]>0))) sum+=comGameValue[y+i][x+j+1];
							break;
						}
					}
				}
			}
		}	
		return sum;
	}
	void comCheck(void){
		int val=0;

		if(comCheck_on==true){
			for(int i=0;i<10;i++){ //초기화
				comX[i]=0;
				comY[i]=0;
				comR[i]=0;
			}
			comXYRP=0;

			for(int i=0;i<HEIGHT;i++){ //value표(+값) 구하기
				for(int j=0;j<WIDTH;j++){
					if(gameOrg[i][j]>0) comGameValue[i][j]=i;
					else comGameValue[i][j]=0;
				}
			}
			for(int i=0;i<HEIGHT-1;i++){ //value표(-값) 구하기
				for(int j=0;j<WIDTH;j++){
					if(comGameValue[i][j]==0&&(comGameValue[i][j-1]>0||comGameValue[i][j+1]>0)) comGameValue[i][j]=(-i);
				}
			}
			for(int i=0;i<HEIGHT;i++){ //under표 초기화
				for(int j=0;j<WIDTH;j++){
					comGameUnder[i][j]=0;
				}
			}
			for(int i=0;i<HEIGHT;i++){ //under mark하기
				for(int j=1;j<WIDTH-1;j++){
					if(gameOrg[i][j]>0&&gameOrg[i-1][j]==0){
						for(int k=i-1;k>0;k--){
							if(gameOrg[k][j]>0){
								for(int m=i-1;m>k;m--) comGameUnder[m][j]=1;
								break;
							}
						}
					}
				}
			}

			for(int i=0;i<21;i++){ // 놓을 위치 및 path찾기
				for(int j=0;j<WIDTH;j++){
					for(int k=0;k<4;k++){		

						if(checkCrush(j,i+1,k)==false&&checkCrush(j,i,k)==true&&comCheckAroundValue(j,i,k)>val){
							bool onUnder=false;//onUnder는 hard drop이 가능한지 판별: false면 가능 , true면 불가능(위에 crush가 있음)						
							for(int m=0;m<4;m++){ 
								for(int n=0;n<4;n++){
									if(block.shape[block.type][k][m][n]==1&&comGameUnder[i+m][j+n]==1){
										onUnder=true;
									}
								}
							}					
							if(onUnder==false){ // hard drop이 가능하면 단일path		
								comXYRP=0;
								val=comCheckAroundValue(j,i,k);
								comX[0]=j;
								comY[0]=i;
								comR[0]=k;						
							}
							if(onUnder==true){ // hard drop이 불가능하면 우회합니다.
								comXYRP=0;

								if(checkCrush(j+1,i,k)==false&&checkCrush(j-1,i,k)==false&&checkCrush(j,i-1,k)==false) break;

								int tempX1=j;
								int tempY1=i;
								int tempX2=j;
								int tempY2=i;
								int tempXYRP=0;

								bool underStuck=true; //이 공간이 갇혀 있는지 아닌지를 판별. 무조건 갇혀 있다고 생각하고 출구가 있으면 false
								for(int s=1;s>-2;s-=2){ //현재 한번 우회해서 갈수 있는지만 판별할수 있음..
									for(int x=0;x<WIDTH;x++){
										if(underStuck==false) break;
										for(int y=0;y<HEIGHT;y++){
											if(tempY1-y==0){
												underStuck=false;
												tempXYRP++;
												comX[tempXYRP]=j+x*s;
												comY[tempXYRP]=tempY1;
												comR[tempXYRP]=k;
												break;
											}
											else if(checkCrush(j+x*s,tempY1-y,k)==true){
												tempX2=j+x*s;
												tempY2=tempY1-y;
											}
											else if(checkCrush(tempX1+x*s,tempY1-y,k)==false) break;
										
										}
										if(tempX1==tempX2 && tempY1==tempY2 && x!=0) break;
										else {
											tempX1=tempX2;
											tempY1=tempY2;
										}
									}
								}

								if(underStuck==false){ //1차로 우회가 가능한 경우 최종 path업데이트
									val=comCheckAroundValue(j,i,k);							
									comX[0]=j;
									comY[0]=i;
									comR[0]=k;	
									comXYRP=tempXYRP;
								}
							}
						}	
					}
				}
			}
		comCheck_on=false;
		}
//실제 path로 찾아가게 하는 부분 <--------- computer용 스테이지에 인수 넘겨서 작동시키기
	if(block.rotation!=comR[comXYRP]) comOutput=ROTATE;
	else if(comX[comXYRP]>block.x) comOutput=RIGHT;
	else if(comX[comXYRP]<block.x) comOutput=LEFT;
	else if(comX[comXYRP]==block.x) if(comXYRP==0) comOutput=SPACE; else  comOutput=DOWN;

	if(comX[comXYRP]==block.x && comY[comXYRP]==block.y && comR[comXYRP]==block.rotation && comXYRP>0) comXYRP--;
}
/////////////////////////////////////////////////////////////////////////////////////////////////*/
}
