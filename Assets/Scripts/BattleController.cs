using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;

public class BattleController : MonoBehaviour
{

    [ Header( "Setup" ) ]
    public float creatureSpacing;
    public float distaceFromCenter;

    // There should only be one battle controller active at a time
    public static BattleController instance = null;

    public enum BattleState
    {
        Normal, // Nothing is being waited on
        AwaitingActions, // Battlecontroller is waiting for actions to be chosen
        Finished
    }

    // The current state the battle controller is in
    private BattleState state;
    public BattleState State
    {
        get { return state; }
    }

    // Should be set up before scene is started
    // Creatures should be grouped by who controls them
    // indicies of the two lists corespond with eachother
    public static List<PlayerHandler> handlers = new List<PlayerHandler>(); // Who is controlling each creature
    public static List<CreatureController> creatures = new List<CreatureController>();
    public static List<int[]> teamRanges; // 0th element is min (inclusive) and 1st element is max(inclusive)
    public static string callbackScene; // The scene to return to after battle
    public const int playerTeam = 0; // Const for now, might change in multiplayer

    // Will be -1 if in any state but Finished
    public static int victorousTeam = -1;

    // Actions that still need to be performed
    // Should be set by action chooseing, removed by us when performed
    // Will be ordered if in the "Normal" state
    public Stack<Action> pendingActions = new Stack<Action>();

    // To allow other handlers to process Action choices asyncrounously
    public delegate void ChooseActionHandler();
    public event ChooseActionHandler chooseAction;

    // Who still needs to submit an action
    // Indicies corespond to the creatures and controllers lists
    private List<bool> waitingOn = new List<bool>();

    [ Header( "Screen Positioning" ) ]
    public RectTransform canvasRect;
    public Camera mainCamera;

    [ Header( "Labels" ) ]
    public GameObject label;
    public float labelSpaceing;
    public float contentOffset;

    public MessageBoxController messageBox;

    // Use this for initialization
    void Start()
    {
        instance = this;

        //Add a slot for each creature in the battle
#       pragma warning disable 0219 // "creature" is not used
        foreach( CreatureController creature in creatures )
        {
            waitingOn.Add( true );
        }
#       pragma warning restore 0219

        // Subscribe to things
        foreach( PlayerHandler controller in handlers )
        {
            controller.actionChosen += new PlayerHandler.ActionChosenHandler( ActionChosen );
            chooseAction += new ChooseActionHandler( controller.ChooseAction );
        }

        PositionCreatures();

        // Attach Labels to creatures
        foreach( CreatureController creature in creatures )
        {
            LabelController newLabel = Instantiate( label ).GetComponent<LabelController>();
            newLabel.gameObject.transform.parent = canvasRect.gameObject.transform; // set up as child of canvas

            newLabel.toTrack = creature;
            PositionLabelOn( creature, newLabel.gameObject );
        }

        StartTurn();
    }

    private IEnumerator RunTurn()
    {
        yield return ProcessActions();

        // Combat will end if someone won
        if( state == BattleState.Finished )
        {
            yield return EndCombat();
        }

        yield return AdvanceTurn();

        if( state == BattleState.Finished )
        {
            yield return EndCombat();
        }

        // Start the next turn
        StartTurn();
    }

    private void PositionLabelOn(CreatureController creature, GameObject label)
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint( creature.gameObject.transform.position );
        Vector2 screenPos = new Vector2( ( ( viewportPos.x * canvasRect.sizeDelta.x ) - ( canvasRect.sizeDelta.x * 0.5f ) ),
                                           ( ( viewportPos.y * canvasRect.sizeDelta.y ) - ( canvasRect.sizeDelta.y * 0.5f ) ) );

        RectTransform labelRect = label.transform.GetComponent<RectTransform>();
        labelRect.anchoredPosition = screenPos;

        // Setup some variables for spacing
        float xOffset = -labelSpaceing;

        // Prefab is for  the right, so we need to change values if the tag should float left
        if( screenPos.x < 0 ) // if one the left
        {
            // flip label and positioning
            xOffset *= -1;

            labelRect.rotation = new Quaternion();
            RectTransform contentRect = label.transform.Find( "Content" ).GetComponent<RectTransform>();
            contentRect.rotation = Quaternion.Euler( 0, 180, 0 );
            contentRect.anchoredPosition = new Vector3( contentOffset, 0, 0 );
        }

