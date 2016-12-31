using UnityEngine;
using System.Collections;

public class DamageAction : Action
{
    public int damageAmount;

    public override IEnumerator Act()
    {
        // TODO: Replace placeholder
        Debug.Log( "[Damage Action \"" + actionName + "\" with " + damageAmount + " damage , targeting \""
            + target.GetComponent<CreatureController>().creatureName + "\" acting]" );
        yield return new WaitForSeconds( 1 );
        yield return target.ApplyDamage( (uint)damageAmount );
    }
}
