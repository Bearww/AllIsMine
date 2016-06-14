using UnityEngine;
using System.Collections;

public class ChooseSceneControl : MonoBehaviour {

    public PlayerItemControl pitem_control = null;

    public TitleSceneControl scene_control = null;

    public BeginnerGuideControl guide_control = null;

    //

    public ItemID[] choose_item;                // 玩家選擇的道具

    public ItemID[] inventory_item;             // 玩家擁有的道具

    public int[] item_amount;                   // 道具的數量

    public Transform item_scene;

    public Transform ball_scene;

    public SpriteRenderer[] choose_frame;       // 道具欄位

    public SpriteRenderer[] inventory_frame;    // 背包欄位

    public int ball_no;

    public Transform[] ball_frame;              // 球的欄位

    public Transform ball_main_frame;           // 中心球欄位

    // ---------------------------------------------------------------- //

    public static Color ITEM_SELECTABLE = new Color(1.0f, 1.0f, 1.0f);

    public static Color ITEM_UNSELETABLE = new Color(0.3f, 0.3f, 0.3f);

    public static Vector3 ITEM_SCENE_DISPLAY = new Vector2();

    public static Vector3 ITEM_SCENE_HIDDEN = new Vector2(6.0f, 0.0f);

    public static Vector3 BALL_SCENE_DISPLAY = new Vector2(0.0f, 0.04f);

    public static Vector3 BALL_SCENE_HIDDEN = new Vector2(0.0f, -11.004f);

    public static Color BALL_HIDDEN = new Color(0.2f, 0.2f, 0.2f);

    public static float BALL_OFFSET = 2.0f;

    public static float DRAG_SPEED_DELTA = 80.0f;

    public static float DRAG_MAX_SPEED = 100.0f;

    public static float DRAG_MAX_DEVIATION = 0.5f;

    public static float DRAG_RANGE_LEFT;

    public static float DRAG_RANGE_RIGHT;

    // ---------------------------------------------------------------- //

    private Vector3 pre_pos;

    // ---------------------------------------------------------------- //

    public enum STEP
    {

        NONE = -1,

        START = 0,              // 開始
        CHOOSE_ITEM,            // 選道具
        CHOOSE_BALL,            // 選球
        SWAP,                   // 道具位置交換
        DRAG,                   // 拉動畫面
        BACK,                   // 將超出的球倒回
        LOCK,                   // 將畫面鎖在球上
        WAIT_MOVE,              // 等球到指定位置

        NUM,
    };

    public STEP step;
    public STEP next_step = STEP.NONE;

    //public float step_timer_prev;
    public float step_timer;

    // ---------------------------------------------------------------- //

    public void create()
    {
        this.pitem_control = GameObject.FindGameObjectWithTag("PlayerItemControl").GetComponent<PlayerItemControl>();

        this.scene_control = GameObject.FindGameObjectWithTag("TitleSceneControl").GetComponent<TitleSceneControl>();

        this.guide_control = GameObject.FindGameObjectWithTag("BeginnerGuideControl").GetComponent<BeginnerGuideControl>();

        //

        this.choose_item = new ItemID[PlayerItemControl.MAX_CARRY_AMOUNT];
        this.inventory_item = new ItemID[PlayerItemControl.MAX_INVENTORY];
        this.item_amount = new int[PlayerItemControl.MAX_INVENTORY];

        // 讀取玩家物品
        //Debug.Log(this.pitem_control);
        for(int i = 0, inv = 0; i < this.pitem_control.player_item.Length; i++)
        {
            if (inv >= this.inventory_item.Length)
            {
                Debug.LogWarning("背包容量不夠");
                break;
            }

            // 只放入可使用的道具
            if (!isElement(this.pitem_control.player_item[i]))
            {
                //Debug.Log(this.pitem_control.player_item[i]);
                this.inventory_item[inv] = this.pitem_control.player_item[i];
                this.item_amount[inv] = this.pitem_control.player_item_amount[i];
                inv++;
            }
        }

        this.ball_no = this.pitem_control.select_ball;
        this.ball_frame = new Transform[this.pitem_control.ballSpirte.Length];

        float offset = 0.0f;
        for(int i = 0; i < this.pitem_control.ballSpirte.Length; i++)
        {
            // 產生球
            this.ball_frame[i] = Instantiate(this.pitem_control.ballSpirte[i]);
            this.ball_frame[i].GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            this.ball_frame[i].GetComponent<SpriteRenderer>().sortingOrder = 50;
            //this.ball_frame[i].GetComponent<BallControl>().enabled = false;
            this.ball_frame[i].parent = this.ball_main_frame;
            this.ball_frame[i].localPosition = new Vector2(offset, 0.0f);
            this.ball_frame[i].localScale = 
                new Vector3(this.pitem_control.ballSpirte[i].localScale.x, this.pitem_control.ballSpirte[i].localScale.y, 1.0f);

            // 尚未獲得球
            if(!this.pitem_control.player_ball[i])
            {
                this.ball_frame[i].GetComponent<SpriteRenderer> ().color = BALL_HIDDEN;
            }

            offset += BALL_OFFSET;
        }

        DRAG_RANGE_RIGHT = 0.0f + DRAG_MAX_DEVIATION;
        DRAG_RANGE_LEFT = -offset + BALL_OFFSET - DRAG_MAX_DEVIATION;
    }

