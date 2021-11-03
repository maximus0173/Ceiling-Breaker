using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ceiling : MonoBehaviour
{

    [SerializeField]
    protected GameObject solid;

    [SerializeField]
    protected GameObject lamps;

    [SerializeField]
    protected GameObject fractured;

    [SerializeField]
    protected AudioSource audioBreaking;

    public void DestroyCeiling()
    {
        this.solid.SetActive(false);
        this.lamps.SetActive(false);
        this.fractured.SetActive(true);
        this.audioBreaking.Play();
    }

}
