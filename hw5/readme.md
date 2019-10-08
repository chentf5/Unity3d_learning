Unity3d作业||与游戏世界交互

编写一个简单的鼠标打飞碟（Hit UFO）游戏

- 游戏内容要求：
  1. 游戏有 n 个 round，每个 round 都包括10 次 trial；
  2. 每个 trial 的飞碟的色彩、大小、发射位置、速度、角度、同时出现的个数都可能不同。它们由该 round 的 ruler 控制；
  3. 每个 trial 的飞碟有随机性，总体难度随 round 上升；
  4. 鼠标点中得分，得分规则按色彩、大小、速度不同计算，规则可自由设定。
- 游戏的要求：
  - 使用带缓存的工厂模式管理不同飞碟的生产与回收，该工厂必须是场景单实例的！具体实现见参考资源 Singleton 模板类
  - 近可能使用前面 MVC 结构实现人机交互与游戏模型分离



### 工厂类模式

- 需要进行频繁的游戏对象的产生和销毁，使用工厂类来减少销毁次数
- 用工厂方法代替new操作的一种模式。这种类型的设计模式属于创建型模式，它提供了一种创建对象的最佳方式。**注意事项：**作为一种创建类模式，在任何需要生成复杂对象的地方，都可以使用工厂方法模式。有一点需要注意的地方就是复杂对象适合使用工厂模式，而简单对象，特别是只需要通过 new 就可以完成创建的对象，无需使用工厂模式。如果使用工厂模式，就需要引入一个工厂类，会增加系统的复杂度。

### 代码实现

- 工厂类

  使用工厂类减少游戏的开销，采用工厂模式对飞碟进行管理，进行飞碟的产生和回收

  ```
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using MyGame;
  
  public class DiskFactory : MonoBehaviour
  {
      private static DiskFactory _instance;
      public FirstControl sceneControler { get; set; }
      GameObject diskPrefab;
      public DiskControl diskData;
      public List<GameObject> used;//使用队列
      public List<GameObject> free;//释放队列
      // Use this for initialization
  	//得到实例
      public static DiskFactory getInstance()
      {
          return _instance;
      }
  
      private void Awake()
      {
          if (_instance == null)
          {
              _instance = Singleton<DiskFactory>.Instance;
              _instance.used = new List<GameObject>();
              _instance.free = new List<GameObject>();
              diskPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk"), new Vector3(40, 0, 0), Quaternion.identity);
          }
          Debug.Log("instance: " + _instance);
      }
      public void Start()
      {
          sceneControler = (FirstControl)Director.getInstance().sceneCtrl;
          Debug.Log(sceneControler);
          //Debug.Log(this);
          //Debug.Log(_instance);
          sceneControler.factory = _instance;
          Debug.Log("DiskFactory: factory");
          //Debug.Log(sceneControler.factory);        
      }
  	//产生新飞碟
      public GameObject getDisk(int round)
      {
          if (sceneControler.scoreRecorder.Score >= round * 10)
          {
              if (sceneControler.user.round < 3)
              {
                  sceneControler.user.round++;
                  sceneControler.user.num = 0;
                  sceneControler.scoreRecorder.Score = 0;
              }
              else
              {
                  sceneControler.user.game = 2;
              }
          }
          else
          {
              if (sceneControler.user.num >= 10)
              {
                  sceneControler.user.game = 1;
              }
          }
          GameObject newDisk;
          RoundControl diskOfCurrentRound = new RoundControl(round);
          if (free.Count == 0) // if no free disk, then create a new disk
          {
              newDisk = GameObject.Instantiate(diskPrefab) as GameObject;
              newDisk.AddComponent<ClickGUI>();
              diskData = newDisk.AddComponent<DiskControl>();
          }
          else // else let the first free disk be the newDisk
          {
              newDisk = free[0];
              free.Remove(free[0]);
              newDisk.SetActive(true);
              Debug.Log("get from free");
          }
          diskData = newDisk.GetComponent<DiskControl>();
          diskData.color = diskOfCurrentRound.color;
          //Debug.Log("free: " + free.Count);
  
          newDisk.transform.localScale = diskOfCurrentRound.scale * diskPrefab.transform.localScale;
          newDisk.GetComponent<Renderer>().material.color = diskData.color;
  
          used.Add(newDisk);
          return newDisk;
      }
  	//释放飞碟
      public void freeDisk(GameObject disk1)
      {
          used.Remove(disk1);
          disk1.SetActive(false);
          free.Add(disk1);
          Debug.Log("free: " + free.Count);
          return;
      }
  	//重新开始
      public void Restart()
      {
          used.Clear();
          free.Clear();
      }
  }
  
  ```

  

