using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class PlayerAbility : MonoBehaviour
{

    [SerializeField] private Collider2D AttackRange;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] public Element element;
    [SerializeField] private float mana = 100;
    [SerializeField] Animator animator;
    private bool isAttacking = false;
    public float Mana {
        get { return mana; }
        set {
            mana = Mathf.Clamp(value, 0, 100);
            if (manaBar != null)
            {
                manaBar.value = mana;
            }
        }
    }
    
    [SerializeField] private float manaCostAmount;
    [SerializeField] private UnityEngine.UI.Slider manaBar;

    //player properties
    private int health;

    // Start is called before the first frame update
    void Start()
    {
        manaBar.maxValue = mana;
        manaBar.value = mana;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUI() && !isAttacking) 
        {
            Debug.Log("move");
            // Trigger the NormalAttack animation
            animator.SetTrigger("LightAttack");
        }

        if (Input.GetKeyDown(KeyCode.E) && mana > manaCostAmount && !IsMouseOverUI() && !isAttacking)
        {
            animator.SetTrigger("HeavyAttack");
        }
    }

    public void NormalAttack()
    {
        isAttacking = true;
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = true;
        AttackRange.enabled = true;
        Physics2D.OverlapCollider(AttackRange, filter, colliders);

        foreach (Collider2D collider in colliders)
        {
            Attack(collider, 20);
        }
        AttackRange.enabled = false;
    }

    public void ElmentalAttack()
    {
        isAttacking = true;
        Mana -= manaCostAmount;
        if (manaBar != null)
        {
            manaBar.value = (float)mana;
        }

        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = true;
        AttackRange.enabled = true;
        Physics2D.OverlapCollider(AttackRange, filter, colliders);

        foreach (Collider2D collider in colliders)
        {
            Attack(collider, 30);
            TileBehavior tile = collider.GetComponent<TileBehavior>();
            if (tile != null) 
            {
                tile.PaintTile(element);
            }
        }
        AttackRange.enabled = false;
    }

    public void RestoreMana(int restoreAmount)
    {
        Mana += restoreAmount;
    }

    public void Attack(Collider2D collider, int damage)
    {
        if(collider.TryGetComponent<HealthSystem>( out HealthSystem entity) && collider.transform != transform)
        {
            entity.TakeDamage(damage, transform);
        }
    }

    private bool IsMouseOverUI(){
        //return EventSystem.current.IsPointerOverGameObject();
        return false;
    }//To detect if the cursor is over UI, don't attack


    public void EndAttack()
    {
        isAttacking = false;
    }
}
