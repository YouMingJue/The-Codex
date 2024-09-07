using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // Event for when the player dies
    public event Action OnPlayerDeath;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Method to handle when the player takes damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage. Current health: " + currentHealth);

        // If health drops to zero, player dies
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        Debug.Log("Player has died!");
        OnPlayerDeath?.Invoke(); // Trigger death event
    }

    // Method to heal the player (if needed)
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        Debug.Log("Player healed. Current health: " + currentHealth);
    }
}