- 场景控制baseCode

  - 导演类

    ```
        public class Director : System.Object
        {
            private static Director _instance;
            public ISceneControl sceneCtrl { get; set; }
    
            public bool playing { get; set; } //
    
            public static Director getInstance()
            {
                if (_instance == null) return _instance = new Director();
                else return _instance;
            }
    
            public int getFPS()
            {
                return Application.targetFrameRate;
            }
    
            public void setFPS(int fps)
            {
                Application.targetFrameRate = fps;
            }
        }
    ```

  - 单例模式的模板

    ```
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
        {
    
            protected static T instance;
            public static T Instance
            {
                get
                {
                    if (instance == null)
                    {
                        //instance = new GameObject(typeof(T).Name).AddComponent<T>();
                        instance = (T)FindObjectOfType(typeof(T));
                        if (instance == null)
                        {
                            Debug.LogError("no scene instance");
                        }
                    }
                    return instance;
                }
            }
        }
    ```

  - 动作控制器，场景控制器，用户控制器，飞碟工厂控制器，回合控制器的一些接口类

    ```
    public interface ISSActionCallback
        {
            void ActionDone(SSAction source);
        }
    
        public interface ISceneControl
        {
            void LoadPrefabs();
            void PlayDisk();
        }
    
        public interface IUserAction
        {
            void Begin();
            void Hit(DiskControl diskCtrl);
            void Restart();
            void SwitchMode();
        }
    
        public class DiskControl : MonoBehaviour
        {
            public float size;
            public Color color;
            public float speed;
            public bool hit = false;
            public SSAction action;
        }
    
        public class RoundControl
        {
            int round = 0;
            public float scale;
            public Color color;
            public RoundControl(int r)
            {
                round = r;
                scale = 5 - r;
                switch (r)
                {
                    case 1:
                        color = Color.blue;
                        break;
                    case 2:
                        color = Color.red;
                        break;
                    case 3:
                        color = Color.yellow;
                        break;
                }            
            }
        }
    
        public class RecordControl : MonoBehaviour
        {
            public int Score = 0;//分数
            public FirstControl sceneControler { get; set; }
            // Use this for initialization
            void Start()
            {
                sceneControler = (FirstControl)Director.getInstance().sceneCtrl;
                sceneControler.scoreRecorder = this;
            }
            public void add()
            {
                Score += sceneControler.user.round;
                sceneControler.user.score = Score;
                //Debug.Log(Score);
            }
            public void miss()
            {
                Score -= sceneControler.user.round;
                sceneControler.user.score = Score;
                //Debug.Log(Score);
            }
        }
    }
    ```

    

