using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageBoxController : MonoBehaviour
{
    public Text messageText;
    public float timeBetweenChars;
    public float timeAfterMessage;
    
    void Start () {
        // Hide the message box
        gameObject.SetActive( false );
    }

    //Displays some message to the user
    public IEnumerator DisplayMessage(string message)
    {
        // Prep the message box
        messageText.text = "";
        gameObject.SetActive( true );

        for( int i = 0; i < message.Length; ++i )
        {
            messageText.text += message[ i ];
            yield return new WaitForSeconds( timeBetweenChars );
        }

        yield return new WaitForSeconds( timeAfterMessage );

        // Clean up
        gameObject.SetActive( false );

        yield return new WaitForSeconds( 0.5f ); // To precent flashing
    }
}
