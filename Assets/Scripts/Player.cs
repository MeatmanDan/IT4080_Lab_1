using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


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

    //  public float playerHealth = 5f;
    public Image healthBar;
    public Image healthBar2;
    public Image healthBar3;
    public Image shieldBar;
    public bool hasShield = true;
    public TMPro.TMP_Text scoreText;
    public bool isDead = false;


    public BulletSpawner newBullet;

    private static Color[] availColors = new Color[]
    {
        Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.yellow
    };

    private int hostColorIndex = 0;

    public NetworkVariable<Color> netPlayerColor = new NetworkVariable<Color>();
    public NetworkVariable<int> netScore = new NetworkVariable<int>();

    public NetworkVariable<int> netHealth = new NetworkVariable<int>();
    public NetworkVariable<Boolean> netShied = new NetworkVariable<Boolean>();
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
        netHealth.Value = 3;
        if (IsClient)
        {
            netHealth.OnValueChanged += OnHealthChanged;
            netScore.OnValueChanged += OnScoreChanged;
        }

        //netShied.OnValueChanged += OnShiedChanged;
        netShied.Value = true; 
    }

    public void ApplyPlayerColor()
    {
        transform.Find("Body").GetComponent<MeshRenderer>().material.color = netPlayerColor.Value;
        Debug.Log("Applying Player Color");
    }

    public void OnHealthChanged(int previous, int current)
    {
        string who = "";
        if (IsOwner)
        {
            who = "ME ";
        }
        else
        {
            who = $"{GetComponent<NetworkObject>().OwnerClientId}  ";
        }

        Debug.Log($"[{NetworkManager.LocalClientId}]{who}Health {previous} -> {current}");
        if (IsOwner)
        {
         UpdateDisplay(); 
        }

    }

   /* public void OnShiedChanged(bool prev, bool curr)
    {
        if (IsOwner)
        {
            if (netShied.Value)
            {
                shieldBar.fillAmount = 1f;
            }
            else
            {
                shieldBar.fillAmount = 0f;
            }
        }
    }
*/
    public void OnScoreChanged(int previous, int current)
    {
        if (IsOwner)
        {

           UpdateDisplay();
            string who = "";
            if (IsOwner)
            {
                who = "ME ";
            }
            else
            {
                who = $"{GetComponent<NetworkObject>().OwnerClientId}  ";
            }

            Debug.Log($"[{NetworkManager.LocalClientId}]{who}Score {previous} -> {current}");
        }

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

        Vector3 rotVect = new Vector3((-mouseDis.y * lookrotatespeed * Time.deltaTime),
            (mouseDis.x * lookrotatespeed * Time.deltaTime),
            (rollInput * rollSpeed * Time.deltaTime));
        // rotVect *= rotationSpeed * delta;
        return rotVect;

    }

    private void OwnerUpdate()
    {
        if (!isDead)
        {
            Vector3 moveBy = CalcMovementFromInput(Time.deltaTime);
            Vector3 rotateBy = CalcRotationFromInput(Time.deltaTime);
            requestmoveplayersServerRpc(moveBy, rotateBy);
            if (Input.GetButtonDown("Fire1"))
            {
                //RequestNextColorServerRpc();
                newBullet.FireServerRpc(Color.black);
                //  Debug.Log("Firing");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            {

                if (collision.gameObject.tag == "bullet")

                {
                    Debug.Log("player bullet Collision");
                    //playerscoreupdateServerRpc();
                    //  RequestNextColorServerRpc();
                    // takeplayerdamageServerRpc();
                    // Destroy(collision.gameObject);
                    ServerHandleBulletCollision(collision.gameObject);
                    //  clientId = NetworkManager.Singleton.LocalClientId;


                }


                if (collision.gameObject.tag == "pup")

                {
                    if (!netShied.Value)
                    {
                        netShied.Value = true;
                        shieldBar.fillAmount = 1;
                        Debug.Log("shield refill");
                    }
                    else
                    {
                        Debug.Log("power up pickup");


                        forwardspeed = 35f;
                        strafespeed = 15f;
                        hoverspeed = 10f;

                    }
                }
            }
        }
    }

    [ServerRpc]
    void requestmoveplayersServerRpc(Vector3 posChange, Vector3 rotChange, ServerRpcParams serverRpcParams = default)
    {
        transform.Translate(posChange);
        transform.Rotate(rotChange, Space.Self);

    }

    public void UpdateDisplay()
    {
        scoreText.SetText("");
        scoreText.SetText($"{netScore.Value.ToString()}");

        if (netHealth.Value == 2f)
        {
            healthBar3.fillAmount = netHealth.Value / 100f;
        }

        if (netHealth.Value == 1f)
        {
            healthBar2.fillAmount = netHealth.Value / 100f;
        }

        if (netHealth.Value == 0f)
        {
            healthBar.fillAmount = netHealth.Value / 100f;
            isDead = true;
            Debug.Log("dead");
        }
        
    }

    [ServerRpc]
    void takeplayerdamageServerRpc(ServerRpcParams serverRpcParams = default)
    {
        netHealth.Value = netHealth.Value - 1;
        healthBar.fillAmount = netHealth.Value / 100f;
        Debug.Log($"Player {serverRpcParams.Receive.SenderClientId} has {netHealth.Value}");





        if (netHealth.Value == 0)
        {
            Debug.Log($"Player {serverRpcParams.Receive.SenderClientId} has died");
        }
    }

    public void handleShield()
    {
        if (IsOwner)
        {
            shieldBar.fillAmount = 0;
        }
    }
    private void ServerHandleBulletCollision(GameObject bullet)
    {
        Bullet BulletSpawner = bullet.GetComponent<Bullet>();
        // netHealth.Value -= 1;
        // Cannot call RPC because we might not be the owner and the RPC
        // requires ownership.  We are on the host, so we can just do the
        // color change instead of requesting it.
        // ServerNextColor();

        ulong owner = bullet.GetComponent<NetworkObject>().OwnerClientId;

        Player otherPlayer =
            NetworkManager.Singleton.ConnectedClients[owner].PlayerObject.GetComponent<Player>();
        otherPlayer.netScore.Value += 1;
        ServerHandleMinusHealth();
        Debug.Log($"point for {owner}");
        Debug.Log($"server:  [{owner}] shot [{GetComponent<NetworkObject>().OwnerClientId}]");

        Destroy(bullet);
    }

    private void ServerHandleMinusHealth()
    {
        
            if (!netShied.Value)
            {
                netHealth.Value -= 1;
                
            }
            else
            {
                netShied.Value = false;
                handleShield();
                
            }

            Debug.Log($"[{NetworkManager.Singleton.LocalClientId}] health = {netHealth.Value}");
    }


    [ServerRpc]
    void playerscoreupdateServerRpc(ServerRpcParams serverRpcParams = default)
    {
        netScore.Value = netScore.Value + 1;
        Debug.Log($"Score = {netScore.Value} for {serverRpcParams.Receive.SenderClientId}");
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