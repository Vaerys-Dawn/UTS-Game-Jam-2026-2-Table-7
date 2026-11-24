using UnityEngine;

// Attach to any obstacle that has a Collider2D.
// Kills NPCs and the player on contact; enemies are ignored.
public class Obstacle : MonoBehaviour
{
    // Works whether the obstacle's collider is solid...
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryKill(collision.collider);
    }

    // ...or set to "Is Trigger".
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryKill(other);
    }

    private void TryKill(Collider2D col)
    {
        GameObject other = col.gameObject;

        // The player dies: LevelManager loads the Lose scene.
        if (other.CompareTag("Player") || (col.attachedRigidbody != null && col.attachedRigidbody.CompareTag("Player")))
        {
            LevelManager.PlayerDied();
            return;
        }

        // Only NPCs die here. Enemies fall through and nothing happens.
        if (!other.CompareTag("NPC"))
            return;

        // Destroy the whole NPC instantly (no animation).
        // GetComponentInParent also finds the NPC if the collider is on a child object.
        NPCController npc = other.GetComponentInParent<NPCController>();
        AudioManager.Play(AudioManager.Sfx.NpcDeath);
        Destroy(npc != null ? npc.gameObject : other);
    }
}
