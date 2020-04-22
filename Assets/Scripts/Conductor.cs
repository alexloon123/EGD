using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{

    public float beatsPerMinute;
    public float secondsPerBeat;
    public float songPosition;
    public float songPositionInBeats;
    public float dspSongTime;
    public AudioSource source;

    public float firstBeatOffset;

    public float beatsPerLoop;
    public int completedLoops = 0;
    public float loopPositionInBeats;

    public float loopPositionInAnalog;
    public static Conductor instance;

    public int numberOfBeatsLeftChannel;
    public int numberOfBeatsRightChannel;

    public float[] leftBeatsBeatStamps;
    public float[] rightBeatsBeatStamps;

    public float beatErrorMargin;

    public int currentLeftBeat;
    public int currentRightBeat;

    public bool left = true;
    public bool right = false;
    public bool pressed = false;

    public GameObject playerMarker;
    public float speed;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        secondsPerBeat = 60f / beatsPerMinute;
        dspSongTime = (float)AudioSettings.dspTime;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);
        songPositionInBeats = songPosition / secondsPerBeat;
        if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
        {
            currentLeftBeat = 0;
            currentRightBeat = 0;
            completedLoops++;
        }
        loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;
        loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;
        if (currentLeftBeat + 1 < numberOfBeatsLeftChannel && leftBeatsBeatStamps[currentLeftBeat + 1] - beatErrorMargin <= loopPositionInBeats)
        {
            currentLeftBeat++;
        }

        if (currentRightBeat + 1 < numberOfBeatsRightChannel && rightBeatsBeatStamps[currentRightBeat + 1] - beatErrorMargin <= loopPositionInBeats)
        {
            currentRightBeat++;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (left)
            {
                if (currentLeftBeat >= numberOfBeatsLeftChannel)
                {

                }
                else
                {
                    float beat = Mathf.Abs(loopPositionInBeats - leftBeatsBeatStamps[currentLeftBeat]);
                    Debug.Log("Space was pressed with the beat: " + beat + ". The currentLeftBeat is: " + currentLeftBeat + " and the loopPos was: " + loopPositionInBeats + " and the left beat stamp was: " + leftBeatsBeatStamps[currentLeftBeat]);
                    if (beat <= beatErrorMargin)
                    {
                        if (playerMarker != null)
                        {
                            playerMarker.transform.position = new Vector3(5f, playerMarker.transform.position.y, playerMarker.transform.position.z);
                            playerMarker.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                            playerMarker.GetComponent<SpriteRenderer>().enabled = false;
                        }
                        speed += (1.0f - beat);
                    }
                    else
                    {
                        if(playerMarker != null)
                        {
                            playerMarker.transform.position = new Vector3(-5f, playerMarker.transform.position.y, playerMarker.transform.position.z);
                            playerMarker.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                            playerMarker.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        speed = (1.0f - beat);
                    }
                }

            }
            else if (right)
            {
                if (currentRightBeat >= numberOfBeatsRightChannel)
                {

                }
                else
                {
                    float beat = Mathf.Abs(loopPositionInBeats - rightBeatsBeatStamps[currentRightBeat]);
                    Debug.Log("Space was pressed with the beat: " + beat + ". The currentRighttBeat is: " + currentRightBeat + " and the loopPos was: " + loopPositionInBeats + " and the right beat stamp was: " + rightBeatsBeatStamps[currentRightBeat]);
                    if (beat <= beatErrorMargin)
                    {
                        playerMarker.transform.position = new Vector3(5f, playerMarker.transform.position.y, playerMarker.transform.position.z);
                    }
                    else
                    {
                        playerMarker.transform.position = new Vector3(-5f, playerMarker.transform.position.y, playerMarker.transform.position.z);
                    }
                }
            }
        }
        
    }


}
