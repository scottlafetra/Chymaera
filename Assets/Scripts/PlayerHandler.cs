using UnityEngine;
using System.Collections;

public class PlayerHandler : MonoBehaviour
{
    public delegate void ActionChosenHandler(CreatureController actor);
    public event ActionChosenHandler actionChosen;

    // Battle Controller will subscribe us with this function
    public virtual void ChooseAction()
    {

    }

    // Advances any ongoing effects
    public virtual IEnumerator AdvanceTurn()
    {
        yield return null;
    }

    // Activate the event from a subclass
    protected void ActionChosen(CreatureController actor)
    {
        actionChosen( actor );
    }

    // Get a new creature from the player's party
    public virtual IEnumerator SwitchCreature()
    {
        // TODO: Allow player to switch out a creature
        BattleController.instance.RemoveCreature( BattleController.instance.HandlerToCreature( this ) );

        yield return null;
    }
}