- firstControll总控制

  ```
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using UnityEngine.UI;
  using MyGame;
  using UnityEngine.SceneManagement;
  
  public class FirstControl : MonoBehaviour, ISceneControl, IUserAction {
  
      //public ActionManager MyActionManager { get; set; }
      public ActionManagerAdapter myAdapter;
      public DiskFactory factory { get; set; }
      public RecordControl scoreRecorder;
      public UserGUI user;
      public static float time = 0;
      
      void Awake()
      {
          Director diretor = Director.getInstance();
          diretor.sceneCtrl = this;
          //Debug.Log("FirstControl: factory");
          //Debug.Log(factory);
          //Debug.Log(diretor);                                
      }
  
      // Use this for initialization
      void Start()
      {
          Begin();
      }
  	
  	// Update is called once per frame
  	void Update () {
  
      }
  
      void FixedUpdate()
      {
          //Debug.Log("first fixedupdate mode: " + myAdapter.mode);
  
          //Time.fixedDeltaTime = 1;
          time += Time.deltaTime;
          if (time < 1)
              return;
          time = 0;
  
          // if round <= 3 and is playing, 
  
          if (user.round <= 3 && user.game == 0)
          {
              PlayDisk();
              user.num++;
          }
      }
  
      public void LoadPrefabs()
      {
      }
  
      public void Begin()
      {
          LoadPrefabs();
          //MyActionManager = gameObject.AddComponent<ActionManager>() as ActionManager;
          //Debug.Log(MyActionManager);
          myAdapter = new ActionManagerAdapter(gameObject);
          scoreRecorder = gameObject.AddComponent<RecordControl>();
          user = gameObject.AddComponent<UserGUI>();
          user.Begin();
      }
  
      public void Hit(DiskControl diskCtrl)
      {        
          if (user.game == 0)
          {            
              Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
              RaycastHit hit;
              
              if (Physics.Raycast(ray, out hit))
              {
                  //Debug.Log(hit.GetType());
                  //Debug.Log(hit.transform);
                  hit.collider.gameObject.SetActive(false);
                  //hit.collider.gameObject.transform.position = new Vector3(6 - Random.value * 12, 0, 0);
                  //factory.freeDisk(hit.collider.gameObject);
                  Debug.Log("Hit");
                  hit.collider.gameObject.GetComponent<DiskControl>().hit = true;
                  scoreRecorder.add();
              }
              else
              {
                  Debug.Log("Miss");
                  scoreRecorder.miss();
              }
          }
          //user.status = Check();
      }
      public void PlayDisk()
      {
          //MyActionManager.playDisk(user.round);
          myAdapter.PlayDisk(user.round);
      }
      public void Restart()
      {
          SceneManager.LoadScene("scene");
      }
      public void SwitchMode()
      {
          Debug.Log("Switch Mode");
          myAdapter.SwitchActionMode();
      }
      public int Check()
      {
          return 0;
      }
  }
  
  ```

  

- 用户  UserGUI,点击事件 ClickGUI

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyGame;

public class UserGUI : MonoBehaviour {
    private IUserAction action;     
    GUIStyle LabelStyle1;
    GUIStyle LabelStyle2;
    GUIStyle ButtonStyle;
    private int timeLeft = 60;
    public int score = 0;
    public static float time = 0;

    public int round = 1;
    public int CoolTimes = 3; 
    public int game = 0; // status
    public int num = 0; // numbers of disk
    public int mode = 0; // action mode

    // Use this for initialization
    void Start () {
        action = (IUserAction)Director.getInstance().sceneCtrl;

        LabelStyle1 = new GUIStyle();
        LabelStyle1.fontSize = 20;
        LabelStyle1.alignment = TextAnchor.MiddleCenter;

        LabelStyle2 = new GUIStyle();
        LabelStyle2.fontSize = 30;
        LabelStyle2.alignment = TextAnchor.MiddleCenter;

        ButtonStyle = new GUIStyle("Button");
        ButtonStyle.fontSize = 20;
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time < 1)
            return;
        time = 0;

        if (game == 3)
        {
            if (CoolTimes > 1) CoolTimes--;
            else game = 0;
        }
        if (game == 0)
        {
            timeLeft--;
        }
    }

    public void Restart()
    {
        timeLeft = 60;
        round = 1;
        CoolTimes = 3;
        game = 3;
        num = 0;
        score = 0;
        mode = 0;
    }

    public void Begin()
    {
        Restart();
    }

    void OnGUI () {
        string str = mode == 0 ? "Normal" : "Physics";
        GUI.Label(new Rect(Screen.width / 2 - 30, 10, 100, 50), "Mode: " + str, LabelStyle1);
        if (GUI.Button(new Rect(20, 20, 100, 50), "Switch", ButtonStyle)) // switch mode
        {            
            action.SwitchMode();
            mode = 1 - mode;
        }
        if (game == 0) // playing
        {
            GUI.Label(new Rect(Screen.width / 2 - 180, Screen.height / 2 - 160, 100, 50), "Round: " + round, LabelStyle1);
            GUI.Label(new Rect(Screen.width / 2 - 30, Screen.height / 2 - 160, 100, 50), "Time: " + timeLeft, LabelStyle1);
            GUI.Label(new Rect(Screen.width / 2 + 120, Screen.height / 2 - 160, 100, 50), "Score: " + score, LabelStyle1);
            if (timeLeft == 0) game = 1;
        }
        else if (game == 1) // game over
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 85, 100, 50), "Gameover!", LabelStyle2);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart", ButtonStyle))
            {
                game = 0;
                action.Restart();
            }
        }
        else if (game == 2) // win a round
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 85, 100, 50), "You win!", LabelStyle2);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart", ButtonStyle))
            {
                game = 0;
                action.Restart();
            }
        }
        else if (game == 3) // ready
        {
            GUI.Label(new Rect(Screen.width / 2 - 30, Screen.height / 2, 100, 50), CoolTimes.ToString(), LabelStyle2);
        }
    }

}

