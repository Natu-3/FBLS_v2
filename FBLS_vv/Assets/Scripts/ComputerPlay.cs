using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlay : MonoBehaviour
{
	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////	
	int comOutput; //��ǻ�Ͱ� getKey�� �����ϴ� �Է°�
		
	void comCheck(void){ //����� ���� ��ġ �� pathã�� �Լ�
			int comX[10]; //��� path X��ǥ������(���� [0]�� [1]�� ���)
			int comY[10]; //��� path Y��ǥ������(���� [0]�� [1]�� ���)
			int comR[10]; //��� path ȸ����������(���� �̻��)
			int comXYRP; //path����(pointer) ����
			int comGameValue[HEIGHT][WIDTH]; //���� �������� ��ǥvalue�� ����               <<<<< �̰Ŵ�� computer�� Blockposition Ŭ���� ����ϱ�
			int comGameUnder[HEIGHT][WIDTH]; //���� �����ǿ��� ��ϾƷ� ������� �ִ� ���� ����  <<<<< ����� ã�� �˰��� �����ؾ���
			bool comCheck_on; //��� ���� ��ġ �� path ������ �ʿ��� ��� true
			int comCheckAroundValue(int x, int y, int rotation); //����� �ֺ����� ���� ���� �� ���
		}
	const int speed[11]={20,20,15,10,20,10,5,15,10,2,1}; //�ӵ� ���� -- �Ⱦ���

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
			for(int i=0;i<10;i++){ //�ʱ�ȭ
				comX[i]=0;
				comY[i]=0;
				comR[i]=0;
			}
			comXYRP=0;

			for(int i=0;i<HEIGHT;i++){ //valueǥ(+��) ���ϱ�
				for(int j=0;j<WIDTH;j++){
					if(gameOrg[i][j]>0) comGameValue[i][j]=i;
					else comGameValue[i][j]=0;
				}
			}
			for(int i=0;i<HEIGHT-1;i++){ //valueǥ(-��) ���ϱ�
				for(int j=0;j<WIDTH;j++){
					if(comGameValue[i][j]==0&&(comGameValue[i][j-1]>0||comGameValue[i][j+1]>0)) comGameValue[i][j]=(-i);
				}
			}
			for(int i=0;i<HEIGHT;i++){ //underǥ �ʱ�ȭ
				for(int j=0;j<WIDTH;j++){
					comGameUnder[i][j]=0;
				}
			}
			for(int i=0;i<HEIGHT;i++){ //under mark�ϱ�
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

			for(int i=0;i<21;i++){ // ���� ��ġ �� pathã��
				for(int j=0;j<WIDTH;j++){
					for(int k=0;k<4;k++){		

						if(checkCrush(j,i+1,k)==false&&checkCrush(j,i,k)==true&&comCheckAroundValue(j,i,k)>val){
							bool onUnder=false;//onUnder�� hard drop�� �������� �Ǻ�: false�� ���� , true�� �Ұ���(���� crush�� ����)						
							for(int m=0;m<4;m++){ 
								for(int n=0;n<4;n++){
									if(block.shape[block.type][k][m][n]==1&&comGameUnder[i+m][j+n]==1){
										onUnder=true;
									}
								}
							}					
							if(onUnder==false){ // hard drop�� �����ϸ� ����path		
								comXYRP=0;
								val=comCheckAroundValue(j,i,k);
								comX[0]=j;
								comY[0]=i;
								comR[0]=k;						
							}
							if(onUnder==true){ // hard drop�� �Ұ����ϸ� ��ȸ�մϴ�.
								comXYRP=0;

								if(checkCrush(j+1,i,k)==false&&checkCrush(j-1,i,k)==false&&checkCrush(j,i-1,k)==false) break;

								int tempX1=j;
								int tempY1=i;
								int tempX2=j;
								int tempY2=i;
								int tempXYRP=0;

								bool underStuck=true; //�� ������ ���� �ִ��� �ƴ����� �Ǻ�. ������ ���� �ִٰ� �����ϰ� �ⱸ�� ������ false
								for(int s=1;s>-2;s-=2){ //���� �ѹ� ��ȸ�ؼ� ���� �ִ����� �Ǻ��Ҽ� ����..
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

								if(underStuck==false){ //1���� ��ȸ�� ������ ��� ���� path������Ʈ
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
//���� path�� ã�ư��� �ϴ� �κ� <--------- computer�� ���������� �μ� �Ѱܼ� �۵���Ű��
	if(block.rotation!=comR[comXYRP]) comOutput=ROTATE;
	else if(comX[comXYRP]>block.x) comOutput=RIGHT;
	else if(comX[comXYRP]<block.x) comOutput=LEFT;
	else if(comX[comXYRP]==block.x) if(comXYRP==0) comOutput=SPACE; else  comOutput=DOWN;

	if(comX[comXYRP]==block.x && comY[comXYRP]==block.y && comR[comXYRP]==block.rotation && comXYRP>0) comXYRP--;
}
/////////////////////////////////////////////////////////////////////////////////////////////////*/
}
