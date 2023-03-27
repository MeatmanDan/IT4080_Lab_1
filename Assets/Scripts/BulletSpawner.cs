using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
    public Rigidbody bullet;

    private float bulletSpeed = 40f;

    private float timeToLive = 3f;

    [ServerRpc]
    public void FireServerRpc(Color colo, ServerRpcParams rpcParams = default)
    {
        Rigidbody newBullet = Instantiate(bullet, transform.position, transform.rotation);
        newBullet.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        newBullet.velocity = transform.forward * bulletSpeed;
        Destroy(newBullet.gameObject, timeToLive);
    }
    
    void OnCollisionEnter(Collision collision)
    {
       // if (IsServer)
       // {
           // if (collision.gameObject.tag == "bullet")
            //{
            Destroy(collision.gameObject,0f);
                Debug.Log( "Bullet Collision");
               // RequestNextColorServerRpc();
                //;
                
            //}
       // }
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
