using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class Player : NetworkBehaviour
{
    private Camera _camera;
    public float forwardspeed = 25f, strafespeed = 7.5f, hoverspeed = 5;
    private float activeforwardspeed, activestrafespeed, activehoverspeed; 
    public int score = 0;
    public float movementSpeed = 50f;
    public float lookrotatespeed = 90f;
    private Vector2 lookInput, screenCenter, mouseDis; 
    public float rotationSpeed = 50f;
    private float rollInput;
    public float rollSpeed = 90f;
    public float rollAcceleration = 3.5f;

    
 
   public BulletSpawner newBullet; 

    private static Color[] availColors = new Color[]
    {
        Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.yellow
    };

    private int hostColorIndex = 0;

    public NetworkVariable<Color> netPlayerColor = new NetworkVariable<Color> ();
    public NetworkVariable<int> netScore = new NetworkVariable<int>();
    public NetworkVariable<int> netHealth = new NetworkVariable<int>(); 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        _camera = transform.Find("Camera").GetComponent<Camera>();
        _camera.enabled = IsOwner;
        screenCenter.x = Screen.width * .5f;
        screenCenter.y = Screen.height * .5f;
        Cursor.lockState = CursorLockMode.Confined;
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
       // bool isShiftKeyDown = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
        //float x_move = 0.0f;
       // float Z_move = Input.GetAxis("Vertical");
       // if(isShiftKeyDown)
      //  {
         //   x_move = Input.GetAxis("Horizontal");
      //  }
        //Vector3 moveVect = new Vector3(x_move, 0, Z_move);
        //moveVect *= movementSpeed * delta;
        //return moveVect;
      //  Vector3 moveVect = new Vector3(0, 0, 0); 
        activeforwardspeed = Input.GetAxisRaw("Vertical") * forwardspeed;
        activestrafespeed = Input.GetAxisRaw("Horizontal") * strafespeed;
        activehoverspeed = Input.GetAxisRaw("Hover") * hoverspeed;
        Vector3 moveVect = new Vector3((activestrafespeed * Time.deltaTime),
                                         (activehoverspeed * Time.deltaTime), (activeforwardspeed * Time.deltaTime));
        return moveVect;
    }

    private Vector3 CalcRotationFromInput(float delta)
    {
      //  bool isShiftKeyDown = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
        float y_rot = 0.0f;
      //  float y_move = Input.GetAxis("Horizontal"); 
       // if (isShiftKeyDown)
      // {
          //  y_rot = Input.GetAxis("Horizontal");
       // }


       lookInput.x = Input.mousePosition.x;
       lookInput.y = Input.mousePosition.y;
       mouseDis.x = (lookInput.x - screenCenter.x) / screenCenter.y;
       mouseDis.y = (lookInput.y - screenCenter.y) / screenCenter.x;
       mouseDis = Vector2.ClampMagnitude(mouseDis, 1);
       rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

        Vector3 rotVect = new Vector3((-mouseDis.y * lookrotatespeed * Time.deltaTime), (mouseDis.x * lookrotatespeed * Time.deltaTime), 
           (rollInput *rollSpeed *Time.deltaTime) );
       // rotVect *= rotationSpeed * delta;
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
    void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.tag == "bullet")
            {
                Debug.Log( "player bullet Collision");
                playerscoreupdateServerRpc();
              //  RequestNextColorServerRpc();
                Destroy(collision.gameObject);
                
              //  clientId = NetworkManager.Singleton.LocalClientId;
;
            }

            if (collision.gameObject.tag == "pup")
            {
                Debug.Log("power up pickup");
                for(int i =0; i <250; i++)
                {
                    forwardspeed = 35f;
                    strafespeed = 15f;
                    hoverspeed = 10f;
                }
                Debug.Log("power up over");
                forwardspeed = 25f;
                strafespeed = 7.5f;
                hoverspeed = 5;
            }
        }
    }

    [ServerRpc]
    public void requestmoveplayersServerRpc(Vector3 posChange, Vector3 rotChange, ServerRpcParams serverRpcParams = default)
    {
        transform.Translate(posChange);
        transform.Rotate(rotChange, Space.Self);

    }

    [ServerRpc]
    void playerscoreupdateServerRpc(ServerRpcParams serverRpcParams = default)
    {
        score++;
        Debug.Log($"Score = {score} for {serverRpcParams.Receive.SenderClientId}");
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
