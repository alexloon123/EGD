using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class NetworkConnectionManager : MonoBehaviourPunCallbacks
{

    public GameObject text_nickname;

    // Start is called before the first frame update
    void Start()
    {
        //Ask for location permission
        #if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
#endif

        ConnectToMaster();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConnectToMaster() {
        PhotonNetwork.OfflineMode = false;
        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "0.6";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ConnectToRoom() {
        if (!PhotonNetwork.IsConnected) {
            return;
        }
        PhotonNetwork.NickName = text_nickname.GetComponent<UnityEngine.UI.Text>().text;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        base.OnDisconnected(cause);
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
