using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalObject : MonoBehaviour
{
    [SerializeField] private TileType element;
    [SerializeField] private float radiateRange;
    [SerializeField] private float radiateCD;

    void Start()
    {
        Invoke("Init", .3f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiateRange);
    }

    public void Init()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radiateRange);
        foreach (Collider2D tileCollider in colliders)
        {
            Tile tile = tileCollider.GetComponent<Tile>();
            if (tile != null)
            {
                tile.originalType = element;
            }
        }
    }
}
