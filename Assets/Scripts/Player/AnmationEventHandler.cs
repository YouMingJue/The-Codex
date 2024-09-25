using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnmationEventHandler : MonoBehaviour
{
    public UnityEvent lightAttackEvent;
    public UnityEvent heavyAttackEvent;
    public UnityEvent endAttackEvent;

    public void LightAttack()
    {
        lightAttackEvent?.Invoke();
    }

    public void HeavyAttack()
    {
        heavyAttackEvent?.Invoke();
    }

    public void EndAttack() 
    { 
        endAttackEvent?.Invoke();
    }
}
