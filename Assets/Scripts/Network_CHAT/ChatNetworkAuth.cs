using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ChatSystem
{

    public class ChatNetworkAuth : NetworkAuthenticator
    {

        public readonly HashSet<NetworkConnection> ConnectionsPendingDisconect = new();
        private string _playerName;


#region Messeges
        public struct AuthRequestMessage : NetworkMessage
        {

            public string AuthUserName;
        }

        public struct AuthResponseMessage : NetworkMessage
        {

            public int Code;
            public string Messege;
        }
#endregion


#region Server
         
        public override void OnStartServer()
        =>NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessege, false);

        public override void OnStopServer()
        => NetworkServer.UnregisterHandler<AuthRequestMessage>();


        public override void OnServerAuthenticate(NetworkConnectionToClient conn) { }


        public void OnAuthRequestMessege(NetworkConnectionToClient cone, AuthRequestMessage msg)
        {

            if (ConnectionsPendingDisconect.Contains(cone)) return;

            if(msg.AuthUserName != string.Empty)
            {
                if (!Player.PlayerNames.Contains(msg.AuthUserName))
                {
                    Player.PlayerNames.Add(msg.AuthUserName);

                    cone.authenticationData = msg.AuthUserName;
                    
                    Debug.Log($"Success From Server for {msg.AuthUserName}!");

                    AuthResponseMessage authResponseMessage = new AuthResponseMessage()
                    {
                        Code = 1,
                        Messege = $"Success From Server for {msg.AuthUserName}! Hello:)"
                    };
                    cone.Send(authResponseMessage);
                    ServerAccept(cone);
                }
                else
                {
                    ConnectionsPendingDisconect.Add(cone);

                    Debug.Log($"UnSuccess From Server for {msg.AuthUserName}!");

                    AuthResponseMessage authResponseMessage = new AuthResponseMessage()
                    {
                        Code = 0,
                        Messege = $"Server discard joint for {msg.AuthUserName}! Already has this userName:)"
                    };
                    cone.isAuthenticated = false;
                    cone.Send(authResponseMessage);
                    StartCoroutine(Disconect(cone,1));
                }

            }
        }


        private IEnumerator Disconect(NetworkConnectionToClient cone, int timeToDisconect)
        {

            yield return new WaitForSeconds(timeToDisconect);

            ServerReject(cone);

            yield return null;

            ConnectionsPendingDisconect.Remove(cone);
        }

#endregion

#region Client
         
        public override void OnStartClient()
        => NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessege, false);


        public override void OnStopClient()
         => NetworkClient.UnregisterHandler<AuthResponseMessage>();


        public void SetPlayerName(string playerName)
        => _playerName = playerName;
        

        public override void OnClientAuthenticate()
        {

            AuthRequestMessage authRequestMessage = new AuthRequestMessage()
            {
                AuthUserName = _playerName,
            };
            NetworkClient.Send(authRequestMessage);
        }


        public void OnAuthResponseMessege(AuthResponseMessage msg)
        {

            if(msg.Code == 1)
            {

                Debug.Log("Auth Client accept succesful!");
                ClientAccept();
            }
            else
            {
                Debug.LogError("Auth Client : Auth ERROR ");

                ChatNetworkManager.Singletone.StopHost();

                //To send logIn UI
            }
        }


#endregion

        }
    }