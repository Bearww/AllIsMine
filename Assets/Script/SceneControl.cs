using UnityEngine;
using System.Collections;

public class SceneControl : MonoBehaviour
{

    public GameObject StackBlockPrefab = null;

    public PlayerControl player_control = null;

    public BallControl ball_control = null;

    public float slider_value = 0.5f;

    // 各色のマテリアル（Blockl.cs）.
    //
    // ・実体を一個にしたい.
    // ・Inspector で変更できるようにしたい
    //
    // ので、インスタンスがひとつしか作られない、SceneControl に持たせています.
    //
    public Material[] block_materials;


    // ---------------------------------------------------------------- //

    public int height_level = 0;

    public static int MAX_HEIGHT_LEVEL = 50;

    public int player_stock;                // プレイヤーの残り.

    // ---------------------------------------------------------------- //

    public enum STEP
    {

        NONE = -1,

        PLAY = 0,           // 遊戲中
        GOAL_ACT,           // 目標產生
        MISS,               // 接球失誤

        GAMEOVER,           // ゲームオーバー

        NUM,
    };

    public STEP step;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;


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

    // ---------------------------------------------------------------- //

    void Start()
    {
        player_control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        player_control.carryBall();

        ball_control = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControl> ();
        ball_control.readyToShoot();

        // Ignore buildings and buildings
        //Physics2D.IgnoreLayerCollision(8, 8);
        // Ignore buildings and base
        //Physics2D.IgnoreLayerCollision(8, 10);
        // Ignore buildings and player
        Physics2D.IgnoreLayerCollision(8, 12);
        // Ignore buildings and border
        Physics2D.IgnoreLayerCollision(8, 13);
        // Igonre buildings and player(obstacle)
        Physics2D.IgnoreLayerCollision(8, 14);
        // Ignore obstacles and ball
        Physics2D.IgnoreLayerCollision(9, 11);
        // Ignore obstacles and player(touch)
        Physics2D.IgnoreLayerCollision(9, 12);
        // Ignore obstacles and border
        Physics2D.IgnoreLayerCollision(9, 13);
        // Igonre obstacles and player(obstacle)
        Physics2D.IgnoreLayerCollision(9, 14);
        // Ignore base and ball
        Physics2D.IgnoreLayerCollision(10, 11);
        // Ignore ball and player(touch)
        Physics2D.IgnoreLayerCollision(11, 12);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Building"))
        {
            // 噴錢

            // 清除物件
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("Ball"))
        {
            // 減少次數

            // life - 1

            // 將球設在手的位置
            ball_control.setBallPosition(player_control.getBallPosition());
            // 重新發球
            ball_control.revive();
            player_control.carryBall();
        }
    }
}
