using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LotteryControl : MonoBehaviour {

    private PlayerItemControl pitem_control = null;

    private ScoreControl jewel;                         // 顯示分數
    private ScoreControl money;                         // 顯示金錢
    private ScoreControl paper_money;                   // 顯示鈔票

    public Vector3 paper_money_position;
    public Vector3 jewel_position;
    public Vector3 money_position;

    public float num_offset = 1.0f;

    public Sprite[] texNum;

    public Transform ask_frame;

    public Transform ball_frame;

    public Transform toy_frame;

    public Transform ball_clone;

    public Transform toy_clone;

    private int toy_id;

    public Transform arrow;

    public static Vector3 FRAME_DISPLAY = new Vector3();

    public static Vector3 FRAME_HIDDEN = new Vector3(6.0f, 0.0f);

    public static Vector3 TOY_DISPLAY = new Vector3(0.0f, -2.55f);

    public static Vector3 ENLARGE_DISPLAY = new Vector3(3.0f, 3.0f, 1.0f);

    public static Vector3 SUSHI_SCALE = new Vector3(3.8f, 3.8f, 1.0f);

    public static int JEWELS_PER_TOY = 5;

    // 進行狀態
    public enum STEP
    {

        NONE = -1,

        TOY = 0,              // 畫面顯示(等待輸入)
        ASK,                  // 詢問玩家
        BUY,                  // 購買道具
        WAIT_AM,              // 等待Animation
        WAIT_AM_END,          // 等待Animation播放結束
        DISPLAY,              // 展示球
        WAIT_SE_END,          // 等待SE播放結束

        NUM,
    };

    private STEP step = STEP.NONE;
    private STEP next_step = STEP.NONE;
    private float step_timer = 0.0f;

    public AudioClip audio_clip;

    // -------------------------------------------------------------------------------- //

    // Use this for initialization
    void Start () {
        this.pitem_control = GameObject.FindGameObjectWithTag("PlayerItemControl").GetComponent<PlayerItemControl>();

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

        this.next_step = STEP.TOY;

        this.GetComponent<AudioSource>().clip = this.audio_clip;

    }
	
	// Update is called once per frame
	void Update () {

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

            case STEP.TOY:
                {
                    // 當滑鼠左鍵按下
                    //
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10.0f);

                        if (hit)
                        {
                            //Debug.Log(hit.transform.gameObject.tag);
                            switch (hit.transform.gameObject.tag)
                            {
                                case "BackToTitle":
                                    {
                                        this.next_step = STEP.WAIT_SE_END;
                                    }
                                    break;

                                case "Ball":
                                    {
                                        this.next_step = STEP.ASK;
                                    }
                                    break;

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
                                    // 確認購買
                                    this.next_step = STEP.BUY;
                                }
                                else
                                {
                                    this.next_step = STEP.TOY;
                                }
                            }
                        }
                    }
                }
                break;

            case STEP.BUY:
                {
                    this.next_step = STEP.WAIT_AM;
                }
                break;

            case STEP.WAIT_AM:
                {
                    // Animation完成時，開始下個動畫

                    bool to_finish = true;

                    do
                    {

                        if (!this.toy_frame.GetComponent<Animation> ().isPlaying)
                        {

                            break;
                        }

                        /*
                        if (this.anim_clip.time >= this.enlarge_clip.length)
                        {

                            break;
                        }
                        */

                        to_finish = false;

                    } while (false);

                    if (to_finish)
                    {

                        this.next_step = STEP.WAIT_AM_END;
                    }
                }
                break;

            case STEP.WAIT_AM_END:
                {
                    // Animation完成時，回到展示畫面

                    bool to_finish = true;

                    do
                    {

                        if (!this.ball_frame.GetComponent<Animation> ().isPlaying)
                        {

                            break;
                        }

                        /*
                        if (this.enlarge_clip.time >= this.enlarge_clip.length)
                        {

                            break;
                        }
                        */

                        to_finish = false;

                    } while (false);

                    if (to_finish)
                    {

                        this.next_step = STEP.DISPLAY;
                    }
                }
                break;

            case STEP.DISPLAY:
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        this.next_step = STEP.TOY;
                    }
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
                case STEP.TOY:
                    {
                        this.arrow.GetComponent<Animation> ().Play();
                        this.ask_frame.localPosition = FRAME_HIDDEN;
                        this.ball_frame.localPosition = FRAME_HIDDEN;
                        this.ball_frame.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        if (this.ball_frame.childCount > 0)
                        {
                            //Debug.Log("摧毀球");
                            Destroy(this.ball_clone.gameObject);
                        }
                    }
                    break;

                case STEP.ASK:
                    {
                        // 顯示詢問畫面
                        this.ask_frame.localPosition = FRAME_DISPLAY;
                        this.arrow.GetComponent<Animation>().Stop();
                        this.arrow.localPosition = FRAME_HIDDEN;
                    }
                    break;

                case STEP.BUY:
                    {
                        this.ask_frame.localPosition = FRAME_HIDDEN;

                        if (this.pitem_control.player_jewel >= JEWELS_PER_TOY)
                        {
                            this.pitem_control.player_jewel -= JEWELS_PER_TOY;

                            // 亂數抽球
                            int ball = Random.Range(1, this.pitem_control.ballSpirte.Length - 1);

                            this.pitem_control.player_ball[ball] = true;

                            // 產生在扭蛋裡
                            this.toy_frame.localPosition = FRAME_HIDDEN;
                            toy_clone = Instantiate(this.pitem_control.ballSpirte[ball], FRAME_HIDDEN, Quaternion.identity) as Transform;
                            toy_clone.parent = this.toy_frame;
                            toy_clone.localPosition = new Vector3();
                            toy_clone.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                            // 產生在框架中
                            this.toy_frame.localPosition = FRAME_HIDDEN;
                            ball_clone = Instantiate(this.pitem_control.ballSpirte[ball], FRAME_HIDDEN, Quaternion.identity) as Transform;
                            ball_clone.parent = this.ball_frame;
                            ball_clone.localPosition = new Vector3();
                            ball_clone.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                            // 壽司
                            if(ball == 1)
                            {
                                toy_clone.localScale = SUSHI_SCALE;
                                ball_clone.localScale = SUSHI_SCALE;
                            }
                        }
                        else
                        {
                            this.next_step = STEP.TOY;
                        }
                    }
                    break;

                case STEP.WAIT_AM:
                    {
                        // 顯示抽球動畫
                        this.toy_frame.localPosition = TOY_DISPLAY;
                        this.toy_frame.GetComponent<Animation> ().Play();
                    }
                    break;

                case STEP.WAIT_AM_END:
                    {
                        if (this.toy_frame.childCount > 0)
                        {
                            //Debug.Log("摧毀玩具");
                            Destroy(this.toy_clone.gameObject);
                        }

                        // 顯示球放大動畫
                        this.ball_frame.localPosition = FRAME_DISPLAY;
                        this.ball_frame.GetComponent<Animation> ().Play();
                    }
                    break;

                case STEP.DISPLAY:
                    {
                        this.ball_frame.localScale = ENLARGE_DISPLAY;
                    }
                    break;

                case STEP.WAIT_SE_END:
                    {
                        // 開始播放SE
                        this.GetComponent<AudioSource> ().Play();
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
}
