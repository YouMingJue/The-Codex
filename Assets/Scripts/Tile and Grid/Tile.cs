using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Tile : MonoBehaviour
{
    public TileType type;
    public TileType originalType;
    [HideInInspector]public SpriteRenderer sprite;

    [HideInInspector]public Vector2 size;

    [HideInInspector] public Vector2Int position;

    [SerializeField] private float convertCD = 3.2f;
    [SerializeField] private float restoreCD = 3;

    private List<Tile> neighboringTiles;

    private void Awake()
    {
        neighboringTiles = new List<Tile>();
    }


    private void Start()
    {
        neighboringTiles = TileManager.instance.GetSurroundingTiles(position);
    }

    private void Update()
    {
        if (type != originalType)
        {
            restoreCD -= Time.deltaTime;
            if (restoreCD < 0) 
            {
                restoreCD = 3;
                ConvertTile(TileType.Neutral);
                StartCoroutine(RestoreElement());
            } 
        }
        foreach (Tile neighboringTile in neighboringTiles)
        {
            if (IsGeneration(neighboringTile.type))
            {

                convertCD  -= Time.deltaTime;
                if (convertCD < 0)
                {
                    ConvertTile(neighboringTile.type);
                }
                break;
            }
   
        }
    }

    public void Init()
    {
        sprite = GetComponent<SpriteRenderer>();
        size = sprite.bounds.size;
    }

    public bool IsGeneration(TileType otherType)
    {
        switch (otherType)
        {
            case TileType.Fire:
                return type == TileType.Wood;
            case TileType.Water:
                return type == TileType.Metal;
            case TileType.Wood:
                return type == TileType.Water;
            case TileType.Metal:
                return type == TileType.Earth;
            case TileType.Earth:
                return type == TileType.Fire;
        }
        return false;
    }

    public bool IsOvercoming(TileType otherType)
    {
        switch (otherType)
        {
            case TileType.Fire:
                return type == TileType.Water;
            case TileType.Water:
                return type == TileType.Earth;
            case TileType.Wood:
                return type == TileType.Metal;
            case TileType.Metal:
                return type == TileType.Fire;
            case TileType.Earth:
                return type == TileType.Wood;
        }
        return false;
    }

    public void PaintTile(TileType paintType)
    {
        if (!IsOvercoming(paintType))
        {
            ConvertTile(paintType);
        }
    }

    public void ConvertTile(TileType convertType)
    {
        type = convertType;
        ChangeColor();
        restoreCD = 5;
        convertCD = 3;
    }

    private void ChangeColor()
    {
        switch (type)
        {
            case TileType.Neutral:
                sprite.color = Color.white;
                break;
            case TileType.Fire:
                sprite.color = Color.red;
                break;
            case TileType.Water:
                sprite.color = Color.blue;
                break;
            case TileType.Wood:
                sprite.color = Color.green;
                break;
            case TileType.Metal:
                sprite.color = Color.gray;
                break;
            case TileType.Earth:
                sprite.color = Color.yellow;
                break;
        }
    }

    private IEnumerator RestoreElement()
    {
        yield return new WaitForSeconds(.4f);
        if(type != originalType) ConvertTile(originalType);

    }
}
