using UnityEngine;
using System.Collections;

public class PlayerItemControl : MonoBehaviour {


    // 玩家金錢
    public int player_money;

    // 玩家鈔票
    public int player_paper_money;

    // 玩家寶石
    public int player_jewel;

    // 玩家擁有的道具
	public ItemID[] player_item;

    // 玩家擁有道具的數量
    public int[] player_item_amount;

    // 最多攜帶物品數量
    public static int MAX_CARRY_AMOUNT = 6;

    // 最多攜帶單一物品數量
    public static int MAX_ITEM_AMOUNT = 1;

    // 最大背包空間
    public static int MAX_INVENTORY = 12;

    // 玩家攜帶的道具
    public ItemID[] carry_item;

    // 玩家攜帶的道具數量
    public int[] carry_item_amount;

    // 玩家擁有的球
    public bool[] player_ball;

    // 玩家選擇的球
    public int select_ball;

    // 物品圖片
    public Sprite[] texItem;

    // 球的Prefab
    public Transform[] texBall;

    // 球的圖片
    public Transform[] ballSpirte;

    void Start () {
        DontDestroyOnLoad(this);

        //this.player_item = new ItemID[MAX_INVENTORY];
        //this.player_item_amount = new int[MAX_INVENTORY];

        //this.carry_item = new ItemID[MAX_CARRY_AMOUNT];
        //this.carry_item_amount = new int[MAX_CARRY_AMOUNT];
        this.player_ball = new bool[this.ballSpirte.Length];

        this.player_ball[0] = true;
        for (int i = 1; i < this.player_ball.Length; i++)
            this.player_ball[i] = true;
	}
}
