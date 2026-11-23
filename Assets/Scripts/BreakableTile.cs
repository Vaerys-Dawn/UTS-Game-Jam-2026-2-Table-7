using UnityEngine;
using UnityEngine.Tilemaps;

// A tile that can be destroyed by bomb explosions after enough hits.
// Create one via: Assets > Create > Tiles > Breakable Tile, then assign a sprite.
[CreateAssetMenu(fileName = "BreakableTile", menuName = "Tiles/Breakable Tile")]
public class BreakableTile : Tile
{
    [Tooltip("How many explosions this tile survives before it is removed.")]
    public int maxHits = 5;

    // Called when the asset is first created: default to a square collider so grass edges don't make bumps.
    private void Reset()
    {
        colliderType = ColliderType.Grid;
    }
}
