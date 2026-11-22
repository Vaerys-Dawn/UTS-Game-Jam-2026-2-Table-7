using UnityEngine;

public interface Damageable
{
    public bool CanBeDamaged();


    /// <summary>
    /// Handle damage from all sources here
    /// </summary>
    /// <returns>Returns true if damage was performed</returns>
    public bool HandleDamage(DamageSource source);
}
