using UnityEngine;
using System.Collections;

public class BuildingMaterial : MonoBehaviour {

    public enum MATERIAL_TYPE
    {

        NONE = -1,

        WOOD = 0,
        STONE,
        GLASS,

        MAGENTA,
        GREEN,
        PINK,

        RED,            // 連鎖之後.
        GRAY,           // 連鎖之後.

        COIL,           // 金幣
        JEWEL,          // 寶石...

        NUM,

    };

    public static int NORMAL_COLOR_NUM = (int)MATERIAL_TYPE.RED;
    public static MATERIAL_TYPE NORMAL_MATERIAL_FIRST = MATERIAL_TYPE.WOOD;
    public static MATERIAL_TYPE NORMAL_COLOR_LAST = MATERIAL_TYPE.PINK;
    public static MATERIAL_TYPE CAKE_COLOR_FIRST = MATERIAL_TYPE.COIL;
    public static MATERIAL_TYPE CAKE_COLOR_LAST = MATERIAL_TYPE.JEWEL;

    public MATERIAL_TYPE material_type = (MATERIAL_TYPE)0;
    // 碰撞前的位置
    private Transform previous_transform;

    public static int NON_BALL_COLLISION = -1;

    // 被球碰撞的程度
    public float ball_collided = 0;

    // 不同耐久度的建材
    public Sprite[] sprites;

    // 目前建材的耐久
    public float current_durability;

    // 耐久最大值
    public float max_durability;

    // 位移最小誤差值
    public static float min_distance = 0.1f;

    // 旋轉最小誤差值
    public static float min_rotation = 1f;

    public float step_timer = 0.0f;

    private float start_time = 0f;

    void Start ()
    {
        this.current_durability = this.max_durability;
        this.previous_transform = this.transform;
    }

    void Update ()
    {
        this.step_timer += Time.deltaTime;
        if(Mathf.Abs(this.step_timer - this.start_time - this.ball_collided) < 0.1f) {
            this.GetComponent<Rigidbody2D>().isKinematic = true;
            this.ball_collided = NON_BALL_COLLISION;
        }
    }

	void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("[BuildingMaterial]Collision Enter");
        if (col.gameObject.GetComponent<Rigidbody2D> () == null)
            return;
        
        // 被建築碰撞時
        if (col.gameObject.CompareTag("Building"))
        {
            // 未包含BuildingMaterial Component
            if (col.gameObject.GetComponent<BuildingMaterial> () == null)
            {
                Debug.Log(col.gameObject);
                Debug.Log("Object needs BuildingMaterial Component");
                return;
            }

            // 非被球碰撞後產生的建築碰撞
            if (ball_collided == NON_BALL_COLLISION)
            {
                return;
            }

            // 延續碰撞，碰撞威力減弱
            if (col.gameObject.GetComponent<BuildingMaterial> ().ball_collided == NON_BALL_COLLISION)
                col.gameObject.GetComponent<BuildingMaterial> ().startCollision(this.ball_collided - 1);
            else
                col.gameObject.GetComponent<BuildingMaterial> ().setCollision(this.ball_collided - 1);

            /*
            if (Vector3.Distance(this.previous_transform.position, this.transform.position) < min_distance)
                this.transform.position = this.previous_transform.position;

            if (Vector3.Angle(this.previous_transform.eulerAngles, this.transform.eulerAngles) < min_rotation)
                this.transform.rotation = this.previous_transform.rotation;
            */
            this.current_durability -= 0.1f;
        }
        // 被球碰撞時
        else if(col.gameObject.CompareTag("Ball"))
        {
            if (col.gameObject.GetComponent<BallControl> () == null)
                return;

            // 被球撞到，碰撞產生
            startCollision(col.gameObject.GetComponent<BallControl> ().getBallPower());

            // 建材耐久降低
            this.current_durability--;
        }

        if (this.current_durability <= 0f)
            Destroy(this.gameObject);
        else
            updateSprite();
    }

    void updateSprite()
    {
        if (this.sprites.Length == 0)
            return;

        float interval = max_durability / this.sprites.Length;

        for(int i = 0; i < this.sprites.Length; i++)
        {
            if(this.current_durability >= max_durability - interval * (i + 1))
            {
                this.GetComponent<SpriteRenderer> ().sprite = this.sprites[i];
                return;
            }
        }
    }

    public void setMaterialType(MATERIAL_TYPE type)
    {
        this.material_type = type;

        switch (this.material_type)
        {

            case MATERIAL_TYPE.RED:
                {
                    
                }
                break;

            case MATERIAL_TYPE.GRAY:
                {
                    
                }
                break;

            case MATERIAL_TYPE.COIL:
                {
                    
                }
                break;

            default:
                {
                    
                }
                break;
        }
    }

    public void setVisible(bool is_visible)
    {
        this.GetComponent<SpriteRenderer> ().enabled = is_visible;
    }

    public bool isVisible()
    {
        return (this.GetComponent<SpriteRenderer> ().enabled);
    }

    public void startCollision(float duration)
    {
        this.ball_collided = duration;
        this.start_time = this.step_timer;
        this.GetComponent<Rigidbody2D> ().isKinematic = false;
    }

    public void setCollision(float duration)
    {
        // 依照最大的碰撞效果
        this.ball_collided = Mathf.Max(this.ball_collided, duration);
    }
}
