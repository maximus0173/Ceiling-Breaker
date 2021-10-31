using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILivesController : MonoBehaviour
{

    [SerializeField]
    protected GameObject livePrefab;

    public void OnLivesChanged()
    {
        this.RemoveAllChilds();
        int lives = MainManager.Instance.CurrentLives;
        for (int i = 0; i < lives; i++)
        {
            Instantiate(this.livePrefab, transform);
        }
    }

    protected void RemoveAllChilds()
    {
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

}
