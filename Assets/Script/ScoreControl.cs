using UnityEngine;
using System.Collections;

public class ScoreControl : MonoBehaviour
{

    public Vector3 position;
    public float digitOffset;
    public int digitNum;
    public bool drawZero;

    public bool isDecreasing;

    public GameObject score;
    private GameObject[] numTex;

    private int targetNum;
    private int currentNum;
    private float timer;
    public Sprite[] texture;
    public int layer = 0;

    public delegate void CallBack();
    public CallBack CallBackCountUp = null;

    // Use this for initialization
    void Start()
    {

        score = new GameObject("score");

        this.numTex = new GameObject[digitNum];
        for (int i = 0; i < digitNum; i++)
        {
            this.numTex[i] = new GameObject(i.ToString());
            this.numTex[i].AddComponent<SpriteRenderer> ();
            this.numTex[i].GetComponent<SpriteRenderer> ().sprite = texture[i];
            this.numTex[i].GetComponent<SpriteRenderer> ().sortingLayerName = "Foreground";
            this.numTex[i].GetComponent<SpriteRenderer> ().sortingOrder = 30;
            this.numTex[i].transform.parent = this.score.transform;
        }

        this.timer = 0.0f;
    }

    public void Update()
    {
        if (this.targetNum != this.currentNum)
        {
            this.timer += Time.deltaTime;

            if (timer > 0.05f)
            {
                this.timer = 0.0f;

                if (isDecreasing)
                {
                    // 加快數字顯示速度
                    if (this.currentNum - this.targetNum > 10)
                    {
                        this.currentNum -= 5;
                    }
                    else
                    {
                        this.currentNum--;
                    }
                }
                else
                {
                    // 加快數字顯示速度
                    if (this.targetNum - this.currentNum > 10)
                    {
                        this.currentNum += 5;
                    }
                    else
                    {
                        this.currentNum++;
                    }
                }
                if (CallBackCountUp != null)
                {
                    CallBackCountUp();
                }
            }
        }
    }

    public void OnGUI()
    {
        GUI.depth = layer;

        drawNum(currentNum, position.x, position.y, position.z, 1.0f);
    }

    private void drawNum(int num, float x, float y, float z, float scale)
    {
        // 數字表示
        string numStr = num.ToString();

        // anchor設在右邊
        float ofs = -this.digitOffset * this.digitNum;

        for (int i = 0; i < this.digitNum - numStr.Length; i++)
        {
            if (this.drawZero)
            {
                this.numTex[i].transform.position = new Vector3(x + ofs, y, z);
                this.numTex[i].transform.localScale = new Vector3(scale, scale, scale);
                this.numTex[i].GetComponent<SpriteRenderer> ().sprite = this.texture[0];
            }
            if (this.isDecreasing)
            {
                this.numTex[i].transform.position = new Vector3(x + ofs, y, z);
                this.numTex[i].transform.localScale = new Vector3(scale, scale, scale);
                this.numTex[i].GetComponent<SpriteRenderer> ().sprite = null;
            }
            ofs += this.digitOffset;
        }

        for (int i = 0, o = digitNum - numStr.Length; i < numStr.Length; i++)
        {
            // Number out of range
            if (o < 0)
            {
                Debug.Log("Number out of range");
                break;
            }

            int idx = int.Parse(numStr[i].ToString());
            this.numTex[i + o].transform.position = new Vector3(x + ofs, y, z);
            this.numTex[i + o].transform.localScale = new Vector3(scale, scale, scale);
            this.numTex[i + o].GetComponent<SpriteRenderer> ().sprite = this.texture[idx];
            ofs += this.digitOffset;
        }
    }

    // 顯示數字
    public void setNum(int num)
    {
        if (this.targetNum == this.currentNum)
        {
            this.timer = 0.0f;
        }
        this.targetNum = num;
    }

    // 將要顯示的數字
    public void setNumForce(int num)
    {
        this.targetNum = num;
        this.currentNum = num;
    }
}
