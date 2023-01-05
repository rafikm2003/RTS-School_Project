using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Unit : MonoBehaviour
{
    
    
    const string ANIMATOR_SPEED="Speed",
    ANIMATOR_ALIVE = "Alive",
    ANIMATOR_ATTACK="Attack";
    Animator animator;
    NavMeshAgent nav;
    public Transform target;
    private void Awake() 
    {
        nav = GetComponent<NavMeshAgent>();    
        animator = GetComponent<Animator>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            nav.SetDestination(target.position);
        }
        Animate();
    }
   
   protected virtual void Animate()
    {
        var speedVector = nav.velocity;
        speedVector.y = 0;
        float speed = speedVector.magnitude;
        animator.SetFloat(ANIMATOR_SPEED, speed);
    }
    
    }
   
   
   

   
   
   
   
   
   
   
    



