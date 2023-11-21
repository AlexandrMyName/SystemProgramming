using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ChatSystem
{

    public class ChatNetworkManager : NetworkManager
    {
         
        public static ChatNetworkManager Singletone { get; private set; }


        public override void Awake()
        {

            base.Awake();
            Singletone = this;
        }


        public void OnNetworkAddressChanged(string idServer)
        => networkAddress = idServer;


        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {

            if(conn.authenticationData != null)
            {
                Player.PlayerNames.Remove((string) conn.authenticationData); 
                ChatUI.ConnNames.Remove(conn);
            }
            base.OnServerDisconnect(conn);
        }

        public override void OnClientDisconnect()
        {

            //UI
            base.OnClientDisconnect();
        }
    }
}