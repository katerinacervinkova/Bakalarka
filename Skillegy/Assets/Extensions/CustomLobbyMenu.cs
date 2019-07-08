using UnityEngine;
using UnityEngine.UI;

namespace Prototype.NetworkLobby
{
    /// <summary>
    /// Class dealing with calls from the UI.
    /// Based on LobbyMenu class
    /// </summary>
    public class CustomLobbyMenu : MonoBehaviour
    {
        public LobbyManager lobbyManager;

        public RectTransform lobbyPanel;

        public InputField ipInput;

        /// <summary>
        /// Initializes the panel.
        /// </summary>
        public void OnEnable()
        {
            lobbyManager.topPanel.ToggleVisibility(true);

            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);
        }

        /// <summary>
        /// Starts the host.
        /// </summary>
        public void OnClickHost()
        {
            lobbyManager.StartHost();
        }

        /// <summary>
        /// Joins the client to the ip in inInput.
        /// </summary>
        public void OnClickJoin()
        {
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.networkAddress = ipInput.text;
            lobbyManager.StartClient();

            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
        }

        /// <summary>
        /// Deals with the text change in ipInput.
        /// </summary>
        /// <param name="text">text in the field</param>
        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                OnClickJoin();
        }
    }
}
