using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Unit : MonoBehaviour
{
    public enum Task 
    {
        idle,move,follow,chase,attack
    }
   protected Task task=Task.idle;
   public bool IsAlive{get{return hp>0;}}
   
    public float HealthPercent {get {return hp/hpMax;}}
    const string ANIMATOR_SPEED="Speed",
    ANIMATOR_ALIVE = "Alive",
    ANIMATOR_ATTACK="Attack";
    protected Animator animator;
   [Header("Unit")]
   [SerializeField]
    GameObject hpBarPrefab;
    [SerializeField]
    protected float attackDamage =0;
    [SerializeField]
    protected float attackDistance =1;
    [SerializeField]
    protected float attackSpeed =1;
    protected NavMeshAgent nav;
    protected Transform target;
    [SerializeField]
    float hp,hpMax=100;
    public static List<ISelectable> SelectableUnits {get {return selectableUnits;}}
    static List<ISelectable> selectableUnits = new List<ISelectable>();
    float attackTimer;
    
    protected HealthBar healthBar;
    [SerializeField]
    protected float stoppingDistance = 1;
    
    
    
    
    
    protected virtual void Awake() 
    {
        nav = GetComponent<NavMeshAgent>();    
        animator = GetComponent<Animator>();
        hp=hpMax;
        healthBar = Instantiate(hpBarPrefab, transform).GetComponent<HealthBar>();
    }
    
    
    private void Start()
    {
        if (this is ISelectable) 
        {
        selectableUnits.Add(this as ISelectable);
        (this as ISelectable).SetSelected(false);
        }
    }

    private void OnDestroy()
    {
       if (this is ISelectable) selectableUnits.Remove(this as ISelectable); 
    }

    // Update is called once per frame
    void Update()
    {
     

     if(IsAlive)
     {
       switch(task)
       {
        case Task.idle:Idling();break;
        case Task.move:Moving();break;
        case Task.follow:Following();break;
        case Task.chase:Chasing();break;
        case Task.attack:Attacking();break;
        }
        }
       
       
       
       
        Animate();
      }
   
protected virtual void Idling() 
{
    nav.velocity = Vector3.zero;
}
protected virtual void Attacking() 
{
    if(target)
    {
        nav.velocity = Vector3.zero;
        transform.LookAt(target);
        float distance = Vector3.Magnitude(target.position-transform.position);
        if (distance <= attackDistance)
        {   
            if((attackTimer -= Time.deltaTime) <=0)
            Attack();
        }
        else
        {
            task = Task.chase;
        }
    
    
    
    }
    else
    {
        task = Task.idle;
    }
}

protected virtual void Moving() 
{
    float distance = Vector3.Magnitude(nav.destination-transform.position);
    if (distance <= stoppingDistance)
    {
        task = Task.idle;
    }

}
protected virtual void Following() 
{
    if (target)
    {
        nav.SetDestination(target.position);
    }
    else 
    {
        task = Task.idle;
    }

}
protected virtual void Chasing() 
{
    if (target)
    {
        nav.SetDestination(target.position);
        float distance = Vector3.Magnitude(nav.destination-transform.position);
        if (distance <= attackDistance)
        {   
        task = Task.attack;
        }
    }
    else 
    {
        task = Task.idle;
    }

}

   protected virtual void Animate()
    {
        var speedVector = nav.velocity;
        speedVector.y = 0;
        float speed = speedVector.magnitude;
        animator.SetFloat(ANIMATOR_SPEED, speed);
        animator.SetBool(ANIMATOR_ALIVE,IsAlive);
    }
    public virtual void Attack()
    {
        Unit unit = target.GetComponent<Unit>();
            if (unit && unit.IsAlive)
            {
                animator.SetTrigger(ANIMATOR_ATTACK);
                attackTimer = attackSpeed;
            
            }
            else 
            {
                target = null;
            }
    
    }

    public virtual void DealDamage()
    {
        if(!IsAlive) return;
        if (target)
        {
            Unit unit = target.GetComponent<Unit>();
            if (unit)
            {
                unit.ReceiveDamage(attackDamage, transform.position);
            }
         
        
        }   

    }
    
    protected virtual void OnTriggerEnter(Collider other) 
    {
        
    }
    protected virtual void OnTriggerExit(Collider other) 
    {
        
    }
    protected virtual void OnDrawGizmosSelected() 
    {
        Gizmos.color=Color.red ;
        Gizmos.DrawWireSphere(transform.position,attackDistance);   
    }
    
    public virtual void ReceiveDamage(float damage, Vector3 damageDealerPosition)
    {   
        if(IsAlive) hp -= damage;
        if(!IsAlive)
        {
            healthBar.gameObject.SetActive(false);
            //enabled = false;
            foreach(var collider in GetComponents<Collider>())
                collider.enabled = false;

        }
    }



    }
   
   
   

   
   
   
   
   
   
   
    



