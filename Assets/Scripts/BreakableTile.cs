using UnityEngine; //Long added more
using UnityEngine.Tilemaps; //Long added more

// A tile that can be destroyed by bomb explosions after enough hits. //Long added more
// Create one via: Assets > Create > Tiles > Breakable Tile, then assign a sprite. //Long added more
[CreateAssetMenu(fileName = "BreakableTile", menuName = "Tiles/Breakable Tile")] //Long added more
public class BreakableTile : Tile //Long added more
{
    [Tooltip("How many explosions this tile survives before it is removed.")] //Long added more
    public int maxHits = 5; //Long added more

    // Called when the asset is first created: default to a square collider so grass edges don't make bumps. //Long added more
    private void Reset() //Long added more
    {
        colliderType = ColliderType.Grid; //Long added more
    }
}
