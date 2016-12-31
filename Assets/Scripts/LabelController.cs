using UnityEngine;
using UnityEngine.UI;

public class LabelController : MonoBehaviour
{

    public CreatureController toTrack;
    public Image hpFillBar;
    public Text nameText;

    void Start()
    {
        // Ensure that we're alerted when Damage is dealt
        toTrack.damaged += new CreatureController.CreatureSignalHandler( OnHealthChange );
        toTrack.healed += new CreatureController.CreatureSignalHandler( OnHealthChange );

        // Update the title feilds
        nameText.text = toTrack.name;

        // Update stats
        OnHealthChange();
    }

    public void OnHealthChange()
    {
        hpFillBar.fillAmount = toTrack.CurrentHealth / (float)toTrack.Health;
    }
}
