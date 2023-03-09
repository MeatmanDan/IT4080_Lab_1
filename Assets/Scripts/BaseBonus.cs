using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Netcode;

public class BaseBonus : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       // Debug.Log("we started");
    } 
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision ball 2 player");
        if (IsServer)
        {
            Debug.Log("Collision ball 2 player");
            Destroy(gameObject); 
            Debug.Log("Collision ball 2 player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
