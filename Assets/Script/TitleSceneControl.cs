using UnityEngine;
using System.Collections;

public class TitleSceneControl : MonoBehaviour {

    private GameControl game_control = null;

    private BeginnerGuideControl guide_control = null;

    private PlayerItemControl pitem_control = null;

    private ScoreBoardControl board_control = null;

    private ChooseSceneControl scene_control = null;

    //

    public SpriteRenderer[] map_sprite;

    public SpriteRenderer[] map_level;

    public static float DARK_GRAY = 51.0f / 255.0f;

    public static Color BUILDING_DISPLAY = new Color(1, 1, 1);

    public static Color BUILDING_HIDDEN = new Color(DARK_GRAY, DARK_GRAY, DARK_GRAY);

    public static Color STAR_SHOW = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    public static Color STAR_HIDDEN = new Color(.0f, .0f, .0f, .0f);

    public static Vector3 SCENE_DISPALY = new Vector2();

    public static Vector3 SCENE_HIDDEN = new Vector2(-6.0f, 0.0f);

    // 進行狀態
    public enum STEP {

		NONE = -1,

		TITLE = 0,				// 標題顯示(等待輸入)
        CHOOSE,                 // 選擇物品與道具
        WAIT_CHOOSE_END,        // 選擇物品與道具結束

		WAIT_SE_END,			// 等待SE結束

		NUM,
	};

	private STEP step = STEP.NONE;
	private STEP next_step = STEP.NONE;
	private float step_timer = 0.0f;

    private SCENE to_scene;

    public bool is_disp_choose = false;
    public bool is_choosed = false;

	public AudioClip audio_clip;

	// -------------------------------------------------------------------------------- //

	void Start () {
        this.game_control = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl> ();

        this.guide_control = GameObject.FindGameObjectWithTag("BeginnerGuideControl").GetComponent<BeginnerGuideControl> ();

        this.pitem_control = GameObject.FindGameObjectWithTag("PlayerItemControl").GetComponent<PlayerItemControl> ();

        this.board_control = GameObject.FindGameObjectWithTag("ScoreBoardControl").GetComponent<ScoreBoardControl> ();

        this.scene_control = GameObject.FindGameObjectWithTag("ChooseSceneControl").GetComponent<ChooseSceneControl> ();
        this.scene_control.create();

        this.next_step = STEP.TITLE;
        this.to_scene = SCENE.NONE;

		this.GetComponent<AudioSource>().clip = this.audio_clip;

        this.is_disp_choose = false;
        this.is_choosed = false;

        if(this.guide_control.is_beginner_guide)
        {
            this.guide_control.startGuide();
            this.guide_control.is_display = true;
        }

        if(this.guide_control.isShopGuide())
        {
            this.guide_control.displayWait();
        }
	}

	void Update ()
	{
        this.step_timer += Time.deltaTime;

		// 轉移至下一個狀態
		switch(this.step) {

			case STEP.TITLE:
			{
				// 當滑鼠左鍵按下
				//
				if(Input.GetMouseButtonDown(0)) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10.0f, LayerMask.NameToLayer("BeginnerGuide"));
                    
                    if (hit)
                    {
                        //Debug.Log(hit.transform);
                        this.to_scene = this.game_control.getScene(hit.transform.tag);
                        
                        // 引導
                        if(this.guide_control.is_beginner_guide)
                        {
                            // 指引進入道具畫面
                            if(this.game_control.isBuildingScene(this.to_scene))
                            {
                                if(this.guide_control.isMainGuide())
                                {
                                    this.guide_control.goalAct();
                                }
                                else
                                {
                                    this.to_scene = SCENE.NONE;
                                }
                            }

                            // 指引進入商店畫面
                            else
                            {
                                if(this.guide_control.isShopGuide())
                                {
                                    this.guide_control.goalAct();
                                }
                                else
                                {
                                    this.to_scene = SCENE.NONE;
                                }
                            }
                        }

                        if (this.to_scene != SCENE.NONE)
                        {
                            this.next_step = STEP.WAIT_SE_END;
                        }
                    }
				}
			}
			break;

