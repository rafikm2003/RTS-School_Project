using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit,ISelectable
{
   [Header("Soldier")]
   [Range(0,.3f),SerializeField]
   float shootDuration = 0;
   [SerializeField]
   ParticleSystem impactEffect,muzzleEfect;
   [SerializeField]
   LayerMask shootingLayerMask;
   LineRenderer lineEffect;
   Light lightEffect;
    
  protected override void Awake() 
  {
    base.Awake();
    lineEffect = muzzleEfect.GetComponent<LineRenderer>();
    lightEffect = muzzleEfect.GetComponent<Light>();
    impactEffect.transform.SetParent(null);
    EndShootEffect();
  }
    
    public void SetSelected(bool selected)
    {
        healthBar.gameObject.SetActive(selected);
    }

    void Command(Vector3 destination)
    {
        if(!IsAlive) return;
        nav.SetDestination(destination);
        task = Task.move;
        target = null;
    }
    void Command(Soldier soldierToFollow)
    {
       if(!IsAlive) return;
       target = soldierToFollow.transform;
       task = Task.follow; 
    }
    void Command(Dragon dragonToKill)
    {
     if(!IsAlive) return;
      target = dragonToKill.transform;
       task = Task.chase;
    }
   
   
   
    public override void DealDamage()
    {
        if(Shoot())
        {
        base.DealDamage();
        }    
    }
    bool Shoot()
    {
        Vector3 start = muzzleEfect.transform.position;
        Vector3 direction = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(start,direction,out hit ,attackDistance , shootingLayerMask))
        {
            StartShootEffect(start,hit.point,true);
            var unit = hit.collider.gameObject.GetComponent<Unit>();
            return unit;

        }
        StartShootEffect(start,start + direction*attackDistance,false);
        return false;
    }
void StartShootEffect(Vector3 lineStart,Vector3 lineEnd, bool hitSomething)
{
    if(hitSomething)
    {
    impactEffect.transform.position = lineEnd;
    impactEffect.Play();
    }
    lineEffect.SetPositions(new Vector3[] {lineStart,lineEnd});
    lightEffect.enabled = true;
    lineEffect.enabled = true;
    muzzleEfect.Play();
    Invoke("EndShootEffect",shootDuration);

}



void EndShootEffect()
{
    lightEffect.enabled = false;
    lineEffect.enabled = false;
}

    public override void ReceiveDamage(float damage, Vector3 damageDealerPosition)
    {
         if(!IsAlive) return;
        base.ReceiveDamage(damage, damageDealerPosition);
        animator.SetTrigger("Get Hit");
    }

}