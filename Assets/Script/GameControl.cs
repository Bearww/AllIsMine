using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameControl : MonoBehaviour {

    public enum LEVEL
    {
        NONE = -1,

        BEGINNER = 0,
        COTTAGE,
        INDIA,
        JAPAN,
        TW101,

        NUM
    };

    public static int HIGHEST_GAME_LEVEL = (int)LEVEL.TW101;

    // 玩家遊玩階段
    public LEVEL player_level;

    // -------------------------------------------------------- //

    // 玩家遊玩場景
    public SCENE play_scene = SCENE.NONE;

    public Transform loading_scene;

    public Vector3 LOADING_DISPLAY = new Vector2(0, 0);

    public Vector3 LOADING_HIDDEN = new Vector2(12, 0);

    // -------------------------------------------------------- //

    void Start () {
        DontDestroyOnLoad(this);

        this.play_scene = SCENE.TITLE;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void loadScene(SCENE scene)
    {
        this.play_scene = scene;

        string scene_name = "";

        switch (scene)
        {
            case SCENE.STORE:
                scene_name = "StoreScene";
                break;
            case SCENE.COTTAGE:
                scene_name = "CottageScene";
                break;
            case SCENE.INDIA:
                scene_name = "IndiaScene";
                break;
            case SCENE.JAPAN:
                scene_name = "JapanScene";
                break;
            case SCENE.TW101:
                scene_name = "TW101";
                break;
            default:
                // 當場景沒有設定時
                Debug.LogWarning("Need add scene");
                return;
        }
        SceneManager.LoadScene(scene_name);
        //loadSceneAsync(scene_name);
    }

    IEnumerator loadSceneAsync(string scene)
    {

        //this.finish_scene.localPosition = LOADING_HIDDEN;
        //this.loading_scene.localPosition = LOADING_DISPLAY;
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        yield return async;

        //this.loading_scene.localPosition = LOADING_HIDDEN;
    }

    public SCENE getScene(string tag)
    {
        SCENE scene = SCENE.NONE;
        switch (tag)
        {
            case "Store":
                scene = SCENE.STORE;
                break;
            case "Cottage":
                scene = SCENE.COTTAGE;
                break;
            case "India":
                scene = SCENE.INDIA;
                break;
            case "Japan":
                scene = SCENE.JAPAN;
                break;
            case "101":
                scene = SCENE.TW101;
                break;
        }

        // 關卡限制
        if ((int)scene < (int)SCENE.TITLE && (int)scene > (int)this.player_level)
        { 
            scene = SCENE.NONE;
        }

        return scene;
    }

    public bool isBuildingScene(SCENE scene)
    {
        return (scene <= SCENE.TW101 && scene >= SCENE.COTTAGE);
    }

    public void upgradeLevel()
    {
        if((int)this.play_scene == (int)this.player_level)
        {
            this.player_level++;
        }
    }
}

public enum SCENE
{
    NONE = -1,

    // 關卡
    COTTAGE = 0,
    INDIA,
    JAPAN,
    TW101,

    // 遊戲功能畫面
    TITLE,
    STORE,
    TOY,
    COOK,

    NUM,
}