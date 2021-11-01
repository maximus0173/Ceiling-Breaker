using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;
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
    protected float ceilingHeight = 0.0749971f;

    [SerializeField]
    protected Ceiling ceilingPrefab;

    [SerializeField]
    protected Bricks bricksPrefab;

    [SerializeField]
    protected DeathZone deathZonePrefab;

    [SerializeField]
    protected Light firstPlanLightPrefab;

    [SerializeField]
    protected CinemachineVirtualCamera virtCameraPrefab;

    [SerializeField]
    protected int floors = 10;

    [SerializeField]
    protected MainManager mainManager;

    [Button(ButtonSizes.Small)]
    public void Build()
    {
        this.RemoveAllChilds();
        List<LevelManager> levels = new List<LevelManager>();
        Vector3 offset = Vector3.zero;
        for (int floor = 0; floor < this.floors; floor++)
        {
            Vector3 levelBasePosition = offset;

            GameObject floorObject = new GameObject("Floor " + (floor + 1));
            floorObject.transform.parent = transform;
            LevelManager levelManager = floorObject.AddComponent<LevelManager>();

            GameObject wallsObject = new GameObject("Walls");
            wallsObject.transform.parent = floorObject.transform;
            for (int floorSegment = 0; floorSegment < this.floorSegments; floorSegment++)
            {
                WallDef[] availableWallDefs = this.GetWallDefsByFloorSegmentAndProbability(floorSegment); 
                foreach (WallPositionDef wallPositionDef in this.wallPositionDefs)
                {
                    int prefabIndex = Mathf.RoundToInt(Random.Range(0, availableWallDefs.Length));
                    this.InstantiateWall(wallsObject, availableWallDefs[prefabIndex], wallPositionDef, offset);
                }
                offset += Vector3.up * this.segmentHeight;
            }

            Bricks bricks = this.InstantiateBricks(floorObject, levelBasePosition);
            DeathZone deathZone = this.InstantiateDeathZone(floorObject, levelBasePosition);
            Light firstPlanLight = this.InstantiateFirstPlanLight(floorObject, levelBasePosition);
            Ceiling ceiling = this.InstantiateCeiling(floorObject, offset);
            CinemachineVirtualCamera virtCamera = this.CreateCamera(floorObject, levelBasePosition);
            levelManager.Init(bricks, ceiling, firstPlanLight, virtCamera);
            levels.Add(levelManager);

            offset += Vector3.up * this.ceilingHeight;
        }
        this.mainManager.SetLevels(levels);
    }

    protected void RemoveAllChilds()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    protected void InstantiateWall(GameObject parent, WallDef wallDef, WallPositionDef wallPositionDef, Vector3 offset)
    {
        GameObject prefab = wallDef.prefab;
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        go.transform.position = offset + wallPositionDef.position;
        go.transform.rotation = Quaternion.Euler(wallPositionDef.rotation);
        go.transform.parent = parent.transform;
    }

    protected Bricks InstantiateBricks(GameObject parent, Vector3 offset)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(this.bricksPrefab.gameObject) as GameObject;
        go.transform.position += offset;
        go.transform.rotation = Quaternion.identity;
        go.transform.parent = parent.transform;
        return go.GetComponent<Bricks>();
    }

    protected DeathZone InstantiateDeathZone(GameObject parent, Vector3 offset)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(this.deathZonePrefab.gameObject) as GameObject;
        go.transform.position += offset;
        go.transform.rotation = Quaternion.identity;
        go.transform.parent = parent.transform;
        return go.GetComponent<DeathZone>();
    }

    protected Light InstantiateFirstPlanLight(GameObject parent, Vector3 offset)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(this.firstPlanLightPrefab.gameObject) as GameObject;
        go.transform.position += offset;
        go.transform.rotation = Quaternion.identity;
        go.transform.parent = parent.transform;
        return go.GetComponent<Light>();
    }

    protected Ceiling InstantiateCeiling(GameObject parent, Vector3 offset)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(this.ceilingPrefab.gameObject) as GameObject;
        go.transform.position = offset;
        go.transform.rotation = Quaternion.identity;
        go.transform.parent = parent.transform;
        return go.GetComponent<Ceiling>();
    }

    protected CinemachineVirtualCamera CreateCamera(GameObject parent, Vector3 offset)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(this.virtCameraPrefab.gameObject) as GameObject;
        go.transform.parent = parent.transform;
        go.transform.position += offset;
        CinemachineVirtualCamera virtCamera = go.GetComponent<CinemachineVirtualCamera>();
        return virtCamera;
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
