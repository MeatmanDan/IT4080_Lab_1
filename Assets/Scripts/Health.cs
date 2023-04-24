using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
public int maxhealth =3;

public int currenthealth; 
    // Start is called before the first frame update
    void Start()
    {
        currenthealth = maxhealth;
    }

    public void takeDamage(int dmg)
    {
        currenthealth -= dmg;
        Debug.Log($"current health {currenthealth}" );
        
        if (currenthealth <= 0)
        {
            Debug.Log("Dead");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
