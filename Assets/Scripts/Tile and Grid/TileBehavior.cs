using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Unity.VisualScripting;
using Mirror;

public class TileBehavior : NetworkBehaviour
{
    [SyncVar]
    public Element element;
    [SyncVar]
    public Element originalElement;
    public ElementalTile _tile;

    [HideInInspector]
    public Collider2D collid;

    public Vector3Int position;

    [SerializeField]
    public float convertCD = 3.2f;
    [SerializeField]
    public float restoreCD = 3;

    private List<TileBehavior> neighboringTiles;

    public void Init(ElementalTile tile, Vector3Int cellPos)
    {
            CmdInit(tile, cellPos);
    }


    [Server]
    public void CmdInit(ElementalTile tile, Vector3Int cellPos)
    {
        if (tile == null)
        {
            Debug.LogError("ElementalTile is null in CmdInit");
        }
        if (cellPos == null)
        {
            Debug.LogError("Vector3Int position is null in CmdInit");
        }

        _tile = tile;
        position = cellPos;
        RpcInitOnClients(tile, cellPos);
    }

    [ClientRpc]
    private void RpcInitOnClients(ElementalTile tile, Vector3Int cellPos)
    {
        Debug.Log("JB!!");
        _tile = tile;
        position = cellPos;
    }

    private void Update()
    {
        if (element != originalElement)
        {
            restoreCD -= Time.deltaTime;
            if (restoreCD < 0)
            {
                restoreCD = 3;
                    CmdRestoreElement();
            }
        }
        if (neighboringTiles != null)
        {
            foreach (TileBehavior neighboringTile in neighboringTiles)
            {
                if (IsGeneration(neighboringTile.element))
                {
                    convertCD -= Time.deltaTime;
                    if (convertCD < 0)
                    {
                        if (isServer)
                        {
                            CmdConvertTile(neighboringTile.element);
                        }
                    }
                    break;
                }
            }
        }
    }

    [Server]
    public void CmdConvertTile(Element convertType)
    {
        Debug.Log($"[{Time.time}] Converting tile to {convertType} on server");
        element = convertType;
        Debug.Log($"[{Time.time}] First Element: {element} on server");
        Debug.Log("you mei you tile?" + _tile);
        _tile.RefreshTile(position, TileManager.instance.tilemap);
        Debug.Log($"[{Time.time}] Second Element: {element} on server");
        restoreCD = 5;
        convertCD = 3;

        if (convertType == Element.Fire)
        {
            //GenerateFireEffect();
        }
        RpcUpdateTileOnClients();
    }

    [ClientRpc]
    public void RpcUpdateTileOnClients()
    {
        _tile.RefreshTile(position, TileManager.instance.tilemap);
    }

    public bool IsGeneration(Element otherType)
    {
        switch (otherType)
        {
            case Element.Fire:
                return element == Element.Wood;
            case Element.Water:
                return element == Element.Metal;
            case Element.Wood:
                return element == Element.Water;
            case Element.Metal:
                return element == Element.Earth;
            case Element.Earth:
                return element == Element.Fire;
        }
        return false;
    }

    public bool IsOvercoming(Element otherType)
    {
        switch (otherType)
        {
            case Element.Fire:
                return element == Element.Water;
            case Element.Water:
                return element == Element.Earth;
            case Element.Wood:
                return element == Element.Metal;
            case Element.Metal:
                return element == Element.Fire;
            case Element.Earth:
                return element == Element.Wood;
        }
        return false;
    }

    public PlayerAbility relatedPlayer;
    public void PaintTile(Element paintType)
    {
        if (!IsOvercoming(paintType))
        {
            CmdConvertTile(paintType);
            PlayerAbility playerAbility = FindObjectOfType<PlayerAbility>();
            if (playerAbility != null)
            {
                playerAbility.PlayerConvertTile(paintType);
            }
        }
    }

    [Server]
    private void CmdRequestTileConversion(Element paintType)
    {
        CmdConvertTile(paintType);
    }


    [Server]
    private void CmdRestoreElement()
    {
        if (element != originalElement)
        {
            CmdConvertTile(originalElement);
        }
        RpcRestoreElementOnClients();
    }

    [ClientRpc]
    private void RpcRestoreElementOnClients()
    {
        if (element != originalElement)
        {
            RpcUpdateTileOnClients();
        }
    }

    private IEnumerator RestoreElement()
    {
        yield return new WaitForSeconds(.4f);
        if (isServer)
        {
            CmdRestoreElement();
        }
    }

    public void ChangeTileSprite(Element element)
    {
        //_tile.ChangeSprite(element, position, TileManager.instance.tilemap);
    }
}
