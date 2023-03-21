using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Unit
{
 float normalSpeed;
   [SerializeField]
   float chasingSpeed=5;
    Vector3 startPoint;
  [SerializeField]
  float patrolRadius = 5;
  [SerializeField]
  float idlingCooldown=2;
  float idlingTimer;
  
    List<Soldier> seenSoldiers = new List<Soldier>();
    Soldier ClosestSoldier
    {
        get
        {
            if (seenSoldiers==null || seenSoldiers.Count<=0) return null;
            float minDistance = float.MaxValue;
            Soldier closestSoldier=null;
            foreach(Soldier soldier in seenSoldiers)
            {
                if(!soldier||!soldier.IsAlive) continue;
                float distance = Vector3.Magnitude(soldier.transform.position - transform.position);
                if(distance<minDistance)
                {
                    minDistance=distance;
                    closestSoldier = soldier;
                }
            }
             return closestSoldier;
        }  
    }

    protected override void Awake()
    {
        base.Awake();
        normalSpeed=nav.speed;
        startPoint = transform.position;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        var soldier = other.gameObject.GetComponent<Soldier>();
        if(soldier && !seenSoldiers.Contains(soldier))
        {
            seenSoldiers.Add(soldier);
        }
    
    }
   
   
      protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        var soldier = other.gameObject.GetComponent<Soldier>();
        if(soldier)
        {
            seenSoldiers.Remove(soldier);
        }
    }
  
  
  
  
    void SetRandomRoamingPosition()
    {
       
        Vector3 destination;
        Vector3 delta = new Vector3(Random.Range(-1f,1f),0,Random.Range(-1f,1f));
        delta.Normalize();
        delta *= patrolRadius;

        nav.SetDestination(startPoint+delta);
    
    }
   


    protected override void Idling()
    {
       
        base.Idling();
        UpdateSight();
        if ((idlingTimer-= Time.deltaTime)<=0)
        {
            idlingTimer = idlingCooldown;
            task = Task.move;
            SetRandomRoamingPosition();
        }
    }

    protected override void Moving()
    {
       
        base.Moving();
        nav.speed = normalSpeed;
        UpdateSight();
    }

    protected override void Chasing()
    {
        base.Chasing();
        nav.speed = chasingSpeed;
    }

void UpdateSight()
    {
        var soldier = ClosestSoldier;
        if(soldier)
        {
            target = soldier.transform;
            task = Task.chase;
        }
    }


    protected override void OnDrawGizmosSelected() 
    {
        base.OnDrawGizmosSelected();    
        Gizmos.color= Color.blue ;
        if(!Application.isPlaying)
            startPoint=transform.position;
        Gizmos.DrawWireSphere(startPoint,patrolRadius);  
    
    }
   
    public override void ReceiveDamage(float damage, Vector3 damageDealerPosition)
    {
       
        base.ReceiveDamage(damage,damageDealerPosition);
        if (!target)
        {
            task = Task.move;
            nav.SetDestination(damageDealerPosition);
        }
        if (HealthPercent > .5f)
        {
            animator.SetTrigger("Get Hit") ;
            nav.velocity = Vector3.zero;
        }
    }
  
   
}