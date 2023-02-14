using System.Collections;
using System.Collections.Generic;
using It4080;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ChatServer : NetworkBehaviour
{
    // Start is called before the first frame update
    public It4080.Chat chat;
    public Chat.ChatMessage chatm; 
    public TMPro.TMP_Text txtChatLog;
    private ulong[] singleClientId = new ulong[1];
    private ulong[] dmClientIds = new ulong[2];
    void Start()
    { 
        
        
    }

    [ClientRpc]
    public void SendChatMessageClientRpc(string message, ClientRpcParams clientRpcParams = default)
    {
        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = singleClientId;
        chatm = new Chat.ChatMessage();
       // chatm.to = singleClientId[0].ToString();
        //chatm.from = NetworkManager.Singleton.LocalClientId.ToString();
        chatm.message = message;
        chat.ShowMessage(chatm);
        
        
    }

    private void SendDirectMessage(string message, ulong from, ulong to)
    {
        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = singleClientId;

        singleClientId[0] = from; 
        SendChatMessageClientRpc($"[you] {message}", rpcParams);

        singleClientId[0] = to;
        SendChatMessageClientRpc($"<whisper> {message}", rpcParams);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessageServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"Host got message: {message}");
        if (message.StartsWith("@"))
        {
            string[] parts = message.Split(" ");
            string clientIdStr = parts[0].Replace("@", "");
            ulong toClientId = ulong.Parse(clientIdStr);

            SendDirectMessage(message, serverRpcParams.Receive.SenderClientId, toClientId);
        }
        else
        {
            SendChatMessageClientRpc(message); 
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void RequestSendChatMessageServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
        
        chat.SystemMessage(message);
    }

    public void DisplayMessageLocally(string message)
    {
        Debug.Log(message);
        txtChatLog.text += $"\n{message}";
    }
    [ServerRpc(RequireOwnership = false)]
    public void SendSystemMessageServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
       RequestSendChatMessageServerRpc(message);
    }
    


    // Update is called once per frame
    void Update() 
    {
        
    }
}
