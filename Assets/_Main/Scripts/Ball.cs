using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public float MaxSpeed { get => this.maxSpeed; }

    [SerializeField]
    protected float minSpeed = 4f;

    [SerializeField]
    protected float maxSpeed = 6f;

    private Rigidbody rb;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }
    
    private void OnCollisionExit(Collision other)
    {
        var velocity = this.rb.velocity;
        
        //after a collision we accelerate a bit
        velocity += velocity.normalized * 0.01f;
        
        //check if we are not going totally vertically as this would lead to being stuck, we add a little vertical force
        if (Vector3.Dot(velocity.normalized, Vector3.up) < 0.1f)
        {
            velocity += velocity.y > 0 ? Vector3.up * 0.5f : Vector3.down * 0.5f;
        }

        //max velocity
        if (velocity.magnitude > this.maxSpeed)
        {
            velocity = velocity.normalized * this.maxSpeed;
        }
        else if (velocity.magnitude < this.minSpeed)
        {
            velocity = velocity.normalized * this.minSpeed;
        }

        this.rb.velocity = velocity;
    }

    public void AddHorizontalForce(float force)
    {
        this.rb.AddForce(Vector3.right * force);
    }
}
