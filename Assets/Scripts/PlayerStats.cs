using UnityEngine;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [Header("Combat")]
    public int damage = 1;
    public float fireCooldown = 0.5f;
    
    [Header("Health")]
    public int maxHP = 3;
    public int currentHP = 3;
    
    [Header("Economy")]
    public int coins = 0;
    
    private void Start()
    {
        currentHP = maxHP;
    }
    
    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        if (currentHP <= 0)
        {
            Die();
        }
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log($"Collected {amount} coins! Total: {coins}");
    }
    
    private void Die()
    {
        Debug.Log("Player died!");
        // TODO: Restart level or game over screen
    }
}