using UnityEngine;
using System.Collections;

public class TargetProjectile : AbilityComponent {

    void Start()
    {

    }
	
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Expended)
        {
            return;
        }
        if (TargetTransform)
        {
            float dist = Vector3.Distance(transform.position, TargetTransform.transform.position);

            transform.position = Vector3.MoveTowards(transform.position, TargetTransform.transform.position, Time.fixedDeltaTime * Ability.AbilityBase.ProjectileSpeed);
            transform.LookAt(TargetTransform.transform);
            if (dist <= 0.7f)
            {
                Expended = true;
                ProjectileHit(Target);
            }
        }   
    }
}
