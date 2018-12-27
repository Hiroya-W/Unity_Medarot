using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController2 : MonoBehaviour
{
    //ゲームコントローラーの参照
    public GameController2 gameController;

    //サウンドプレイヤーの参照
    public SoundsPlayer soundPlayer;

    //リザルトテキスト
    public Text ResultText;

    //ゲーム終了フラグ
    public bool GameEndflag;

    //HPCheckerRunningフラグ
    public bool HPCRunnig = false;

    //HPバーの構造体
    [System.Serializable]
    public struct _HPBars
    {
        public GameObject Object;
        public Slider[] HPBar;
    }

    public _HPBars[] HPBar;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (!HPCRunnig)
        {
            HPCRunnig = true;
            HPChecker();
        }
        */
    }

    public void HPChecker1()
    {
        //フェーズが実行した時ではない
        if (!(gameController.phase == GameController2.Phase.None || gameController.phase == GameController2.Phase.Roboinfo))
        {
            //頭パーツのHPがなくなったら消す
            //プレイヤー
            for (int i = 0; i < gameController.selectedMedarotnum; i++)
            {
                //物体がアクティブ状態の時
                if (gameController.SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
                {
                    //パーツ分ループ
                    for (int j = 0; j < 4; j++)
                    {
                        //パーツがアクティブのとき
                        if (gameController.SelectPlayers[i].PlayerStatus.Parts[j].PartsAvailable == true)
                        {
                            //頭パーツが
                            if (HPBar[i].HPBar[0].value == 0)
                            {
                                //パーツ破壊メッセージ
                                gameController.BattleMessage.text = "頭パーツを破壊した！";

                                StartCoroutine(gameController.DelayMethod(1f, () =>
                                {
                                    gameController.BattleMessage.text = "機能停止！";
                                    gameController.SelectPlayers[i].PlayerGameObject.SetActive(false);
                                }));
                            }
                            //右腕、左腕、脚部パーツのとき
                            else if (HPBar[i].HPBar[j].value == 0)
                            {
                                //バトルメッセージ表示
                                switch (j)
                                {
                                    case 1:
                                        gameController.BattleMessage.text = "右腕パーツを破壊した！";
                                        break;
                                    case 2:
                                        gameController.BattleMessage.text = "左腕パーツを破壊した！";
                                        break;
                                    case 3:
                                        gameController.BattleMessage.text = "脚部パーツを破壊した！";
                                        break;
                                }
                                //パーツを使用できなくする
                                gameController.SelectPlayers[i].PlayerStatus.Parts[j].PartsAvailable = false;
                                StartCoroutine(gameController.DelayMethod(1f, () =>
                                {
                                    //フェーズを切り替える
                                    gameController.phase = GameController2.Phase.Move;
                                }));
                            }
                        }
                    }
                }
            }
            //敵
            for (int i = 0; i < gameController.maxEnemy; i++)
            {
                //物体がアクティブ状態の時
                if (gameController.SelectEnemys[i].PlayerGameObject.activeInHierarchy == true)
                {
                    //パーツ分ループ
                    for (int j = 0; j < 4; j++)
                    {
                        //パーツがアクティブのとき
                        if (gameController.SelectEnemys[i].PlayerStatus.Parts[j].PartsAvailable == true)
                        {
                            //頭パーツが
                            if (HPBar[i + 3].HPBar[0].value == 0)
                            {
                                //パーツ破壊メッセージ
                                gameController.BattleMessage.text = "頭パーツを破壊した！";

                                StartCoroutine(gameController.DelayMethod(1f, () =>
                                {
                                    gameController.BattleMessage.text = "機能停止！";
                                    gameController.SelectEnemys[i].PlayerGameObject.SetActive(false);
                                }));
                            }
                            //右腕、左腕、脚部パーツのとき
                            else if (HPBar[i].HPBar[j].value == 0)
                            {
                                //バトルメッセージ表示
                                switch (j)
                                {
                                    case 1:
                                        gameController.BattleMessage.text = "右腕パーツを破壊した！";
                                        break;
                                    case 2:
                                        gameController.BattleMessage.text = "左腕パーツを破壊した！";
                                        break;
                                    case 3:
                                        gameController.BattleMessage.text = "脚部パーツを破壊した！";
                                        break;
                                }
                                //パーツを使用できなくする
                                gameController.SelectEnemys[i].PlayerStatus.Parts[j].PartsAvailable = false;
                                StartCoroutine(gameController.DelayMethod(1f, () =>
                                {
                                    //フェーズを切り替える
                                    gameController.phase = GameController2.Phase.Move;
                                }));
                            }
                        }
                    }
                }
            }

            //HPが0になったら
            if (HPBar[0].HPBar[0].value == 0)
            {
                //フェーズ移行
                gameController.phase = GameController2.Phase.Defeat;

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
                gameController.phase = GameController2.Phase.Winning;

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
        HPCRunnig = false;
    }

    public void SetUpBar(int Playernum, int Enemynum)
    {
        //選ばれたプレイヤーの数だけ設定
        for (int i = 0; i < Playernum; i++)
        {
            //オブジェクトを表示
            HPBar[i].Object.SetActive(true);

            for (int j = 0; j < 4; j++)
            {
                //4パーツ分
                HPBar[i].HPBar[j].maxValue = gameController.SelectPlayers[i].PlayerStatus.Parts[j].HP;
                HPBar[i].HPBar[j].value    = gameController.SelectPlayers[i].PlayerStatus.Parts[j].HP;
            }
        }

        //選ばれたエネミーの数だけ設定
        for (int i = 0; i < Enemynum; i++)
        {
            int n = i + 3;
            //オブジェクトを表示
            HPBar[n].Object.SetActive(true);

            for (int j = 0; j < 4; j++)
            {
                //4パーツ分
                HPBar[n].HPBar[j].maxValue = gameController.SelectEnemys[i].PlayerStatus.Parts[j].HP;
                HPBar[n].HPBar[j].value = gameController.SelectEnemys[i].PlayerStatus.Parts[j].HP;
            }
        }
    }

    public void ChangeValue(int Targetnum, int TargetPart, int damage)
    {
        HPBar[Targetnum].HPBar[TargetPart].value -= damage;
    }
    
    public void SetValue(int Targetnum, int TargetPart, int value)
    {
        HPBar[Targetnum].HPBar[TargetPart].value = value;
    }
}
