using UnityEngine;

// Attach to any obstacle that has a Collider2D.
// Kills NPCs, enemies and the player on contact.
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

        // NPCs and enemies die here. The enemy counter treats a destroyed enemy as dead.
        NPCController npc = other.GetComponentInParent<NPCController>();
        GameObject target = npc != null ? npc.gameObject : other;
        if (!target.CompareTag("NPC") && !target.CompareTag("Enemy"))
            return;

        // Destroy it instantly (no animation).
        // GetComponentInParent also finds the NPC/enemy if the collider is on a child object.
        AudioManager.Play(AudioManager.Sfx.NpcDeath);
        Destroy(target);
    }
}