			case STEP.WAIT_SE_END:
			{
				// SE完成時，讀取遊戲場景

				bool to_finish = true;

				do {

					if(!this.GetComponent<AudioSource>().isPlaying) {
						break;
					}

					if(this.GetComponent<AudioSource>().time >= this.GetComponent<AudioSource>().clip.length) {
						break;
					}

					to_finish = false;
				} while(false);

				if(to_finish)
                {
                    do {
                        // 正在選擇
                        if(this.is_disp_choose)
                        {
                            // 選擇完畢
                            if(this.is_choosed)
                            {
                                this.game_control.loadScene(this.to_scene);
                            }
                            else
                            {
                                this.next_step = STEP.WAIT_CHOOSE_END;
                            }
                            break;
                        }

                        if(this.to_scene == SCENE.NONE)
                        {
                            this.next_step = STEP.TITLE;
                            break;
                        }

                        // 進入建築畫面
                        if(this.game_control.isBuildingScene(this.to_scene))
                        {
                            Debug.Log("選擇道具");
                            this.next_step = STEP.CHOOSE;
                        }
                        else
                        {
                            this.game_control.loadScene(this.to_scene);
                        }
                    } while(false);
                }
			}
			break;
		}

		// 下一狀態初始化

		if(this.next_step != STEP.NONE) {

			switch(this.next_step) {

                case STEP.TITLE:
                {
                    this.is_disp_choose = false;
                    this.board_control.canvas.enabled = false;
                }
                break;

                case STEP.CHOOSE:
                {
                    this.scene_control.start();
                }
                break;

				case STEP.WAIT_SE_END:
				{
					// 開始播放SE
					this.GetComponent<AudioSource>().Play();
				}
				break;
			}

			this.step = this.next_step;
			this.next_step = STEP.NONE;

			this.step_timer = 0.0f;
		}
        
		// 各個狀態的處理

		switch(this.step) {

			case STEP.TITLE:
			{
                for(int i = 0; i < 4; i++)
                {
                    // 顯示建築
                    if((int)this.game_control.player_level >= i)
                    {
                        this.map_sprite[i].color = BUILDING_DISPLAY;
                    }
                    else
                    {
                        this.map_sprite[i].color = BUILDING_HIDDEN;
                    }

                    // 顯示星星
                    if(this.board_control.isPlayed(i))
                    {
                        this.map_level[i].color = STAR_SHOW;
                    }
                    else
                    {
                        this.map_level[i].color = STAR_HIDDEN;
                    }
                }
            }
			break;

            case STEP.CHOOSE:
            case STEP.WAIT_CHOOSE_END:
            {
                this.scene_control.execute();
            }
            break;
		}
        
	}

    void OnGUI()
    {
        if(this.is_disp_choose)
        {
            this.transform.position = SCENE_HIDDEN;
        }
        else
        {
            this.transform.position = SCENE_DISPALY;
        }
    }

    public void waitSE()
    {
        this.next_step = STEP.WAIT_SE_END;
    }

    public void backToTitle()
    {
        this.scene_control.hidden();

        this.is_disp_choose = false;
        this.to_scene = SCENE.NONE;
        this.next_step = STEP.WAIT_SE_END;
    }

    /*
    private void loadOnAsset(TextAsset asset)
    {
        if (asset != null)
        {
            string txtMapData = asset.text;

            // 刪除空的元素
            System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;

            // 切割換行符號
            string[] lines = txtMapData.Split(new char[] { '\r', '\n' }, option);

            // "," 切出符號
            char[] spliter = new char[1] { ',' };
        }
        else
        {
            Debug.LogWarning("Data asset is null");
        }
    }
    */
}
