using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoardControl : MonoBehaviour {

    public GameControl game_control = null;

    public SceneControl scene_control = null;

    public PlayerItemControl pitem_control = null;

    private const int SCORE_BOARD_NUM = 6;

    // 預設玩家圖片
    public Sprite[] player_default_sprite;

    // 遊戲結算畫面
    public Canvas canvas;

    // 畫面文字
    public Text ui_collision;
    public Text ui_money;
    public Text ui_jewel;
    public Text[] ui_player_score;

    // 分數星等
    public SpriteRenderer[] star_sprite;

    struct MapData
    {
        public int money;
        public int low_score;
        public int medium_score;
        public int high_score;
        public int[] scoreboard;
        public Sprite[] playerboard;
    };

    private int map_amount = 4;
    private MapData[] m_data;

    // ---------------------------------------------------------------- //

    public enum STEP
    {

        NONE = -1,

        START = 0,              // 開始

        NUM,
    };

    public STEP step;
    public STEP next_step = STEP.NONE;

    public float step_timer_prev;
    public float step_timer;

    // ---------------------------------------------------------------- //

    void Start () {
        DontDestroyOnLoad(this);

        this.game_control = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl> ();

        this.pitem_control = GameObject.FindGameObjectWithTag("PlayerItemControl").GetComponent<PlayerItemControl> ();

        // 讀檔

        // 預設檔
        LoadDefaultData();

        clear();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // ---------------------------------------------------------------- //

    public void create()
    {
        this.step_timer_prev = 0.0f;
        this.step_timer = 0.0f;

        this.canvas.worldCamera = Camera.main;
        
        this.ui_collision.text = "";
        this.ui_money.text = "";
        this.ui_jewel.text = "";

        GameObject[] stars = GameObject.FindGameObjectsWithTag("Star");
        //Debug.Log(stars.Length);
        this.star_sprite = new SpriteRenderer[3];
        for (int i = 0; i < star_sprite.Length; i++)
            this.star_sprite[i] = stars[i].GetComponent<SpriteRenderer> ();

        this.canvas.enabled = false;
    }

    public void start()
    {
        this.scene_control.gui_control.is_disp_goal = true;

        this.ui_collision.text = this.scene_control.collision_time.ToString();
        this.ui_money.text = this.scene_control.player_money.ToString();
        this.ui_jewel.text = this.scene_control.player_jewels.ToString();

        Debug.Log("寫入玩家資料");
        this.pitem_control.player_money += this.scene_control.player_money;
        this.pitem_control.player_jewel += this.scene_control.player_jewels;

        Debug.Log(this.game_control.play_scene);
        for (int i = 0; i < this.ui_player_score.Length; i++)
            this.ui_player_score[i].text = this.m_data[(int)this.game_control.play_scene].scoreboard[i].ToString();
            //Debug.Log(this.m_data[(int)this.game_control.play_scene].scoreboard[i].ToString());

        // 顯示星等
        Debug.Log("顯示星等");
        int star = 0;
        if (this.scene_control.player_money >= this.m_data[(int)this.game_control.play_scene].low_score)
            star = 1;
        if (this.scene_control.player_money >= this.m_data[(int)this.game_control.play_scene].medium_score)
            star = 2;
        if (this.scene_control.player_money >= this.m_data[(int)this.game_control.play_scene].high_score)
            star = 3;

        for(int i = 0; i < star; i++)
        {
            this.star_sprite[i].enabled = true;
        }

        this.step = STEP.NONE;
        this.next_step = STEP.START;
    }

    public void execute()
    {
        this.step_timer_prev = this.step_timer;
        this.step_timer += Time.deltaTime;

        // -------------------------------------------------------- //
        // 檢查是否換到下一個狀態

        /*
        switch (this.step)
        {
        }
        */

        // -------------------------------------------------------- //
        // 狀態轉變初始化

        if (this.next_step != STEP.NONE)
        {
            switch(this.next_step)
            {
                case STEP.START:
                {
                    this.scene_control.player_control.setControlable(false);

                    this.canvas.enabled = true;

                    int map = (int)this.game_control.play_scene;
                    
                    // 更新分數
                    this.m_data[map].money = Mathf.Max(this.m_data[map].money, this.scene_control.player_money);

                    // 判斷升級
                    Debug.Log("判斷升級");
                    if (this.m_data[map].low_score <= this.scene_control.player_money)
                    {
                        this.game_control.upgradeLevel();
                    }
                }
                break;
            }

            this.step = this.next_step;
            this.next_step = STEP.NONE;

            this.step_timer = 0.0f;
        }

        /*
        // -------------------------------------------------------- //
        // 各個狀態執行

        switch (this.step)
        {
        }
        */
    }

    public void clear()
    {
        this.canvas.enabled = false;
    }

    void LoadDefaultData()
    {
        Debug.Log("Initialize map data");

        this.m_data = new MapData[this.map_amount];

        for(int i = 0; i < this.map_amount; i++)
        {
            // 初始金錢
            this.m_data[i].money = 0;

            // 初始玩家分數
            this.m_data[i].scoreboard = new int[SCORE_BOARD_NUM];

            // 初始玩家圖片
            this.m_data[i].playerboard = new Sprite[SCORE_BOARD_NUM];
        }

        int map;

        // 第一關排行榜
        map = (int)SCENE.COTTAGE;
        this.m_data[map].low_score = 50;
        this.m_data[map].medium_score = 100;
        this.m_data[map].high_score = 150;
        for(int i = 0; i < SCORE_BOARD_NUM; i++)
        {
            this.m_data[map].scoreboard[i] = this.m_data[map].high_score - 20 * i;
        }

        // 第二關排行榜
        map = (int)SCENE.INDIA;
        this.m_data[map].low_score = 250;
        this.m_data[map].medium_score = 350;
        this.m_data[map].high_score = 400;
        for (int i = 0; i < SCORE_BOARD_NUM; i++)
        {
            this.m_data[map].scoreboard[i] = this.m_data[map].high_score - 40 * i;
        }

        // 第三關排行榜
        map = (int)SCENE.JAPAN;
        this.m_data[map].low_score = 300;
        this.m_data[map].medium_score = 500;
        this.m_data[map].high_score = 700;
        for (int i = 0; i < SCORE_BOARD_NUM; i++)
        {
            this.m_data[map].scoreboard[i] = this.m_data[map].high_score - 80 * i;
        }

        // 第四關排行榜
        map = (int)SCENE.TW101;
        this.m_data[map].low_score = 50;
        this.m_data[map].medium_score = 100;
        this.m_data[map].high_score = 150;
        for (int i = 0; i < SCORE_BOARD_NUM; i++)
        {
            this.m_data[map].scoreboard[i] = this.m_data[map].high_score - 20 * i;
        }
    }

    public bool isPlayed(int map)
    {
        if(map < this.map_amount)
        {
            return (this.m_data[map].money != 0);
        }
        return false;
    }
}
