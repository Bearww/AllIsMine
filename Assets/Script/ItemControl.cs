using UnityEngine;
using System.Collections;

public class ItemControl : MonoBehaviour {

    private GUIControl gui_control = null;

    private SceneControl scene_control = null;

    private BallControl ball_control = null;

    private PlayerControl player_control = null;

    private PlayerItemControl pitem_control = null;

    private BeginnerGuideControl guide_control = null;

    // 使用中的道具
    public ItemID[] using_item;

    // 物品欄位陣列
    public Item[] item_frame;

    public float HAND_EXPAND = 0.2f;

    public float BALL_HEAVIER = 1.0f;

    public float BALL_EXPAND = 0.2f;

    public float BALL_NARROW = 0.2f;

    public float BALL_FASTER = 2.0f;

    public int BALL_MORE = 4;

    private bool to_clear;

    void Awake()
    {
        // 在地圖中產生玩家選得球
        Debug.Log("產生球");
        //Debug.Log(this.pitem_control.texBall[this.pitem_control.select_ball]);
        this.pitem_control = GameObject.FindGameObjectWithTag("PlayerItemControl").GetComponent<PlayerItemControl>();
        Instantiate(this.pitem_control.texBall[this.pitem_control.select_ball], new Vector2(0, -2), Quaternion.identity);
    }

    void Start()
    {
        this.gui_control = GameObject.FindGameObjectWithTag("GUIControl").GetComponent<GUIControl>();

        this.scene_control = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<SceneControl>();

        this.ball_control = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControl>();

        this.player_control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

        this.guide_control = GameObject.FindGameObjectWithTag("BeginnerGuideControl").GetComponent<BeginnerGuideControl>();

        this.using_item = new ItemID[PlayerItemControl.MAX_CARRY_AMOUNT];

        // 讀取物品欄位的ID, SPRITE, 數量
        for (int i = 0; i < this.item_frame.Length; i++)
        {
            // 道具設定
            this.item_frame[i].player_control = this.player_control;
            this.item_frame[i].item_control = this;
            this.item_frame[i].item_id = this.pitem_control.carry_item[i];
            this.item_frame[i].item_amount = this.pitem_control.carry_item_amount[i];

            this.using_item[i] = ItemID.NONE;

            switch (this.item_frame[i].item_id)
            {
                case ItemID.NONE:
                    {
                        // 預設圖案
                    }
                    break;
                default:
                    {
                        this.item_frame[i].GetComponent<SpriteRenderer>().sprite = this.pitem_control.texItem[(int)this.item_frame[i].item_id];
                    }
                    break;
            }
        }

        this.to_clear = false;
    }

    void Update () {
        if(this.scene_control.isMiss())
        {
            if(this.to_clear)
            {
                // 清空使用中的道具
                for(int i = 0; i < this.using_item.Length; i++)
                {
                    // 清除道具效果
                    //Debug.Log("去除道具效果");
                    removeItemEffect(this.using_item[i]);
                    this.using_item[i] = ItemID.NONE;
                }
                this.to_clear = false;
            }
        }
    }

    public bool isUsing()
    {
        return (this.to_clear);
    }

    public bool isUsableItem(ItemID item_id)
    {
        for (int i = 0; i < this.using_item.Length; i++)
        {
            // 物品效果衝突檢查
            switch (this.using_item[i])
            {
                case ItemID.NONE:
                case ItemID.DEFAULT:        return true;
                case ItemID.BUBBLE:
                    switch (item_id)
                    {
                        case ItemID.BUBBLE: return false;
                    }
                    break;
                case ItemID.CAKE:
                    switch (item_id)
                    {
                        case ItemID.CAKE:   return false;
                    }
                    break;
                case ItemID.IRON:
                    switch (item_id)
                    {
                        case ItemID.IRON:   return false;
                    }
                    break;
                case ItemID.TORCH:
                    switch (item_id)
                    {
                        case ItemID.TORCH:
                        case ItemID.POT:    return false;
                    }
                    break;
                case ItemID.POT:
                    switch (item_id)
                    {
                        case ItemID.TORCH:  return false;
                        case ItemID.POT:    return false;
                    }
                    break;
                case ItemID.POSION:
                    switch (item_id)
                    {
                        case ItemID.POSION: return false;
                    }
                    break;
                case ItemID.COFFEE:
                    switch (item_id)
                    {
                        case ItemID.COFFEE: return false;
                    }
                    break;
                case ItemID.WATERING_CAN:
                    switch (item_id)
                    {
                        case ItemID.WATERING_CAN: return false;
                    }
                    break;
            }
        }
        // 已超過最大上限
        return false;
    }

    public void useItem(ItemID item_id)
    {
        // 記錄使用的道具
        for (int i = 0; i < this.using_item.Length; i++) {
            if (this.using_item[i] == ItemID.NONE)
            {
                if(this.guide_control.isUseItemGuide() && i == 0)
                {
                    this.guide_control.forceGoalAct();
                }

                Debug.Log("使用道具");
                this.using_item[i] = item_id;
                this.to_clear = true;
                addItemEffect(item_id);
                return;
            }
        }
    }

    void addItemEffect(ItemID id)
    {
        switch(id)
        {
            case ItemID.DEFAULT:
                break;
            case ItemID.BUBBLE:                 // 泡泡(球變多)
                this.ball_control.ball_amount += BALL_MORE;
                break;
            case ItemID.CAKE:                   // 蛋糕(手變長)
                this.player_control.GetComponentInChildren<Transform>().localScale += new Vector3(HAND_EXPAND, 0.0f);
                break;
            case ItemID.IRON:                   // 鐵(威力變強)
                this.ball_control.GetComponent<Rigidbody2D>().mass += BALL_HEAVIER;
                break;
            case ItemID.TORCH:                  // 火把(球變小)
                this.ball_control.transform.localScale -= new Vector3(BALL_NARROW, BALL_NARROW);
                break;
            case ItemID.POT:                    // 盆栽(球變大)
                this.ball_control.transform.localScale += new Vector3(BALL_EXPAND, BALL_EXPAND);
                break;
            case ItemID.POSION:                 // 藥水(速度變快)
                this.ball_control.ball_magnitude += BALL_FASTER;
                break;
            case ItemID.COFFEE:                 // 咖啡
                break;
            case ItemID.WATERING_CAN:           // 灑水壺
                break;
        }
    }

    void removeItemEffect(ItemID id)
    {
        switch (id)
        {
            case ItemID.DEFAULT:
                break;
            case ItemID.BUBBLE:                 // 泡泡(球變多)
                this.ball_control.ball_amount = 0;
                break;
            case ItemID.CAKE:                   // 蛋糕(手變長)
                this.player_control.GetComponentInChildren<Transform>().localScale -= new Vector3(HAND_EXPAND, 0.0f);
                break;
            case ItemID.IRON:                   // 鐵(威力變強)
                this.ball_control.GetComponent<Rigidbody2D>().mass -= BALL_HEAVIER;
                break;
            case ItemID.TORCH:                  // 火把(球變小)
                this.ball_control.transform.localScale += new Vector3(BALL_NARROW, BALL_NARROW);
                break;
            case ItemID.POT:                    // 盆栽(球變大)
                this.ball_control.transform.localScale -= new Vector3(BALL_EXPAND, BALL_EXPAND);
                break;
            case ItemID.POSION:                 // 藥水(速度變快)
                this.ball_control.ball_magnitude -= BALL_FASTER;
                break;
            case ItemID.COFFEE:                 // 咖啡
                break;
            case ItemID.WATERING_CAN:           // 灑水壺
                break;
        }
    }
}