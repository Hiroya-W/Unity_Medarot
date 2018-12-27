using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController1 : MonoBehaviour {

    //ゲームコントローラーの参照
    public GameController1 gameController;

    //サウンドプレイヤーの参照
    public SoundsPlayer soundPlayer;

    //リザルトテキスト
    public Text ResultText;

    //ゲーム終了フラグ
    public bool GameEndflag;


    //HPバーの構造体
    [System.Serializable]
    public struct _HPBars
    {
        public GameObject Object;
        public Slider[] HPBar;
    }

    public _HPBars[] HPBar;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //フェーズが実行した時ではない
        if (!(gameController.phase == GameController1.Phase.None || gameController.phase == GameController1.Phase.Roboinfo))
        {
            //HPが0になったら
            if (HPBar[0].HPBar[0].value == 0)
            {
                //フェーズ移行
                gameController.phase = GameController1.Phase.Defeat;

                if (!GameEndflag)
                {
                    //音楽を止める
                    soundPlayer.StopBattleMusic();
                    //敗北BGM
                    //未実装
                }

                //テキストを表示
                ResultText.gameObject.SetActive(true);
                ResultText.text = "LOSE";
                //フラグを立てる
                GameEndflag = true;
            }
            else if (HPBar[3].HPBar[0].value == 0)
            {
                //フェーズ移行
                gameController.phase = GameController1.Phase.Winning;

                if (!GameEndflag)
                {
                    //音楽を止める
                    soundPlayer.StopBattleMusic();
                    //勝利BGM
                    soundPlayer.PlayWinBGM();
                }
                //テキスト表示
                ResultText.gameObject.SetActive(true);
                ResultText.text = "WIN";
                //フラグを立てる
                GameEndflag = true;
            }
        }

    }

    public void SetUpBar( int Playernum, int Enemynum) 
    {
        //選ばれたプレイヤーの数だけ設定
        for (int i = 0; i < Playernum; i++)
        {
            //オブジェクトを表示
            HPBar[i].Object.SetActive(true);

            //4パーツ分
            HPBar[i].HPBar[0].maxValue = gameController.SelectPlayers[i].PlayerStatus.Head.HP;
            HPBar[i].HPBar[0].value    = gameController.SelectPlayers[i].PlayerStatus.Head.HP;
            HPBar[i].HPBar[1].maxValue = gameController.SelectPlayers[i].PlayerStatus.RightArm.HP;
            HPBar[i].HPBar[1].value    = gameController.SelectPlayers[i].PlayerStatus.RightArm.HP;
            HPBar[i].HPBar[2].maxValue = gameController.SelectPlayers[i].PlayerStatus.LeftArm.HP;
            HPBar[i].HPBar[2].value    = gameController.SelectPlayers[i].PlayerStatus.LeftArm.HP;
            HPBar[i].HPBar[3].maxValue = gameController.SelectPlayers[i].PlayerStatus.Leg.HP;
            HPBar[i].HPBar[3].value    = gameController.SelectPlayers[i].PlayerStatus.Leg.HP;
        }

        //選ばれたエネミーの数だけ設定
        for (int i = 0; i < Enemynum; i++)
        {
            int n = i + 3;
            //オブジェクトを表示
            HPBar[n].Object.SetActive(true);

            //4パーツ分
            HPBar[n].HPBar[0].maxValue = gameController.SelectEnemys[i].PlayerStatus.Head.HP;
            HPBar[n].HPBar[0].value    = gameController.SelectEnemys[i].PlayerStatus.Head.HP;
            HPBar[n].HPBar[1].maxValue = gameController.SelectEnemys[i].PlayerStatus.RightArm.HP;
            HPBar[n].HPBar[1].value    = gameController.SelectEnemys[i].PlayerStatus.RightArm.HP;
            HPBar[n].HPBar[2].maxValue = gameController.SelectEnemys[i].PlayerStatus.LeftArm.HP;
            HPBar[n].HPBar[2].value    = gameController.SelectEnemys[i].PlayerStatus.LeftArm.HP;
            HPBar[n].HPBar[3].maxValue = gameController.SelectEnemys[i].PlayerStatus.Leg.HP;
            HPBar[n].HPBar[3].value    = gameController.SelectEnemys[i].PlayerStatus.Leg.HP;
        }
    }

    public void ChangeValue(int Targetnum, int TargetPart, int damage)
    {
        HPBar[Targetnum].HPBar[TargetPart].value -= damage; 
    }
}
