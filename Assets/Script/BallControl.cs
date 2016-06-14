using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

    public SceneControl scene_control;

    public PlayerControl player_control;

    public int ball_amount = 0;

    public float ball_magnitude = 0f;

    // 球撞擊碰撞持續時間
    public float ball_power = 0f;

    public Transform fly_prefab;

    private Vector3 previous_col;
    private Vector3 previous_vec;

    private int repeat_time = 0;

    public static int MAX_REPEAT = 6;
    public static float MIN_ANGLE = 8.0f;

    private float collision_time;
    private Vector3 collision_pos;

    public static float MIN_COLLISION_TIME = 0.1f;
    public static float MIN_COLLISION_DISTANCE = 0.05f;

    private bool is_clone = false;

    public enum STEP
    {

        NONE = -1,

        NORMAL = 0,         // 球已發出
        READY,              // 準備碰撞
        CHARGE,             // 蓄力瞄準

        NUM,
    };

    AudioSource audio;

    public STEP step;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;

    void Start ()
    {
        this.audio = this.GetComponent<AudioSource>();
    }

	void Update ()
    {
        this.step_timer += Time.deltaTime;

        // ---------------------------------------------------------------- //
        // 檢查是否換到下一個狀態
        switch (this.step)
        {
            case STEP.NORMAL:
            case STEP.READY:
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
                        // 與手部碰撞
                        player_control.setPlayerCollision(true);
                    }
                    break;
                case STEP.READY:
                    {
                        // 忽略手部碰撞
                        player_control.setPlayerCollision(false);
                    }
                    break;
                case STEP.CHARGE:
                    {
                        // 忽略手部碰撞
                        player_control.setPlayerCollision(false);
                        // 球由玩家操作
                        this.GetComponent<Rigidbody2D>().isKinematic = true;
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
                {
                    if ((Mathf.Abs(this.transform.position.x) > 4.0f) || (Mathf.Abs(this.transform.position.y) > 10.0f))
                    {
                        if(!this.scene_control.isMiss())
                            this.scene_control.is_missing = true;
                    }
                }
                break;
            case STEP.READY:
                {
                    if ((Mathf.Abs(this.transform.position.x) > 4.0f) || (Mathf.Abs(this.transform.position.y) > 10.0f))
                    {
                        this.setBallPosition(this.player_control.getBallPosition());
                    }
                }
                break;
        }
    }

    // 當碰撞發生時
    void OnCollisionEnter2D (Collision2D col)
    {
        if (this.step == STEP.NORMAL)
        {
            ballCollision();
            return;
        }

        if (this.step == STEP.READY && !col.gameObject.CompareTag("Player"))
        {
            ballCollision();
            this.next_step = STEP.NORMAL;
            return;
        }
    }

    /*
    // 碰撞存在，檢查有無卡球
    void OnCollisionExist2D (Collision2D col)
    {
        if(this.step_timer - this.collision_time > MIN_COLLISION_TIME)
        {
            if(Vector3.Distance(this.transform.position, this.collision_pos) < MIN_COLLISION_DISTANCE)
            {
                // 朝相反方向
                Vector3 dir = this.collision_pos - this.transform.position;
                dir.Normalize();
                this.GetComponent<Rigidbody2D> ().velocity = dir * this.ball_magnitude;
            }
        }
    }
    */

    // 當碰撞完，速度變回原本的速度
    void OnCollisionExit2D (Collision2D col)
    {
        Vector2 dir = this.GetComponent<Rigidbody2D> ().velocity;

        // 撞到蒼蠅
        if(col.gameObject.CompareTag("Fly"))
        {
            // 改變方向
            dir.Normalize();
            dir += new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        }

        dir.Normalize();
        this.GetComponent<Rigidbody2D> ().velocity = dir * this.ball_magnitude;
    }

    private void ballCollision()
    {
        // 記錄碰撞資訊
        collision_time = this.step_timer;
        collision_pos = this.transform.position;

        // 碰撞次數增加
        this.scene_control.collision_time++;

        // 發出碰撞聲響
        this.audio.Play();

        // 檢查球的軌跡
        Vector3 v = this.previous_col - this.transform.position;
        float angle = Vector3.Angle(v, this.previous_vec);

        // 記錄相似軌跡次數
        //Debug.Log(angle);
        if (180.0f - angle < MIN_ANGLE)
            this.repeat_time++;
        else
            this.repeat_time = 0;

        if(this.repeat_time >= MAX_REPEAT)
        {
            // 在下次路徑中產生蒼蠅
            Debug.Log("產生蒼蠅");
            Instantiate(this.fly_prefab, this.transform.position - this.previous_vec / 2, Quaternion.identity);
            this.repeat_time = 0;
        }
        
        this.previous_col = this.transform.position;
        this.previous_vec = v;
    }

    public void beginShoot (Vector3 dir)
    {
        if(dir.x == 0 && dir.y == 0)
        {
            dir = new Vector3(0, 1);
        }

        // 複製球產生
        //Debug.Log("複製球");
        for(int i = 0; i < this.ball_amount; i++)
        {
            Transform ball_clone = Instantiate(this.transform);
            ball_clone.GetComponent<BallControl>().scene_control = this.scene_control;
            ball_clone.GetComponent<BallControl> ().ball_amount = 0;
            ball_clone.GetComponent<BallControl> ().beginShoot(dir);
            ball_clone.GetComponent<BallControl> ().is_clone = true; ;
        }

        // 依照該方向射出球
        //Debug.Log(dir);
        dir.Normalize();
        this.GetComponent<Rigidbody2D> ().isKinematic = false;
        this.GetComponent<Rigidbody2D> ().velocity = dir * ball_magnitude;
        // 進入球準備碰撞階段
        this.next_step = STEP.READY;
    }

    public void readyToShoot()
    {
        this.next_step = STEP.CHARGE;
    }

    // 確認是否為複製的球
    public bool isClone()
    {
        return (this.is_clone);
    }

    // 確認是否還有複製的球存在
    public bool isAnyClone()
    {
        return (this.ball_amount != 0);
    }

    // 確認是否準備發球
    public bool isReadyToShoot ()
    {
        return (this.step == STEP.CHARGE);
    }

    // 確認是否遊玩中
    public bool isPlaying()
    {
        return (this.step == STEP.NORMAL);
    }

    // 重新準備發球
    public void revive ()
    {
        this.next_step = STEP.CHARGE;
    }

    // 設定球的動力學
    public void setKinematic(bool setting)
    {
        this.GetComponent<Rigidbody2D>().isKinematic = setting;

        //
        if(this.ball_amount > 0)
        {
            GameObject[] clone = GameObject.FindGameObjectsWithTag("Ball");

            for(int i = 0; i < clone.Length; i++)
            {
                if (clone[i].GetComponent<BallControl> ().is_clone)
                    clone[i].GetComponent<Rigidbody2D> ().isKinematic = setting;
            }
        }
    }

    // 改變球的位置
    public void setBallPosition (Vector3 pos)
    {
        this.transform.position = pos;
        this.transform.rotation = Quaternion.identity;
    }

    // 移動球的位置
    public void moveBallPosition (Vector3 v)
    {
        this.transform.position += v;
    }

    public float getBallPower()
    {
        return ball_power;
    }
}
