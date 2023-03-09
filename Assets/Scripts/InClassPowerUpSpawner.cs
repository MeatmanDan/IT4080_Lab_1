using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Unity.Netcode;
//using UnityEditor.UIElements;

public class InClassPowerUpSpawner : NetworkBehaviour
{
    public bool spawnOnLoad = true;
    public float refreshTime = 15f;
    public float count; 
    public GameObject bonusPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer && bonusPrefab != null)
        SpawnBonus();
    }


    private void SpawnBonus()
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 2;
        GameObject bonusSpawn = Instantiate(bonusPrefab, spawnPosition, Quaternion.identity);
        bonusSpawn.GetComponent<NetworkObject>().Spawn();
       // Destroy(bonusSpawn.gameObject, refreshTime); 
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
        count++; 
        checkCount();
    }
}
