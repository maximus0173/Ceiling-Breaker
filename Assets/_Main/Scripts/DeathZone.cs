using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    [SerializeField]
    protected AudioSource audioLost;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(other.gameObject);
            MainManager.Instance.BallLost();
            this.audioLost.Play();
        }
    }

}
