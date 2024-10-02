using System;
using UnityEngine;
using UnityEngine.UI; // Import UI namespace for Slider
using Mirror; // Import Mirror for networking

public class HealthSystem : NetworkBehaviour
{
    public int maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int currentHealth;

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

    public void Death()
    {
        OnDeath?.Invoke();

        GameplayManager.Instance.AddScoreToOppositeTeam(playerObjectController.playerTeam);
    }

    [Command]
    // Method to handle when the player takes damage
    public void TakeDamage(float damage, Transform attacker)
    {
        Debug.Log("I am getting hit");
        if (isImmune) return;
        // 显式地将浮点数转换为整数
        int damageInt = (int)Mathf.Floor(damage);
        currentHealth -= damageInt;
        OnDamage?.Invoke(damage, attacker);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            CmdPlayerDeath();
        }
    }

    [Command]
    private void CmdPlayerDeath()
    {
        Debug.Log("Player has died!");
        GameplayManager.Instance.AddScoreToOppositeTeam(playerObjectController.playerTeam);
        ResetHealth();
        OnDeath?.Invoke(); // Trigger death event
    }

    // Method to heal the player (if needed)
    [Command]
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        Debug.Log("Player healed. Current health: " + currentHealth);

        // Update slider value
        if (healthSlider != null && isLocalPlayer)
        {
            healthSlider.value = (float)currentHealth;
        }
    }

    void OnHealthChanged(int oldHealth, int newHealth)
    {
        // Update slider value on all clients
        if (healthSlider != null)
        {
            healthSlider.value = (float)newHealth;
        }
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
}