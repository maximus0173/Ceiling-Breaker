using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{

    public event System.Action<Brick> OnDestroy;

    protected bool isActive = true;

    private void OnCollisionEnter(Collision other)
    {
        if (!this.isActive)
            return;

        OnDestroy?.Invoke(this);
        MainManager.Instance.AddPoint(1);
        
        //slight delay to be sure the ball have time to bounce
        Destroy(gameObject, 0.2f);
        this.isActive = false;
    }

}
