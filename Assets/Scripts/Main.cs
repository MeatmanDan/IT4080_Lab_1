using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP; 
public class Main : NetworkBehaviour 
{
    public It4080.NetworkSettings netSettings;
    // Start is called before the first frame update
    void Start()
    {
        netSettings.startServer += NetSettingsOnServerStart;
        netSettings.startHost += NetSettingsOnHostStart;
        netSettings.startClient += NetSettingsOnClientStart;
    }

    private void startClient(IPAddress ip, ushort port)
    {
        var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
      utp.ConnectionData.Address = ip.ToString();
      utp.ConnectionData.Port = port;
      NetworkManager.Singleton.StartClient();
      netSettings.hide();
      Debug.Log("started client");


    }

    private void startHost(IPAddress ip, ushort port)
    {
        var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.ConnectionData.Address = ip.ToString();
        utp.ConnectionData.Port = port;

        NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected; 
        NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;  
        
        
        NetworkManager.Singleton.StartHost();
        netSettings.hide();
        Debug.Log("started host");
    }

    private void startServer(IPAddress ip, ushort port)
    {
        var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.ConnectionData.Address = ip.ToString();
        utp.ConnectionData.Port = port;
        
        NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected; 
        NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected; 
        
        NetworkManager.Singleton.StartServer(); 
        netSettings.hide();
        Debug.Log("started server");
    }
    private void printIs(string msg)
    {
        Debug.Log($"server:{IsServer} host:{IsHost} client:{IsClient} owner:{IsOwner}");
    } 
    // ----------
    //Events 
    private void HostOnClientConnected(ulong clientId)
    {
        Debug.Log($"Client Connected: {clientId}");
    }
    private void HostOnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client Disconnected: {clientId}");
    }
    private void NetSettingsOnClientStart(IPAddress ip, ushort port)
    {
        startClient(ip,port);
    }
    private void NetSettingsOnHostStart(IPAddress ip, ushort port)
    {
        startHost(ip, port); 
    }
    private void NetSettingsOnServerStart(IPAddress ip, ushort port)
    {
        startServer(ip,port);
    }

    
    
    //Update 
    void Update()
    { 
        
    }
}
