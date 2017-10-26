using UnityEngine;
using System.Collections;

public class BasicFrontAbility : AbilityComponent {

    private Vector3 direction;
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
        
        //transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * AbilityBase.ProjectileSpeed);


        float dist = Vector3.Distance(transform.position, Attacker.transform.position);
        transform.Translate(Vector3.forward * (Time.fixedDeltaTime * Ability.AbilityBase.ProjectileSpeed), Space.Self);
        //transform.position += direction.normalized * (Time.fixedDeltaTime * AbilityBase.ProjectileSpeed);
        //transform.position = Vector3.MoveTowards(transform.position, TargetTransform.transform.position, Time.fixedDeltaTime * AbilityBase.ProjectileSpeed);

        if (dist >= Ability.Range)
        {
            Expended = true;
            AbilityMiss();
        }
          
    }

    public override void OnStart()
    {
        base.OnStart();
        transform.rotation = Attacker.transform.rotation;
        //Vector3 destination = new Vector3(TargetPos.x, transform.position.y, TargetPos.z);
        //direction = destination - transform.position;
    }
}
