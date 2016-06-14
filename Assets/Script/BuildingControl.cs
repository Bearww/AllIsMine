using UnityEngine;
using System.Collections;

public class BuildingControl : MonoBehaviour {

    public SceneControl scene_control;

    public GUIControl gui_control;

    public float destroy_level = 0;

    public AudioClip[] audio_clips; 

    private float MAX_DESTROY_LEVEL = 100;

    void Start()
    {
        scene_control = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<SceneControl> ();

        gui_control = GameObject.FindGameObjectWithTag("GUIControl").GetComponent<GUIControl> ();
        loadBuildingLevel();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Building"))
            return;
        
        if (!other.GetComponent<BuildingMaterial> ())
            return;

        other.GetComponent<BuildingMaterial> ().ignoreControl();
    }

    public void loadBuildingLevel()
    {
        float level = 0;

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        foreach(GameObject building in buildings)
        {
            if(!building.GetComponent<BuildingMaterial> ())
            {
                Debug.Log(building);
                Debug.Log("Building no BuildingMaterial Component");
                continue;
            }
            level += building.GetComponent<BuildingMaterial> ().max_durability;
            building.GetComponent<BuildingMaterial> ().building_control = this;

            switch(building.GetComponent<BuildingMaterial> ().material_type)
            {
                case BuildingMaterial.MATERIAL_TYPE.WOOD:
                case BuildingMaterial.MATERIAL_TYPE.THATCH:
                    building.GetComponent<AudioSource>().clip = this.audio_clips[(int)BuildingMaterial.MATERIAL_TYPE.WOOD];
                    break;
                case BuildingMaterial.MATERIAL_TYPE.BRICK:
                case BuildingMaterial.MATERIAL_TYPE.STONE:
                    building.GetComponent<AudioSource>().clip = this.audio_clips[(int)BuildingMaterial.MATERIAL_TYPE.STONE];
                    break;
                case BuildingMaterial.MATERIAL_TYPE.GLASS:
                    building.GetComponent<AudioSource>().clip = this.audio_clips[(int)BuildingMaterial.MATERIAL_TYPE.GLASS];
                    break;
                case BuildingMaterial.MATERIAL_TYPE.PAPER:
                    break;
            }
        }

        this.MAX_DESTROY_LEVEL = level;
    }

    public bool isBroken()
    {
        return (Mathf.Abs(this.destroy_level - this.MAX_DESTROY_LEVEL) < 1.0f);
    }

    public void updateBuildingLevel(float level)
    {
        this.destroy_level += level;
        // Update UI
    }

    public void updatePlayerMoney(float money)
    {
        // Money Bonus
        this.scene_control.player_money += 
            Mathf.FloorToInt(money * (1.0f + money / this.MAX_DESTROY_LEVEL * 10.0f));
    }

    public int getDestroyPercent()
    {
        // 受損百分比
        return Mathf.CeilToInt((this.MAX_DESTROY_LEVEL - this.destroy_level) / this.MAX_DESTROY_LEVEL * 100);
    }
}
