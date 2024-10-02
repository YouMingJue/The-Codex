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

    // �����������ڷ���ת��Tile������
    public void SendCmdConvertTile(Element convertType)
    {
        if (playerObjectController.isOwned)
        {
            // �������ͨ����ǰ���λ�û�ȡTile��������Ҫ����ʵ���������
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
        // �����ҵ���ǰ�������λ�ö�Ӧ��Tile
        Vector3Int currentTilePos = TileManager.instance.tilemap.WorldToCell(transform.position);
        TileBehavior currentTile = TileManager.instance.tilemap.GetInstantiatedObject(currentTilePos)?.GetComponent<TileBehavior>();
        if (currentTile != null)
        {
            // ��Tile��ִ��ת���߼�
            currentTile.element = convertType;
            currentTile._tile.RefreshTile(currentTile.position, TileManager.instance.tilemap);
            currentTile.restoreCD = 5;
            currentTile.convertCD = 3;
            if (convertType == Element.Fire)
            {
                // �������GenerateFireEffect��Tile�ϵķ����������Ҫ����ҽű��������߼���Ҫ����
                // currentTile.GenerateFireEffect();
            }
            currentTile.RpcUpdateTileOnClients();
        }
    }

    // Start���ڶ����ʼ��ʱ����һ��
    void Start()
    {
        manaBar.maxValue = mana;
        manaBar.value = mana;
        UpdateTilePos();

        playerObjectController = GetComponent<PlayerObjectController>();
        health.OnDeath += ResetPlayer;

        networkAnimator = GetComponent<NetworkAnimator>();
    }

    // Update��ÿһ֡����һ��
    void Update()
    {
        if (playerObjectController.isOwned)
        {
            switch (element)
            {
                case Element.Water:
                    // �������Ƿ���ˮ��Ƭ�ϲ��Ұ�����Shift��
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

            // ������������ṥ����
            if (Input.GetMouseButtonDown(0) && !IsMouseOverUI() && !isAttacking)
            {
                isAttacking = true;
                networkAnimator.SetTrigger("LightAttack");
            }

            // ���E�����ع�����
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
            if (!collider.gameObject.GetComponent<NetworkIdentity>())
                Attack(collider.gameObject.GetComponent<NetworkIdentity>(), 20);
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
            Attack(collider.gameObject.GetComponent<NetworkIdentity>(), 30);
            TileBehavior tile = collider.GetComponent<TileBehavior>();
            if (tile != null)
            {
                // ����Player��Tile�����ù�ϵ
                tile.relatedPlayer = this;
                tile.PaintTile(element);
            }
        }
    }

    public void RestoreMana(int restoreAmount)
    {
        Mana += restoreAmount;
    }

    [Command]
    public void Attack(NetworkIdentity target, int damage)
    {
        if (target == null)
        {
            Debug.LogError("Attack command: target is null");
            return;
        }

        Collider2D collider = target.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError("Attack command: Collider2D component not found on target");
            return;
        }

        if (collider.transform != transform)
        {
            HealthSystem entity = collider.GetComponent<HealthSystem>();
            if (entity == null)
            {
                Debug.LogError("Attack command: HealthSystem component not found on target");
                return;
            }

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
                // �������Ƿ�վ��ˮ��Ƭ��
                if (currentTile == null || currentTile.element != Element.Water)
                {
                    // �����뵱ǰ��Ƭ�Ľ�������ʹ����ˮ��Ƭ��
                    onTileInteraction?.Invoke(currentTile);
                }
                break;
        }
    }

    void UpdateTilePos()
    {
        if (SceneManager.GetActiveScene().name == "Playground")
        {
            // ����������ת��Ϊ��Ƭ��ͼ�ĵ�Ԫ��λ��
            Vector3Int cellPosition = TileManager.instance.tilemap.WorldToCell(transform.position);

            // �������ƶ������µĵ�Ԫ�񣬸��µ�ǰ��Ƭ
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
