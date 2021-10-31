using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{

    protected bool isActive = true;

    private void OnCollisionEnter(Collision other)
    {
        if (!this.isActive)
            return;
        MainManager.Instance.AddPoint(1);
        
        //slight delay to be sure the ball have time to bounce
        Destroy(gameObject, 0.2f);
        this.isActive = false;
    }

}