    public void start()
    {
        this.scene_control.is_disp_choose = true;

        this.step = STEP.NONE;
        this.next_step = STEP.START;
    }

    public void execute()
    {
        this.step_timer += Time.deltaTime;

        // 轉移至下一個狀態
        switch (this.step)
        {
            case STEP.START:
            {
                this.next_step = STEP.CHOOSE_ITEM;
            }
            break;
            case STEP.CHOOSE_ITEM:
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("[Choose item]ButtonDown");
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10.0f, LayerMask.NameToLayer("BeginnerGuide"));
                    
                    //Debug.Log(hit.transform);
                    if (hit)
                    {
                        do {
                            //Debug.Log(hit.transform);
                            //Debug.Log(hit.transform.tag);
                            if(hit.transform.CompareTag("ChooseItemScene"))
                            {
                                Debug.Log(hit.transform);
                                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                
                                if(pos.x > 0) {
                                    // 完成引導選道具
                                    if(this.guide_control.isItemOkGuide())
                                    {
                                        Debug.Log("Force display");
                                        this.guide_control.forceHidden();
                                        this.guide_control.forceGoalAct();
                                        //this.guide_control.displayWait();
                                    }
                                    this.next_step = STEP.CHOOSE_BALL;
                                    this.scene_control.waitSE();
                                }
                                else
                                {
                                    if(!this.guide_control.is_beginner_guide)
                                    {
                                        // 回到主畫面
                                        this.scene_control.backToTitle();
                                    }
                                }
                                break;
                            }

                            if(hit.transform.CompareTag("InventoryItem"))
                            {
                                do {
                                    // 取得物件名稱
                                    int index = int.Parse(hit.transform.name);
                                    //Debug.Log(index);
                                
                                    // 索引錯誤
                                    if(index < 0 || index >= this.inventory_item.Length)
                                        break;
                                
                                    // 數量不夠
                                    if(this.item_amount[index] <= 0)
                                        break;

                                    for (int i = 0; i < this.choose_item.Length; i++)
                                    {
                                        // 完成指引選道具
                                        if (this.guide_control.isItemGuide())
                                        {
                                            if(i == 0 && index == 0)
                                            {
                                                this.guide_control.forceGoalAct();
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        if(this.choose_item[i] == ItemID.DEFAULT)
                                        {
                                            Debug.Log(this.inventory_item[index]);

                                            this.choose_item[i] = this.inventory_item[index];
                                            this.item_amount[index]--;
                                            break;
                                        }
                                    }
                                } while(false);

                                this.next_step = STEP.SWAP;
                                break;
                            }

                            if (hit.transform.CompareTag("ChooseItem"))
                            {
                                // 取得物件名稱
                                int index = int.Parse(hit.transform.name);
                                Debug.Log(index);
                                
                                // 索引錯誤
                                if(index < 0 || index >= this.choose_item.Length)
                                    break;

                                if(this.guide_control.is_beginner_guide)
                                    break;
                                
                                for(int i = 0; i < this.inventory_item.Length; i++)
                                {
                                    if(this.inventory_item[i] == this.choose_item[index])
                                    {
                                        Debug.Log(this.choose_item[index]);
                                        this.choose_item[index] = ItemID.DEFAULT;
                                        this.item_amount[i]++;
                                        break;
                                    }
                                }

                                this.next_step = STEP.SWAP;
                                break;
                            }

                        } while(false);
                    }
                }
            }
            break;
            case STEP.CHOOSE_BALL:
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // 開始拖動
                    this.next_step = STEP.DRAG;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10.0f);

