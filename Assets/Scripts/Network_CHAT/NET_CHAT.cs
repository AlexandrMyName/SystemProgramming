using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NET_CHAT : NetworkBehaviour
{

    private Dictionary<NetworkConnectionToClient, string> _chatConnections = new();
    [SerializeField] private Button SendButton;
    [SerializeField] private TMP_Text _messageContextTMP;
    [SerializeField] private TMP_InputField _messageField;
    [SerializeField] private Scrollbar _scrollbar;

    public static string LocalPlayerName;


    public void SendChatMessage()
    {
         
        CmdSendMessage(_messageField.text, connectionToClient);
        _messageField.text = "";
        _messageField.ActivateInputField();
    }


    public override void OnStartClient()
    {

        _messageContextTMP.text = string.Empty;
        base.OnStartClient();
    }


    public override void OnStartServer()
    {

        _chatConnections.Clear();
        base.OnStartServer();
    }


    private void Update()
    {
        
        if(_messageField.text != string.Empty)
        {
            if (Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.KeypadEnter) ||
                Input.GetButtonDown("Submit"))
            {
                SendChatMessage();
            }
        }
    }


    [Command(requiresAuthority = false)]
    private void CmdSendMessage(string message, NetworkConnectionToClient conn)
    {
        if (!_chatConnections.ContainsKey(conn))
        {
            _chatConnections.Add(conn, (string)conn.authenticationData);
        }
        
        if(message != string.Empty)
        {
            RpcSendMessage(message.Trim(), _chatConnections[conn]);
        }

    }


    [ClientRpc]
    private void RpcSendMessage(string message, string playerName)
    {
        string comletedMessage = playerName == LocalPlayerName ?
            $"<color=red>{playerName} </color> {message}" :
            $"<color=green>{playerName} </color> {message}";

        StartCoroutine(AppendMessage(comletedMessage));
    }

    private IEnumerator AppendMessage(string completedMessage)
    {

        yield return new WaitForSeconds(.3f);
        _messageContextTMP.text += completedMessage + "\n";

        yield return null;
        yield return null;

        _scrollbar.value = 0;
    }
}
