using UnityEngine;
using System.Collections;

public class BeginnerGuideControl : MonoBehaviour {

    public bool is_beginner_guide;

    public bool is_fail;
    public bool is_display;
    public bool is_goal_act;

    public enum STEP
    {
        NONE = -1,

        GUIDE = 0,          // 玩家指引
        ACTION,             // 玩家操作
        GOAL,               // 達成目標

        WAIT,

        NUM,
    };

    public STEP step = STEP.NONE;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;

    // ---------------------------------------------------------------- //

    public enum GUIDE
    {
        NONE = -1,

        MAIN = 0,           // 遊戲主畫面，選關卡
        ITEM,               // 選擇道具
        ITEM_OK,            // 點擊道具畫面OK
        BALL,               // 選擇球
        START,              // 開始遊戲說明
        USE_ITEM,           // 使用道具
        DRAG_BALL,          // 拉動球
        VICTORY,            // 遊戲勝利
        GO_SHOP,            // 進入商店
        BUY_ELEMENT,        // 買元素
        COOK,               // 合成道具
        LOTTERY,            // 抽球
        BUY_ITEM,           // 購買道具
        DONE,               // 完成新手教學

        NUM,
    };

    public GUIDE guide = GUIDE.NONE;
    public Transform[] guide_sprite;

    public static Vector2 GUIDE_DISPLAY = new Vector2();
    public static Vector2 GUIDE_HIDDEN = new Vector2(-6, 0);

    // ---------------------------------------------------------------- //

    void Start() {
        this.guide = GUIDE.MAIN;

        DontDestroyOnLoad(this);
    }

    void Update() {
        this.step_timer += Time.deltaTime;

        // 轉移至下一個狀態
        switch (this.step)
        {
            case STEP.GUIDE:
            {
                if(this.guide_sprite[(int)this.guide].GetComponent<Collider2D> ())
                {
                    //Debug.Log("Guide");
                    // 判斷是否點擊目標區域內
                    if(Input.GetMouseButtonDown(0))
                    {
                        //Debug.Log("Guide");
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10.0f, LayerMask.NameToLayer("BeginnerGuide"));

                        //Debug.Log(hit.transform);                    
                        if (hit)
                        {
                            this.next_step = STEP.ACTION;
                        }
                    }
                }
                else
                {
                    //Debug.Log("Click Guide");
                    if(Input.GetMouseButtonDown(0))
                    {
                        if(this.guide == GUIDE.DONE)
                        {
                            // 離開新手引導
                            this.is_display = false;
                            this.is_beginner_guide = false;
                            this.step = STEP.NONE;
                            this.step_timer = 0.0f;
                        }
                        else
                        {
                            this.next_step = STEP.ACTION;
                            this.is_display = false;
                        }
                    }
                }
            }
            break;

            case STEP.ACTION:
            {
                do
                {
                    // 操作失敗，重新嘗試
                    if(this.is_fail)
                    {
                        this.next_step = STEP.GUIDE;
                        break;
                    }
                    if(this.is_goal_act)
                    {
                        Debug.Log("GOAL ACT!");
                        this.next_step = STEP.GOAL;
                        break;
                    }
                } while(false);
            }
            break;

            case STEP.GOAL:
            {
                // 下次引導開始
                this.next_step = STEP.GUIDE;
            }
            break;

            case STEP.WAIT:
            {
                if(this.step_timer > 0.5f)
                {
                    forceDisplay();
                    this.next_step = STEP.GUIDE;
                }
            }
            break;
        }

        // 下一狀態初始化

        if (this.next_step != STEP.NONE)
        {

            switch (this.next_step)
            {
                case STEP.GUIDE:
                {
                    Debug.Log(this.guide);
                    this.is_fail = false;
                    this.is_goal_act = false;
                    //this.is_display = false;
                    //this.guide_sprite[(int)this.guide].localPosition = GUIDE_HIDDEN;
                }
                break;

                case STEP.ACTION:
                {
                    //this.guide_sprite[(int)this.guide].localPosition = GUIDE_HIDEN;
                }
                break;

                case STEP.GOAL:
                {
                    //Debug.Log("Goal");
                    //this.guide_sprite[(int)this.guide].localPosition = GUIDE_HIDDEN;
                    this.is_display = false;
                    this.guide++;
                }
                break;
            }
            this.step = this.next_step;
            this.next_step = STEP.NONE;

            this.step_timer = 0.0f;
        }

        /*
        // 各個狀態的處理

        switch (this.step)
        {
        }
        */
    }

    void OnGUI()
    {
        if (is_display)
        {
            this.guide_sprite[(int)this.guide].localPosition = GUIDE_DISPLAY;
        }
        else
        {
            this.guide_sprite[(int)this.guide].localPosition = GUIDE_HIDDEN;
        }
    }

    public void startGuide()
    {
        this.next_step = STEP.GUIDE;
    }

    public void displayWait()
    {
        this.is_display = false;
        this.next_step = STEP.WAIT;
    }

    public void forceDisplay()
    {
        this.is_display = true;
        this.guide_sprite[(int)this.guide].localPosition = GUIDE_DISPLAY;
    }

    public void forceHidden()
    {
        this.is_display = false;
        this.guide_sprite[(int)this.guide].localPosition = GUIDE_HIDDEN;
    }

    public void forceGoalAct()
    {
        this.is_display = false;
        this.guide_sprite[(int)this.guide].localPosition = GUIDE_HIDDEN;
        this.guide++;
        this.next_step = STEP.GUIDE;
        this.is_display = true;
        this.guide_sprite[(int)this.guide].localPosition = GUIDE_DISPLAY;
    }

    public void goalAct()
    {
        this.is_display = false;
        this.is_goal_act = true;
    }

    public void goalFail()
    {
        this.is_fail = true;
    }

    public bool isAction()
    {
        return (this.step == STEP.ACTION);
    }

    public bool isMainGuide()
    {
        return (this.guide == GUIDE.MAIN);
    }

    public bool isItemGuide()
    {
        return (this.is_beginner_guide && this.guide == GUIDE.ITEM);
    }

    public bool isItemOkGuide()
    {
        return (this.is_beginner_guide && this.guide == GUIDE.ITEM_OK);
    }

    public bool isBallGuide()
    {
        return (this.is_beginner_guide && this.guide == GUIDE.BALL);
    }

    public bool isStartGuide()
    {
        return (this.is_beginner_guide && this.guide == GUIDE.START);
    }

    public bool isUseItemGuide()
    {
        return (this.is_beginner_guide && this.guide == GUIDE.USE_ITEM);
    }

    public bool isDragGuide()
    {
        return (this.is_beginner_guide && this.guide == GUIDE.DRAG_BALL);
    }

    public bool isVictoryGuide()
    {
        return (this.is_beginner_guide && this.guide == GUIDE.VICTORY);
    }

    public bool isShopGuide()
    {
        return (this.guide == GUIDE.GO_SHOP);
    }

    public bool isElementGuide()
    {
        return (this.guide == GUIDE.BUY_ELEMENT);
    }

    public bool isCookGuide()
    {
        return (this.guide == GUIDE.COOK);
    }

    public bool isLotteryGuide()
    {
        return (this.guide == GUIDE.LOTTERY);
    }

    public bool isBuyGuide()
    {
        return (this.guide == GUIDE.BUY_ITEM);
    }
}
