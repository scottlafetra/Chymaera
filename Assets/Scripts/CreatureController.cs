using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureController : MonoBehaviour
{
    // Signals for other scripts to act on
    public delegate void CreatureSignalHandler();
    public event CreatureSignalHandler damaged;
    public event CreatureSignalHandler healed;
    public event CreatureSignalHandler fainted;

    public enum StatusEffect
    {
        Burn, Confusion, Fear, Freeze, Irradiate, Poisioned
    }

    //TODO: replace test values
    List<StatusEffect> status = new List<StatusEffect>();
    public List<StatusEffect> Status // Multiple Statuses are allowed
    {
        get { return status; }
    }

    //TODO: replace test values
    public double AttackSpeed
    {
        get { return 7; }
    }

    //TODO: replace test values
    public int Health
    {
        get { return 77; }
    }

    //TODO: replace test values
    int currentHealth = 77;
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    // Methods for battle damage/heal
    public IEnumerator ApplyDamage(uint amount)
    {
        currentHealth -= (int)amount;
        currentHealth = Mathf.Max( 0, currentHealth );

        if( damaged != null )
        {
            damaged();
        }

        if( currentHealth <= 0 )
        {
            yield return Faint();
        }
    }

    public IEnumerator Heal(uint amount)
    {
        currentHealth += (int)amount;

        if( healed != null )
        {
            healed();
        }

        if( currentHealth >= Health )
        {
            currentHealth = Health;
        }
        yield return null;
    }

    //TODO replace test values
    public double Dodge
    {
        get { return 7; }
    }

    //TODO replace test values
    public double PhysicalDefense
    {
        get { return 7; }
    }

    //TODO: replace test values
    public double MagicDamage
    {
        get { return 7; }
    }

    //TODO: replace test values
    public double MagicDefense
    {
        get { return 7; }
    }

    //TODO: replace test values
    public double CriticalPercent
    {
        get { return 7; }
    }

    public string creatureName;
    public List<Action> moveSet;

    // Settable from inside the unity editor;
    [ Header( "Visuals" ) ]
    public GameObject frontImage;
    public GameObject backImage;

    void Start()
    {
        // We don't want our creatures to be reset between scenes!
        DontDestroyOnLoad( this );
    }

    public IEnumerator Faint()
    {
        // Tell everyone that we fainted
        if( fainted != null )
        {
            fainted();
        }
        yield return BattleController.instance.DisplayMessage( creatureName + " has fainted!" );

        // Switch out
        yield return BattleController.instance.CreatureToHandler( this ).SwitchCreature();
    }
}
