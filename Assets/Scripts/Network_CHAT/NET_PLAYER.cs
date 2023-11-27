using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NETWORKING.Player
{

    public class NET_PLAYER : NetworkBehaviour
    {

        [SyncVar] public string Name;
        public static HashSet<string> PlayerNames = new();


        public override void OnStartClient()
        {
            base.OnStartClient();

            Name = (string) connectionToClient.authenticationData;

        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            //
        }
    }
}