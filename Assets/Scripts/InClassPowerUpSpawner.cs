using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Unity.Netcode;
//using UnityEditor.UIElements;

public class InClassPowerUpSpawner : NetworkBehaviour
{
    public bool spawnOnLoad = true;
    public float timeUntilSpawn = 0;
    public float spawnDelay = 15f;
    public float count; 
    public GameObject bonusPrefab;
    public GameObject currPowerUp;

    public override void OnNetworkSpawn()
    {
        //OnNetworkSpawn();
        if(IsServer && bonusPrefab != null)
        SpawnBonus();
    }


   /* private void SpawnBonus()
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 2;
        GameObject bonusSpawn = Instantiate(bonusPrefab, spawnPosition, Quaternion.identity);
        bonusSpawn.GetComponent<NetworkObject>().Spawn();
       // Destroy(bonusSpawn.gameObject, refreshTime); 
    }
    */
   
   private void SpawnBonus() {
       Vector3 spawnPosition = transform.position;
       spawnPosition.y = 3; 
       GameObject pu = Instantiate(bonusPrefab, spawnPosition, Quaternion.identity);
       pu.GetComponent<NetworkObject>().Spawn();
       currPowerUp = pu;
   }
   private void ServerUpdate()
   {
       if (timeUntilSpawn > 0f) {
           timeUntilSpawn -= Time.deltaTime;
           if (timeUntilSpawn <= 0) {
               SpawnBonus();
           }
       } else if(currPowerUp == null){
           Debug.Log("Powerup is null");
           timeUntilSpawn = spawnDelay;
       }
   }


    private void checkCount()
    {
        return;
        if (count > 350f)
        {
            SpawnBonus();
            count = 0;
        }

    }
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ServerUpdate();
    }
}
