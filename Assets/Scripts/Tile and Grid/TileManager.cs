using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab; // 地块预制体
    public int width = 10; // 网格宽度
    public int height = 10; // 网格高度
    public Vector2Int playerStartPosition; // 玩家生成位置

    private Tile[,] tiles; // 存储网格中的地块

    void Start()
    {
        GenerateGrid();
    }

    public Vector3 GetPlayerStartPosition()
    {
        return new Vector3(playerStartPosition.x, 0.8f, playerStartPosition.y);
    }

    void GenerateGrid()
    {
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0.8f, y);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tiles[x, y] = tile.GetComponent<Tile>();
            }
        }
    }
}