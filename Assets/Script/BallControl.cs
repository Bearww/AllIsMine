using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

    public float ball_magnitude = 0f;

    // 球撞擊碰撞持續時間
    public float ball_power = 0f;

    public enum STEP
    {

        NONE = -1,

        NORMAL = 0,         // 球已發出
        READY,              // 等待發球

        NUM,
    };

    public STEP step;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;

    void Start () {
	}
	

	void Update () {
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
                        //this.GetComponent<Rigidbody2D> ().isKinematic = false;
                    }
                    break;
                case STEP.READY:
                    {
                        this.GetComponent<Rigidbody2D> ().isKinematic = true;
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
            case STEP.READY:
                {

                }
                break;
        }
    }

    // 當碰撞發生時
    void OnCollisionEnter2D (Collision2D col)
    {
        if(col.gameObject.CompareTag("Jewel"))
        {
            // 加分

            // 播放特效

            // 消除物件
            Destroy(col.gameObject);
        }
    }

    // 當碰撞完，速度變回原本的速度
    void OnCollisionExit2D (Collision2D col)
    {
        Vector2 dir = this.GetComponent<Rigidbody2D> ().velocity;
        dir.Normalize();
        this.GetComponent<Rigidbody2D> ().velocity = dir * ball_magnitude;
    }

    public void beginShoot (Vector3 dir)
    {
        if(dir.x == 0 && dir.y == 0)
        {
            dir = new Vector3(0, 1);
        }
        // 依照該方向射出球
        Debug.Log(dir);
        this.GetComponent<Rigidbody2D> ().isKinematic = false;
        this.GetComponent<Rigidbody2D> ().velocity = dir * ball_magnitude;
        this.next_step = STEP.NORMAL;
    }

    public void readyToShoot()
    {
        this.next_step = STEP.READY;
    }

    // 確認是否準備發球
    public bool isReadyToShoot ()
    {
        return (this.step == STEP.READY);
    }

    // 重新準備發球
    public void revive ()
    {
        this.next_step = STEP.READY;
    }

    // 設定球的動力學
    public void setKinematic(bool setting)
    {
        this.GetComponent<Rigidbody2D>().isKinematic = setting;
    }

    // 改變球的位置
    public void setBallPosition (Vector3 pos)
    {
        this.transform.position = pos;
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
