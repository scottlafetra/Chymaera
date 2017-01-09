using UnityEngine;
using System.Collections;

public class DamageAction : Action
{
    public int damageAmount;

    public override IEnumerator Act()
    {
        // TODO: Replace placeholder
        yield return BattleController.instance.messageBox.DisplayMessage( actor.creatureName + " uses " + actionName + " on " + target.GetComponent<CreatureController>().creatureName + "!" );

        yield return target.ApplyDamage( (uint)damageAmount );
    }
}
