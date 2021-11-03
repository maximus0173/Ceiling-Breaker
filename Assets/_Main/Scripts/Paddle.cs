using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    protected float speed = 2.0f;
    [SerializeField]
    protected float maxMovement = 2.0f;

    [SerializeField]
    protected AudioSource audioBallBounce;

    protected float vSpeed = 0f;
    protected Vector3 lastPosition;
    
    void Update()
    {
        float input = Input.GetAxis("Horizontal");

        Vector3 pos = transform.position;
        pos.x += input * speed * Time.deltaTime;

        if (pos.x > maxMovement)
        {
            pos.x = maxMovement;
        }
        else if (pos.x < -maxMovement)
        {
            pos.x = -maxMovement;
        }

        transform.position = pos;

        this.vSpeed = (transform.position.x - this.lastPosition.x) / Time.deltaTime;

        this.lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            Ball ball = collision.collider.GetComponent<Ball>();
            ball.AddHorizontalForce(this.vSpeed);
            this.audioBallBounce.Play();
        }
    }

    public void DestroyPaddle()
    {
        Destroy(this.gameObject, 0.2f);
    }
}
