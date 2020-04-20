using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    //Printer for debugging
    public TextDisplay disp;

    //PREFABS
    public GameObject PREFAB_playerPosition;

    //Player data set
    List<Vector2> positions = new List<Vector2>();
    List<string> names = new List<string>();

    //Player location icons
    List<GameObject> dots = null;

    //Position of the pointer last frame
    //(-1,-1) means no pointer last frame
    public Vector2 pointer = new Vector2(-1, -1);
    Vector2 v_null = new Vector2(-1, -1);

    //CONSTS
    public float max_lat;
    public float min_lat;
    public float max_long;
    public float min_long;
    float SENSITIVITY = 20.0f;

    public void OnEnable() {
        Photon.Pun.PhotonNetwork.AddCallbackTarget(this);
        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public void OnDisable() {
        Photon.Pun. PhotonNetwork.RemoveCallbackTarget(this);
        Photon.Pun.PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj) {
        //New player has connected
        if (obj.Code == 1) {
            //Pull data from event
            object[] data = (object[])obj.CustomData;
            Vector2 position = (Vector2)data[0];
            string nickname = (string)data[1];
            //Convert position to map coords
            position = GlobeToMap(position);
            //Add new player to the data set
            positions.Add(position);
            names.Add(name);
            disp.QueueMsg(nickname + " connected");
            DisplayPlayers();
        }
    }

    public void DisplayPlayers() {
        //Clear the map
        if (dots != null) {
            for (int i = 0; i < dots.Count; i++) {
                Destroy(dots[i]);
            }
        }
        dots = new List<GameObject>();

        //Displays all players in the data set
        for (int i = 0; i < positions.Count; i++) {
            Vector2 position = positions[i];
            GameObject dot = Instantiate(PREFAB_playerPosition, position, Quaternion.identity, this.gameObject.transform);
            dot.GetComponent<RectTransform>().localPosition = position;
            dots.Add(dot);
        }
    }

    //This converts <Latitude,Longitude> (globe) to <x,y> (map)
    public Vector2 GlobeToMap(Vector2 globe) {
        if (globe.x > max_lat || globe.x < min_lat || globe.y > max_long || globe.y < min_long) {
            disp.QueueMsg("OUT OF MAP BOUNDS");
            return globe;
        }

        Vector3 position = new Vector3(-1, -1, 0);
        position.x += 2 * ((globe.y - min_long) / (max_long - min_long));
        position.y += 2 * ((globe.x - min_lat) / (max_lat - min_lat));

        position.x *= GetComponent<RectTransform>().rect.width / 2;
        position.y *= GetComponent<RectTransform>().rect.height / 2;
        return position;
    }

    public void Update() {
        if (Input.GetMouseButton(0)) {
            if(pointer != v_null) {
                Vector2 diff = new Vector2(Input.mousePosition.x - pointer.x, Input.mousePosition.y - pointer.y);
                gameObject.transform.Translate(SENSITIVITY * Camera.main.ScreenToViewportPoint(new Vector3(diff.x, diff.y, 0)));
            }
            pointer.Set(Input.mousePosition.x, Input.mousePosition.y);
        } else if(pointer != v_null) {
            //End of swipe
            pointer.Set(-1, -1);
        }
    }

}