                    if (hit)
                    {
                        do {
                            //Debug.Log(hit.transform.tag);
                            if(hit.transform.CompareTag("ChooseBallScene"))
                            {
                                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                if(pos.x < 0)
                                {
                                    if(!this.guide_control.is_beginner_guide)
                                    {
                                        // 回到選擇道具畫面
                                        this.next_step = STEP.CHOOSE_ITEM;
                                        this.scene_control.waitSE();
                                    }
                                }
                                else
                                {
                                    // 選取得球已解鎖
                                    if(this.pitem_control.player_ball[this.ball_no])
                                    {
                                        if(this.guide_control.isBallGuide())
                                        {
                                            this.guide_control.goalAct();
                                        }

                                        // 讀取道具
                                        loadPlayerInfo();
                                        // 進入遊戲
                                        this.scene_control.is_choosed = true;
                                        this.scene_control.waitSE();
                                    }
                                }
                                break;
                            }

                        } while(false);
                    }
                }
            }
            break;
            case STEP.SWAP:
            {
                this.next_step = STEP.CHOOSE_ITEM;
            }
            break;
            case STEP.DRAG:
            {
                do {
                    // 球超出範圍
                    if(this.ball_main_frame.position.x > DRAG_RANGE_RIGHT)
                    {
                        this.next_step = STEP.BACK;
                    }
                    if(this.ball_main_frame.position.x < DRAG_RANGE_LEFT)
                    {
                        this.next_step = STEP.BACK;
                    }

                    if(Input.GetMouseButtonUp(0))
                    {
                        this.next_step = STEP.LOCK;
                        break;
                    }
                } while(false);
            }
            break;

            case STEP.BACK:
            {
                if(this.ball_main_frame.localPosition.x < DRAG_RANGE_RIGHT && this.ball_main_frame.localPosition.x > DRAG_RANGE_LEFT)
                {
                    this.next_step = STEP.LOCK;
                }
            }
            break;

            case STEP.LOCK:
            {
                float loc = -this.ball_main_frame.localPosition.x / BALL_OFFSET;
                int index = -1;
                
                // 取得移動方向同向最近的球
                //Debug.Log(this.ball_main_frame.GetComponent<Rigidbody2D>().velocity.x);
                if(this.ball_main_frame.GetComponent<Rigidbody2D>().velocity.x > 0)
                {
                    index = Mathf.FloorToInt(loc);
                }
                else if(this.ball_main_frame.GetComponent<Rigidbody2D>().velocity.x < 0)
                {
                    index = Mathf.CeilToInt(loc);
                }
                else
                {
                    index = Mathf.RoundToInt(loc);
                }

                //Debug.Log(index);

                if(index < 0)
                    index = 0;
                if(index >= this.ball_frame.Length)
                    index = this.ball_frame.Length - 1;

                this.ball_no = index;
                
                Debug.Log(this.ball_no);

                Vector3 current = this.ball_main_frame.localPosition;
                Vector3 target = new Vector2(-index * BALL_OFFSET, 0);
                forceMove(target - current);

                this.next_step = STEP.WAIT_MOVE;
            }
            break;
            case STEP.WAIT_MOVE:
            {
                if(this.ball_main_frame.localPosition.x + this.ball_no * BALL_OFFSET < DRAG_MAX_DEVIATION)
                {
                    this.next_step = STEP.CHOOSE_BALL;
                }
            }
            break;
        }

        // 下一狀態初始化
        if (this.next_step != STEP.NONE)
        {
            switch(this.next_step)
            {
                case STEP.START:
                    // 指引選取道具
                    if (this.guide_control.isItemGuide())
                    {
                        //Debug.Log("Choose Item Guide");
                        this.guide_control.goalAct();
                        this.guide_control.forceDisplay();
                    }
                    break;
                case STEP.CHOOSE_ITEM:
                    this.item_scene.position = ITEM_SCENE_DISPLAY;
                    this.ball_scene.position = BALL_SCENE_HIDDEN;
                break;
                case STEP.CHOOSE_BALL:
                    this.item_scene.position = ITEM_SCENE_HIDDEN;
                    this.ball_scene.position = BALL_SCENE_DISPLAY;
                    this.ball_main_frame.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    this.ball_main_frame.position = new Vector2(-this.ball_no * BALL_OFFSET, 0);
                break;
                case STEP.DRAG:
                    this.pre_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                break;
                case STEP.BACK:
                    this.ball_main_frame.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    break;
            }

            this.step = this.next_step;
            this.next_step = STEP.NONE;

            this.step_timer = 0.0f;
        }

        // 各個狀態的處理
        switch (this.step)
        {
            case STEP.CHOOSE_ITEM:
            {
                for(int i = 0; i < PlayerItemControl.MAX_CARRY_AMOUNT; i++)
                {
                    if(this.choose_item[i] != ItemID.NONE)
                    {
                        this.choose_frame[i].sprite = this.pitem_control.texItem[(int)this.choose_item[i]];
                    }
                }

                for(int i = 0; i < PlayerItemControl.MAX_INVENTORY; i++)
                {
                    if(i >= this.inventory_frame.Length)
                        break;

                    this.inventory_frame[i].sprite = this.pitem_control.texItem[(int)this.inventory_item[i]];
                    
                    // 物品數量確認
                    if(this.item_amount[i] <= 0)
                    {
                        this.inventory_frame[i].color = ITEM_UNSELETABLE;
                    }
                    else
                    {
                        this.inventory_frame[i].color = ITEM_SELECTABLE;
                    }
                }
            }
            break;

            case STEP.DRAG:
            {
                Vector3 cur_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // 朝滑鼠方向移動
                forceMove(cur_pos - this.pre_pos);
                pre_pos = cur_pos;
            }
            break;

            case STEP.BACK:
            {
                Vector3 cur_pos = this.ball_main_frame.localPosition;
                Vector3 target_pos = new Vector3();
                
                // 超出右邊界
                if(this.ball_main_frame.position.x > DRAG_RANGE_RIGHT)
                {
                    target_pos = new Vector2();
                }
                // 超出左邊界
                if(this.ball_main_frame.position.x < DRAG_RANGE_LEFT)
                {
                    target_pos = new Vector2(-this.ball_frame.Length * BALL_OFFSET + BALL_OFFSET, 0);
                }

                // 朝邊界方向移動
                forceMove(target_pos - cur_pos);
            }
            break;
        }
    }

    bool isElement(ItemID item)
    {
        return (item >= ItemID.WATER && item <= ItemID.WOOD);
    }

    void forceMove(Vector3 dir)
    {
        dir.y = dir.z = 0;

        if (this.ball_main_frame.GetComponent<Rigidbody2D>().velocity.x * dir.x < 0.0f)
        {
            this.ball_main_frame.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        float dist = dir.magnitude;
        float v = dist / Time.deltaTime;

        this.ball_main_frame.GetComponent<Rigidbody2D>().AddForce(dir.normalized * v * Time.deltaTime * DRAG_SPEED_DELTA);
    }

    void loadPlayerInfo()
    {
        // 讀取道具
        for(int i = 0, idx = 0; i < this.choose_item.Length; i++)
        {
            if (idx >= this.pitem_control.carry_item.Length)
                break;

            if(this.choose_item[i] > ItemID.DEFAULT)
            {
                // 尋找玩家背包中的物品
                for (int item = 0; item < this.pitem_control.player_item.Length; item++)
                {
                    if(this.choose_item[i] == this.pitem_control.player_item[item])
                    {
                        // 確認數量足夠
                        if(this.pitem_control.player_item_amount[item] > 0)
                        {
                            Debug.Log(this.choose_item[i]);
                            this.pitem_control.carry_item[idx] = this.choose_item[i];
                            this.pitem_control.carry_item_amount[idx] = 1;
                            this.pitem_control.player_item_amount[item]--;
                            idx++;
                        }
                    }
                }
            }
        }

        // 讀取玩家選擇的球
        this.pitem_control.select_ball = this.ball_no;
        Debug.Log(this.pitem_control.select_ball);
    }

    public void hidden()
    {
        this.item_scene.position = ITEM_SCENE_HIDDEN;
        this.ball_scene.position = BALL_SCENE_HIDDEN;
    }
}
