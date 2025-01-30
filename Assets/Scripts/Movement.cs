using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 10f;
    public float turnSpeed = 10f;
    Rigidbody rb;
    float currentSpeed = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Move();
    }
    public void Move()
    {
        float moveY = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        if (moveY != 0)
        {
            currentSpeed += moveY * acceleration * Time.deltaTime;
        }
        else if (Mathf.Abs(currentSpeed) > .01f)
        {
            currentSpeed -= currentSpeed * acceleration * .1f * Time.deltaTime;
        }
        // else currentSpeed = 0;

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
        Vector3 moveDirection = transform.forward * currentSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        if (moveX != 0)
        {
            float turn = moveX * turnSpeed * Time.deltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turn, 0));
        }
    }
}
