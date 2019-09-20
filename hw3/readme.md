### 空间与运动

1、简答并用程序验证

- 游戏对象运动的本质是什么？

  游戏运动本质就是使用矩阵变换（平移、旋转、缩放）改变游戏对象的空间属性。

- 请用三种方法以上方法，实现物体的抛物线运动。（如，修改Transform属性，使用向量Vector3的方法…）

  使用transform

  ```
      private float xSpeed = 0.3f*Mathf.Cos(Mathf.PI/4);
      private float ySpeed = 0.3f*Mathf.Sin(Mathf.PI/4);
      //private float angle = Mathf.PI / 2;
      public float gravity = 3f;
  
      void Start()
      {
          
      }
      //Update() 每一帧调用一次
  
      // Update is called once per frame
      void Update()
      {
          this.transform.position += xSpeed*Vector3.left * Time.deltaTime;
          this.transform.position += ySpeed * Vector3.down * Time.deltaTime;
          ySpeed += gravity * Time.deltaTime;
      }
  
  
  
  ```

  使用Vector3

  ```
   private GameObject MyCapsule;
      private Vector3 vx;
      private Vector3 vz;
      private Vector3 a;
      private float t;
      // Use this for initialization
      void Start()
      {
          MyCapsule = GameObject.Find("Capsule");
          vx = (float)0.5 * Vector3.right;
          vz = Vector3.zero;
          a = (float)0.1 * Vector3.forward;
          t = Time.deltaTime;
      }
  
      // Update is called once per frame
      void Update()
      {
          MyCapsule.transform.position += vx;
          MyCapsule.transform.position += vz;
          vz += a * t;
      }
  ```

  使用Rigidbody

  ```
  private Rigidbody rigid;
      private Vector3 v0;
  
      // Use this for initialization
      void Start()
      {
          rigid = this.GetComponent<Rigidbody>();
          v0 = new Vector3(3, 10, 0);
          rigid.velocity = v0;
      }
  
      // Update is called once per frame
      void Update()
      {
  
      }
  ```

  

- 写一个程序，实现一个完整的太阳系， 其他星球围绕太阳的转速必须不一样，且不在一个法平面上。

  自转

  ```
  Earth.Rotate(Vector3.up * 30 * Time.deltaTime);
  ```

  

  公转

  ```
  
  Earth.RotateAround(Sun.position, axis3, 10 * Time.deltaTime);
  ```

  轨迹

  添加一个Trail Render组件记录行星运动轨迹

  ![](D:\c31\3d\Unity3d_learning\hw3\1.PNG)

  ```
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  
  public class Solar : MonoBehaviour
  {
  
      public Transform Sun;
      public Transform Mercury;
      public Transform Venus;
      public Transform Earth;
      public Transform Moon;
      public Transform Mars;
      public Transform Jupiter;
      public Transform Saturn;
      public Transform Uranus;
      public Transform Neptune;
      public Transform Pluto;
      public Vector3 axis1;
      public Vector3 axis2;
      public Vector3 axis3;
      public Vector3 axis4;
      public Vector3 axis5;
      public Vector3 axis6;
      public Vector3 axis7;
      public Vector3 axis8;
      public Vector3 axis9;
  
      // Use this for initialization
      void Start()
      {
          //初始化公转轴
          axis1 = new Vector3(0, 1, 5);
          axis2 = new Vector3(0, 1, 4);
          axis3 = new Vector3(0, 1, 3);
          axis4 = new Vector3(0, 1, 2);
          axis5 = new Vector3(0, 1, 1);
          axis6 = new Vector3(0, 2, 1);
          axis7 = new Vector3(0, 3, 1);
          axis8 = new Vector3(0, 4, 1);
          axis9 = new Vector3(0, 5, 1);
      }
  
      // Update is called once per frame
      void Update()
      {
          //获取对象
          Sun = GameObject.Find("Sun").transform;
          Mercury = GameObject.Find("Mercury").transform;
          Venus = GameObject.Find("Venus").transform;
          Earth = GameObject.Find("Earth").transform;
          Moon = GameObject.Find("Moon").transform;
          Mars = GameObject.Find("Mars").transform;
          Jupiter = GameObject.Find("Jupiter").transform;
          Saturn = GameObject.Find("Saturn").transform;
          Uranus = GameObject.Find("Uranus").transform;
          Neptune = GameObject.Find("Neptune").transform;
          Pluto = GameObject.Find("Pluto").transform;
  
          Sun.Rotate(Vector3.up * 10 * Time.deltaTime);
  
          Mercury.RotateAround(Sun.position, axis1, 47 * Time.deltaTime);
          Mercury.Rotate(Vector3.up * 50 * Time.deltaTime);
  
          Venus.RotateAround(Sun.position, axis2, 35 * Time.deltaTime);
          Venus.Rotate(Vector3.up * 30 * Time.deltaTime);
  
          Earth.RotateAround(Sun.position, axis3, 10 * Time.deltaTime);//公转
          Earth.Rotate(Vector3.up * 30 * Time.deltaTime);//自转
  
          Moon.RotateAround(Earth.position, Vector3.up, 359 * Time.deltaTime);
          Moon.Rotate(Vector3.up * 30 * Time.deltaTime);
  
          Mars.RotateAround(Sun.position, axis4, 24 * Time.deltaTime);
          Mars.Rotate(Vector3.up * 30 * Time.deltaTime);
  
          Jupiter.RotateAround(Sun.position, axis5, 13 * Time.deltaTime);
          Jupiter.Rotate(Vector3.up * 30 * Time.deltaTime);
  
          Saturn.RotateAround(Sun.position, axis6, 9 * Time.deltaTime);
          Saturn.Rotate(Vector3.up * 30 * Time.deltaTime);
  
          Uranus.RotateAround(Sun.position, axis7, 6 * Time.deltaTime);
          Uranus.Rotate(Vector3.up * 30 * Time.deltaTime);
  
          Neptune.RotateAround(Sun.position, axis8, 5 * Time.deltaTime);
          Neptune.Rotate(Vector3.up * 30 * Time.deltaTime);
  
          Pluto.RotateAround(Sun.position, axis9, 3 * Time.deltaTime);
          Pluto.Rotate(Vector3.up * 30 * Time.deltaTime);
      }
  }
  ```

  

