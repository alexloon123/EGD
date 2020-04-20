using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkConnectionManager : MonoBehaviourPunCallbacks
{
    public GameObject button_master;
    public GameObject button_room;
    public GameObject text_nickname;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConnectToMaster() {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.NickName = text_nickname.GetComponent<UnityEngine.UI.Text>().text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "v0.2";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ConnectToRoom() {
        if (!PhotonNetwork.IsConnected) {
            return;
        }
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        base.OnDisconnected(cause);
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();
        button_master.SetActive(false);
        button_room.SetActive(true);
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();
        SceneManager.LoadScene("MultiplayerRoom");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        base.OnJoinRandomFailed(returnCode, message);
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }
}
