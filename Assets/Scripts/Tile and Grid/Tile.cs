using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public TileType type;
    public TileState state;

    public enum TileType
    {
        Dirt,
        Sand,
    }

    public enum TileState
    {
        Dry,
        Wet,
        Burning,
    }

    public abstract void Interact();
}
