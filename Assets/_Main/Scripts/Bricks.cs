using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class Bricks : MonoBehaviour
{

    [SerializeField]
    protected GameObject brickPrefab;

    [SerializeField]
    protected float horizontalOffset = 0.95f;
    [SerializeField]
    protected float verticalOffset = 0.3f;
    [SerializeField]
    protected int leftCount = 6;
    [SerializeField]
    protected int rightCount = 6;
    [SerializeField]
    protected int linesCount = 2;

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
        float[] verticalValues = this.CreateVerticalValues();
        float[] horizontalValues = this.CreateHorizontalValues();
        foreach (float y in verticalValues)
        {
            foreach (float x in horizontalValues)
            {
                this.InstantiateBrick(new Vector3(x, y, 0f));
            }
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
        for (int i = 0; i < linesCount; i++)
        {
            verticalValues.Add(y);
            y -= verticalOffset;
        }
        return verticalValues.ToArray();
    }

    protected float[] CreateHorizontalValues()
    {
        List<float> horizontalValues = new List<float>();
        horizontalValues.Add(0f);
        float x;
        x = -horizontalOffset;
        for (int i = 0; i < leftCount; i++)
        {
            horizontalValues.Add(x);
            x -= horizontalOffset;
        }
        x = horizontalOffset;
        for (int i = 0; i < rightCount; i++)
        {
            horizontalValues.Add(x);
            x += horizontalOffset;
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

}
