using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab; // �ؿ�Ԥ����
    public int width = 10; // ������
    public int height = 10; // ����߶�
    public Vector2Int playerStartPosition; // �������λ��

    private Tile[,] tiles; // �洢�����еĵؿ�

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