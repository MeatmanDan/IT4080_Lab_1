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
        SpawnAllPlayers();
    }

 //   private Player SpawnPlayerForClient(ulong clientId)
 //   {
        
   // }

    private void SpawnAllPlayers()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayerForClient(clientId);
        }
    }

    private Player SpawnPlayerForClient(ulong clientId)
    {
       
            Vector3 spawnPosition = new Vector3(0,1 ,clientId *5);
            Player player1 = Instantiate(playerfella, 
                spawnPosition, 
                Quaternion.identity);
            player1.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            return player1;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
