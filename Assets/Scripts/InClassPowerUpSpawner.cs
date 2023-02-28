using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//using UnityEditor.UIElements;

public class InClassPowerUpSpawner : NetworkBehaviour
{
    public bool spawnOnLoad = true;
    public float refreshTime = 2f;

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
        GameObject bonusSpawn = Instantiate(bonusPrefab, spawnPosition, Quaternion.identity);
        bonusSpawn.GetComponent<NetworkObject>().Spawn();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
