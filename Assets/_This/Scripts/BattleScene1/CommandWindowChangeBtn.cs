using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandWindowChangeBtn : MonoBehaviour {

    public GameObject CommandWindow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeBtn()
    {
        if(CommandWindow.activeInHierarchy == false)
        {
            CommandWindow.SetActive(true);
        }
        else if(CommandWindow.activeInHierarchy == true)
        {
            CommandWindow.SetActive(false);
        }
    }
}
