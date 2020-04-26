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

    string[] lines_good = {
    "You came to a fork in the road and went down the same path as your companion, your bond is clearly deepening.",
    "Your path was blocked but with the help of your companion you climbed through.",
    "The path you’ve been following disappeared, but you followed the sound of your companions voice and weren’t led astray.",
    "Out of nowhere a cacophony of voices emerged, all encouraging you to follow a different path. You stayed true to the path you’ve been on, and kept going the same way.",
    "A stranger appeared blocking your path, and said you could only pass if you and your companion answered the same. Fortunately, you both succeeded and your paths remained intertwined."
    };
    string[] lines_bad = {
    "You came to a fork in the road and lost your companion, hopefully you can find them again.",
    "Your path was blocked and while your companion passed, you failed to and are lost without them.",
    "Your path disappeared, and although you tried to follow the voice leading you, you became lost in the mist.",
    "A cacophony of voices appeared and led you astray, you tried your best to go the right way but to no avail for you’ve lost your way.",
    "A gated door appeared out of nowhere, blocking your path. A question was asked, and the answers given by you and your companion differed. You have lost your path."
    };

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

    public void QueueGoodMsg() {
        int i = Random.Range(0, (lines_good.Length-1));
        QueueMsg(lines_good[i]);
    }

    public void QueueBadMsg() {
        int i = Random.Range(0, (lines_bad.Length - 1));
        QueueMsg(lines_bad[i]);
    }

}
