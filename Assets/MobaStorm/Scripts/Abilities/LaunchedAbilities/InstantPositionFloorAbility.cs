using UnityEngine;
using System.Collections;

public class InstantPositionFloorAbility : AbilityComponent {

    public override void OnStart()
    {
        base.OnStart();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Expended)
        {
            return;
        }
        Expended = true;
        ProjectileHit(null);
    }
}
