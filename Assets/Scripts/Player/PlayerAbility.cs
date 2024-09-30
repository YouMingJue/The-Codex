using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using UnityEngine.SceneManagement;

public enum Buff
{
    None,
    WaterState
}
public class PlayerAbility : MonoBehaviour
{
    private Buff currentState = Buff.None;
    [SerializeField] private Collider2D AttackRange;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] public Element element;
    [SerializeField] private float mana = 100;
    [SerializeField] Animator animator;
    private bool isAttacking = false;
    public Action<TileBehavior> onTileInteraction;
    private Vector3Int currentTilePos;
    TileBehavior currentTile;
    [SerializeField] private HealthSystem health;

    private PlayerObjectController playerObjectController;

    public float Mana
    {
        get { return mana; }
        set
        {
            mana = Mathf.Clamp(value, 0, 100);
            if (manaBar != null)
            {
                manaBar.value = mana;
            }
        }
    }

    [SerializeField] private float manaCostAmount;
    [SerializeField] private UnityEngine.UI.Slider manaBar;
    [SerializeField] private float fireDamage;

    // Start is called before the first frame update
    void Start()
    {
        manaBar.maxValue = mana;
        manaBar.value = mana;
        UpdateTilePos();

        playerObjectController = GetComponent<PlayerObjectController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObjectController.hasAuthority)
        {
            switch (element)
            {
                case Element.Water:
                    // Check if the player is on a water tile and presses LeftShift
                    if (currentTile != null && currentTile.element == Element.Water && currentState == Buff.None)
                    {
                        if (Input.GetKeyDown(KeyCode.LeftShift) && Mana >= manaCostAmount)
                        {
                            currentState = Buff.WaterState;
                            health.isImmune = true;
                            Debug.Log("Player entered WaterState.");
                        }
                    }
                    else if (currentState == Buff.WaterState && (Input.GetKeyDown(KeyCode.LeftShift) || Mana < manaCostAmount))
                    {
                        currentState = Buff.None;
                        health.isImmune = false;
                        Debug.Log("Player exited WaterState.");
                    }
                    break;
            }

            // Check for left mouse click (light attack)
            if (Input.GetMouseButtonDown(0) && !IsMouseOverUI() && !isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("LightAttack");
            }

            // Check for E key (heavy attack)
            if (Input.GetKeyDown(KeyCode.E) && mana > manaCostAmount && !IsMouseOverUI() && !isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("HeavyAttack");
            }
        }
    }

    public void NormalAttack()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = true;
        AttackRange.enabled = true;
        Physics2D.OverlapCollider(AttackRange, filter, colliders);

        foreach (Collider2D collider in colliders)
        {
            Attack(collider, 20);
        }
    }

    public void ElementalAttack()
    {
        Mana -= manaCostAmount;
        if (manaBar != null)
        {
            manaBar.value = (float)mana;
        }

        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = true;
        AttackRange.enabled = true;
        Physics2D.OverlapCollider(AttackRange, filter, colliders);

        foreach (Collider2D collider in colliders)
        {
            Attack(collider, 30);
            TileBehavior tile = collider.GetComponent<TileBehavior>();
            if (tile != null)
            {
                tile.PaintTile(element);
            }
        }
    }

    public void RestoreMana(int restoreAmount)
    {
        Mana += restoreAmount;
    }

    public void Attack(Collider2D collider, int damage)
    {
        if (collider.TryGetComponent(out HealthSystem entity) && collider.transform != transform)
        {
            entity.TakeDamage(damage, transform);
        }
    }

    private bool IsMouseOverUI()
    {
        return false;
    }

    public void EndAttack()
    {
        isAttacking = false;
        AttackRange.enabled = false;
    }

    private void FixedUpdate()
    {
        BuffStateEffect(currentState);
        UpdateTilePos();
        TileEvent();
    }

    void BuffStateEffect(Buff buffstate)
    {
        switch (buffstate)
        {
            case Buff.WaterState:
                Mana -= ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                       ? manaCostAmount * 1.5f : manaCostAmount * 0.4f) * Time.fixedDeltaTime;
                // Check if the player is standing on a water tile
                if (currentTile == null || currentTile.element != Element.Water)
                {

                    // Trigger interaction with the current tile (even if it's not water)
                    onTileInteraction?.Invoke(currentTile);
                }
                break;
        }
    }

    void UpdateTilePos()
    {
        if (SceneManager.GetActiveScene().name == "Playground")
        {
            // Convert world position to tilemap cell position
            Vector3Int cellPosition = TileManager.instance.tilemap.WorldToCell(transform.position);

            // If the player has moved to a new cell, update the current tile
            if (cellPosition != currentTilePos)
            {
                currentTilePos = cellPosition;
                currentTile = TileManager.instance.tilemap.GetInstantiatedObject(cellPosition)?.GetComponent<TileBehavior>();
            }
        }
    }

    void TileEvent()
    {
        switch (currentTile?.element) 
        {
            case Element.Fire:
                if (element == Element.Fire) return;
                GetComponent<HealthSystem>().TakeDamage(fireDamage * Time.fixedDeltaTime, transform);
                break;
        }
    }
}
