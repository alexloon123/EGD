using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimator : MonoBehaviour {

    //Lock starts unlocked
    public bool animating = false;

    //Prints msg at speed with hang time
    public IEnumerator AnimPrint(string msg, float speed, float hang) {

        //Wait for animator to finish printing
        while (animating) {
            yield return new WaitForSeconds(speed);
        }

        //Lock animator
        animating = true;

        //Display this string
        string temp = "";
        for(int i = 0; i < msg.Length; i++) {
            //Add another char to the string and display
            temp += msg[i];
            this.GetComponent<UnityEngine.UI.Text>().text = temp;
            //Animation speed wait
            yield return new WaitForSeconds(speed);
        }

        //Hang time wait
        yield return new WaitForSeconds(hang);

        //Clear text
        this.GetComponent<UnityEngine.UI.Text>().text = "";

        //Unlock animator
        animating = false;

    }

}
