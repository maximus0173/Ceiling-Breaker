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
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    protected float[] CreateVerticalValues()
    {
        List<float> verticalValues = new List<float>();
        float y;
        y = 0f;
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
    }

}
