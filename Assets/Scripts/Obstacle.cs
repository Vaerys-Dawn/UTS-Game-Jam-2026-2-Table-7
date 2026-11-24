using UnityEngine;

// Attach to any obstacle that has a Collider2D.
// Kills NPCs on contact; Player and Enemy are ignored.
public class Obstacle : MonoBehaviour
{
    // Works whether the obstacle's collider is solid...
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryKill(collision.gameObject);
    }

    // ...or set to "Is Trigger".
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryKill(other.gameObject);
    }

    private void TryKill(GameObject other)
    {
        // Only NPCs die. Player and Enemy fall through and nothing happens.
        if (!other.CompareTag("NPC"))
            return;

        // Destroy the whole NPC instantly (no animation).
        // GetComponentInParent also finds the NPC if the collider is on a child object.
        NPCController npc = other.GetComponentInParent<NPCController>();
        AudioManager.Play(AudioManager.Sfx.NpcDeath);
        Destroy(npc != null ? npc.gameObject : other);
    }
}