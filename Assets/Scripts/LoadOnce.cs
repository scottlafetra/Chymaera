using UnityEngine;
using System.Collections.Generic;

public class LoadOnce : MonoBehaviour
{

    public List<GameObject> loadOnce;
    public List<Transform> loadTransforms;
    private static List<GameObject> instances = new List<GameObject>();

    private static bool loaded = false;

    void Awake()
    {
        if( loaded )
        {// Set transforms
            for( int i = 0; i < instances.Count; ++i )
            {
                instances[ i ].transform.position = loadTransforms[ i ].position;
                instances[ i ].transform.rotation = loadTransforms[ i ].rotation;
            }
        }
        else
        {
            for( int i = 0; i < loadOnce.Count; ++i )
            {
                GameObject instance = (GameObject)Instantiate( loadOnce[ i ], transform.position, transform.rotation );
                
                instance.transform.position = loadTransforms[ i ].position;
                instance.transform.rotation = loadTransforms[ i ].rotation;

                instances.Add( instance );

                //Make sure it stays loaded
                DontDestroyOnLoad( instance );
            }

            loaded = true;
        }
    }

    public GameObject GetInstance( int instanceIndex )
    {
        return instances[ instanceIndex ];
    }
}
