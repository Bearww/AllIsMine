using UnityEngine;
using System.Collections;

public class BuildingMaterial : MonoBehaviour {

    public enum MATERIAL_TYPE
    {

        NONE = -1,

        WOOD = 0,
        STONE,
        GLASS,
        BRICK,
        PAPER,
        THATCH,

        MAGENTA,
        GREEN,
        PINK,

        RED,            // 連鎖之後.
        GRAY,           // 連鎖之後.

        COIL,           // 金幣
        JEWEL,          // 寶石...

        NUM,

    };

    public MATERIAL_TYPE material_type = (MATERIAL_TYPE)0;

    public BuildingControl building_control;

    // 碰撞前的位置
    private Transform previous_transform;

    public static int NON_BALL_COLLISION = -1;

    // 碰撞威力減弱速度
    public static float COLLIED_REDUCE_RATE = 1.5f;

    // 被球碰撞的程度
    public float ball_collided = 0;

    // 不同耐久度的建材
    public Sprite[] sprites;

    // 目前建材的耐久
    private float current_durability;

    // 耐久最大值
    public float max_durability;

    // 位移最小誤差值
    public static float min_distance = 0.1f;

    // 旋轉最小誤差值
    public static float min_rotation = 1f;

    public float step_timer = 0.0f;

    private float start_time = 0f;

    private bool is_ignore = false;

    void Start ()
    {
        // 取得建築重量
        getBuildingMass();
        this.current_durability = this.max_durability;
        this.previous_transform = this.transform;
    }

    void Update ()
    {
        this.step_timer += Time.deltaTime;
        if (is_ignore)
        {
            return;
        }

        if (Mathf.Abs(this.step_timer - this.start_time - this.ball_collided) < 0.1f)
        {
            this.GetComponent<Rigidbody2D> ().isKinematic = true;
            this.ball_collided = NON_BALL_COLLISION;
        }

        if (!this.GetComponent<Rigidbody2D> ().isKinematic)
        {
            // 取得支柱控制
            if (!this.GetComponent<PillarControl> ())
                return;

            // 更新被支撐的建築
            this.GetComponent<PillarControl> ().updateBuilding();
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
                col.gameObject.GetComponent<BuildingMaterial> ().startCollision(this.ball_collided - COLLIED_REDUCE_RATE);
            else
                col.gameObject.GetComponent<BuildingMaterial> ().setCollision(this.ball_collided - COLLIED_REDUCE_RATE);

            /*
            // 碰撞最小誤差設定
            if (Vector3.Distance(this.previous_transform.position, this.transform.position) < min_distance)
                this.transform.position = this.previous_transform.position;

            if (Vector3.Angle(this.previous_transform.eulerAngles, this.transform.eulerAngles) < min_rotation)
                this.transform.rotation = this.previous_transform.rotation;
            */
            destroyLevel(0.1f);
        }
        // 被球碰撞時
        else if(col.gameObject.CompareTag("Ball"))
        {
            if (col.gameObject.GetComponent<BallControl> () == null)
                return;

            // 被球撞到，碰撞產生
            startCollision(col.gameObject.GetComponent<BallControl> ().getBallPower());

            // 播放音樂
            this.GetComponent<AudioSource> ().Play();

            // 建材耐久降低
            destroyLevel(1);
        }
    }

    void getBuildingMass()
    {
        float mass = 1.0f;
        switch(this.material_type)
        {
            case MATERIAL_TYPE.WOOD:
                mass = 1.5f;
                break;
            case MATERIAL_TYPE.STONE:
                mass = 3.0f;
                break;
            case MATERIAL_TYPE.GLASS:
                mass = 0.8f;
                break;
            case MATERIAL_TYPE.BRICK:
                mass = 2.0f;
                break;
            case MATERIAL_TYPE.PAPER:
                mass = 0.6f;
                break;
            case MATERIAL_TYPE.THATCH:
                mass = 0.8f;
                break;
        }

        this.GetComponent<Rigidbody2D> ().mass = mass;
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

    public void destroyLevel(float level)
    {
        if (level >= this.current_durability)
            level = this.current_durability;

        // BuildingControl destroyed level
        this.building_control.updateBuildingLevel(level);
        this.building_control.updatePlayerMoney(level);

        this.current_durability -= level;
        if (this.current_durability < 0.001f)
            destroyMaterial();
        else
            updateSprite();
    }

    public void destroyBuilding()
    {
        //Debug.Log("建材掉落");
        destroyLevel(this.current_durability);
    }

    private void destroyMaterial()
    {
        // 噴錢
        Debug.Log("噴錢囉");

        // 清除物件
        Destroy(this.gameObject);
    }

    public void setVisible(bool is_visible)
    {
        this.GetComponent<SpriteRenderer> ().enabled = is_visible;
    }

    public bool isVisible()
    {
        return (this.GetComponent<SpriteRenderer> ().enabled);
    }

    public void ignoreControl()
    {
        // 建築持續移動
        this.is_ignore = true;
        this.GetComponent<Rigidbody2D> ().isKinematic = false;
        this.start_time = 0;
    }

    public void startCollision(float duration)
    {
        if (duration <= 0) return;

        // 設定碰撞持續時間
        this.ball_collided = duration;
        this.start_time = this.step_timer;
        this.GetComponent<Rigidbody2D> ().isKinematic = false;
    }

    public void setCollision(float duration)
    {
        if (duration <= 0) return;

        // 依照最大的碰撞效果
        //this.ball_collided = Mathf.Max(this.ball_collided, duration);
        this.ball_collided = duration;
        this.start_time = this.step_timer;
    }
}
