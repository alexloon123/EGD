using UnityEngine;
using System.Collections;
using UnityEngine.Android;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public class Location : MonoBehaviour {

    //Printer for debugging
    public TextDisplay disp;

    public Map map;
    public Conductor conductor;

    public void Start() {

        //Get the player's location
        StartCoroutine(Locate());

    }

    IEnumerator Locate() {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser) {
            disp.QueueMsg("Location disabled");
            yield break; 
        }
        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1f);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1) {
            disp.QueueMsg("Location timed out");
            yield break;
        }

        Vector2 position;

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed) {
            disp.QueueMsg("Unable to determine device location");
            yield break;
        }
        // Access granted and location value could be retrieved
        else {
            disp.QueueMsg("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            position = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

        //Prepare PUN event
        byte b = 1;
        object[] content = new object[] { position, PhotonNetwork.NickName };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        // Send new player data to master client
        Photon.Pun.PhotonNetwork.RaiseEvent(b, content, eventOptions, sendOptions);
        
        conductor.playerMarker = map.DisplayPlayer(map.GlobeToMap(position));

    }

}
