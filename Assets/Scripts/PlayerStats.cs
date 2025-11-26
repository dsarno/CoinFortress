using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [Header("Combat")]
    public int damage = 1;
    public float fireCooldown = 0.5f;
    
    [Header("Ammo")]
    public int ammo = 10;
    public int maxAmmo = 10;
    public int ammoTier = 0; // 0 = Standard, 1 = Heavy, 2 = Explosive
    
    [Header("Weak Fallback Shot")]
    // public int weakShotDamage = 1; // Deprecated, now defined in WeakProjectile prefab
    public float weakShotCooldown = 2f;
    
    [Header("Health")]
    public int maxHP = 3;
    public int currentHP = 3;
    
    [Header("Shield")]
    public bool shieldUnlocked = false;
    public int shieldMaxHP = 5;
    public int shieldCurrentHP = 0;
    
    [Header("Economy")]
    public int coins = 100; // Start with some coins for testing
    
    [Header("Upgrade Levels")]
    public int damageLevel = 1;
    public int fireRateLevel = 1;
    public int ammoCapacityLevel = 1;
    
    [Header("Cannon Upgrades")]
    public bool doubleBarrelUnlocked = false;

    // Events for UI updates
    public UnityEvent onStatsChanged = new UnityEvent();
    public UnityEvent onAmmoChanged = new UnityEvent();
    public UnityEvent onCoinsChanged = new UnityEvent();
    
    private void Start()
    {
        currentHP = maxHP;
        ammo = maxAmmo;
        if (shieldUnlocked)
        {
            shieldCurrentHP = shieldMaxHP;
        }
    }
    
    // Reset stats for new level
    public void RefillForNewLevel()
    {
        ammo = maxAmmo;
        if (shieldUnlocked)
        {
            // Shield doesn't auto-repair, must buy repair in store
        }
        onStatsChanged?.Invoke();
        onAmmoChanged?.Invoke();
    }

    public void ResetForNewGame()
    {
        // Reset to initial values
        damage = 1;
        fireCooldown = 0.5f;
        ammo = 10;
        maxAmmo = 10;
        ammoTier = 0;
        currentHP = maxHP;
        shieldUnlocked = false;
        shieldCurrentHP = 0;
        coins = 100; // Reset coins to starting amount
        
        damageLevel = 1;
        fireRateLevel = 1;
        ammoCapacityLevel = 1;
        doubleBarrelUnlocked = false;
        
        onStatsChanged?.Invoke();
        onAmmoChanged?.Invoke();
        onCoinsChanged?.Invoke();
    }
    
    public void ConsumeAmmo()
    {
        if (ammo > 0)
        {
            ammo--;
            onAmmoChanged?.Invoke();
        }
    }
    
    public bool HasAmmo()
    {
        return ammo > 0;
    }
    
    public void TakeDamage(int amount)
    {
        // Shield absorbs damage first
        if (shieldUnlocked && shieldCurrentHP > 0)
        {
            shieldCurrentHP -= amount;
            if (shieldCurrentHP < 0)
            {
                // Overflow damage goes to player
                int overflow = Mathf.Abs(shieldCurrentHP);
                shieldCurrentHP = 0;
                currentHP = Mathf.Max(0, currentHP - overflow);
            }
            onStatsChanged?.Invoke();
        }
        else
        {
            currentHP = Mathf.Max(0, currentHP - amount);
        }
        
        if (currentHP <= 0)
        {
            Die();
        }
        
        onStatsChanged?.Invoke();
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        onCoinsChanged?.Invoke();
    }
    
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            onCoinsChanged?.Invoke();
            return true;
        }
        return false;
    }
    
    // Upgrade methods
    public void UpgradeDamage()
    {
        damageLevel++;
        damage = damageLevel;
        onStatsChanged?.Invoke();
    }
    
    public void UpgradeFireRate()
    {
        fireRateLevel++;
        fireCooldown = 0.5f / fireRateLevel; // Gets faster
        onStatsChanged?.Invoke();
    }
    
    public void UpgradeAmmoCapacity(int amount)
    {
        ammoCapacityLevel++;
        maxAmmo += amount;
        onStatsChanged?.Invoke();
    }
    
    public void UnlockShield()
    {
        shieldUnlocked = true;
        shieldCurrentHP = shieldMaxHP;
        onStatsChanged?.Invoke();
    }
    
    public void UpgradeShieldMaxHP(int amount)
    {
        shieldMaxHP += amount;
        onStatsChanged?.Invoke();
    }
    
    public void RepairShield()
    {
        if (shieldUnlocked)
        {
            shieldCurrentHP = shieldMaxHP;
            onStatsChanged?.Invoke();
        }
    }
    
    public void UpgradeAmmoTier()
    {
        if (ammoTier < 2)
        {
            ammoTier++;
            onStatsChanged?.Invoke();
        }
    }

    public void UnlockDoubleBarrel()
    {
        doubleBarrelUnlocked = true;
        onStatsChanged?.Invoke();
    }
    
    private void Die()
    {
        Debug.Log("Player died!");
        // TODO: Restart level or game over screen
    }
}