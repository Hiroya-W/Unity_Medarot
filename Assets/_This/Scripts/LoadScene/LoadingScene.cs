using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{

    public LoadSoundPlayer LSoundPlayer;

    private AsyncOperation async;
    public GameObject LoadingUi;
    public Slider Slider;
    public Text NowLoading;
    public bool isRunning;
    public int count;

    private void Start()
    {
        isRunning = false;
        count = 0;
    }

    public void LoadNextScene()
    {
        LoadingUi.SetActive(true);
        LSoundPlayer.StopTitleBGM();
        LSoundPlayer.PlayMedarotiBGM();
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync("BattleScene2");
        while (!async.isDone)
        {
            if (!isRunning)
            {
                isRunning = true;
                StartCoroutine(NLText());
            }
                
            Slider.value = async.progress;
            yield return null;
        }
    }

    IEnumerator NLText()
    {
        switch (count)
        {
            case 0:
                NowLoading.text = "NOW LOADING";
                count++;
                break;
            case 1:
                NowLoading.text += ".";
                count++;
                break;
            case 2:
                NowLoading.text += ".";
                count++;
                break;
            case 3:
                NowLoading.text += ".";
                count = 0;
                break;
        }

        yield return new WaitForSeconds(0.5f);
        isRunning = false;
    }
}