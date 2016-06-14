using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public BallControl ball;

    public ItemControl item_control;

    public BeginnerGuideControl guide_control;

    private Vector3 ball_offset;

    public static float MIN_DRAG_ANGLE = 105.0f;

    public static float MAX_DRAG_ANGLE = 100.0f;

    public static float MIN_DRAG_RADIUS = 0.2f;

    public static float MAX_DRAG_RADIUS = 1.0f;

    public float screen_border;

    public float hand_border;

    private Vector3 touch_offset = new Vector3();

    public enum STEP
    {

        NONE = -1,

        NORMAL = 0,         // 一般接球
        CARRY,              // 準備發球
        LAUNCH,             // 發球瞄準
        HURT,               // 受到傷害

        GOAL_ACT,           // 得分

        NUM,
    };

    public STEP step;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;

    public bool is_controlable = true;

    void Start () {
        this.ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControl> ();
        this.ball.player_control = this;

        this.item_control = GameObject.FindGameObjectWithTag("ItemControl").GetComponent<ItemControl> ();

        this.guide_control = GameObject.FindGameObjectWithTag("BeginnerGuideControl").GetComponent<BeginnerGuideControl> ();

        //

        this.ball_offset = this.transform.position - this.ball.transform.position;

        //

        this.is_controlable = true;
    }
	
	void Update () {
        this.step_timer += Time.deltaTime;

        // ---------------------------------------------------------------- //
        // 檢查是否換到下一個狀態
        switch (this.step)
        {
            case STEP.NORMAL:
                break;
            case STEP.CARRY:
                break;
            case STEP.LAUNCH:
                if(Input.GetMouseButtonUp(0))
                {
                    // 當拉動距離最小誤差距離才發射
                    if (Vector3.Distance(this.transform.position - this.ball_offset, this.ball.transform.position) > MIN_DRAG_RADIUS)
                    {
                        if (this.guide_control.isDragGuide())
                        {
                            this.guide_control.goalAct();
                        }

                        // 朝拖曳中心點方向發射
                        Debug.Log("發射球");
                        this.ball.beginShoot(this.transform.position - this.ball_offset - this.ball.transform.position);
                        this.next_step = STEP.NORMAL;
                    }
                    else
                    {
                        //Debug.Log("Drag distance too short");
                        // 重新指引
                        
                        this.ball.setBallPosition(this.transform.position - this.ball_offset);
                        this.next_step = STEP.CARRY;
                    }
                }
                break;
        }

        // ---------------------------------------------------------------- //
        // 狀態轉變初始化

        if (this.next_step != STEP.NONE)
        {

            switch (this.next_step)
            {
                case STEP.NORMAL:
                    {
                    }
                    break;
                case STEP.CARRY:
                    {
                        this.ball.setKinematic(true);
                    }
                    break;
                case STEP.GOAL_ACT:
                    {
                        
                    }
                    break;
            }

            this.step = this.next_step;
            this.next_step = STEP.NONE;

            this.step_timer = 0.0f;
        }

        // ---------------------------------------------------------------- //
        // 各個狀態執行

        switch (this.step)
        {
            case STEP.NORMAL:
            case STEP.CARRY:
                {
                }
                break;
        }
    }

    // 當滑鼠按下
    void OnMouseDown()
    {
        if (!this.is_controlable)
            return;

        Vector3 curScrennPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScrennPoint);
        touch_offset = this.transform.position - curPosition;
        touch_offset.z = 0;

        switch(this.step)
        {
            case STEP.CARRY:
            {
                this.next_step = STEP.LAUNCH;
            }
            break;
        }
    }

    // 拖動時的動作
    void OnMouseDrag()
    {
        if (!this.is_controlable)
            return;

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);

        // 只計算二維平面
        curPosition.z = 0;

        switch (this.step) {
            case STEP.NORMAL:
                {
                    curPosition.x += touch_offset.x;
                    curPosition.y = this.transform.position.y;

                    // 移動邊界
                    if (curPosition.x < -screen_border + hand_border)
                        curPosition.x = -screen_border + hand_border;

                    if (curPosition.x > screen_border - hand_border)
                        curPosition.x = screen_border - hand_border;

                    this.transform.position = curPosition;
                }
                break;
            case STEP.LAUNCH:
                {
                    // 滑鼠點擊點與滑鼠目前點的距離
                    float drag_dist = Vector3.Distance(curPosition, this.transform.position - this.touch_offset);

                    Vector3 dir = curPosition - (this.transform.position - this.touch_offset);
                    dir.Normalize();

                    if (drag_dist > MAX_DRAG_RADIUS)
                    {
                        curPosition = this.transform.position - this.touch_offset + dir * MAX_DRAG_RADIUS;
                    }

                    // 與(0, 1)夾角
                    float drag_angle = Vector3.Angle(dir, Vector2.up);
                    if (drag_angle < MIN_DRAG_ANGLE)
                    {
                        ball.setBallPosition(this.transform.position - this.ball_offset);
                        return;
                    }
                    /*
                    if (drag_angle < MAX_DRAG_ANGLE)
                    {
                        dir = new Vector2(0.683f, 0.25f);
                        dir.Normalize();
                        curPosition = this.transform.position - this.touch_offset + dir * MAX_DRAG_RADIUS;
                    }
                    */

                    ball.moveBallPosition(curPosition + this.touch_offset - this.ball_offset - this.ball.transform.position);
                }
                break;
        }
    }

    public void carryBall()
    {
        this.next_step = STEP.CARRY;
    }

    public bool isCarry()
    {
        return (this.step == STEP.CARRY);
    }

    public Vector3 getBallPosition()
    {
        return (this.transform.position - ball_offset);
    }

    public bool isHurt()
    {
        return (this.step == STEP.HURT);
    }

    // 設定玩家控制
    public void setControlable(bool sw)
    {
        this.is_controlable = sw;
    }

    public void setPlayerCollision(bool enabled)
    {
        GameObject player_child = GameObject.FindGameObjectWithTag("PlayerObstacle");
        if (enabled) player_child.gameObject.layer = 14;    // Player(Obstacle)
        else player_child.gameObject.layer = 12;            // Player(Touch)
    }
}