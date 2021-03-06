using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace Com.Afik.Pun2Demo
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;


        #endregion

        #region Private Serializable Fields

        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;


        #endregion

        #region Private Fields

        bool isConnecting;

        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        string gameVersion = "1";


        #endregion


        #region MonoBehaviour CallBacks


        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.



        private void Awake()
        {
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// MonoBehaviour method called on GameObject by Unity during initialization phase.

        private void Start()
        {
            //Connect();

            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

        }

        #endregion

        #region Public Methods

        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network

        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);


            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                //we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }   
            else
            {
                //we must first and foremost connect to Photon Online Server.
                ///PhotonNetwork.ConnectUsingSettings();

                // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;

            }

            
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks


        public override void OnConnectedToMaster()
        {
            {
                if (isConnecting)
                {
                    Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
                    //The first we try to do is to join a potential existing room. If there is, good. else, we'll be called back with OnJoinRandomFailed()
                    PhotonNetwork.JoinRandomRoom();
                    isConnecting = false;
                }
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            //we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");

            //We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");
                PhotonNetwork.LoadLevel("Room For 1");
            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {

            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        #endregion
    }
}