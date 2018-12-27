using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour {

    //スライダーの参照
    //エネミー
    public Slider EnemyHead;
    public Slider EnemyRArm;
    public Slider EnemyLArm;
    public Slider EnemyLeg;
    //プレイヤー
    public Slider PlayerHead;
    public Slider PlayerRArm;
    public Slider PlayerLArm;
    public Slider PlayerLeg;

    //GameControllerの参照
    public GameController gameController;

    //比較時のフラグ
    public bool Compareflag = false;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

	}

    //HPバーのセット
    public void EnemyHpBarSetUp(int i)
    {
        EnemyHead.maxValue = gameController.enemy[i].Head.HP;
        EnemyHead.value    = gameController.enemy[i].Head.HP;
        EnemyRArm.maxValue = gameController.enemy[i].RightArm.HP;
        EnemyRArm.value    = gameController.enemy[i].RightArm.HP;
        EnemyLArm.maxValue = gameController.enemy[i].LeftArm.HP;
        EnemyLArm.value    = gameController.enemy[i].LeftArm.HP;
        EnemyLeg.maxValue  = gameController.enemy[i].Leg.HP;
        EnemyLeg.value     = gameController.enemy[i].Leg.HP;
    }
    public void PlayerHpBarSetUp(int i)
    {
        PlayerHead.maxValue = gameController.player[i].Head.HP;
        PlayerHead.value    = gameController.player[i].Head.HP;
        PlayerRArm.maxValue = gameController.player[i].RightArm.HP;
        PlayerRArm.value    = gameController.player[i].RightArm.HP;
        PlayerLArm.maxValue = gameController.player[i].LeftArm.HP;
        PlayerLArm.value    = gameController.player[i].LeftArm.HP;
        PlayerLeg.maxValue  = gameController.player[i].Leg.HP;
        PlayerLeg.value     = gameController.player[i].Leg.HP;
    }

    public void CompareEnemyStatus(int targetenemy, int targetParts, int damage)
    {
        Compareflag = false;
        float HP;
        switch (targetParts)
        {
            case 0:
                //HPは保持
                HP = EnemyHead.value;
                //受けたダメージ分のゲージが減るまで
                if(!(EnemyHead.value == HP - damage || EnemyHead.value == 0))
                {
                    EnemyHead.value -= 1;
                }
                //減りきったら
                else
                {
                    Compareflag = true;
                }
                break;
            case 1:
                //HPは保持
                HP = EnemyRArm.value;
                //受けたダメージ分のゲージが減るまで
                if (!(EnemyRArm.value == HP - damage || EnemyRArm.value == 0))
                {
                    EnemyRArm.value -= 1;
                }
                //減りきったら
                else
                {
                    Compareflag = true;
                }
                break;
            case 2:
                //HPは保持
                HP = EnemyLArm.value;
                //受けたダメージ分のゲージが減るまで
                if (!(EnemyLArm.value == HP - damage || EnemyLArm.value == 0))
                {
                    EnemyLArm.value -= 1;
                }
                //減りきったら
                else
                {
                    Compareflag = true;
                }
                break;
            case 3:
                //HPは保持
                HP = EnemyLeg.value;
                //受けたダメージ分のゲージが減るまで
                if (!(EnemyLeg.value == HP - damage || EnemyLeg.value == 0))
                {
                    EnemyLeg.value -= 1;
                }
                //減りきったら
                else
                {
                    Compareflag = true;
                }
                break;
        }
        //0.1f待つ
        //yield return new WaitForSeconds(0.1f);
        //再起
        if (!(Compareflag))
        {
            CompareEnemyStatus(targetenemy, targetParts, damage - 1);
        }
    }

    public void ComparePlayerStatus(int targetplayer, int targetParts, int damage)
    {
        Compareflag = false;
        float HP;
        switch (targetParts)
        {
            case 0:
                //HPは保持
                HP = PlayerHead.value;
                //受けたダメージ分のゲージが減るまで
                if (!(PlayerHead.value == HP - damage || PlayerHead.value == 0))
                {
                    PlayerHead.value -= 1;
                }
                //減りきったら
                else
                {
                    Compareflag = true;
                }
                break;
            case 1:
                //HPは保持
                HP = PlayerRArm.value;
                //受けたダメージ分のゲージが減るまで
                if (!(PlayerRArm.value == HP - damage || PlayerRArm.value == 0))
                {
                    PlayerRArm.value -= 1;
                }
                //減りきったら
                else
                {
                    Compareflag = true;
                }
                break;
            case 2:
                //HPは保持
                HP = PlayerLArm.value;
                //受けたダメージ分のゲージが減るまで
                if (!(PlayerLArm.value == HP - damage || PlayerLArm.value == 0))
                {
                    PlayerLArm.value -= 1;
                }
                //減りきったら
                else
                {
                    Compareflag = true;
                }
                break;
            case 3:
                //HPは保持
                HP = PlayerLeg.value;
                //受けたダメージ分のゲージが減るまで
                if (!(PlayerLeg.value == HP - damage || PlayerLeg.value == 0))
                {
                    PlayerLeg.value -= 1;
                }
                //減りきったら
                else
                {
                    Compareflag = true;
                }
                break;
        }
        //0.1f待つ
        //yield return new WaitForSeconds(0.1f);
        //再起
        if (!(Compareflag))
        {
            ComparePlayerStatus(targetplayer, targetParts, damage - 1);
        }
    }
    public void funcCES(int targetenemy, int targetParts, int damage)
    {
        Coroutine coroutine = StartCoroutine(DelayMethod(0.5f, () => {
            CompareEnemyStatus(targetenemy,targetParts,damage);//動作
        }));
    }

    public void funcCPS(int targetplayer, int targetParts, int damage)
    {
        Coroutine coroutine = StartCoroutine(DelayMethod(0.5f, () => {
            ComparePlayerStatus(targetplayer, targetParts, damage);//動作
        }));
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}

