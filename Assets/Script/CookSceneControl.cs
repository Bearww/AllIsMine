using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;

public class CookSceneControl : MonoBehaviour
{

    public GameControl game_control = null;

    public PlayerItemControl pitem_control = null;

    public BeginnerGuideControl guide_control = null;

    //

    public Transform[] frame = null;

    public Transform cooked_frame = null;

    public Text[] txt_amount = null;

    private ItemID cooked_item;

    private ItemID[] player_item;

    private int[] item_amount;

    private ItemID[] choose_item;

    private int choose_amount;

    private SCENE to_scene = SCENE.NONE;

    public static int ITEM_AMOUNT = 6;

    public static int ELEMENT_AMOUNT = 4;

    public static int MAX_COOK_NUM = 3;

    // 進行狀態
    public enum STEP
    {

        NONE = -1,

        CHOOSE = 0,             // 選擇合成的元素
        WAIT_COOK_END,
        COOK_END,
        WAIT_SE_END,            // 等待SE結束

        NUM,
    };

    private STEP step = STEP.NONE;
    private STEP next_step = STEP.NONE;
    private float step_timer = 0.0f;

    public AudioClip audio_clip;

    public Transform[] anims;

    public float[] anim_times;

    public static Vector3 ANIM_DISPLAY = new Vector3(0, 0);

    public static Vector3 ANIM_HIDDEN = new Vector3(-12, 0);

    // -------------------------------------------------------------------------------- //

    private ItemID[,] cookTable = { { ItemID.BUBBLE,    ItemID.DEFAULT,     ItemID.WATER,   ItemID.DIRT },
                                    { ItemID.POT,       ItemID.DEFAULT,     ItemID.WATER,   ItemID.WOOD },
                                    { ItemID.TORCH,     ItemID.DEFAULT,     ItemID.FIRE,    ItemID.WOOD },
                                    { ItemID.CAKE,      ItemID.WATER,       ItemID.DIRT,    ItemID.WOOD },
                                    { ItemID.POSION,    ItemID.DEFAULT,     ItemID.WATER,   ItemID.FIRE },
                                    { ItemID.IRON,      ItemID.DEFAULT,     ItemID.FIRE,    ItemID.DIRT }, };

    // -------------------------------------------------------------------------------- //

    void Start()
    {
        this.pitem_control = GameObject.FindGameObjectWithTag("PlayerItemControl").GetComponent<PlayerItemControl> ();

        this.guide_control = GameObject.FindGameObjectWithTag("BeginnerGuideControl").GetComponent<BeginnerGuideControl> ();

        //

        this.player_item = new ItemID[ELEMENT_AMOUNT];
        this.item_amount = new int[ELEMENT_AMOUNT];
        this.choose_item = new ItemID[MAX_COOK_NUM];

        this.choose_amount = 0;

        this.next_step = STEP.CHOOSE;

        this.GetComponent<AudioSource>().clip = this.audio_clip;

        //

        // 讀取玩家物品資訊
        for(int i = 0, inv = 0; i < this.pitem_control.player_item.Length; i++)
        {
            if (inv == ELEMENT_AMOUNT)
                break;

            if(isElement(this.pitem_control.player_item[i]))
            {
                //Debug.Log(this.pitem_control.player_item[i]);
                this.player_item[inv] = this.pitem_control.player_item[i];
                this.item_amount[inv] = this.pitem_control.player_item_amount[i];
                inv++;
            }
        }
    }

