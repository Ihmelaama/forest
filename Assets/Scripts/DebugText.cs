using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{

    public static DebugText instance;

    private Text text;

    void Awake() 
    {

        instance=this;
        text=GetComponent<Text>();

    }

    public void SetText(string str) {

        text.text=str;

    }

    public void AppendText(string str) {

        text.text+=str;

    }

}
