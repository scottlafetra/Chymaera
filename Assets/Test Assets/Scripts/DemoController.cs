using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DemoController : MonoBehaviour
{
    public CreatureController playerCreature;
    public CreatureController AICreature;

    public PlayerHandler playerHandler;
    public PlayerHandler AIHandler;

    public void StartBattle()
    {
        // Prep both creatures for battle
        playerCreature.CurrentHealth = playerCreature.Health;
        AICreature.CurrentHealth = AICreature.Health;

        // Set up battle
        BattleController.creatures = new List<CreatureController> { playerCreature, AICreature };
        BattleController.handlers = new List<PlayerHandler> { playerHandler, AIHandler };
        BattleController.teamRanges = new List<int[]> { new int[] { 0, 0 }, new int[] { 1, 1 } }; // Set up two one creature teams      
        BattleController.callbackScene = "main";

        //Enter Battle
        SceneManager.LoadScene( "battle" );
    }
}
