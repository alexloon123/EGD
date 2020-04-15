using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDisplay : MonoBehaviour {
    public TextAnimator txt;
    public Stack<string> buf = new Stack<string>();

    public void QueueMsg(string msg) {
        //Add msg to Queue
        buf.Push(msg);
        //AUTO-DISPLAY
        DisplayMsg();
    }

    public void DisplayMsg() {
        if(buf.Count == 0) {
            return;
        }

        string msg = buf.Pop();
        StartCoroutine(txt.AnimPrint(msg, 0.05f, 1.0f));

    }

    public IEnumerator delay1() {
        yield return new WaitForSeconds(1);
    }
}
