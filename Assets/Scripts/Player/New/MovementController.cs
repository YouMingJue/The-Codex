using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [Range(1,5)]
    [SerializeField] private float frictionFactor;
    private Vector2 velocity;
    private CircleCollider2D circleCollider;

    private SpriteRenderer sprite;

    [Header("Test Only")]

    [SerializeField] private float detectRadius;

    [SerializeField] private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        sprite = anim.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Movement();
    }

    void FixedUpdate()
    {
        Vector2 position = rb.position;
        position += velocity * Time.fixedDeltaTime;
        rb.MovePosition(position);
    }

    void Movement()
    {
        float xInputAxis = Input.GetAxis("Horizontal");
        float yInputAxis = Input.GetAxis("Vertical");
        Vector2 InputDirection = new Vector2(xInputAxis, yInputAxis);
        if(xInputAxis != 0) FlipHorizontally(xInputAxis);
        velocity = Vector2.MoveTowards(velocity, InputDirection * moveSpeed, moveSpeed * Time.deltaTime * frictionFactor);
        velocity = Vector2.ClampMagnitude(velocity, moveSpeed);
        anim.SetFloat("Velocity", Mathf.Abs(velocity.magnitude));
    }

    void FlipHorizontally(float inputX)
    {
        Vector3 newScale = anim.transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(inputX);
        anim.transform.localScale = newScale;
    }
}

