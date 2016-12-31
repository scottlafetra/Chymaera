using UnityEngine;
using System.Collections;

public class Bobs : MonoBehaviour {

    public float offset = 0;
    public float period = 1;
    public float amplitude = 1;

    void Start()
    {
        transform.position = new Vector3( transform.position.x,
                                          transform.position.y + amplitude * Mathf.Cos( ( 2 * Mathf.PI ) * ( ( Time.time + offset ) / period ) ),
                                          transform.position.z );
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate( new Vector3( 0,
                                          Time.deltaTime * amplitude * Mathf.Cos( ( 2 * Mathf.PI ) * ( ( Time.time + offset ) / period) ),
                                          0 ) );
	}
}