```



- ActionControl，基础动作管理器

  ```
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using MyGame;
  
  public class ActionManager : SSActionManager
  {
      public FirstControl sceneController;
      public DiskFactory diskFactory;
      public RecordControl scoreRecorder;
      public Emit EmitDisk;
      public GameObject Disk;
      int count = 0;
      // Use this for initialization
      protected void Start()
      {
          sceneController = (FirstControl)Director.getInstance().sceneCtrl;
          diskFactory = sceneController.factory;
          scoreRecorder = sceneController.scoreRecorder;
          sceneController.MyActionManager = this;
      }
  
      // Update is called once per frame
      protected new void Update()
      {
          // if round <= 3 and is playing, 
          if (sceneController.user.round <= 3 && sceneController.user.game == 0)
          {
              count++;
              if (count == 60)
              {
                  playDisk(sceneController.user.round);
                  sceneController.user.num++;
                  count = 0;
              }
              base.Update();
          }
      }
  
      public void playDisk(int round)
      {
          //Debug.Log(diskFactory);
          EmitDisk = Emit.GetSSAction();
          Disk = diskFactory.getDisk(round);
          this.AddAction(Disk, EmitDisk, this);
          Disk.GetComponent<DiskControl>().action = EmitDisk;
      }
  
      public void SSActionEvent(SSAction source)
      {
          if (!source.GameObject.GetComponent<DiskControl>().hit)
              scoreRecorder.miss();
          diskFactory.freeDisk(source.GameObject);
          source.GameObject.GetComponent<DiskControl>().hit = false;
      }
  }
  
  public class SSAction : ScriptableObject
  {
      public bool enable = true;
      public bool destroy = false;
  
      public GameObject GameObject { get; set; }
      public Transform Transform { get; set; }
      public ISSActionCallback Callback { get; set; }
  
      public virtual void Start()
      {
          throw new System.NotImplementedException();
      }
  
      public virtual void Update()
      {
          throw new System.NotImplementedException();
      }
  
      //public virtual void FixedUpdate()
      //{
      //    //throw new System.NotImplementedException();
      //}
  }
  
  public class SSMoveToAction : SSAction
  {
      public Vector3 des;
      public float speed;
  
      private SSMoveToAction() { }
  
      public static SSMoveToAction GetSSMoveToAction(Vector3 target, float speed)
      {
          SSMoveToAction action = CreateInstance<SSMoveToAction>();
          action.des = target;
          action.speed = speed;
          return action;
      }
  
      public override void Start() { }
  
      public override void Update()
      {
          Transform.position = Vector3.MoveTowards(Transform.position, des, speed * Time.deltaTime);
          if (Transform.position == des)
          {
              destroy = true;
              Callback.ActionDone(this);
          }
      }
  }
  public class SequenceAction : SSAction, ISSActionCallback
  {
      public List<SSAction> sequence;
      public int repeat = -1;
      public int start = 0;
  
      public static SequenceAction GetSequenceAction(int repeat, int start, List<SSAction> sequence)
      {
          SequenceAction action = CreateInstance<SequenceAction>();
          action.sequence = sequence;
          action.repeat = repeat;
          action.start = start;
          return action;
      }
  
      public override void Update()
      {
          if (sequence.Count == 0) return;
          if (start < sequence.Count)
          {
              sequence[start].Update();
          }
      }
  
      public void ActionDone(SSAction source)
      {
          source.destroy = false;
          start++;
          if (start >= sequence.Count)
          {
              start = 0;
              if (repeat > 0) repeat--;
              if (repeat == 0)
              {
                  destroy = true;
                  Callback.ActionDone(this);
              }
          }
      }
  
      public override void Start()
      {
          foreach (SSAction action in sequence)
          {
              action.GameObject = GameObject;
              action.Transform = Transform;
              action.Callback = this;
              action.Start();
          }
      }
  
      void OnDestroy()
      {
          foreach (SSAction action in sequence)
          {
              DestroyObject(action);
          }
      }
  }
  
  public class SSActionManager : MonoBehaviour, ISSActionCallback
  {
      private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
      private List<SSAction> waitingAdd = new List<SSAction>();
      private List<int> waitingDelete = new List<int>();
  
      protected void Update()
      {
          foreach (SSAction action in waitingAdd)
          {
              actions[action.GetInstanceID()] = action;
          }
          waitingAdd.Clear();
  
          foreach (KeyValuePair<int, SSAction> kv in actions)
          {
              SSAction action = kv.Value;
              if (action.destroy)
              {
                  waitingDelete.Add(action.GetInstanceID());
              }
              else if (action.enable)
              {
                  action.Update();
                  //action.FixedUpdate();
              }
          }
  
          foreach (int key in waitingDelete)
          {
              SSAction action = actions[key];
              actions.Remove(key);
              DestroyObject(action);
          }
          waitingDelete.Clear();
      }
  
      public void AddAction(GameObject gameObject, SSAction action, ISSActionCallback callback)
      {
          action.GameObject = gameObject;
          action.Transform = gameObject.transform;
          action.Callback = callback;
          waitingAdd.Add(action);
          action.Start();
      }
  
      public void ActionDone(SSAction source) { }
  }
  ```

- Emit 飞碟动作控制器

  ```
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using MyGame;
  
  public class Emit : SSAction
  {
      public FirstControl sceneControler = (FirstControl)Director.getInstance().sceneCtrl;
      public Vector3 target;     
      public float speed;     
      private float distanceToTarget;    
      float startX;
      float targetX;
      float targetY;
  
      public override void Start()
      {
          speed = sceneControler.user.round * 5;
          GameObject.GetComponent<DiskControl>().speed = speed;
          startX = 6 - Random.value * 12;
          if (Random.value > 0.5)
          {
              targetX = 36 - Random.value * 36;
          }
          else
          {
              targetX = -36 + Random.value * 36;
          }
          if (Random.value > 0.5)
          {
              targetY = 25 - Random.value * 25;
          }
          else
          {
              targetY = -25 + Random.value * 25;
          }
          //targetY = (Random.value * 25);
          this.Transform.position = new Vector3(startX, 0, 0);
          target = new Vector3(targetX, targetY, 30);
          //计算两者之间的距离  
          distanceToTarget = Vector3.Distance(this.Transform.position, target);
      }
      public static Emit GetSSAction()
      {
          Emit action = ScriptableObject.CreateInstance<Emit>();
          return action;
      }
      public override void Update()
      {
          Vector3 targetPos = target;
  
          //让始终它朝着目标  
          GameObject.transform.LookAt(targetPos);
  
          //计算弧线中的夹角  
          float angle = Mathf.Min(1, Vector3.Distance(GameObject.transform.position, targetPos) / distanceToTarget) * 45;
          GameObject.transform.rotation = GameObject.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
          float currentDist = Vector3.Distance(GameObject.transform.position, target);
          GameObject.transform.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
          if (this.Transform.position == target)
          {
              Debug.Log("Destroy");
              //DiskFactory.getInstance().freeDisk(gameobject);
              GameObject.SetActive(false);
              GameObject.transform.position = new Vector3(startX, 0, 0);
              sceneControler.factory.freeDisk(GameObject);
              this.destroy = true;
              this.Callback.ActionDone(this);
          }
      }
  }
  ```

  

- 添加天空盒





### 实验结果

![](1.PNG)



