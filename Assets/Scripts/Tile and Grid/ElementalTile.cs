using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;
using JetBrains.Annotations;
using System.Xml.Linq;
using System.Linq;

[Serializable]
public class ElementSprite
{
    public Element element;
    public Sprite sprite;
}

[CreateAssetMenu(menuName = "CodexTool/Custom Tile/ElementalTile")]
public class ElementalTile : Tile
{
    [SerializeField] ElementSprite[] sprites;
    public delegate void TileRefreshEvent(ref UnityEngine.Tilemaps.TileData tileData);
    public TileRefreshEvent onTileUpdateEvent;
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (go != null)
        {
            TileBehavior tileObject = go.GetComponent<TileBehavior>();
            tileObject.Init(this, position);
        }
        return base.StartUp(position, tilemap, go);
    }
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref UnityEngine.Tilemaps.TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        var _tilemap = tilemap.GetComponent<Tilemap>();
        TileBehavior behavior = TileManager.instance?.tiles.FirstOrDefault((t) => t.position == position);
        if(behavior != null) UpdateColor(behavior.element,ref tileData);
        Debug.Log(tileData.color);
    }

    public void ChangeSprite(Element element,Vector3Int position, ITilemap tilemap) 
    {
        foreach (ElementSprite elementSprite in sprites) 
        { 
            if(elementSprite.element == element) sprite = elementSprite.sprite;
        }
        //RefreshTile(position, tilemap);
    }

    private void UpdateColor(Element element ,ref UnityEngine.Tilemaps.TileData tileData)
    {
        Debug.Log($"[{Time.time}] Updating color for element: {element}");
        switch (element)
        {
            case Element.Neutral:
                tileData.color = Color.white;
                break;
            case Element.Fire:
                tileData.color = Color.red;
                break;
            case Element.Water:
                tileData.color = Color.blue;
                break;
            case Element.Wood:
                tileData.color = Color.green;
                break;
            case Element.Metal:
                tileData.color = Color.gray;
                break;
            case Element.Earth:
                tileData.color = Color.yellow;
                break;
        }
    }
}
