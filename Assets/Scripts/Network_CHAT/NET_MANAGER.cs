using Mirror;
using NETWORKING.Player;


namespace NETWORKING.Manager
{

    public class NET_MANAGER : NetworkManager
    {

        public static NET_MANAGER Singletone;

        public override void Awake()
        {
         
            Singletone = this;
        }


        public void OnAddressChanged(string address)
        {

            networkAddress = address;
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {

           NET_PLAYER.PlayerNames.Remove(conn.authenticationData.ToString());

           base.OnServerDisconnect(conn);
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
        }
    }
}