using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GUITest : MonoBehaviour
{
    //几个变量
    private int player = 0;  // 1 player1 2 player2
    private int[,] qipan = new int[3, 3]; //记录棋盘 0 为空 1，player1，2player2
    private int isWin = 0; // 0 win,1 p1 win ,2 p2 win

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
    }

    void OnGUI()
    {
        //检测输赢
        isWin = check();

        //构建棋盘


        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {

                if (qipan[i, j] == 1 && player != 0)
                {
                    GUI.Button(new Rect(50 + i * 50, 100 + j * 50, 50, 50), "1");
                }
                else if (qipan[i, j] == 2 && player != 0)
                {
                    GUI.Button(new Rect(50 + i * 50, 100 + j * 50, 50, 50), "2");
                }
                else if (qipan[i, j] == 0 && player != 0)
                {
                    if (GUI.Button(new Rect(50 + i * 50, 100 + j * 50, 50, 50), " "))
                    {
                        if (isWin == 0)
                        {
                            qipan[i, j] = player;
                        }
                        player = player % 2 + 1;
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(50 + i * 50, 100 + j * 50, 50, 50), " "))
                    {

                    }
                }


            }
        }
        //开始键
        if(player == 0)
        {
            if (GUI.Button(new Rect(75, 50, 100, 30), "START"))
                    {
                        player = 1;
                    }
        }
        else
        {
            if (GUI.Button(new Rect(75, 50, 100, 30), "RESTAET"))
            {
                reset();
                player = 1;
            }
        }

        //标识player

        //显示文字
        if (isWin == 1)
        {
            GUI.Label(new Rect(300, 80, 200, 50), "Player1 win!!!");

        }
        else if (isWin == 2)
        {
            GUI.Label(new Rect(300, 80, 200, 50), "Player2 win!!!");
        }
        else
        {

            GUI.Label(new Rect(300, 80, 200, 50), "No one win...");
        }
        //title
        GUI.Label(new Rect(300, 20, 200, 50), "welcome to 井字棋");
    }
    void reset()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                qipan[i, j] = 0;
            }
        }
    }

    int check()
    {

        for (int i = 0; i < 3; i++)
        {
            if (qipan[0, i] == qipan[1, i] && qipan[1, i] == qipan[2, i])
            {
                return qipan[0, i];
            }
            if (qipan[i, 0] == qipan[i, 1] && qipan[i, 1] == qipan[i, 2])
            {
                return qipan[i, 0];
            }
        }

        if ((qipan[0, 0] == qipan[1, 1]) && (qipan[1, 1] == qipan[2, 2])) return qipan[0, 0];
        if ((qipan[0, 2] == qipan[1, 1]) && (qipan[1, 1] == qipan[2, 0])) return qipan[0, 0];
        return 0;
    }
}