    void Update()
    {
        this.step_timer += Time.deltaTime;

        // 檢查是否往下一階段
        switch (this.step)
        {

            case STEP.CHOOSE:
                {
                    // 等待玩家選擇
                    //
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10.0f);

                        if(hit)
                        {
                            do
                            {
                                if (hit.transform.CompareTag("Element"))
                                {
                                    //Debug.Log(hit.transform.name);
                                    int id = int.Parse(hit.transform.name);

                                    ItemID element = ItemID.NONE;

                                    switch(id)
                                    {
                                        case 0: element = ItemID.WOOD; break;
                                        case 1: element = ItemID.FIRE; break;
                                        case 2: element = ItemID.DIRT; break;
                                        case 3: element = ItemID.WATER; break;
                                        default:
                                            Debug.LogWarning("Error ItemID");
                                            break;
                                    }

                                    // 選擇空的Frame放入
                                    for(int i = 0; i < this.frame.Length; i++)
                                    {
                                        if (element == ItemID.NONE)
                                            break;

                                        for (int inv = 0; inv < this.player_item.Length; inv++)
                                        {
                                            if (this.player_item[inv] == element)
                                            {
                                                if(this.item_amount[inv] < 1)
                                                {
                                                    element = ItemID.NONE;
                                                    break;
                                                }

                                                if (this.choose_item[i] == ItemID.DEFAULT)
                                                {
                                                    this.choose_item[i] = element;
                                                    this.choose_amount++;
                                                    this.frame[i].GetComponent<SpriteRenderer>().sprite = this.pitem_control.texItem[(int)element];
                                                    this.item_amount[inv]--;

                                                    element = ItemID.NONE;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                                if (hit.transform.CompareTag("Frame"))
                                {
                                    do
                                    {
                                        int id = int.Parse(hit.transform.name);

                                        if (id < 0 || id >= MAX_COOK_NUM)
                                            break;

                                        // 放回道具
                                        for (int i = 0; i < this.player_item.Length; i++)
                                        {
                                            //Debug.Log(this.player_item[i]);
                                            if (this.choose_item[id] == this.player_item[i])
                                            {
                                                this.item_amount[i]++;
                                                this.frame[id].GetComponent<SpriteRenderer>().sprite = null;
                                                this.choose_item[id] = ItemID.DEFAULT;
                                                this.choose_amount--;
                                                break;
                                            }
                                        }
                                    } while (false);
                                    break;
                                }

                                if (hit.transform.CompareTag("BackToTitle"))
                                {
                                    if(this.guide_control.isCookGuide())
                                    {
                                        this.guide_control.goalAct();
                                    }

                                    updatePlayerInventory();
                                    this.to_scene = SCENE.STORE;
                                    this.next_step = STEP.WAIT_SE_END;
                                    break;
                                }

                                if (hit.transform.CompareTag("Cook"))
                                {
                                    if (this.choose_amount > 1)
                                    {
                                        this.next_step = STEP.WAIT_COOK_END;
                                    }
                                    break;
                                }

                            } while (false);
                        }
                    }
                }
                break;

            case STEP.WAIT_COOK_END:
                {
                    //bool is_done = true;

                    /*
                    do
                    {
                        
                        // 合成動畫播放
                        if (!this.GetComponent<Animation>().isPlaying)
                        {

                            break;
                        }
                        

                        // 動畫時間
                        
                        if (this.GetComponent<Animation>().time >= this.GetComponent<AudioSource>().clip.length)
                        {

                            break;
                        }
                        

                        is_done = false;
                    } while (false);
                    */

                    //if(is_done)
                    if (this.cooked_item > ItemID.DEFAULT)
                    {
                        if (this.step_timer >= this.anim_times[(int)this.cooked_item - 1])
                        {
                            // 顯示合成成功或失敗
                            Debug.Log("Anim End");
                            //this.anims[(int)this.cooked_item - 1].localPosition = ANIM_HIDDEN;

                            this.next_step = STEP.COOK_END;
                        }
                    }
                    else
                    {
                        this.next_step = STEP.COOK_END;
                    }
                }
                break;

            case STEP.COOK_END:
                if(Input.GetMouseButtonDown(0))
                {
                    this.cooked_frame.GetComponent<SpriteRenderer>().sprite = null;
                    for(int i = 0; i < this.pitem_control.player_item.Length; i++)
                    {
                        if(this.pitem_control.player_item[i] == this.cooked_item)
                        {
                            this.pitem_control.player_item_amount[i]++;
                            break;
                        }
                    }

                    this.next_step = STEP.CHOOSE;
                }
                break;

            case STEP.WAIT_SE_END:
                {
                    // 等待SE播放結束

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

                    if (to_finish && this.to_scene != SCENE.NONE)
                    {
                        SceneManager.LoadScene("StoreScene");
                    }
                }
                break;
        }

        // 下一狀態初始化

        if (this.next_step != STEP.NONE)
        {

            switch (this.next_step)
            {
                case STEP.WAIT_COOK_END:
                    {
                        // 動畫初始化

                        // 取得合成道具
                        cookItem();

                        Debug.Log(this.cooked_item);
                        if(this.cooked_item > ItemID.DEFAULT)
                        {
                            this.anims[(int)this.cooked_item - 1].localPosition = ANIM_DISPLAY;
                        }
                    }
                    break;

                case STEP.COOK_END:
                    {
                        // 清除元素畫面
                        //Debug.Log("Clear Elements");
                        for(int i = 0; i < this.frame.Length; i++)
                        {
                            this.frame[i].GetComponent<SpriteRenderer>().sprite = null;
                        }

                        if (this.cooked_item > ItemID.DEFAULT)
                        {
                            this.anims[(int)this.cooked_item - 1].localPosition = ANIM_HIDDEN;
                        }

                        if (this.cooked_item != ItemID.NONE)
                        {
                            this.cooked_frame.GetComponent<SpriteRenderer>().sprite = this.pitem_control.texItem[(int)this.cooked_item];
                            this.cooked_item = ItemID.NONE;
                        }
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

        // 各狀態執行

        /*switch(this.step) {

			case STEP.TITLE:
			{
			}
			break;
		}*/

    }

    void OnGUI()
    {
        for(int i = 0; i < this.txt_amount.Length; i++)
        {
            this.txt_amount[i].text = this.item_amount[i].ToString();
        }
    }

    void cookItem()
    {
        //Debug.Log("組合道具");
        Array.Sort(choose_item);

        for(int i = 0; i < ITEM_AMOUNT; i++)
        {
            if(checkSum(this.choose_item[0], this.choose_item[1], this.choose_item[2])
                == checkSum(this.cookTable[i, 1], this.cookTable[i, 2], this.cookTable[i, 3]))
            {
                bool is_same = true;
                for(int idx = 0; idx < MAX_COOK_NUM; idx++)
                {
                    if(this.choose_item[idx] != this.cookTable[i, idx + 1])
                    {
                        is_same = false;
                        break;
                    }
                }

                if(is_same)
                {
                    this.cooked_item = this.cookTable[i, 0];
                    break;
                }
            }
        }

        // 清除道具
        for(int i = 0; i < this.choose_item.Length; i++)
        {
            this.choose_item[i] = ItemID.DEFAULT;
        }
    }

    int checkSum(ItemID num1, ItemID num2, ItemID num3)
    {
        return ((int)num1 + (int)num2 + (int)num3);
    }

    bool isElement(ItemID item)
    {
        return (item >= ItemID.WATER && item <= ItemID.WOOD);
    }

    void updatePlayerInventory()
    {
        for(int i = 0; i < this.player_item.Length; i++)
        {
            for(int inv = 0; inv < this.pitem_control.player_item.Length; inv++)
            {
                if(this.player_item[i] == this.pitem_control.player_item[inv])
                {
                    this.pitem_control.player_item_amount[inv] = item_amount[i];
                    break;
                }
            }
        }
    }
}
