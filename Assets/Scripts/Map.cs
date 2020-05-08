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

    //Waiting panel
    public GameObject wait_panel;
    public bool wait_pause = true;

    public bool connecting = false;
    int target_index = -1;
    int self_index = -1;
    public float connectedErrorMargin = 30.0f;

    public Conductor conductor;

    //Disconnecting
    bool dis = false;

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

    //Returns the number of players in the room, according to this player
    //Returns -1 if this player has conflicting data
    public int PlayerCount() {
        if(positions.Count != names.Count || names.Count != icons.Count) {
            return -1;
        }
        return positions.Count;
    }

    //Returns true if two players are in room
    public void PlayerCheck() {
        int p = PlayerCount();
        if (p == 2) {       //Allow play
            wait_panel.SetActive(false);
            wait_pause = false;
        } 
        else if(p == 1) {   //Do not allow play
            wait_panel.SetActive(true);
            wait_pause = true;
            connecting = false;
        } 
        else if(p == -1){   //Something has gone very wrong  
            disp.QueueMsg("Something has gone wrong, debug.logging lists...");
            Debug.Log(names);
            Debug.Log(positions);
        }
    }

    private void NetworkingClient_EventReceived(EventData obj) {
        if (dis) return;

        //New player has connected
        if (obj.Code == 1) {
            //Pull data from event
            object[] data = (object[])obj.CustomData;
            Vector2 position = (Vector2)data[0];
            string nickname = (string)data[1];
            //Convert position to map coords

            //Add new player to the data set
            positions.Add(position);
            names.Add(nickname);
            //disp.QueueMsg(nickname + " connected");
            icons.Add(DisplayPlayer(position));
            PlayerCheck();
            disp.QueueMsg(nickname + " has connected!");

            //Prepare PUN event
            byte b = 2;
            object[] content = new object[] { my_position, my_name };
            RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            // Send new player data to other clients
            Photon.Pun.PhotonNetwork.RaiseEvent(b, content, eventOptions, sendOptions);

        }
        //Old player sending data to new player
        else if (obj.Code == 2) {
            //Pull data from event
            object[] data = (object[])obj.CustomData;
            Vector2 position = (Vector2)data[0];
            string nickname = (string)data[1];
            //Convert position to map coords

            //Add new player to the data set
            positions.Add(position);
            names.Add(nickname);
            //disp.QueueMsg(nickname + " connected");
            icons.Add(DisplayPlayer(position));
            PlayerCheck();
            disp.QueueMsg(nickname + " has connected!");
        }
        //client disconnecting
        else if (obj.Code == 3) {
            object[] data = (object[])obj.CustomData;
            string new_name = (string)data[0];
            disp.QueueMsg(new_name + " has disconnected!");
            for (int j = 0; j < positions.Count; j++) {
                if (names[j] == new_name) {
                    positions.RemoveAt(j);
                    names.RemoveAt(j);
                    GameObject temp = icons[j];
                    icons.RemoveAt(j);
                    Destroy(temp);
                }
            }
            PlayerCheck();
        }
        //updating an existing icon
        else if (obj.Code == 4) {
            //Pull data from event
            object[] data = (object[])obj.CustomData;
            Vector2 position = (Vector2)data[0];
            string nickname = (string)data[1];
            for (int j = 0; j < positions.Count; j++) {
                if (names[j] == nickname) {
                    positions[j] = position;
                    icons[j].GetComponent<RectTransform>().localPosition = new Vector3(position.x, position.y, 0.0f);
                }
            }
        }
    }

    public GameObject DisplayPlayer(Vector2 position) {
        GameObject temp = Instantiate(PREFAB_playerPosition, position, Quaternion.identity, this.gameObject.transform);
        temp.GetComponent<RectTransform>().localPosition = new Vector3(position.x, position.y, 0.0F);
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


    public void BeginConnecting()
    {
        if(!connecting)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (my_position.Equals(positions[i]) && my_name.Equals(names[i]))
                {
                    self_index = i;
                }
                else
                {
                    target_index = i;
                }
            }
        }
        connecting = true;
    }

    
    public void MoveToTarget()
    {
        Debug.Log(self_index + " / " + target_index);
        Vector2 player_position = positions[self_index];
        Vector2 target_position = positions[target_index];
        float current_speed = conductor.getSpeed() / 10;

        positions[self_index] = new Vector2(Mathf.Lerp(icons[self_index].GetComponent<RectTransform>().localPosition.x, icons[target_index].GetComponent<RectTransform>().localPosition.x, current_speed * Time.deltaTime), Mathf.Lerp(icons[self_index].GetComponent<RectTransform>().localPosition.y, icons[target_index].GetComponent<RectTransform>().localPosition.y, current_speed * Time.deltaTime));
        icons[self_index].GetComponent<RectTransform>().localPosition = new Vector3(positions[self_index].x, positions[self_index].y, 0.0f);

        //Prepare PUN event
        byte b = 4;
        object[] content = new object[] { positions[self_index], names[self_index] };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        // Send new player data to other clients
        Photon.Pun.PhotonNetwork.RaiseEvent(b, content, eventOptions, sendOptions);

        float x_difference = Mathf.Abs(icons[self_index].GetComponent<RectTransform>().localPosition.x - icons[target_index].GetComponent<RectTransform>().localPosition.x);
        float y_difference = Mathf.Abs(icons[self_index].GetComponent<RectTransform>().localPosition.y - icons[target_index].GetComponent<RectTransform>().localPosition.y);

        Debug.Log("x: " + x_difference + " y: " + y_difference);   

        if (x_difference <= connectedErrorMargin && y_difference <= connectedErrorMargin)
        {
            Debug.Log("Connected!");
            GameObject.FindGameObjectWithTag("ConnectedDisplay").gameObject.transform.position = icons[self_index].GetComponent<RectTransform>().localPosition;
            GameObject.FindGameObjectWithTag("ConnectedDisplay").GetComponent<SpriteRenderer>().enabled = true;
            StartCoroutine(DisplayConnected());
        }
    }

    IEnumerator DisplayConnected()
    {
        yield return new WaitForSeconds(3f);
        GameObject.FindGameObjectWithTag("ConnectedDisplay").GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Update() {
        /*
        for(int i = 0; i < icons.Count; i++)
        {
            icons[i].GetComponent<RectTransform>().transform.position.Set(icons[i].GetComponent<RectTransform>().transform.position.x, icons[i].GetComponent<RectTransform>().transform.position.y, 0.0f);
            Debug.Log("name: " + names[i] + " pos: " + icons[i].GetComponent<RectTransform>().transform.position.ToString());
        }
        */
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
        if(!wait_pause && !connecting && positions.Count > 1)
        {
            Debug.Log("Begin connecting!");
            BeginConnecting();
        }
        else if(!wait_pause && connecting && positions.Count > 1)
        {
            Debug.Log("Start moving!");
            MoveToTarget();
        }   
    }

    public void Disconnect() {
        if (dis) return;
        dis = true;
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
        yield return new WaitForSeconds(1);
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene("Titlescreen");
    }


}
