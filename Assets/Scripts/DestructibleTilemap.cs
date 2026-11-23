using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Put this on a Tilemap (e.g. "Ground"). Counts explosion hits per cell and removes BreakableTiles that reach maxHits.
[RequireComponent(typeof(Tilemap))]
public class DestructibleTilemap : MonoBehaviour
{
    private Tilemap tilemap;
    private readonly Dictionary<Vector3Int, int> hitCounts = new Dictionary<Vector3Int, int>();

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    /// <summary>
    /// Adds one hit to every BreakableTile whose cell touches the circle (center, radius).
    /// </summary>
    public void ApplyExplosion(Vector2 center, float radius)
    {
        Vector3Int min = tilemap.WorldToCell(center - Vector2.one * radius);
        Vector3Int max = tilemap.WorldToCell(center + Vector2.one * radius);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                BreakableTile tile = tilemap.GetTile<BreakableTile>(cell);
                if (tile == null) continue; // empty or unbreakable tile

                if (!CellTouchesCircle(cell, center, radius)) continue;

                int hits = hitCounts.TryGetValue(cell, out int current) ? current + 1 : 1;
                if (hits >= tile.maxHits)
                {
                    tilemap.SetTile(cell, null); // composite collider regenerates (Generation Type = Synchronous)
                    hitCounts.Remove(cell);
                }
                else
                {
                    hitCounts[cell] = hits;
                }
            }
        }
    }

    // True if the closest point of the cell's square is within the radius (works with scaled Grids too).
    private bool CellTouchesCircle(Vector3Int cell, Vector2 center, float radius)
    {
        Vector2 cellMin = tilemap.CellToWorld(cell);
        Vector2 cellMax = tilemap.CellToWorld(cell + new Vector3Int(1, 1, 0));
        Vector2 closest = new Vector2(
            Mathf.Clamp(center.x, Mathf.Min(cellMin.x, cellMax.x), Mathf.Max(cellMin.x, cellMax.x)),
            Mathf.Clamp(center.y, Mathf.Min(cellMin.y, cellMax.y), Mathf.Max(cellMin.y, cellMax.y)));
        return Vector2.Distance(closest, center) <= radius;
    }
}
