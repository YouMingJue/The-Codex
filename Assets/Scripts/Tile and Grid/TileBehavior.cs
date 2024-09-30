using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class TileBehavior : NetworkBehaviour
{
    public Element element;
    public Element originalElement;
    public ElementalTile _tile;

    [HideInInspector] public Collider2D collid;

    public Vector3Int position;

    [SerializeField] private float convertCD = 3.2f;
    [SerializeField] private float restoreCD = 3;

    private List<TileBehavior> neighboringTiles;

    // SyncVar to sync the element type across the network
    [SyncVar(hook = nameof(OnElementChanged))]
    public Element syncElement;

    private void Start()
    {
        Init(_tile, position);
    }

    // Call this method to initialize the tile behavior
    public void Init(ElementalTile tile, Vector3Int cellPos)
    {
        _tile = tile;
        position = cellPos;
        originalElement = element;
        syncElement = element; // Set the initial element
        neighboringTiles = TileManager.instance.GetSurroundingTiles(position);
        if (isServer)
        {
            TileManager.instance.tiles.Add(this);
        }
        else
        {
            InitializeTile();
        }
    }

    private void InitializeTile()
    {
        // Set the initial element and tile visual
        element = syncElement;
        _tile.RefreshTile(position, TileManager.instance.tilemap);
    }

    private void Update()
    {
        if (isServer)
        {
            // Server side logic for converting tiles
            restoreCD -= Time.deltaTime;
            if (element != originalElement && restoreCD < 0)
            {
                restoreCD = 3;
                ConvertTile(originalElement);
                StartCoroutine(RestoreElement());
            }
            foreach (TileBehavior neighboringTile in neighboringTiles)
            {
                if (IsGeneration(neighboringTile.element))
                {
                    convertCD -= Time.deltaTime;
                    if (convertCD < 0)
                    {
                        ConvertTile(neighboringTile.element);
                        convertCD = 3.2f;
                    }
                    break;
                }
            }
        }
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

    public void PaintTile(Element paintType)
    {
        if (!IsOvercoming(paintType) && isServer)
        {
            ConvertTile(paintType);
        }
    }

    public void ConvertTile(Element convertType)
    {
        if (isServer)
        {
            element = convertType;
            _tile.RefreshTile(position, TileManager.instance.tilemap);
            restoreCD = 5;
            convertCD = 3.2f;
            RpcConvertTile(convertType);
        }
    }

    [ClientRpc]
    public void RpcConvertTile(Element newElement)
    {
        if (!isLocalPlayer) return;

        // Update the local client's view
        syncElement = newElement;
        _tile.RefreshTile(position, TileManager.instance.tilemap);
    }

    private void OnElementChanged(Element oldElement, Element newElement)
    {
        if (!isServer) return;

        // Handle the element change on the server
        element = newElement;
        _tile.RefreshTile(position, TileManager.instance.tilemap);
    }

    private IEnumerator RestoreElement()
    {
        yield return new WaitForSeconds(0.4f);
        if (element != originalElement)
        {
            ConvertTile(originalElement);
        }
    }

    public void ChangeTileSprite(Element element)
    {
        // Update the tile's sprite based on the element
        // _tile.ChangeSprite(element, position, TileManager.instance.tilemap);
    }
}