using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElementalTower : MonoBehaviour
{
    [SerializeField] private float AddValue;
    private HealthSystem healthSystem;
    // Start is called before the first frame update
    void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        healthSystem.OnDamage += OnGetHit;
        healthSystem.OnPlayerDeath += OnGetDestroyed;
    }

    private void OnGetHit(float damage, Transform attacker)
    {
        if(attacker.TryGetComponent<PlayerAbility>(out PlayerAbility player))
        {
           player.RestoreMana((int)damage);
        }
    }

    private void OnGetDestroyed()
    {
        StartCoroutine(RebuildTower());
    }

    IEnumerator RebuildTower()
    {
        healthSystem.OnDamage -= OnGetHit;
        healthSystem.isImmune = true;
        Debug.Log(healthSystem);
        while(healthSystem.currentHealth != healthSystem.maxHealth)
        {
            healthSystem.Heal(20);
            yield return new WaitForSeconds(1f);
        }
        healthSystem.OnDamage += OnGetHit;
        healthSystem.isImmune = false;
    }
}
