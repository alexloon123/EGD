using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    //Printer for debugging
    public TextDisplay disp;

    //PREFABS
    public GameObject PREFAB_playerPosition;

    //Player data set
    public List<Vector2> positions = new List<Vector2>();
    public Vector2 my_position;
    public List<string> names = new List<string>();
    public string my_name;
    public List<GameObject> icons = new List<GameObject>();

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
            
            //Add new player to the data set
            positions.Add(position);
            names.Add(name);
            //disp.QueueMsg(nickname + " connected");
            icons.Add(DisplayPlayer(position));


                //Prepare PUN event
                byte b = 2;
                object[] content = new object[] { my_position, my_name };
                RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                // Send new player data to other clients
                Photon.Pun.PhotonNetwork.RaiseEvent(b, content, eventOptions, sendOptions);
            
        }
        //validating other clients
        else if(obj.Code == 2) {
            object[] data = (object[])obj.CustomData;
            Vector2 new_position = (Vector2)data[0];
            string new_name = (string)data[1];

                bool found = false;
                //Check for this player
                for(int j = 0; j < positions.Count; j++) {
                    if(positions[j] == new_position && names[j] == new_name) {
                        found = true;
                        break;
                    }
                }
                if (found) return;
                //Add this player
                positions.Add(new_position);
                names.Add(new_name);
                //disp.QueueMsg(new_name + " connected");
                icons.Add(DisplayPlayer(new_position));
        }
        //client disconnecting
        else if(obj.Code == 3) {
            object[] data = (object[])obj.CustomData;
            string new_name = (string)data[0];

            for(int j = 0; j < positions.Count; j++) {
                if(names[j] == new_name) {
                    positions.RemoveAt(j);
                    names.RemoveAt(j);
                    GameObject temp = icons[j];
                    icons.RemoveAt(j);
                    Destroy(temp);
                }
            }
        }
        
    }

    public GameObject DisplayPlayer(Vector2 position) {
        GameObject temp = Instantiate(PREFAB_playerPosition, position, Quaternion.identity, this.gameObject.transform);
        temp.GetComponent<RectTransform>().localPosition = position;
        return temp;
    }

    //This converts <Latitude,Longitude> (globe) to <x,y> (map)
    public Vector2 GlobeToMap(Vector2 globe) {
        if (globe.x > max_lat || globe.x < min_lat || globe.y > max_long || globe.y < min_long) {
            //disp.QueueMsg("OUT OF MAP BOUNDS");
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

    public void Disconnect() {
        //Broadcast disconnect event
        byte b = 3;
        object[] content = new object[] { my_name };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        Photon.Pun.PhotonNetwork.RaiseEvent(b, content, eventOptions, sendOptions);

        //Disconnect
        StartCoroutine(Leave());
    }

    IEnumerator Leave() {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene("Titlescreen");
    }

}
