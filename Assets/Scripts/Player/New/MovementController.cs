using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Steamworks;
using Mirror;


public class PlayerController : NetworkBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    [Range(1,5)]
    [SerializeField] private float frictionFactor;
    private Vector2 velocity;
    private Collider2D collid;

    private SpriteRenderer sprite;

    [Header("Test Only")]

    [SerializeField] private float detectRadius;

    [SerializeField] private Animator anim;

    [SerializeField] private LayerMask targetlayer;

    private PlayerObjectController playerObjectController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collid = GetComponent<Collider2D>();
        sprite = anim.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GetComponent<PlayerAbility>().onTileInteraction += DiveTile;

        playerObjectController = GetComponent<PlayerObjectController>();
        GetComponent<HealthSystem>().OnDeath += SetPosition;
    }

    private void Update()
    {
        if (playerObjectController.hasAuthority)
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

    public void FlipPlayerHorizontally(float inputX)
    {
        if (isServer)
        {
            CmdFlipPlayerHorizontally(inputX);
        }
    }

    [Command]
    private void CmdFlipPlayerHorizontally(float inputX)
    {
        FlipHorizontally(inputX);
        RpcFlipPlayerHorizontallyOnClients(inputX);
    }

    [ClientRpc]
    private void RpcFlipPlayerHorizontallyOnClients(float inputX)
    {
        FlipHorizontally(inputX);
    }

    public void SetPosition()
    {
        Vector3 position = TileManager.instance.GetPlayerStartPosition();
        transform.position = new Vector3(position.x, position.y, 0f);
    }
    void DiveTile(TileBehavior tile)
    {
        if (tile == null || tile.element != Element.Water)
        {
           transform.position -= (Vector3)velocity * Time.fixedDeltaTime * 3f;
            velocity = Vector2.zero;
        }else velocity *= 200f;
    }

}

