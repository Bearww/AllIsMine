using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GUIControl : MonoBehaviour {

    public BarControl red_bar;
    public BarControl yellow_bar;
    public BarControl green_bar;

    public SpriteRenderer[] chance;
    public SpriteRenderer[] frame;
    public Sprite player_chance;
    public Sprite chance_frame;

    public float life_rate = 1.0f;                      // 血量條顏色變化速率

    public int high_level_percent;
    public int medium_level_percent;

    public Transform goal_sprite;
    public Transform gameover_sprite;

    public bool is_disp_goal = false;                   // 目標是否達成
    public bool is_disp_gameover = false;               // 遊戲是否結束

    public static Vector3 BOARD_DISPLAY = new Vector3();
    public static Vector3 BOARD_HIDDEN = new Vector3();

    public float num_offset = 1.0f;
    
    public Vector3 jewel_position;
    public Vector3 collide_position;
    public Vector3 money_position;

    public int jewel_digit = 1;
    public int collide_digit = 1;
    public int money_digit = 1;

    private SceneControl scene_control = null;
    private BuildingControl building_control = null;
    private ScoreControl jewel;                         // 顯示分數
    private ScoreControl collide;                       // 顯示撞擊次數
    private ScoreControl money;                         // 顯示金錢
    private int destroy_percent;

    public Sprite[] texNum;

    // ---------------------------------------------------------------- //

    // Use this for initialization
    void Start()
    {
        this.scene_control = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<SceneControl> ();

        this.building_control = GameObject.FindGameObjectWithTag("Base").GetComponent<BuildingControl> ();

        // 寶石數量初始化        
        this.jewel = gameObject.AddComponent<ScoreControl> ();
        this.jewel.texture = texNum;
        this.jewel.digitOffset = num_offset;
        this.jewel.digitNum = jewel_digit;
        this.jewel.drawZero = true;
        
        // 碰撞次數初始化
        this.collide = gameObject.AddComponent<ScoreControl> ();
        this.collide.texture = texNum;
        this.collide.digitOffset = num_offset;
        this.collide.digitNum = collide_digit;
        this.collide.drawZero = true;

        // 金錢數量出始化
        this.money = gameObject.AddComponent<ScoreControl> ();
        this.money.texture = texNum;
        this.money.digitOffset = num_offset;
        this.money.digitNum = money_digit;
        this.money.drawZero = true;

        //

        this.is_disp_goal = false;
        this.is_disp_gameover = false;

        for(int i = 0; i < chance.Length; i++)
        {
            chance[i].sprite = player_chance;
            chance[i].sortingOrder = 10;
            frame[i].sprite = chance_frame;
            frame[i].sortingOrder = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        // 更新數字資訊
        this.jewel.position = jewel_position;
        this.jewel.digitOffset = num_offset;
        this.jewel.setNum(this.scene_control.player_jewels);
        
        this.collide.position = collide_position;
        this.collide.digitOffset = num_offset;
        this.collide.setNum(this.scene_control.collision_time);

        this.money.position = money_position;
        this.money.digitOffset = num_offset;
        this.money.setNum(this.scene_control.player_money);

        for (int i = 0; i < SceneControl.MAX_PLAYER_CHANCE - this.scene_control.player_chance; i++)
        {
            if (i == SceneControl.MAX_PLAYER_CHANCE)
                break;
            if(this.chance[i].enabled)
                this.chance[i].enabled = false;
        }

        destroy_percent = this.building_control.getDestroyPercent();
        if (destroy_percent > high_level_percent)
        {
            this.green_bar.setValue(destroy_percent);
            this.yellow_bar.setValue(destroy_percent);
            this.red_bar.setValue(destroy_percent);
        }
        else if(destroy_percent > medium_level_percent)
        {
            this.green_bar.isVisible(false);
            this.yellow_bar.setValue(destroy_percent);
            this.red_bar.setValue(destroy_percent);
        }
        else
        {
            this.green_bar.isVisible(false);
            this.yellow_bar.isVisible(false);
            this.red_bar.setValue(destroy_percent);
        }

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, 10.0f);

            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log(hit.transform.tag);
                if (hit.transform.CompareTag("GUIControl"))
                {
                    // 回主畫面
                    Debug.Log("回主畫面");
                    SceneManager.LoadScene("TitleScene");
                }
            }
        }
    }

    void OnGUI()
    {
        if (this.is_disp_goal)
        {
            this.goal_sprite.position = BOARD_DISPLAY;
        }

        if (this.is_disp_gameover)
        {
            this.gameover_sprite.position = BOARD_DISPLAY;
        }
    }
}
