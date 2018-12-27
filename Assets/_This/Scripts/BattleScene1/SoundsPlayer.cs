using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SoundsPlayer : MonoBehaviour {
    public AudioSource[] Source;

    AudioSource Encountmusic;
    AudioSource Battlemusic1;
    AudioSource Battlemusic2;
    AudioSource Battlemusic3;
    AudioSource Battlemusic4;
    AudioSource Battlemusic5;
    AudioSource decision;
    AudioSource selection;
    AudioSource cansel;
    AudioSource cursorwarning;
    AudioSource attack;
    AudioSource winBGM;
    AudioSource loseBGM;
    AudioSource gameOver;
 

    // Use this for initialization
    void Start () {
        Encountmusic  = Source[0];
        Battlemusic1  = Source[1];
        Battlemusic2  = Source[2];
        Battlemusic3  = Source[3];
        decision      = Source[4];
        selection     = Source[5];
        cansel        = Source[6];
        cursorwarning = Source[7];
        attack        = Source[8];
        winBGM        = Source[9];
        loseBGM       = Source[10];
        Battlemusic4  = Source[11];
        Battlemusic5  = Source[12];
        gameOver      = Source[13];
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayEncountMusic()
    {
        LoadSoundPlayer.Instance.DestroyObject();
        Encountmusic.loop = true;
        Encountmusic.Play();
    }

    public void StopEncountMusic()
    {
        Encountmusic.Stop();
    }

    //BattleMusic切り替えのトグル
    public Toggle BattleMusic1TG;
    public Toggle BattleMusic2TG;
    public Toggle BattleMusic3TG;
    public Toggle BattleMusic4TG;
    public Toggle BattleMusic5TG;

    public Toggle BattleEndMusic1TG;
    public Toggle BattleEndMusic2TG;


    public void PlayBattleMusic()
    {
        //一つだけ有効になっている物を取り出す
        //Toggle TG = BattleMusicTG.ActiveToggles().FirstOrDefault();

        //名前を取り出す
        //string TGText = TG.gameObject.transform.Find("Label").gameObject.GetComponent<Text>().text;

        if (BattleMusic1TG.isOn == true)
        {
            Battlemusic1.loop = true;
            //Battlemusic1.PlayOneShot(BattleMusic1, 1f);
            Battlemusic1.Play();
        }
        else if (BattleMusic2TG.isOn == true)
        {
            Battlemusic2.loop = true;
            //Battlemusic2.PlayOneShot(Battlemusic2.clip, 1f);
            Battlemusic2.Play();
        }
        else if (BattleMusic3TG.isOn == true)
        {
            Battlemusic3.loop = true;
            //Battlemusic2.PlayOneShot(Battlemusic3.clip, 1f);
            Battlemusic3.Play();
        }
        else if (BattleMusic4TG.isOn == true)
        {
            Battlemusic4.loop = true;
            Battlemusic4.Play();
        }
        else if (BattleMusic5TG.isOn == true)
        {
            Battlemusic5.loop = true;
            Battlemusic5.Play();
        }

    }

    public void StopBattleMusic()
    {
        if (BattleMusic1TG.isOn == true)
        {
            Battlemusic1.Stop();
        }
        else if (BattleMusic2TG.isOn == true)
        {
            Battlemusic2.Stop();
        }
        else if (BattleMusic3TG.isOn == true)
        {
            Battlemusic3.Stop();
        }
        else if (BattleMusic4TG.isOn == true)
        {
            Battlemusic4.Stop();
        }
        else if (BattleMusic5TG.isOn == true)
        {
            Battlemusic5.Stop();
        }
    }

    public void PlayDecisionSound()
    {
        decision.PlayOneShot(decision.clip, 1f);
    }

    public void PlaySelectionSound()
    {
        selection.PlayOneShot(selection.clip, 1f);
    }

    public void PlayCanselSound()
    {
        cansel.PlayOneShot(cansel.clip, 1f);
    }

    public void PlayAttackSound()
    {
        attack.PlayOneShot(attack.clip, 1f);
    }

    public void PlayCursorWarningSound()
    {
        cursorwarning.PlayOneShot(cursorwarning.clip, 1f);
    }

    public void PlayWinBGM()
    {
        winBGM.PlayOneShot(winBGM.clip, 1f);
    }

    public void PlayLoseBGM()
    {
        if (BattleEndMusic1TG.isOn == true)
        {
            loseBGM.loop = true;
            loseBGM.Play();
        }
        else if (BattleEndMusic2TG.isOn == true)
        {
            gameOver.loop = true;
            gameOver.Play();
        }
    }
}
