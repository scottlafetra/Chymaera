using UnityEngine;
using System.Collections.Generic;

public class CreatureController : MonoBehaviour
{
    public enum StatusEffect
    {
        Burn, Confusion, Fear, Freeze, Irradiate, Poisioned
    }

    //TODO: replace test values
    public List<StatusEffect> Status // Multiple Statuses are allowed
    {
        get { return new List<StatusEffect>();  }
    }

    //TODO: replace test values
    public double AttackSpeed
    {
        get { return 7; }
    }

    //TODO: replace test values
    public double Health
    {
        get { return 77; }
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
}
