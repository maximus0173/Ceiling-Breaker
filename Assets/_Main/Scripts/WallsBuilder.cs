using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class WallsBuilder : MonoBehaviour
{

    [System.Serializable]
    [InlineProperty(LabelWidth = 70)]
    public struct WallPositionDef
    {
        public Vector3 position;
        public Vector3 rotation;
    }

    [System.Serializable]
    [InlineProperty(LabelWidth = 100)]
    public struct WallDef
    {
        public GameObject prefab;
        [MinMaxSlider(0, 100, true)]
        public Vector2 segmentsRange;
        public int probability;
    }

    [SerializeField]
    [ListDrawerSettings()]
    protected List<WallPositionDef> wallPositionDefs = new List<WallPositionDef>();

    [SerializeField]
    [ListDrawerSettings(NumberOfItemsPerPage = 10)]
    protected List<WallDef> wallDefs = new List<WallDef>();

    [SerializeField]
    protected float segmentHeight = 3f;

    [SerializeField]
    protected int floorSegments = 5;

    [SerializeField]
    protected int floors = 10;

    [Button(ButtonSizes.Small)]
    public void Build()
    {
        this.RemoveAllChilds();
        Vector3 offset = Vector3.zero;
        for (int floor = 0; floor < this.floors; floor++)
        {
            for (int floorSegment = 0; floorSegment < this.floorSegments; floorSegment++)
            {
                WallDef[] availableWallDefs = this.GetWallDefsByFloorSegmentAndProbability(floorSegment); 
                foreach (WallPositionDef wallPositionDef in this.wallPositionDefs)
                {
                    int prefabIndex = Mathf.RoundToInt(Random.Range(0, availableWallDefs.Length));
                    this.InstantiateWall(availableWallDefs[prefabIndex], wallPositionDef, offset);
                }
                offset += Vector3.up * this.segmentHeight;
            }
        }
    }

    protected void RemoveAllChilds()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    protected void InstantiateWall(WallDef wallDef, WallPositionDef wallPositionDef, Vector3 offset)
    {
        GameObject prefab = wallDef.prefab;
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        go.transform.position = offset + wallPositionDef.position;
        go.transform.rotation = Quaternion.Euler(wallPositionDef.rotation);
        go.transform.parent = this.transform;
    }

    protected WallDef[] GetWallDefsByFloorSegmentAndProbability(int segment)
    {
        List<WallDef> segmentWallDefs = this.wallDefs.FindAll(
            wallDef => segment >= wallDef.segmentsRange.x && segment <= wallDef.segmentsRange.y
        );
        List<WallDef> availableWallDefs = new List<WallDef>();
        foreach (WallDef wallDef in segmentWallDefs)
        {
            for (int i = 0; i < wallDef.probability; i++)
            {
                availableWallDefs.Add(wallDef);
            }
        }
        return availableWallDefs.ToArray();
    }

}
