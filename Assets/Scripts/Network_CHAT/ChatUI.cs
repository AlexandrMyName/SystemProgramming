using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ChatSystem
{

    public class ChatUI : NetworkBehaviour
    {

        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _sendButton;
        [SerializeField] private Scrollbar _scrollbar;
        [SerializeField] private TMP_InputField _msgInputField;

        public static string LocalPlayerName;

        public static readonly Dictionary<NetworkConnectionToClient, string> ConnNames
            = new Dictionary<NetworkConnectionToClient, string>();


        public override void OnStartServer()
        {
            ConnNames.Clear();
        }

        public override void OnStartClient()
        {

            _text.text = "";
            //_msgInputField.ActivateInputField();
        }


        public void OnButtonSendClick()
        {

            if(
                Input.GetKeyDown(KeyCode.Return)  || 
                Input.GetKeyDown(KeyCode.KeypadEnter) || 
                Input.GetButtonDown("Submit")){

                Send();
            }
        }
 
        public void Send()
        {

            CmdSendMessage(_msgInputField.text, connectionToClient);
            _msgInputField.text = "";
            _msgInputField.ActivateInputField();
        }


        [Command(requiresAuthority = false)]
        private void CmdSendMessage(string msg, NetworkConnectionToClient networkConnection)
        {
             
            if (!ConnNames.ContainsKey(networkConnection))
            {
                ConnNames.Add(networkConnection, (string) networkConnection.authenticationData);
            }
            
            if(!string.IsNullOrEmpty(msg)){

                RpcSendMessage(msg.Trim(), ConnNames[networkConnection]);
            }
        }


        [ClientRpc]
        private void RpcSendMessage(string msg, string playerName)
        {

            string completeMessage = playerName == LocalPlayerName ?
                 $"<color=red>{playerName}</color>:  {msg}" :
                 $"<color=blue>{playerName}</color> {msg}";
           // Debug.Log(completeMessage);
            StartCoroutine(AppendMessage(completeMessage));
        }


        private IEnumerator AppendMessage(string completionMessage)
        {

            yield return new WaitForSeconds(0.5f);
            _text.text += completionMessage + "\n";

            yield return null; 
            yield return null;

            _scrollbar.value = 0;
        }
    }
}