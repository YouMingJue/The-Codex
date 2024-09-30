public interface ITileEffect
{
      abstract void GetTileEffect(Element tileElement);
}

public interface ITileInteractable : ITileEffect
{
    void InteractTile(Element tileElement);
}