2、编程实践

- 阅读以下游戏脚本

> Priests and Devils
>
> Priests and Devils is a puzzle game in which you will help the Priests and Devils to cross the river within the time limit. There are 3 priests and 3 devils at one side of the river. They all want to get to the other side of this river, but there is only one boat and this boat can only carry two persons each time. And there must be one person steering the boat from one side to the other side. In the flash game, you can click on them to move them and click the go button to move the boat to the other direction. If the priests are out numbered by the devils on either side of the river, they get killed and the game is over. You can try it in many > ways. Keep all priests alive! Good luck!

程序需要满足的要求：

- play the game ( http://www.flash-game.net/game/2535/priests-and-devils.html )
- 列出游戏中提及的事物（Objects）

牧师Priest，魔鬼Devil，船Boat，河流River，两岸边Coast

- 用表格列出玩家动作表（规则表），注意，动作越少越好

| **牧师，魔鬼上船** | **船靠岸且船中人数不到2人**                        |
| ------------------ | -------------------------------------------------- |
| **牧师魔鬼下船**   | **船靠岸**                                         |
| **船过河**         | **船上不少于1人**                                  |
| **失败**           | **同一岸上人数加此时船靠此岸人数魔鬼人数大于牧师** |
| **成功**           | **魔鬼牧师都在左岸**                               |

- 请将游戏中对象做成预制

![](D:\c31\3d\Unity3d_learning\hw3\2.PNG)

- 在 GenGameObjects 中创建 长方形、正方形、球 及其色彩代表游戏中的对象。
- 使用 C# 集合类型 有效组织对象
- 整个游戏仅 主摄像机 和 一个 Empty 对象， **其他对象必须代码动态生成！！！** 。 整个游戏不许出现 Find 游戏对象， SendMessage 这类突破程序结构的 通讯耦合 语句。 **违背本条准则，不给分**
- 请使用课件架构图编程，**不接受非 MVC 结构程序**
- 注意细节，例如：船未靠岸，牧师与魔鬼上下船运动中，均不能接受用户事件！

代码分为四个文件，分别进行用户界面控制，点击物体事件控制，加载预制控制，和总体游戏控制

效果图

![](D:\c31\3d\Unity3d_learning\hw3\3.PNG)



