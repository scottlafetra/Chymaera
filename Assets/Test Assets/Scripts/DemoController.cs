using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DemoController : MonoBehaviour
{
    // Set by fetching from loader [ index ]
    private CreatureController playerCreature; // [ 0 ]
    private CreatureController AICreature;     // [ 1 ]

    private PlayerHandler playerHandler;       // [ 2 ]
    private PlayerHandler AIHandler;           // [ 3 ]

    public LoadOnce loader;

    void Start()
    {
        playerCreature = loader.GetInstance( 0 ).GetComponent<CreatureController>();
        AICreature     = loader.GetInstance( 1 ).GetComponent<CreatureController>();
        playerHandler  = loader.GetInstance( 2 ).GetComponent<PlayerHandler>();
        AIHandler      = loader.GetInstance( 3 ).GetComponent<PlayerHandler>();
    }

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