        labelRect.anchoredPosition = new Vector2( labelRect.anchoredPosition.x + xOffset, labelRect.anchoredPosition.y );
    }

    private void PositionCreatures()
    {
        if( teamRanges.Count == 2 )
        {
            // Team 0
            int teamSize = teamRanges[ 0 ][ 1 ] - teamRanges[ 0 ][ 0 ];
            for( int i = teamRanges[ 0 ][ 0 ]; i < teamSize; ++i )
            {
                creatures[ teamRanges[ 0 ][ 0 ] + i ].transform.position = new Vector3(
                                                                            distaceFromCenter,
                                                                            0,
                                                                            -creatureSpacing * ( teamSize - 1 ) + i * 2 * creatureSpacing
                                                                            );
            }


            // Team 1
            teamSize = teamRanges[ 0 ][ 1 ] - teamRanges[ 0 ][ 0 ];
            for( int i = teamRanges[ 0 ][ 0 ]; i < teamSize; ++i )
            {
                creatures[ teamRanges[ 0 ][ 0 ] + i ].transform.position = new Vector3(
                                                                            distaceFromCenter,
                                                                            0,
                                                                            -creatureSpacing * ( teamSize - 1 ) + i * 2 * creatureSpacing
                                                                            );
            }
        }
        else
        {
            // TODO: Setup more than two teams
            Debug.LogError( "Code not implemented!" );
        }
    }

    private void StartTurn()
    {
        // Send out the call for action(s)
        state = BattleState.AwaitingActions;
        ResetWaitingOn();
        chooseAction();
    }

    //Resets the waiting on queue
    private void ResetWaitingOn()
    {
        waitingOn.Clear();
#       pragma warning disable 0219 // "creature" is not used
        foreach( CreatureController creature in creatures )
        {
            waitingOn.Add( true );
        }
#       pragma warning restore 0219
    }

    

    public void ActionChosen(CreatureController creature)
    {
        waitingOn[ creatures.IndexOf( creature ) ] = false;

        // Check if there are still creatures that we're waiting on
        bool stillWaiting = false;
        foreach( bool waiting in waitingOn )
        {
            stillWaiting |= waiting;
        }

        if( !stillWaiting )
        {
            state = BattleState.Normal;
            StartCoroutine( RunTurn() );
        }
    }

    // Processes each action in a coroutine way
    private IEnumerator ProcessActions()
    {
        while( pendingActions.Count > 0 )
        {
            yield return pendingActions.Pop().Act();
        }
    }

    // Advances to the next turn
    private IEnumerator AdvanceTurn()
    {
        foreach( PlayerHandler controller in handlers )
        {
            yield return StartCoroutine( controller.AdvanceTurn() );
        }
    }


    //Conversion functions
    public CreatureController HandlerToCreature(PlayerHandler handler)
    {
        return creatures[ handlers.IndexOf( handler ) ];
    }

    public PlayerHandler CreatureToHandler(CreatureController creature)
    {
        return handlers[ creatures.IndexOf( creature ) ];
    }

    // Returns -1 if team not found
    public int GetTeamOf(CreatureController creature)
    {
        int creatureNum = creatures.IndexOf( creature );
        int teamNum = -1;
        for( int i = 0; i < teamRanges.Count; ++i )
        {
            if( creatureNum >= teamRanges[ i ][ 0 ] && creatureNum <= teamRanges[ i ][ 1 ] )
            {
                teamNum = i;
                break;
            }
        }

        return teamNum;
    }

    // For enemy AI
    public List<CreatureController> GetOpponents(CreatureController creature)
    {
        int teamNum = GetTeamOf( creature );

        // Remove creature's team from a copy of the list 
        List<CreatureController> opponents = new List<CreatureController>( creatures );
        opponents.RemoveRange( teamRanges[ teamNum ][ 0 ], teamRanges[ teamNum ][ 1 ] - teamRanges[ teamNum ][ 0 ] + 1 );

        return opponents;
    }

    // Swap a creature out (handler is not changed)
    public void SwitchCreature(CreatureController remove, CreatureController add)
    {
        creatures[ creatures.IndexOf( remove ) ] = add;
    }

    // Add a new creature (handler is added) 
    public void AddCreature(CreatureController creature, PlayerHandler handler, int team)
    {
        int teamNum = GetTeamOf( creature );

        // Insert creature and handler at back of team
        creatures.Insert( teamRanges[ teamNum ][ 1 ], creature );
        handlers.Insert( teamRanges[ teamNum ][ 1 ], handler );

        // Increase team size by one
        teamRanges[ teamNum ][ 1 ]++;

        // Scoot all team ranges up one
        for( int i = teamNum + 1; i < teamRanges.Count; ++i )
        {
            teamRanges[ i ][ 0 ]++;
            teamRanges[ i ][ 1 ]++;
        }
    }

    // remove a creature (handler is removed) 
    public void RemoveCreature(CreatureController creature)
    {
        int teamNum = GetTeamOf( creature );
        teamRanges[ teamNum ][ 1 ]--;

        // Scoot all team ranges down one
        for( int i = teamNum + 1; i < teamRanges.Count; ++i )
        {
            teamRanges[ i ][ 0 ]--;
            teamRanges[ i ][ 1 ]--;
        }

        // Remove creature and handler
        int creatureIndex = creatures.IndexOf( creature );
        creatures.RemoveAt( creatureIndex );
        handlers.RemoveAt( creatureIndex );

        CheckForFinished();
    }

    // Will update the state of the game to finished if only one team stands
    public void CheckForFinished()
    {
        int teamsStanding = 0;
        int winningTeam = -1;

        for( int i = 0; i < teamRanges.Count; ++i )
        {
            if( teamRanges[ i ][ 0 ] <= teamRanges[ i ][ 1 ] )
            {
                teamsStanding++;
                winningTeam = i;

                // Check for continue fight
                if( teamsStanding >= 2 )
                {
                    return;
                }
            }
        }

        victorousTeam = winningTeam;
        state = BattleState.Finished;
        // No more actions may be taken
        pendingActions.Clear();
    }

    private IEnumerator EndCombat()
    {
        if( victorousTeam == playerTeam )// If the player lost
        {
            yield return messageBox.DisplayMessage( "You have won the battle!" );
        }
        else
        {
            yield return messageBox.DisplayMessage( "You have lost the battle..." );
        }

        yield return new WaitForSeconds( 3 );
        SceneManager.LoadScene( callbackScene );
    }
}
