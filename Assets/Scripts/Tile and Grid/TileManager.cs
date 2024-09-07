using UnityEngine;
using UnityEditor;
using System.Collections.Generic; // Required to use Editor functionality

[ExecuteInEditMode] // This attribute allows the script to run in the editor
public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;
    public Vector2Int playerStartPosition;
    public static TileManager instance { get; private set; }

    private Tile[,] tiles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Optionally you can remove runtime grid generation
    }

    [ContextMenu("Generate Grid")] // Adds this function to the right-click menu in the inspector
    public void GenerateGrid()
    {
        // Clear any existing tiles to avoid duplicates
        ClearGrid();

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tile = PrefabUtility.InstantiatePrefab(tilePrefab, transform) as GameObject;
                if(tile.TryGetComponent<Tile>(out Tile tileEntity))
                {
                    tileEntity.Init();
                    Vector3 position = new Vector3(x * tileEntity.size.x, y * tileEntity.size.y, 0);
                    tileEntity.position = new Vector2(x, y);
                    Debug.Log(tileEntity.size);
                    tiles[x, y] = tileEntity;
                    tile.transform.position = position;
                }

                // Mark the tile as part of the scene for saving
                Undo.RegisterCreatedObjectUndo(tile, "Create Tile");
            }
        }

        // Mark the scene as dirty to ensure changes are saved
        EditorUtility.SetDirty(gameObject);
    }

    // Clear the grid in the editor
    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        // Destroy all existing tiles
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
        }
    }

    public Vector3 GetPlayerStartPosition()
    {
        return new Vector3(playerStartPosition.x, playerStartPosition.y, 0);
    }

    public List<Tile> GetSurroundingTiles(Vector2Int positon)
    {
        List<Tile> neighboringTiles = new List<Tile> ();
        neighboringTiles.Add(tiles[positon.x - 1, positon.y]);
        neighboringTiles.Add(tiles[positon.x + 1, positon.y]);
        neighboringTiles.Add(tiles[positon.x, positon.y + 1]);
        neighboringTiles.Add(tiles[positon.x, positon.y - 1]);
        return neighboringTiles;
    }
}
