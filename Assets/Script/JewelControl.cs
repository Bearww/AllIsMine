using UnityEngine;
using System.Collections;

public class JewelControl : MonoBehaviour {

    private SceneControl scene_control;

	// Use this for initialization
	void Start () {
        scene_control = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<SceneControl> ();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // 加分
            scene_control.player_jewels++;
            // 播放特效

            // 消除物件
            Destroy(this.gameObject);
        }
    }
}
