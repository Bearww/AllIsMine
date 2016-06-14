using UnityEngine;
using System.Collections;

public class BarControl : MonoBehaviour {

    private Vector3 bar_position;
    private Vector3 bar_scale;

    public float bar_width;

    public bool is_increase;

    public int targetBar;
    private int currentBar;
    private float timer;
    private bool is_update_bar;
    private bool is_visible;

    public int layer = 0;

    public delegate void CallBack();
    public CallBack CallBackCountUp = null;

    // Use this for initialization
    void Start () {
        this.bar_position = this.transform.position;
        this.bar_scale = this.transform.localScale;

        if(is_increase)
        {
            currentBar = targetBar = 0;
        }
        else
        {
            currentBar = targetBar = 100;
        }
        is_update_bar = true;
        is_visible = true;
        this.timer = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (this.targetBar != this.currentBar)
        {
            this.timer += Time.deltaTime;

            if (timer > 0.05f)
            {
                this.timer = 0.0f;

                if (is_increase) {
                    // 加快Bar變化速度
                    if (this.targetBar - this.currentBar > 10)
                    {
                        this.currentBar += 5;
                    }
                    else
                    {
                        this.currentBar++;
                    }
                }
                else
                {
                    if (this.currentBar - this.targetBar > 10)
                    {
                        this.currentBar -= 5;
                    }
                    else
                    {
                        this.currentBar--;
                    }
                }
                is_update_bar = true;

                if (CallBackCountUp != null)
                {
                    CallBackCountUp();
                }
            }
        }
    }

    
    public void OnGUI()
    {
        //Debug.Log("OnGUI() call");
        GUI.depth = layer;

        drawBar();
    }
    

    private void drawBar()
    {
        if (is_update_bar)
        {
            Vector3 p, s;

            // 計算位置
            p = this.bar_position;
            p.x -= (float)(100 - this.currentBar) / 100.0f * this.bar_width / 2.0f;

            // 計算長度
            s = this.bar_scale;
            s *= (float)this.currentBar / 100.0f;
            s.y = s.z = 1.0f;

            this.transform.position = p;
            this.transform.localScale = s;

            is_update_bar = false;
        }
    }

    // 設定數字
    public void setValue(int value)
    {
        if (this.targetBar == this.currentBar)
        {
            this.timer = 0.0f;
        }
        this.targetBar = value;
    }

    // 將要顯示的數字
    public void setValueForce(int value)
    {
        this.targetBar = value;
        this.currentBar = value;
    }

    public void isVisible(bool visible)
    {
        if (this.is_visible != visible)
        {
            this.is_visible = visible;
            this.GetComponent<SpriteRenderer>().enabled = visible;
        }
    }
}
