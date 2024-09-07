  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Tile : MonoBehaviour
{
    public TileType type;
    [HideInInspector]public SpriteRenderer sprite;

    [HideInInspector]public Vector2 size;

    [HideInInspector] public Vector2 position;

    private void Start()
    {
    }

    public enum TileType
    {
        Neutral,
        Fire,
        Water,
        Wood,
        Metal,
        Earth
    }

    public void Interact() { }

    public void Init()
    {
        sprite = GetComponent<SpriteRenderer>();
        Debug.Log(sprite);
        size = sprite.bounds.size;
    }

    public bool isGeneration(TileType otherType)
    {
        switch (otherType)
        {
            case TileType.Fire:
                return type == TileType.Earth;
            case TileType.Water:
                return type == TileType.Wood;
            case TileType.Wood:
                return type == TileType.Fire;
            case TileType.Metal:
                return type == TileType.Water;
            case TileType.Earth:
                return type == TileType.Metal;
        }
        return false;
    }

    public bool IsOvercoming(TileType otherType)
    {
        switch (otherType)
        {
            case TileType.Fire:
                return type == TileType.Metal;
            case TileType.Water:
                return type == TileType.Fire;
            case TileType.Wood:
                return type == TileType.Earth;
            case TileType.Metal:
                return type == TileType.Wood;
            case TileType.Earth:
                return type == TileType.Water;
        }
        return false;
    }
}
