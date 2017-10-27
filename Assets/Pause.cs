using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Pause : MonoBehaviour {

    public Button pauza;
    public Button x01,x05,x1,x2,x5;
    private float timeTemp = 1f;

    void Start () {
        Button btn = pauza.GetComponent<Button>();

        Button btn1 = x01.GetComponent<Button>();
        Button btn2 = x05.GetComponent<Button>();
        Button btn3 = x1.GetComponent<Button>();
        Button btn4 = x2.GetComponent<Button>();
        Button btn5 = x5.GetComponent<Button>();

        btn.onClick.AddListener(Pauzowanie);

        btn1.onClick.AddListener(ZmienCzas01);
        btn2.onClick.AddListener(ZmienCzas05);
        btn3.onClick.AddListener(ZmienCzas1);
        btn4.onClick.AddListener(ZmienCzas2);
        btn5.onClick.AddListener(ZmienCzas5);
    }

    private void Pauzowanie()
    {
        if (Time.timeScale != 0)
        {
            timeTemp = Time.timeScale;
            Time.timeScale = 0;
        }
        else
            Time.timeScale = timeTemp;
    }

    private void ZmienCzas01()
    {
        if (Time.timeScale != 0)
            Time.timeScale = 0.1f;
    }

    private void ZmienCzas05()
    {
        if (Time.timeScale != 0)
            Time.timeScale = 0.5f;
    }

    private void ZmienCzas1()
    {
        if (Time.timeScale != 0)
            Time.timeScale = 1f;
    }

    private void ZmienCzas2()
    {
        if (Time.timeScale != 0)
            Time.timeScale = 2f;
    }

    private void ZmienCzas5()
    {
        if (Time.timeScale != 0)
            Time.timeScale = 5f;
    }


    // Update is called once per frame
    void Update () {
		
	}
}
