using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class ArenaManager : NetworkBehaviour
{
    public Player playerfella; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

 //   private Player SpawnPlayerForClient(ulong clientId)
 //   {
        
   // }

    private void SpawnAllPlayers()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
           // SpawnPlayerForClient(clientId);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
