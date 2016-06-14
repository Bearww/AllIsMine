using UnityEngine;
using System.Collections;

public class PillarControl : MonoBehaviour {

    // 被支撐建材
    public Transform[] support_buildings;

    // 被支撐建材的間距
    private Vector3[] building_offset;

    public static float MIN_DISTANCE = 0.001f;

    public static float MAX_DISTANCE = 0.005f;

    public static float COLLISION_DURATION = 3f;

    void Start()
    {
        if(support_buildings.Length > 0)
            building_offset = new Vector3[support_buildings.Length];

        for (int i = 0; i < building_offset.Length; i++)
            building_offset[i] = this.transform.position - support_buildings[i].position;
    }

    public void updateBuilding()
    {
        // 建材與支柱距離
        for(int i = 0; i < building_offset.Length; i++)
        {
            // 小於最小誤差值
            //Debug.Log(Vector3.Distance(building_offset[i], new Vector3()));
            if (Vector3.Distance(building_offset[i], new Vector3()) < MIN_DISTANCE)
                continue;

            // 物件已摧毀
            if (!support_buildings[i])
                continue;

            // 超過最大誤差值
            //Debug.Log(Vector3.Distance(this.transform.position - building_offset[i], support_buildings[i].position));
            if (Vector3.Distance(this.transform.position - building_offset[i], support_buildings[i].position) > MAX_DISTANCE)
            {
                if (!support_buildings[i].GetComponent<BuildingMaterial> ())
                    continue;

                // 啟用建築碰撞
                support_buildings[i].GetComponent<BuildingMaterial> ().startCollision(COLLISION_DURATION);

                // 不再更新此建築
                building_offset[i] = new Vector3();
            }
        }
    }
}
