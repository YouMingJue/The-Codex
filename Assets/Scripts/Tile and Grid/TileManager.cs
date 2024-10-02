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
            // If another instance exists, destroy this one
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
            // ֱ�ӻ�ȡ�������ص�Tilemap���
            Tilemap tilemap = GetComponent<Tilemap>();
            if (tilemap != null)
            {
                foreach (TileBehavior tileBehavior in tiles)
                {
                    NetworkServer.Spawn(tileBehavior.gameObject);
                }
                if (NetworkServer.active)
                {
                    // �����Ҫ�ض���Ȩ�����ã����Ը���ʵ���������
                    // ������轫������������ΪȨ�޸���
                    NetworkIdentity[] identities = GetComponentsInChildren<NetworkIdentity>();
                    foreach (NetworkIdentity identity in identities)
                    {
                        identity.AssignClientAuthority(NetworkServer.localConnection);
                    }
                }
                else
                {
                    // ����ǿͻ��ˣ�������Ҫ�Ƴ�Ȩ�ޣ����֮ǰ�����ã�
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

    public void Init()
    {
        // Check if the instance already exists and enforce singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // If another instance exists, destroy this one
            Destroy(gameObject);
        }
    }
}