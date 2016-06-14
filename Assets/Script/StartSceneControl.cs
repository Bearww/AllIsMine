using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartSceneControl : MonoBehaviour
{

    // 進行状態.
    public enum STEP
    {

        NONE = -1,

        START = 0,
        TITLE,              // 標題顯示(等待輸入).
        WAIT_SE_END,            // 等待SE結束

        NUM,
    };

    private STEP step = STEP.NONE;
    private STEP next_step = STEP.NONE;
    private float step_timer = 0.0f;

    public AudioClip audio_clip;

    // -------------------------------------------------------------------------------- //

    void Start()
    {
        //Debug.Log(Application.streamingAssetsPath);
        Handheld.PlayFullScreenMovie("story.mp4", Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFill);

        this.next_step = STEP.TITLE;

        this.GetComponent<AudioSource>().clip = this.audio_clip;
    }

    void Update()
    {
        this.step_timer += Time.deltaTime;

        // 轉移至下一個狀態
        switch (this.step)
        {

            case STEP.TITLE:
                {
                    // 當滑鼠左鍵按下
                    //
                    if (Input.GetMouseButtonDown(0))
                    {
                        this.next_step = STEP.WAIT_SE_END;
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

                        SceneManager.LoadScene("TitleScene");
                    }
                }
                break;
        }

        // 下一狀態初始化

        if (this.next_step != STEP.NONE)
        {

            switch (this.next_step)
            {

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

        /*switch(this.step) {

			case STEP.TITLE:
			{
			}
			break;
		}*/

    }
}
