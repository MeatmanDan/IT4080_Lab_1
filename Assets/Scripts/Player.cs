using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class Player : NetworkBehaviour
{
    public float movementSpeed = 50f;

    public float rotationSpeed = 50f;

    private Camera _camera;
 
   public BulletSpawner newBullet; 

    private static Color[] availColors = new Color[]
    {
        Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.yellow
    };

    private int hostColorIndex = 0;

    public NetworkVariable<Color> netPlayerColor = new NetworkVariable<Color> ();
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        _camera = transform.Find("Camera").GetComponent<Camera>();
        _camera.enabled = IsOwner;
        netPlayerColor.OnValueChanged += OnPlayerColorChanged;
    }

    public void ApplyPlayerColor()
    {
        transform.Find("Body").GetComponent<MeshRenderer>().material.color = netPlayerColor.Value;
        Debug.Log("Applying Player Color");
    }

    public void OnPlayerColorChanged(Color previous, Color current)
    {
        ApplyPlayerColor();
    }
    

    private Vector3 CalcMovementFromInput(float delta)
    {
        bool isShiftKeyDown = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
        float x_move = 0.0f;
        float Z_move = Input.GetAxis("Vertical");
       // if(isShiftKeyDown)
      //  {
            x_move = Input.GetAxis("Horizontal");
      //  }
        Vector3 moveVect = new Vector3(x_move, 0, Z_move);
        moveVect *= movementSpeed * delta;
        return moveVect;
    }

    private Vector3 CalcRotationFromInput(float delta)
    {
        bool isShiftKeyDown = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
        float y_rot = 0.0f;
      //  float y_move = Input.GetAxis("Horizontal"); 
       // if (isShiftKeyDown)
      // {
          //  y_rot = Input.GetAxis("Horizontal");
       // }

        Vector3 rotVect = new Vector3(0, y_rot, 0);
        rotVect *= rotationSpeed * delta;
        return rotVect; 
    }

    private void OwnerUpdate()
    {
        Vector3 moveBy = CalcMovementFromInput(Time.deltaTime);
        Vector3 rotateBy = CalcRotationFromInput(Time.deltaTime);
        requestmoveplayersServerRpc(moveBy,rotateBy);
        if (Input.GetButtonDown("Fire1"))
        {
            //RequestNextColorServerRpc();
            newBullet.FireServerRpc(Color.black);
            Debug.Log("Firing");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Power up")
        {
            RequestNextColorServerRpc();
            Debug.Log("Collision");
        }
    }

    [ServerRpc]
    public void requestmoveplayersServerRpc(Vector3 posChange, Vector3 rotChange, ServerRpcParams serverRpcParams = default)
    {
        transform.Translate(posChange);
        transform.Translate(rotChange);
        
    }

    [ServerRpc]
    void RequestNextColorServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("calling server RPC");
        hostColorIndex += 1;
        if (hostColorIndex > availColors.Length - 1)
        {
            hostColorIndex = 0;
        }
        Debug.Log($"Host colors index = {hostColorIndex} for {serverRpcParams.Receive.SenderClientId}");
        netPlayerColor.Value = availColors[hostColorIndex];
    }
    

    // Update is called once per frame
    void Update()
    {
        
        if (IsOwner)
        {
            OwnerUpdate();
        }
        
        
    }
}
