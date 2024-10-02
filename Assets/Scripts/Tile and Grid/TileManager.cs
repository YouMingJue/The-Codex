using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;
using Mirror;

[ExecuteInEditMode]
public class TileManager : MonoBehaviour
{
    public Vector2Int playerStartPosition;
    public Tilemap tilemap;
    public List<TileBehavior> tiles = new List<TileBehavior>();
    public static TileManager instance { get; private set; }

    private void Awake()
    {
        // Check if the instance already exists and enforce singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetPlayerStartPosition()
    {
        return new Vector3(playerStartPosition.x, playerStartPosition.y, 0);
    }

    public List<TileBehavior> GetSurroundingTiles(Vector3Int positon)
    {
        List<TileBehavior> neighboringTiles = new List<TileBehavior>();
        // Check each neighboring tile and only add it if it's within the grid bounds
        for (int xd = -1; xd <= 1; xd++)
        {
            for (int yd = -1; yd <= 1; yd++)
            {
                if ((xd == 0 && yd == 0) || (xd == 1 && yd == 1) || (xd == -1 && yd == -1) || (xd == -1 && yd == 1) || (xd == 1 && yd == -1)) continue;
                Vector3Int targetPosition = positon + new Vector3Int(xd, yd, 0);
                GameObject go = tilemap.GetInstantiatedObject(targetPosition);
                if (go != null && go.TryGetComponent(out TileBehavior neighborBehavior))
                {
                    if (neighborBehavior != null) neighboringTiles.Add(neighborBehavior);
                }
            }
        }
        return neighboringTiles;
    }

    private bool isTileInitialized = false;
    public void InitializeTilesForNetwork(List<TileBehavior> tiles)
    {
        if (!isTileInitialized)
        {
            // 直接获取自身挂载的Tilemap组件
            Tilemap tilemap = GetComponent<Tilemap>();
            if (tilemap != null)
            {
                if (NetworkServer.active)
                {
                    // 如果需要特定的权限设置，可以根据实际情况调整
                    // 这里假设将服务器连接作为权限赋予
                    NetworkIdentity[] identities = GetComponentsInChildren<NetworkIdentity>();
                    foreach (NetworkIdentity identity in identities)
                    {
                        identity.AssignClientAuthority(NetworkServer.localConnection);
                    }
                }
                else
                {
                    // 如果是客户端，可能需要移除权限（如果之前有设置）
                    NetworkIdentity[] identities = GetComponentsInChildren<NetworkIdentity>();
                    foreach (NetworkIdentity identity in identities)
                    {
                        identity.RemoveClientAuthority();
                    }
                }
            }
            isTileInitialized = true;
        }
    }
}