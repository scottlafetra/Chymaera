using UnityEngine;
using System.Collections;

public abstract class Action : MonoBehaviour
{
    // Should be set when choosing action or placing it on the pending actions stack
    // Usually, the actor is set in the editor
    public CreatureController target;
    public CreatureController actor;

    public string actionName;

    public abstract IEnumerator Act();
}
