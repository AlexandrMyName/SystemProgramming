using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ChatSystem
{

    public class Player : NetworkBehaviour
    {

        public static readonly HashSet<string> PlayerNames = new HashSet<string>();

        [SyncVar]
        public string PlayerName;

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        public void OnResetStatics()
        => PlayerNames.Clear();
       

        public override void OnStartServer()
        => PlayerName = (string) connectionToClient.authenticationData;
        

        public override void OnStartLocalPlayer()
        {
            ChatUI.LocalPlayerName = PlayerName;
        }
    }
}