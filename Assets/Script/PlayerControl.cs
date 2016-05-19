using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public BallControl ball;

    private Vector3 ball_offset;

    public float ball_drag_radius;

    public float screen_border;

    public float hand_border;

    private Vector3 touch_offset = new Vector3();

    private bool is_touched = false;

    public enum STEP
    {

        NONE = -1,

        NORMAL = 0,         // 一般接球
        CARRY,              // 準備發球

        GOAL_ACT,           // 得分

        NUM,
    };

    public STEP step;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;

    void Start () {
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControl> ();
        ball_offset = this.transform.position - ball.transform.position;
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
                if(is_touched && Input.GetMouseButtonUp(0))
                {
                    is_touched = false;
                    this.next_step = STEP.NORMAL;
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
                        // 朝拖曳中心點方向發射
                        ball.beginShoot(this.transform.position - ball_offset - ball.transform.position);
                    }
                    break;
                case STEP.CARRY:
                    {
                        ball.setKinematic(true);
                        is_touched = false;
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
        Vector3 curScrennPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScrennPoint);
        touch_offset = curPosition - this.transform.position;
        touch_offset.z = 0;

        switch(this.step)
        {
            case STEP.CARRY:
                {
                    is_touched = true;
                }
                break;
        }
    }

    // 拖動時的動作
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);

        // 只計算二維平面
        curPosition.z = 0;

        switch (this.step) {
            case STEP.NORMAL:
                {
                    curPosition.x -= touch_offset.x;
                    curPosition.y = this.transform.position.y;

                    // 移動邊界
                    if (curPosition.x < -screen_border + hand_border)
                        curPosition.x = -screen_border + hand_border;

                    if (curPosition.x > screen_border - hand_border)
                        curPosition.x = screen_border - hand_border;

                    this.transform.position = curPosition;
                }
                break;
            case STEP.CARRY:
                {
                    if (is_touched)
                    {
                        // 扣除點擊位置與球的間距
                        curPosition -= touch_offset + ball_offset;
                        
                        // 球原本位置
                        Vector3 ball_center = this.transform.position - ball_offset;
                        
                        // 點擊位置與球原本位置距離
                        float ball_dist = Vector3.Distance(ball_center, curPosition);
                        
                        if (ball_dist > ball_drag_radius)
                        {
                            curPosition = (curPosition - ball_center) / (ball_dist / ball_drag_radius) + ball_center;
                        }
                        
                        ball.moveBallPosition(curPosition - ball.transform.position);
                    }
                }
                break;
        }
    }

    public void carryBall()
    {
        this.next_step = STEP.CARRY;
    }

    public Vector3 getBallPosition()
    {
        return (this.transform.position - ball_offset);
    }
}