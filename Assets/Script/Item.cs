using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

    public PlayerControl player_control = null;

    public ItemControl item_control = null;

    // 物品編號
    public ItemID item_id = ItemID.NONE;

    // 物品數量
    public int item_amount = 0;

    //

    public static int MAX_ITEM_AMOUNT = (int)ItemID.WATERING_CAN;

    //

    // 當滑鼠按下
	void OnMouseDown()
    {
        if(this.player_control.isCarry())
        {
            if(this.item_amount == 0)
            {
                // 顯示遊戲中商店
                Debug.Log("剩餘不足");
                return;
            }

            if (this.item_control.isUsableItem(this.item_id))
            {
                // 是否使用GUI

                // 是，使用道具
                //Debug.Log("Use item");
                this.item_amount--;
                this.item_control.useItem(this.item_id);
                this.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            }
        }
    }
}

public enum ItemID
{
    NONE = -1,

    DEFAULT = 0,

    // 道具
    BUBBLE,                 // 泡泡(球變多)   
    CAKE,                   // 蛋糕(手變長)
    IRON,                   // 鐵(威力變強)
    TORCH,                  // 火把(球變小)
    POT,                    // 盆栽(球變大)
    POSION,                 // 藥水(速度變快)
    COFFEE,                 // 咖啡
    WATERING_CAN,           // 灑水壺

    // 元素
    WATER,
    FIRE,
    DIRT,
    WOOD,

    NUM,
};
