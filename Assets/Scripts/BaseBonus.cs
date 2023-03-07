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
        Debug.Log("we started");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
           Destroy(this, 0); 
            Debug.Log("Collision ball 2 player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
