using UnityEngine;
using System.Collections.Generic;

public class AIHandler : PlayerHandler
{

    public override void ChooseAction()
    {
        // Choose randomly from the creature's moveset
        CreatureController creature = BattleController.instance.HandlerToCreature( this );
        Action move = creature.moveSet[ Random.Range( 0, creature.moveSet.Count - 1 ) ];

        // Action targets random opponent
        List<CreatureController> opponents = BattleController.instance.GetOpponents( creature );
        move.target = opponents[ Random.Range( 0, opponents.Count - 1 ) ];

        BattleController.instance.pendingActions.Push( move );

        // Tell the BattleController that we're done choosing
        ActionChosen( creature );
    }
}
