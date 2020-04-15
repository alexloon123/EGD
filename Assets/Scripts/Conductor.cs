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

    public float errorMargin;

    public int currentLeftBeat;
    public int currentRightBeat;

    public bool left = true;
    public bool right = false;
    public bool pressed = false;

    public GameObject cubeMarker;

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
            loopPositionInBeats = 0;
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
                    if (beat <= errorMargin)
                    {
                        cubeMarker.transform.position = new Vector3(5f, cubeMarker.transform.position.y, cubeMarker.transform.position.z);
                    }
                    else
                    {
                        cubeMarker.transform.position = new Vector3(-5f, cubeMarker.transform.position.y, cubeMarker.transform.position.z);
                    }
                }

            }
            else if (right)
            {
                currentRightBeat++;
            }
        }
        loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;
        loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;
        if (currentLeftBeat < numberOfBeatsLeftChannel && leftBeatsBeatStamps[currentLeftBeat] <= loopPositionInBeats)
        {
            currentLeftBeat++;
        }
    }


}
