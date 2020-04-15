using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{

    public bool animating = false;


    public IEnumerator AnimPrint(string msg, float speed, float hang) {

        //Wait for animator to finish
        while (animating) {
            yield return new WaitForSeconds(speed);
        }


        animating = true;
        string temp = "";

        for(int i = 0; i < msg.Length; i++) {
            temp += msg[i];
            this.GetComponent<UnityEngine.UI.Text>().text = temp;
            yield return new WaitForSeconds(speed);
        }
        yield return new WaitForSeconds(hang);
        this.GetComponent<UnityEngine.UI.Text>().text = "";
        animating = false;

    }

}
