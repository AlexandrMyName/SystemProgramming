using Mirror;
using NETWORKING.Manager;
using NETWORKING.Player;
using System.Collections.Generic;
using UnityEngine;


namespace NETWORKING
{
    public class NET_AUTH : NetworkAuthenticator
    {

        private HashSet<NetworkConnection> _connectionsPendingToDisconect = new();
        private string _localUserName;

        #region Message

        private struct AuthMessageRequest : NetworkMessage
        {
            public string AuthUserName;

            public AuthMessageRequest(string authName)
            {
                AuthUserName = authName;
            }
        }

        private struct AuthMessageResponse : NetworkMessage
        {
            public int Code;
            public string AuthUserName;

            public AuthMessageResponse(string authName, int code)
            {
                AuthUserName = authName;
                Code = code;
            }
        }

        #endregion

        public override void OnStartServer()
        => NetworkServer.RegisterHandler<AuthMessageRequest>(OnAuthMessageRequest, false);


        public override void OnStopServer()
        => NetworkServer.UnregisterHandler<AuthMessageRequest>();


        public override void OnServerAuthenticate(NetworkConnectionToClient conn) { }


        private void OnAuthMessageRequest(NetworkConnectionToClient conn, AuthMessageRequest msg)
        {

            if (msg.AuthUserName != string.Empty)
            {
                AuthMessageResponse response = new AuthMessageResponse();
                response.Code = 0;
                response.AuthUserName = msg.AuthUserName;

                if (_connectionsPendingToDisconect.Contains(conn)) return;

                if (!NET_PLAYER.PlayerNames.Contains(msg.AuthUserName))
                {
                    conn.authenticationData = msg.AuthUserName;
                    NET_PLAYER.PlayerNames.Add(msg.AuthUserName);

                    response.Code = 1;

                    conn.Send(response);
                    ServerAccept(conn);
                    Debug.Log("Server accept !");
                }
                else
                {
                    _connectionsPendingToDisconect.Add(conn);

                    response.Code = 0;
                    conn.Send(response);
                    ServerReject(conn);
                    Debug.Log("Server reject !");
                }
            }
        }


        public override void OnStartClient()
        => NetworkClient.RegisterHandler<AuthMessageResponse>(OnAuthMessageResponse, false);


        public override void OnStopClient()
        => NetworkClient.UnregisterHandler<AuthMessageResponse>();


        public override void OnClientAuthenticate()
        {
            AuthMessageRequest request = new AuthMessageRequest();

            request.AuthUserName = _localUserName;

            NetworkClient.Send(request);
        }


        private void OnAuthMessageResponse(AuthMessageResponse msg)
        {

            switch (msg.Code)
            {
                case 0:
                    Debug.Log("Client error");
                    NET_MANAGER.Singletone.StopHost();  
                    break;
                case 1:
                    Debug.Log("Client accept !");
                    NET_CHAT.LocalPlayerName = msg.AuthUserName;
                    ClientAccept();
                    break;


                default:
                    break;
            }
        }


        public void OnLocalUserNameChanged(string userName) => _localUserName = userName;

    }
}