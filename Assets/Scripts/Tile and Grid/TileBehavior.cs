using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Unity.VisualScripting;

public  class TileBehavior : MonoBehaviour
{
    public Element element;
    public Element originalElement;
    public ElementalTile _tile;

    [HideInInspector]public Collider2D collid;

    public Vector3Int position;

    [SerializeField] private float convertCD = 3.2f;
    [SerializeField] private float restoreCD = 3;

    private List<TileBehavior> neighboringTiles;

    private void Start()
    {
        TileManager.instance.tiles.Add(this);
        neighboringTiles = TileManager.instance.GetSurroundingTiles(position);
    }


     public void Init(ElementalTile tile, Vector3Int cellPos)
     {
        _tile = tile;
        position = cellPos;

     }

    private void Update()
    {
        if (element != originalElement)
        {
            restoreCD -= Time.deltaTime;
            if (restoreCD < 0) 
            {
                restoreCD = 3;
                ConvertTile(originalElement);
                StartCoroutine(RestoreElement());
            } 
        }
        foreach (TileBehavior neighboringTile in neighboringTiles)
        {
            if (IsGeneration(neighboringTile.element))
            {

                convertCD  -= Time.deltaTime;
                if (convertCD < 0)
                {
                    ConvertTile(neighboringTile.element);
                }
                break;
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
        if (!IsOvercoming(paintType))
        {
            ConvertTile(paintType);
        }
    }

    public void ConvertTile(Element convertType)
    {
        Debug.Log($"[{Time.time}] Converting tile to {convertType}");
        element = convertType;
        Debug.Log($"[{Time.time}] First Element: {element}");
        _tile.RefreshTile(position, TileManager.instance.tilemap);
        Debug.Log($"[{Time.time}] Second Element: {element}");
        restoreCD = 5;
        convertCD = 3;

        if (convertType == Element.Fire)
        {
            //GenerateFireEffect();
        }
    }

    private void GenerateFireEffect()
    {
        string path = "Fire FX";
            GameObject fireEffectPrefab = Resources.Load<GameObject>(path);
        if (fireEffectPrefab != null)
        {
            GameObject fireEffect = Instantiate(fireEffectPrefab, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Fire effect prefab not found!");
        }
    }

    private IEnumerator RestoreElement()
    {
        yield return new WaitForSeconds(.4f);
        if(element != originalElement) ConvertTile(originalElement);

    }

    public void ChangeTileSprite(Element element)
    {
        //_tile.ChangeSprite(element, position, TileManager.instance.tilemap);
    }
}
