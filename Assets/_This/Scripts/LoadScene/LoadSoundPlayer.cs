using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSoundPlayer : SingletonMonoBehaviour<LoadSoundPlayer>
{

    public AudioSource[] Source;

    public bool DontDestroyEnabled = true;

    AudioSource TitleBGM;
    AudioSource MedarotiBGM;

    // Use this for initialization
    void Start()
    {
        TitleBGM = Source[0];
        MedarotiBGM = Source[1];

        if (DontDestroyEnabled)
        {
            // Sceneを遷移してもオブジェクトが消えないようにする
            DontDestroyOnLoad(this);
        }
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    public void PlayTitleBGM()
    {
        TitleBGM.Play();
    }

    public void StopTitleBGM()
    {
        TitleBGM.Stop();
    }

    public void PlayMedarotiBGM()
    {
        MedarotiBGM.Play();
    }

    public void StopMedarotiBGM()
    {
        MedarotiBGM.Stop();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
