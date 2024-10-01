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
//using UnityEngine.WSA;
using UnityEngine.SceneManagement;
using Mirror;

public enum Buff
{
    None,
    WaterState
}

public class PlayerAbility : NetworkBehaviour
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

    private NetworkAnimator networkAnimator;

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

    // 新增方法用于发送转换Tile的命令
    public void SendCmdConvertTile(Element convertType)
    {
        if (playerObjectController.isOwned)
        {
            // 这里假设通过当前玩家位置获取Tile，可能需要根据实际情况调整
            Vector3Int currentTilePos = TileManager.instance.tilemap.WorldToCell(transform.position);
            TileBehavior currentTile = TileManager.instance.tilemap.GetInstantiatedObject(currentTilePos)?.GetComponent<TileBehavior>();
            if (currentTile != null)
            {
                currentTile.CmdConvertTile(convertType);
            }
        }
    }

    public void PlayerConvertTile(Element convertType)
    {
        CmdConvertTileOnPlayer(convertType);
    }

    [Command]
    public void CmdConvertTileOnPlayer(Element convertType)
    {
        // 首先找到当前玩家所在位置对应的Tile
        Vector3Int currentTilePos = TileManager.instance.tilemap.WorldToCell(transform.position);
        TileBehavior currentTile = TileManager.instance.tilemap.GetInstantiatedObject(currentTilePos)?.GetComponent<TileBehavior>();
        if (currentTile != null)
        {
            // 在Tile上执行转换逻辑
            currentTile.element = convertType;
            currentTile._tile.RefreshTile(currentTile.position, TileManager.instance.tilemap);
            currentTile.restoreCD = 5;
            currentTile.convertCD = 3;
            if (convertType == Element.Fire)
            {
                // 这里假设GenerateFireEffect是Tile上的方法，如果需要在玩家脚本处理，逻辑需要调整
                // currentTile.GenerateFireEffect();
            }
            currentTile.RpcUpdateTileOnClients();
        }
    }

    // Start是在对象初始化时调用一次
    void Start()
    {
        manaBar.maxValue = mana;
        manaBar.value = mana;
        UpdateTilePos();

        playerObjectController = GetComponent<PlayerObjectController>();
        health.OnDeath += ResetPlayer;

        networkAnimator = GetComponent<NetworkAnimator>();
    }

    // Update是每一帧调用一次
    void Update()
    {
        if (playerObjectController.isOwned)
        {
            switch (element)
            {
                case Element.Water:
                    // 检查玩家是否在水瓦片上并且按下左Shift键
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

            // 检查左键点击（轻攻击）
            if (Input.GetMouseButtonDown(0) && !IsMouseOverUI() && !isAttacking)
            {
                isAttacking = true;
                networkAnimator.SetTrigger("LightAttack");
            }

            // 检查E键（重攻击）
            if (Input.GetKeyDown(KeyCode.E) && mana > manaCostAmount && !IsMouseOverUI() && !isAttacking)
            {
                isAttacking = true;
                networkAnimator.SetTrigger("HeavyAttack");
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
                // 建立Player与Tile的引用关系
                tile.relatedPlayer = this;
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
                // 检查玩家是否站在水瓦片上
                if (currentTile == null || currentTile.element != Element.Water)
                {
                    // 触发与当前瓦片的交互（即使不是水瓦片）
                    onTileInteraction?.Invoke(currentTile);
                }
                break;
        }
    }

    void UpdateTilePos()
    {
        if (SceneManager.GetActiveScene().name == "Playground")
        {
            // 将世界坐标转换为瓦片地图的单元格位置
            Vector3Int cellPosition = TileManager.instance.tilemap.WorldToCell(transform.position);

            // 如果玩家移动到了新的单元格，更新当前瓦片
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

    void ResetPlayer()
    {
        Mana = 100;
    }
}
