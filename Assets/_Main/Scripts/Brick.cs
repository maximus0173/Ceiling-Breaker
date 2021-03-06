using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{

    [SerializeField]
    protected GameObject solid;

    [SerializeField]
    protected GameObject fractured;

    [SerializeField]
    protected AudioSource audioExplosion;

    [SerializeField]
    protected bool isHealing = true;

    [SerializeField]
    protected GameObject healingPrefab;

    public event System.Action<Brick> OnDestroy;

    protected bool isActive = true;

    private void OnCollisionEnter(Collision other)
    {
        if (!this.isActive)
            return;

        if (other.gameObject.CompareTag("Ball"))
        {
            this.solid.SetActive(false);
            this.GetComponent<BoxCollider>().enabled = false;
            this.fractured.SetActive(true);
            for (int i = 0; i < this.fractured.transform.childCount; i++)
            {
                this.fractured.transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(1000f, transform.position, 0.5f);
            }

            OnDestroy?.Invoke(this);
            MainManager.Instance.AddPoint(1);

            this.audioExplosion.pitch = Random.Range(0.9f, 1.2f);
            this.audioExplosion.Play();

            this.isActive = false;

            if (this.isHealing)
            {
                Instantiate(this.healingPrefab, this.transform);
            }
        }
    }

    public void SetAsHealing()
    {
        this.isHealing = true;
    }

}
