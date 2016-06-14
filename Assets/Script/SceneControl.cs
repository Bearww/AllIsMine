using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneControl : MonoBehaviour
{

    public PlayerControl player_control = null;

    public BallControl ball_control = null;

    public BuildingControl building_control = null;

    public GUIControl gui_control = null;

    public ScoreBoardControl board_control = null;

    public BeginnerGuideControl guide_control = null;

    // ---------------------------------------------------------------- //

    public int player_health;

    public static int MAX_PLAYER_CHANCE = 5;

    public int player_chance = 0;

    public int player_jewels = 0;

    public int player_money = 0;

    public int collision_time = 0;

    // ---------------------------------------------------------------- //

    public enum STEP
    {

        NONE = -1,

        PLAY = 0,           // 遊戲中
        WAIT_AN_END,
        GOAL_ACT,           // 目標產生
        MISS,               // 接球失誤
        SHOPPING,           // 購買道具

        GAMEOVER,           // 遊戲結束

        NUM,
    };

    public STEP step;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;

    public bool is_missing;


    // ---------------------------------------------------------------- //

    public enum SE
    {

        NONE = -1,

        DROP = 0,           // ブロックをドロップしたとき.
        DROP_CONNECT,       // ブロックが消えるとき（同じ色のブロックが４つ並んだとき）.
        LANDING,            // 上から降ってきたブロックが着地したとき.
        SWAP,               // 上下のブロックが回転して入れ替わるとき.
        EATING,             // ケーキを食べるとき.
        JUMP,               // 上から降ってきたブロックが着地したとき.
        COMBO,              // 連鎖したとき.

        CLEAR,              // クリアー.
        MISS,               // ミス.

        NUM,
    };

    public AudioClip[] audio_clips;

    public AudioSource[] audios;

    public int ball_id;

    public Transform anim;

    public static Vector3 ANIM_DISPLAY = new Vector3(0, 0);

    public static Vector3 ANIM_HIDDEN = new Vector3(-12, 0);

    // ---------------------------------------------------------------- //

    void Start()
    {
        this.player_control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        this.player_control.carryBall();

        this.ball_control = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControl> ();
        this.ball_control.scene_control = this;
        this.ball_control.readyToShoot();

        this.building_control = GameObject.FindGameObjectWithTag("Base").GetComponent<BuildingControl> ();

        this.gui_control = GameObject.FindGameObjectWithTag("GUIControl").GetComponent<GUIControl> ();

        this.board_control = GameObject.FindGameObjectWithTag("ScoreBoardControl").GetComponent<ScoreBoardControl> ();
        this.board_control.scene_control = this;
        this.board_control.create();

        this.guide_control = GameObject.FindGameObjectWithTag("BeginnerGuideControl").GetComponent<BeginnerGuideControl> ();

        /* Ignore buildings and buildings */
        //Physics2D.IgnoreLayerCollision(8, 8);
        /* Ignore buildings and obstacles */
        Physics2D.IgnoreLayerCollision(8, 9);
        /* Ignore buildings and base */
        //Physics2D.IgnoreLayerCollision(8, 10);
        /* Ignore buildings and player */
        Physics2D.IgnoreLayerCollision(8, 12);
        /* Ignore buildings and border */
        //Physics2D.IgnoreLayerCollision(8, 13);
        /* Igonre buildings and player(obstacle) */
        Physics2D.IgnoreLayerCollision(8, 14);
        /* Ignore obstacles and obstacles */
        Physics2D.IgnoreLayerCollision(9, 9);
        /* Ignore obstacles and ball */
        //Physics2D.IgnoreLayerCollision(9, 11);
        /* Ignore obstacles and player(touch) */
        Physics2D.IgnoreLayerCollision(9, 12);
        /* Ignore obstacles and border */
        Physics2D.IgnoreLayerCollision(9, 13);
        /* Igonre obstacles and player(obstacle) */
        Physics2D.IgnoreLayerCollision(9, 14);
        /* Ignore base and ball */
        Physics2D.IgnoreLayerCollision(10, 11);
        /* Ignore ball and ball */
        Physics2D.IgnoreLayerCollision(11, 11);
        /* Ignore ball and player(touch) */
        Physics2D.IgnoreLayerCollision(11, 12);
        /* Ignore ball and border */
        //Physics2D.IgnoreLayerCollision(11, 13);
        /* Igonre player(touch) and player(obstacle) */
        Physics2D.IgnoreLayerCollision(12, 14);

        this.player_health = 5;

        this.player_chance = MAX_PLAYER_CHANCE;

        this.is_missing = false;

        this.next_step = STEP.PLAY;

        // 使用新手指引
        if (this.guide_control.isStartGuide())
        {
            Debug.Log("遊戲指引");
            this.guide_control.displayWait();
        }
    }

    void Update()
    {
        this.step_timer += Time.deltaTime;

        // -------------------------------------------------------- //
        // 檢查是否換到下一個狀態

        switch (this.step)
        {
            case STEP.PLAY:
                {
                    if(this.guide_control.isStartGuide())
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            this.guide_control.forceGoalAct();
                        }
                    }

                    if (this.is_missing)
                    {
                        do
                        {
                            if (this.ball_control.isAnyClone())
                            {
                                // 仍有複製的球存在
                                break;
                            }
                            else
                            {
                                is_missing = false;
                                this.next_step = STEP.MISS;
                            }
                        } while (false);
                    }

                    if (this.player_control.isHurt())
                    {
                        this.player_health--;
                        this.player_health = Mathf.Max(0, this.player_health);
                        Debug.Log("玩家受傷");
                        break;
                    }

                    if (this.building_control.isBroken())
                    {
                        this.next_step = STEP.WAIT_AN_END;
                    }
                }
                break;

            case STEP.MISS:
                {
                    // 減少次數
                    this.player_chance--;
                    this.player_chance = Mathf.Max(0, this.player_chance);
                    if (this.player_chance == 0)
                    {
                        this.next_step = STEP.GAMEOVER;
                    }
                    else
                    {
                        ball_control.setBallPosition(player_control.getBallPosition());
                        // 重新發球
                        ball_control.revive();
                        player_control.carryBall();
                        this.next_step = STEP.PLAY;
                    }
                }
                break;

            case STEP.WAIT_AN_END:
                {
                    // 滑鼠點擊
                    if(this.step_timer > 1.0f && Input.GetMouseButtonDown(0))
                    {
                        this.next_step = STEP.GOAL_ACT;
                    }
                }
                break;

            case STEP.GOAL_ACT:
            case STEP.GAMEOVER:
                {
                    // 滑鼠點擊
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, 10.0f);

                        foreach(RaycastHit2D hit in hits)
                        {
                            Debug.Log(hit.transform.tag);
                            if (hit.transform.CompareTag("Board"))
                            {
                                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                if (pos.x > 0)
                                {
                                    if (!this.guide_control.isVictoryGuide())
                                    {
                                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                                    }
                                }
                                else
                                {
                                    if(this.guide_control.isVictoryGuide())
                                    {
                                        this.guide_control.forceGoalAct();
                                    }
                                    SceneManager.LoadScene("TitleScene");
                                }
                            }
                        }

                        /*
                        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10.0f);

                        if (hit)
                        {
                            Debug.Log(hit.transform.tag);
                            if (hit.transform.CompareTag("Board"))
                            {
                                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                if (pos.x > 0)
                                {
                                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                                }
                                else
                                {
                                    SceneManager.LoadScene("TitleScene");
                                }
                            }
                        }
                        */
                    }
                }
                break;
        }

        // -------------------------------------------------------- //
        // 狀態轉變初始化

        if (this.next_step != STEP.NONE)
        {

            switch (this.next_step)
            {
                case STEP.PLAY:
                    {
                    }
                    break;
                case STEP.MISS:
                    {
                        //this.playSe(SE.MISS);
                    }
                    break;

                case STEP.WAIT_AN_END:
                    {
                        this.board_control.pitem_control.player_ball[this.ball_id] = true;
                        this.anim.localPosition = ANIM_DISPLAY;
                    }
                    break;

                case STEP.GAMEOVER:
                    {
                        Debug.Log("Fail :(");
                        this.ball_control.setKinematic(true);
                        this.gui_control.is_disp_gameover = true;
                    }
                    break;

                case STEP.GOAL_ACT:
                    {
                        Debug.Log("Win!!!");
                        this.anim.localPosition = ANIM_HIDDEN;
                        this.ball_control.setKinematic(true);
                        this.board_control.start();

                        if(this.guide_control.is_beginner_guide)
                        {
                            this.guide_control.forceGoalAct();
                        }
                        //this.goal_scene.start();
                    }
                    break;
            }

            this.step = this.next_step;
            this.next_step = STEP.NONE;

            this.step_timer = 0.0f;
        }

        // -------------------------------------------------------- //
        // 各個狀態執行

        switch (this.step)
        {
            case STEP.GOAL_ACT:
                {
                    this.board_control.execute();
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Building"))
        {
            if(!other.GetComponent<BuildingMaterial> ())
            {
                Debug.Log(other);
                Debug.Log("Building no BuildingMaterial Component");
                Destroy(other.gameObject);
            }
            other.GetComponent<BuildingMaterial> ().destroyBuilding();
        }
        else if (other.gameObject.CompareTag("Ball"))
        {
            if (other.GetComponent<BallControl> ().isClone())
            {
                this.ball_control.ball_amount--;
                Destroy(other.gameObject);
            }

            if (this.ball_control.isPlaying())
            {
                // 漏接球
                //Debug.Log("Miss ball!");
                this.is_missing = true;
            }
        }
    }

    public bool isMiss()
    {
        return (this.step == STEP.MISS);
    }
}
