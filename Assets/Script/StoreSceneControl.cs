using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StoreSceneControl : MonoBehaviour
{

    private PlayerItemControl pitem_control = null;

    public BeginnerGuideControl guide_control = null;

    private ItemID to_buy;

    private ScoreControl jewel;                         // 顯示分數
    private ScoreControl money;                         // 顯示金錢
    private ScoreControl paper_money;                   // 顯示鈔票

    public Vector3 paper_money_position;
    public Vector3 jewel_position;
    public Vector3 money_position;

    public float num_offset = 1.0f;

    public Sprite[] texNum;

    public Transform ask_frame;

    public static Vector3 ASK_DISPLAY = new Vector3();

    public static Vector3 ASK_HIDDEN = new Vector3(6.0f, 0.0f);

    // 進行狀態
    public enum STEP
    {

        NONE = -1,

        STORE = 0,              // 畫面顯示(等待輸入)
        ASK,                    // 詢問玩家
        BUY,                    // 購買道具
        WAIT_SE_END,            // 等待SE播放結束

        NUM,
    };

    private STEP step = STEP.NONE;
    private STEP next_step = STEP.NONE;
    private float step_timer = 0.0f;

    private SCENE to_scene;

    public AudioClip audio_clip;

    // -------------------------------------------------------------------------------- //

    private int[] price = {     0,      // Default
                              150,      // Bubble
                               80,      // Cake
                              120,      // Iron
                               60,      // Torch
                               60,      // Pot
                               90,      // Posion
                              100,      // Coffee
                              100,      // Watering Can

                              /* Element */
                              200,      // Water
                              150,      // Fire
                              150,      // Dirt
                              250       // Wood
                          };

    // -------------------------------------------------------------------------------- //

    void Start()
    {

        this.pitem_control = GameObject.FindGameObjectWithTag("PlayerItemControl").GetComponent<PlayerItemControl> ();

        this.guide_control = GameObject.FindGameObjectWithTag("BeginnerGuideControl").GetComponent<BeginnerGuideControl>();

        //

        // 紙鈔數量初始化
        this.paper_money = gameObject.AddComponent<ScoreControl>();
        this.paper_money.texture = this.texNum;
        this.paper_money.digitOffset = this.num_offset;
        this.paper_money.digitNum = 3;
        this.paper_money.drawZero = false;
        this.paper_money.isDecreasing = true;
        this.paper_money.setNumForce(this.pitem_control.player_paper_money);

        // 寶石數量初始化        
        this.jewel = gameObject.AddComponent<ScoreControl>();
        this.jewel.texture = this.texNum;
        this.jewel.digitOffset = this.num_offset;
        this.jewel.digitNum = 3;
        this.jewel.drawZero = false;
        this.jewel.isDecreasing = true;
        this.jewel.setNumForce(this.pitem_control.player_jewel);

        // 金錢數量出始化
        this.money = gameObject.AddComponent<ScoreControl>();
        this.money.texture = this.texNum;
        this.money.digitOffset = this.num_offset;
        this.money.digitNum = 4;
        this.money.drawZero = false;
        this.money.isDecreasing = true;
        this.money.setNumForce(this.pitem_control.player_money);

        //

        this.next_step = STEP.STORE;
        this.to_scene = SCENE.NONE;

        if (this.guide_control.isElementGuide())
        {
            this.guide_control.forceDisplay();
        }

        if(this.guide_control.isLotteryGuide())
        {
            this.guide_control.is_display = true;
        }

        if(this.guide_control.isBuyGuide())
        {
            this.guide_control.displayWait();
        }

        this.GetComponent<AudioSource>().clip = this.audio_clip;

    }

    void Update()
    {
        this.step_timer += Time.deltaTime;

        // 更新數字資訊
        this.paper_money.position = this.paper_money_position;
        this.paper_money.digitOffset = this.num_offset;
        this.paper_money.setNum(this.pitem_control.player_paper_money);

        this.jewel.position = this.jewel_position;
        this.jewel.digitOffset = this.num_offset;
        this.jewel.setNum(this.pitem_control.player_jewel);

        this.money.position = this.money_position;
        this.money.digitOffset = this.num_offset;
        this.money.setNum(this.pitem_control.player_money);

        // 轉移至下一個狀態
        switch (this.step)
        {

            case STEP.STORE:
                {
                    // 當滑鼠左鍵放開
                    //
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, 10.0f);

                        foreach (RaycastHit2D hit in hits)
                        {
                            Debug.Log(hit.transform.gameObject.tag);
                            switch (hit.transform.gameObject.tag)
                            {
                                case "BackToTitle":
                                {
                                    this.to_scene = SCENE.TITLE;
                                }
                                break;

                                case "Toy":
                                {
                                    this.to_scene = SCENE.TOY;
                                }
                                break;

                                case "Cook":
                                {
                                    this.to_scene = SCENE.COOK;
                                }
                                break;

                                case "Element":
                                {
                                    int id = int.Parse(hit.transform.name);

                                    switch(id)
                                    {
                                        case 0: this.to_buy = ItemID.FIRE; break;
                                        case 1: this.to_buy = ItemID.WATER; break;
                                        case 2: this.to_buy = ItemID.WOOD; break;
                                        case 3: this.to_buy = ItemID.DIRT; break;
                                    }
                                    
                                    if(!this.guide_control.isElementGuide())
                                    {
                                        this.to_buy = ItemID.NONE;
                                    }

                                    if(this.to_buy != ItemID.NONE)
                                    {
                                        this.next_step = STEP.ASK;
                                    }
                                }
                                break;

                                case "ItemControl":
                                {
                                    int id = int.Parse(hit.transform.name);

                                    this.to_buy = (ItemID)id;

                                    if(!this.guide_control.isBuyGuide())
                                    {
                                        this.to_buy = ItemID.NONE;
                                    }

                                    if (this.to_buy != ItemID.NONE)
                                    {
                                        this.next_step = STEP.ASK;
                                    }
                                }
                                break;
                            }

                            if(this.guide_control.isElementGuide())
                            {
                                if(this.to_scene != SCENE.COOK)
                                {
                                    this.to_scene = SCENE.NONE;
                                }
                            }

                            if(this.guide_control.isLotteryGuide())
                            {
                                if (this.to_scene != SCENE.TOY)
                                {
                                    this.to_scene = SCENE.NONE;
                                }
                            }

                            if (this.to_scene != SCENE.NONE)
                            {
                                this.next_step = STEP.WAIT_SE_END;
                            }
                        }
                    }
                }
                break;

            case STEP.ASK:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10.0f);

                        if (hit)
                        {
                            // Debug.Log(hit.transform.name);
                            if (hit.transform.CompareTag("Frame"))
                            {
                                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                if (pos.x < -1.0f)
                                {
                                    this.next_step = STEP.BUY;
                                }
                                else
                                {
                                    this.next_step = STEP.STORE;
                                }
                            }
                        }
                    }
                }
                break;

            case STEP.BUY:
                {
                    // 點擊繼續
                    if(this.guide_control.isBuyGuide())
                    {
                        this.guide_control.forceGoalAct();
                    }

                    this.next_step = STEP.STORE;
                }
                break;

            case STEP.WAIT_SE_END:
                {
                    // SE完成時，讀取遊戲場景

                    bool to_finish = true;

                    do
                    {

                        if (!this.GetComponent<AudioSource>().isPlaying)
                        {

                            break;
                        }

                        if (this.GetComponent<AudioSource>().time >= this.GetComponent<AudioSource>().clip.length)
                        {

                            break;
                        }

                        to_finish = false;

                    } while (false);

                    if (to_finish)
                    {

                        switch (this.to_scene)
                        {
                            case SCENE.TITLE:
                                SceneManager.LoadScene("TitleScene");
                                break;
                            case SCENE.TOY:
                                if(this.guide_control.isLotteryGuide())
                                {
                                    this.guide_control.goalAct();
                                }
                                SceneManager.LoadScene("ToyScene");
                                break;
                            case SCENE.COOK:
                                if (this.guide_control.isElementGuide())
                                {
                                    this.guide_control.forceGoalAct();
                                }
                                SceneManager.LoadScene("CookScene");
                                break;
                            default:
                                // 當場景沒有設定時
                                Debug.Log("Need add scene");
                                break;
                        }
                    }
                }
                break;
        }

        // 下一狀態初始化

        if (this.next_step != STEP.NONE)
        {

            switch (this.next_step)
            {
                case STEP.STORE:
                    {
                        this.to_buy = ItemID.NONE;
                        this.ask_frame.position = ASK_HIDDEN;
                    }
                    break;
                
                case STEP.ASK:
                    {
                        // 顯示詢問畫面
                        this.ask_frame.position = ASK_DISPLAY;
                    }
                    break;

                case STEP.BUY:
                    {
                        bool is_enough = false;

                        // 購買元素
                        if (isElement(this.to_buy))
                        {
                            if (this.pitem_control.player_money >= this.price[(int)this.to_buy])
                            {
                                this.pitem_control.player_money -= this.price[(int)this.to_buy];
                                is_enough = true;
                            }
                        }
                        // 購買道具
                        else
                        {
                            if (this.pitem_control.player_paper_money >= this.price[(int)this.to_buy])
                            {
                                this.pitem_control.player_paper_money -= this.price[(int)this.to_buy];
                                is_enough = true;
                            }
                        }

                        if (is_enough)
                        {
                            // 加至玩家物品
                            for (int i = 0; i < this.pitem_control.player_item.Length; i++)
                            {
                                if (this.pitem_control.player_item[i] == this.to_buy)
                                {
                                    this.pitem_control.player_item_amount[i]++;
                                    break;
                                }
                            }
                        }

                        this.to_buy = ItemID.NONE;
                    }
                    break;

                case STEP.WAIT_SE_END:
                    {
                        // 開始播放SE
                        this.GetComponent<AudioSource>().Play();
                    }
                    break;
            }

            this.step = this.next_step;
            this.next_step = STEP.NONE;

            this.step_timer = 0.0f;
        }
        /*
		// 各個狀態的處理

		switch(this.step) {

			case STEP.TITLE:
			{
            }
			break;
		}
        */
    }

    private bool isElement(ItemID item)
    {
        return (item >= ItemID.WATER && item <= ItemID.WOOD);
    }
}