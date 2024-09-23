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
    private Animator animator;

    [SerializeField] private Collider2D normalAttackRange;
    [SerializeField] private Collider2D elementalAttackRange;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] public TileType element;
    [SerializeField] private float mana = 100;
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
        // Get the Animator component attached to the current object
        animator = GetComponent<Animator>();
        manaBar.maxValue = mana;
        manaBar.value = mana;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("move");
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUI()) 
        {
            //Debug.Log("move");
            // Trigger the NormalAttack animation
            animator.SetTrigger("NormalAttack");
        }

        if (Input.GetKeyDown(KeyCode.E) && mana > manaCostAmount && !IsMouseOverUI())
        {

            animator.SetTrigger("ElementAttack");
        }
    }

    public void NormalAttack()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = true;
        normalAttackRange.enabled = true;
        Physics2D.OverlapCollider(normalAttackRange, filter, colliders);

        foreach (Collider2D collider in colliders)
        {
            Attack(collider, 20);
        }
        normalAttackRange.enabled = false;
    }

    public void ElmentalAttack()
    {
        Mana -= manaCostAmount;
        if (manaBar != null)
        {
            manaBar.value = (float)mana;
        }

        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = true;
        elementalAttackRange.enabled = true;
        Physics2D.OverlapCollider(elementalAttackRange,filter, colliders);

        foreach (Collider2D collider in colliders)
        {
            Attack(collider, 30);
            Tile tile = collider.GetComponent<Tile>();
            if (tile != null) 
            {
                tile.PaintTile(element);
            }
        }
        elementalAttackRange.enabled = false;
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
        return EventSystem.current.IsPointerOverGameObject();
    }//To detect if the cursor is over UI, don't attack
}
