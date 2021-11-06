using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class Bricks : MonoBehaviour
{

    [System.Serializable]
    public struct LineDef
    {
        public List<int> positions;
    }

    [SerializeField]
    protected GameObject brickPrefab;

    [SerializeField]
    protected float horizontalOffset = 0.95f;
    [SerializeField]
    protected float verticalOffset = 0.3f;
    [SerializeField]
    protected List<LineDef> lines = new List<LineDef>();

    [SerializeField]
    [ListDrawerSettings()]
    protected List<Brick> bricks = new List<Brick>();

    protected bool isActive = true;

    public event System.Action OnAllBricksDestroyed;

    private void Start()
    {
        foreach (Brick brick in this.bricks)
        {
            brick.OnDestroy += HandleBrickDestroy;
        }
    }

    [Button(ButtonSizes.Small)]
    public void Build()
    {
        this.RemoveAllChilds();
        int lineIndex = 0;
        float[] verticalValues = this.CreateVerticalValues();
        foreach (float y in verticalValues)
        {
            float[] horizontalValues = this.CreateHorizontalValues(lineIndex);
            foreach (float x in horizontalValues)
            {
                this.InstantiateBrick(new Vector3(x, y, 0f));
            }
            lineIndex++;
        }
    }

    protected void RemoveAllChilds()
    {
        this.bricks.Clear();
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    protected float[] CreateVerticalValues()
    {
        List<float> verticalValues = new List<float>();
        float y;
        y = -verticalOffset;
        for (int i = 0; i < this.lines.Count; i++)
        {
            verticalValues.Add(y);
            y -= verticalOffset;
        }
        return verticalValues.ToArray();
    }

    protected float[] CreateHorizontalValues(int lineIndex)
    {
        List<float> horizontalValues = new List<float>();
        foreach (int position in this.lines[lineIndex].positions)
        {
            float x = (float)position * horizontalOffset;
            horizontalValues.Add(x);
        }
        return horizontalValues.ToArray();
    }

    protected void InstantiateBrick(Vector3 position)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(this.brickPrefab) as GameObject;
        go.transform.parent = this.transform;
        go.transform.localPosition = position;
        go.transform.localRotation = Quaternion.identity;
        go.layer = gameObject.layer;
        Brick brick = go.GetComponent<Brick>();
        this.bricks.Add(brick);
    }

    protected void HandleBrickDestroy(Brick brick)
    {
        if (!this.isActive)
            return;
        this.bricks.Remove(brick);
        if (this.bricks.Count == 0)
        {
            this.isActive = false;
            this.OnAllBricksDestroyed?.Invoke();
        }
    }

    public void AddHealing()
    {
        if (this.bricks.Count > 0)
        {
            int index = Random.Range(0, this.bricks.Count - 1);
            this.bricks[index].SetAsHealing();
        }
    }

}
