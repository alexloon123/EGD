using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDisplay : MonoBehaviour {

    //Printer
    public TextAnimator txt;

    //Message buffer
    Stack<string> buf = new Stack<string>();

    //Constants
    const float DEFAULT_SPEED = 0.1f;
    const float DEFAULT_HANG = 1.0f;

    public void QueueMsg(string msg) {
        //Add msg to Queue
        buf.Push(msg);

        //!!!!!!!!!!!!!!!!!!!!
        //AUTO-DISPLAY THE MSG
        //!!!!!!!!!!!!!!!!!!!!
        DisplayMsg();
    }

    public void DisplayMsg(float speed = DEFAULT_SPEED, float hang = DEFAULT_HANG) {
        //If the buffer is empty, return
        if(buf.Count == 0) {
            return;
        }

        //Pop the msg off the buf and send it to the printer
        string msg = buf.Pop();
        StartCoroutine(txt.AnimPrint(msg, speed, hang));

    }

}
