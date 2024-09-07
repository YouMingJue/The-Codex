using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    private Animator animator;

    //player properties
    private int health;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to the current object
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0)) 
        {
            // Trigger the NormalAttack animation
            animator.SetTrigger("NormalAttack");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            animator.SetTrigger("ElementAttack");
        }
    }
}
