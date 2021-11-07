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
        else if (other.CompareTag("Brick") || other.CompareTag("Ceiling"))
        {
            Destroy(other.gameObject, 2f);
        }
        else if (other.CompareTag("Healing"))
        {
            Destroy(other.gameObject);
        }
    }

}
