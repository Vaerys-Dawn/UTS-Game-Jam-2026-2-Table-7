using System.Collections.Generic; //Long added more
using UnityEngine; //Long added more
using UnityEngine.Tilemaps; //Long added more

// Put this on a Tilemap (e.g. "Ground"). Counts explosion hits per cell and removes BreakableTiles that reach maxHits. //Long added more
[RequireComponent(typeof(Tilemap))] //Long added more
public class DestructibleTilemap : MonoBehaviour //Long added more
{
    private Tilemap tilemap; //Long added more
    private readonly Dictionary<Vector3Int, int> hitCounts = new Dictionary<Vector3Int, int>(); //Long added more

    void Awake() //Long added more
    {
        tilemap = GetComponent<Tilemap>(); //Long added more
    }

    /// <summary> //Long added more
    /// Adds one hit to every BreakableTile whose cell touches the circle (center, radius). //Long added more
    /// </summary> //Long added more
    public void ApplyExplosion(Vector2 center, float radius) //Long added more
    {
        Vector3Int min = tilemap.WorldToCell(center - Vector2.one * radius); //Long added more
        Vector3Int max = tilemap.WorldToCell(center + Vector2.one * radius); //Long added more

        for (int x = min.x; x <= max.x; x++) //Long added more
        {
            for (int y = min.y; y <= max.y; y++) //Long added more
            {
                Vector3Int cell = new Vector3Int(x, y, 0); //Long added more
                BreakableTile tile = tilemap.GetTile<BreakableTile>(cell); //Long added more
                if (tile == null) continue; // empty or unbreakable tile //Long added more

                if (!CellTouchesCircle(cell, center, radius)) continue; //Long added more

                int hits = hitCounts.TryGetValue(cell, out int current) ? current + 1 : 1; //Long added more
                if (hits >= tile.maxHits) //Long added more
                {
                    tilemap.SetTile(cell, null); // composite collider regenerates (Generation Type = Synchronous) //Long added more
                    hitCounts.Remove(cell); //Long added more
                }
                else //Long added more
                {
                    hitCounts[cell] = hits; //Long added more
                }
            }
        }
    }

    // True if the closest point of the cell's square is within the radius (works with scaled Grids too). //Long added more
    private bool CellTouchesCircle(Vector3Int cell, Vector2 center, float radius) //Long added more
    {
        Vector2 cellMin = tilemap.CellToWorld(cell); //Long added more
        Vector2 cellMax = tilemap.CellToWorld(cell + new Vector3Int(1, 1, 0)); //Long added more
        Vector2 closest = new Vector2( //Long added more
            Mathf.Clamp(center.x, Mathf.Min(cellMin.x, cellMax.x), Mathf.Max(cellMin.x, cellMax.x)), //Long added more
            Mathf.Clamp(center.y, Mathf.Min(cellMin.y, cellMax.y), Mathf.Max(cellMin.y, cellMax.y))); //Long added more
        return Vector2.Distance(closest, center) <= radius; //Long added more
    }
}
