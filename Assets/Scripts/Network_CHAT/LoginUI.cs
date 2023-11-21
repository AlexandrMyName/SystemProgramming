using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChatSystem
{

    public class LoginUI : MonoBehaviour
    {
        
        public static LoginUI Singletone;

        public Button _hostButton;
        public Button _clientButton;


        private void Awake()
        {
            Singletone = this;
        }


        public void OnPlayerNameChange(string playerName)
        {

            _hostButton.interactable = !string.IsNullOrEmpty(playerName);
            _clientButton.interactable = !string.IsNullOrEmpty(playerName);
        }

    }
}