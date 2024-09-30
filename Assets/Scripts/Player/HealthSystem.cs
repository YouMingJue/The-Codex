using System;
using UnityEngine;
using UnityEngine.UI; // Import UI namespace for Slider

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public float currentHealth;

    public bool isImmune = false;

    // Reference to the UI Slider for health
    public Slider healthSlider;

    // Event for when the player dies
    public event Action OnDeath;
    public event Action<float, Transform> OnDamage;

    public PlayerObjectController playerObjectController;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        // Initialize the slider to full health
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        playerObjectController = GetComponent<PlayerObjectController>();
    }

    // Method to handle when the player takes damage
    public void TakeDamage(float damage, Transform attacker)
    {
        if (isImmune) return;
        currentHealth -= damage;
        Debug.Log("Player took damage. Current health: " + currentHealth);

        OnDamage?.Invoke(damage, attacker);

        // Update slider value
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth;
        }

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
        GameplayManager.Instance.AddScoreToOppositeTeam(playerObjectController.playerTeam);
        OnDeath?.Invoke(); // Trigger death event
    }

    // Method to heal the player (if needed)
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        Debug.Log("Player healed. Current health: " + currentHealth);

        // Update slider value
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth;
        }
    }
}
