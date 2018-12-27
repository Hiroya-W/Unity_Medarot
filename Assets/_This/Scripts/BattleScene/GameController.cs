using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    //HpBarControllerの参照
    public HpBarController hpbarctrl;

    //フェーズを表す数字を文字列としておく
    public enum Phase
    {
        None,
        Command,
        Move,
        Attack,
        Defence,
        Winning,
        Defeat,
    }

    //Phase型の変数を用意 最初はNone(0)である
    public Phase phase = Phase.None;

    //移動量
    //倍率
    private float PlayerMovePower = 1.5f;
    private float EnemyMovePower  = 1.0f;
    //移動方向はZ方向のみ
    private Vector3 Movement = new Vector3(0, 0, 1);
    private Vector3 PlayerMovement;
    private Vector3 EnemyMovement;

    //プレイヤーのゲームオブジェクト
    public GameObject Player;
    //エネミーのゲームオブジェクト
    public GameObject Enemy;

    //コンソールテキスト
    public Text ConsoleText;
    //スクロールバー
    public Scrollbar consoleScrollbar;

    //リザルトテキスト
    public Text ResultText;
    public bool Endflag = false;

    //パーツの構造体
    [System.Serializable]
    public struct _Parts
    {
        public bool PartsAvailable;
        public int HP;
    }

    //プレイヤーステータスの構造体
    [System.Serializable]
    public struct _PlayerStatus
    {
        public bool Charge;
        public _Parts Head;
        public _Parts RightArm;
        public _Parts LeftArm;
        public _Parts Leg;
    }
    //自機は三体固定？
    public _PlayerStatus[] player = new _PlayerStatus[3];
    //プレイヤーの数 
    public int playernum = 1;
    
    //自機は三体固定？
    public _PlayerStatus[] enemy  = new _PlayerStatus[3];
    //エネミーの数 
    public int enemynum = 1;

    void Start()
    {
        //コンソールテキストの初期化
        ConsoleText.text = "";
        //リザルトテキストの初期化
        ResultText.text = "";

        //移動量に倍率と変化量をかけておく
        PlayerMovement = Movement * Time.deltaTime * PlayerMovePower;
        EnemyMovement  = Movement * Time.deltaTime * EnemyMovePower * -1;

        //プレイヤーの初期化

        for (int i = 0; i < player.Length; i++)
        {
            if(i < playernum)
            {
                //プレイヤーの充電 == true, 冷却 == flase 
                player[i].Charge = true;
                //プレイヤーの頭パーツ
                player[i].Head.PartsAvailable       = true;
                player[i].Head.HP                   = 150;
                //プレイヤーの右腕パーツ
                player[i].RightArm.PartsAvailable   = true;
                player[i].RightArm.HP               = 50;
                //プレイヤーの左腕パーツ
                player[i].LeftArm.PartsAvailable    = true;
                player[i].LeftArm.HP                = 50;
                //プレイヤーの脚部パーツ
                player[i].Leg.PartsAvailable        = true;
                player[i].Leg.HP                    = 100;

                //HPバーの設定
                hpbarctrl.PlayerHpBarSetUp(i);
            }
        }

        //エネミーの初期化

        for (int i = 0; i < enemy.Length; i++)
        {
            if (i < enemynum)
            {                
                //エネミーの充電 == true, 冷却 == flase 
                enemy[i].Charge = true;
                //エネミーの頭パーツ
                enemy[i].Head.PartsAvailable        = true;
                enemy[i].Head.HP                    = 150;
                //エネミーの右腕パーツ
                enemy[i].RightArm.PartsAvailable    = true;
                enemy[i].RightArm.HP                = 50;
                //エネミーの左腕パーツ
                enemy[i].LeftArm.PartsAvailable     = true;
                enemy[i].LeftArm.HP                 = 50;
                //エネミーの脚部パーツ
                enemy[i].Leg.PartsAvailable         = true;
                enemy[i].Leg.HP                     = 100;

                //HPバーの設定
                hpbarctrl.EnemyHpBarSetUp(i);
            }
        }
    }

    void Update()
    {
        consoleScrollbar.value = 0;
        switch (phase)
        {
            //何もしていない時
            case Phase.None:
                break;
            //コマンド選択中
            case Phase.Command:
                break;
            //プレイ中
            case Phase.Move:
                PhaseMovefunc();
                break;
            //攻撃中
            case Phase.Attack:
                PhaseAttackfunc();
                break;
            //防御中
            case Phase.Defence:
                PhaseDefencefunc();
                break;
            //勝利
            case Phase.Winning:
                if(Endflag == false)
                {
                    ConsoleTextWriter("勝利しました");
                    ResultText.text = "勝利！";
                    Endflag = true;
                }
                break;
            //敗北
            case Phase.Defeat:
                if(Endflag == false)
                {
                    ConsoleTextWriter("敗北しました");
                    ResultText.text = "敗北！";
                    Endflag = true;
                }
                break;
        }
    }

    //フェーズがMoveの時の動作
    private void PhaseMovefunc()
    {
        //フェーズがMoveだったら
        //自機を移動させる
        for (int i = 0; i < player.Length; i++)
        {
            //存在するプレイヤーの数以内だったら
            if (i < playernum)
            {
                //プレイヤーが充電中であれば前進
                if (player[i].Charge)
                {
                    Player.transform.position += PlayerMovement;
                }
                //プレイヤーは冷却中であれば後退
                else
                {
                    Player.transform.position -= PlayerMovement;
                }
            }
        }
        //敵を移動させる
        for (int i = 0; i < enemy.Length; i++)
        {
            //存在するエネミーの数以内だったら
            if (i < enemynum)
            {
                //エネミーが充電中であれば前進
                if (enemy[i].Charge)
                {
                    Enemy.transform.position += EnemyMovement;
                }
                //エネミーは冷却中であれば後退
                else
                {
                    Enemy.transform.position -= EnemyMovement;
                }
            }
        }
    }

    //フェーズがAttackの時の動作
    private void PhaseAttackfunc()
    {
        //フェーズがAttackだったら
        //敵に攻撃する
        //敵の数を数える(Enemyとタグのついたオブジェクトを数える)
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag("Enemy");
        ConsoleTextWriter("敵の数は" + enemyUnits.Length + "体");

        //敵はランダムで選ぶ
        int targetEnemy = Random.Range(0, enemyUnits.Length - 1);
        ConsoleTextWriter("ターゲットは" + targetEnemy);

        //パーツをランダムで選ぶ
        int targetParts = Random.Range(0, 4);
        //int targetParts = 3;
        ConsoleTextWriter("ターゲットパーツは" + targetParts);

        //ダメージ量
        int damage = Random.Range(10, 100);

        //攻撃
        //yield return StartCoroutine(AttacktoParts(targetEnemy, targetParts, damage));
        AttacktoParts(targetEnemy, targetParts, damage);
        //StartCoroutine(AttacktoParts(targetEnemy, targetParts, damage));

        //攻撃が終了した時、勝敗が確定していなければ続行
        if (!(phase == Phase.Winning || phase == Phase.Defeat))
        {
            //攻撃が終了すれば
            Player.transform.position -= PlayerMovement;

            //冷却に入る
            player[0].Charge = false;

            //フェーズはMoveに移動
            PhasetoMove();
        }
        
        if (enemyUnits.Length == 1)
        {
            //敵全滅時の処理
        }
    }

    //フェーズがDefenceの時の動作
    private void PhaseDefencefunc()
    {
        //フェーズがDefenceだったら
        //自機に敵が攻撃する
        //自機の数を数える(Playerとタグのついたオブジェクトを数える)
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("Player");
        ConsoleTextWriter("敵の数は" + playerUnits.Length + "体");

        //攻撃対象の自機はランダムで選ぶ
        int targetPlayer = Random.Range(0, playerUnits.Length - 1);
        ConsoleTextWriter("ターゲットは" + targetPlayer);

        //パーツをランダムで選ぶ
        int targetParts = Random.Range(0, 4);
        //int targetParts = 3;
        ConsoleTextWriter("ターゲットパーツは" + targetParts);

        //ダメージ量
        int damage = Random.Range(10, 100);

        //攻撃
        AttackFromEnemy(targetPlayer, targetParts, damage);

        //攻撃が終了した時、勝敗が確定していなければ続行
        if (!(phase == Phase.Winning || phase == Phase.Defeat))
        {
            Enemy.transform.position -= EnemyMovement;

            //冷却に入る
            enemy[0].Charge = false;

            //フェーズはMoveに移動
            PhasetoMove();
        }
    }

    //自機が敵へ攻撃
    public void AttacktoParts( int Enemynum, int Parts, int damage )
    {
        //ゲームオブジェクトEnemyがtrueであるなら
        if (Enemy == true)
        {

            //頭パーツ
            if (Parts == 0)
            {
                //パーツがtrueの時
                if (enemy[Enemynum].Head.PartsAvailable == true)
                {
                    //前のHPを保持
                    int oldhp = enemy[Enemynum].Head.HP;

                    //ダメージ分を引く
                    ConsoleTextWriter("Enemy[" + Enemynum + "]の頭パーツに" + damage + "ダメージ");
                    enemy[Enemynum].Head.HP -= damage;

                    //HPバー
                    //yield return StartCoroutine(hpbarctrl.CompareStatus(Enemynum, Parts, damage));
                    hpbarctrl.funcCES(Enemynum, Parts, damage);

                    //もしHPが0以下になったら
                    if (enemy[Enemynum].Head.HP <= 0)
                    {
                        //HPを0にする
                        enemy[Enemynum].Head.HP = 0;
                        //パーツをfalseにする
                        enemy[Enemynum].Head.PartsAvailable = false;
                        ConsoleTextWriter("頭パーツが破壊された");

                        Enemy.SetActive(false);
                        ConsoleTextWriter("機能停止");
                        //勝利
                        PhasetoWinning();
                    }

                    ConsoleTextWriter("頭パーツのHPは" + oldhp + "→" + enemy[Enemynum].Head.HP);
                    
                }
            }
            //右腕パーツ
            else if (Parts == 1)
            {
                //パーツがtrueの時
                if (enemy[Enemynum].RightArm.PartsAvailable == true)
                {
                    //前のHPを保持
                    int oldhp = enemy[Enemynum].RightArm.HP;

                    //ダメージ分を引く
                    ConsoleTextWriter("Enemy[" + Enemynum + "]の右腕パーツに" + damage + "ダメージ");
                    enemy[Enemynum].RightArm.HP -= damage;

                    //HPバー
                    //yield return StartCoroutine(hpbarctrl.CompareStatus(Enemynum, Parts, damage));
                    hpbarctrl.funcCES(Enemynum, Parts, damage);

                    //もしHPが0以下になったら
                    if (enemy[Enemynum].RightArm.HP <= 0)
                    {
                        //HPを0にする
                        enemy[Enemynum].RightArm.HP = 0;
                        //パーツをfalseにする
                        enemy[Enemynum].RightArm.PartsAvailable = false;
                        ConsoleTextWriter("右腕パーツが破壊された");
                    }

                    ConsoleTextWriter("右腕パーツのHPは" + oldhp + "→" + enemy[Enemynum].RightArm.HP);
                }
                //パーツがfalseの時　別のパーツをランダムで選ぶ
                else
                {
                    AttacktoParts(Enemynum, Random.Range(0, 3), damage);
                }
            }
            //左腕パーツ
            else if (Parts == 2)
            {
                //パーツがtrueの時
                if (enemy[Enemynum].LeftArm.PartsAvailable == true)
                {
                    //前のHPを保持
                    int oldhp = enemy[Enemynum].LeftArm.HP;

                    //ダメージ分を引く
                    ConsoleTextWriter("Enemy[" + Enemynum + "]の左腕パーツに" + damage + "ダメージ");
                    enemy[Enemynum].LeftArm.HP -= damage;

                    //HPバー
                    //yield return StartCoroutine(hpbarctrl.CompareStatus(Enemynum, Parts, damage));
                    hpbarctrl.funcCES(Enemynum, Parts, damage);

                    //もしHPが0以下になったら
                    if (enemy[Enemynum].LeftArm.HP <= 0)
                    {
                        //HPを0にする
                        enemy[Enemynum].LeftArm.HP = 0;
                        //パーツをfalseにする
                        enemy[Enemynum].LeftArm.PartsAvailable = false;
                        ConsoleTextWriter("左腕パーツが破壊された");
                    }

                    ConsoleTextWriter("左腕パーツのHPは" + oldhp + "→" + enemy[Enemynum].LeftArm.HP);
                }
                //パーツがfalseの時　別のパーツをランダムで選ぶ
                else
                {
                    AttacktoParts(Enemynum, Random.Range(0, 3), damage);
                }
            }
            //脚部パーツ
            else if (Parts == 3)
            {
                //パーツがtrueの時
                if (enemy[Enemynum].Leg.PartsAvailable == true)
                {
                    //前のHPを保持
                    int oldhp = enemy[Enemynum].Leg.HP;

                    //ダメージ分を引く
                    ConsoleTextWriter("Enemy[" + Enemynum + "]の脚部パーツに" + damage + "ダメージ");
                    enemy[Enemynum].Leg.HP -= damage;

                    //HPバー
                    //yield return StartCoroutine(hpbarctrl.CompareStatus(Enemynum, Parts, damage));
                    hpbarctrl.funcCES(Enemynum, Parts, damage);

                    //もしHPが0以下になったら
                    if (enemy[Enemynum].Leg.HP <= 0)
                    {
                        //HPを0にする
                        enemy[Enemynum].Leg.HP = 0;
                        //パーツをfalseにする
                        enemy[Enemynum].Leg.PartsAvailable = false;
                        ConsoleTextWriter("脚部パーツが破壊された");
                    }

                    ConsoleTextWriter("脚部パーツのHPは" + oldhp + "→" + enemy[Enemynum].Leg.HP);
                }
                //パーツがfalseの時　別のパーツをランダムで選ぶ
                else
                {
                    AttacktoParts(Enemynum, Random.Range(0, 3), damage);
                }
            }
        }
    }

    //敵が自機へ攻撃
    public void AttackFromEnemy(int Playernum, int Parts, int damage)
    {
        //ゲームオブジェクトEnemyがtrueであるなら
        if (Player == true)
        {

            //頭パーツ
            if (Parts == 0)
            {
                //パーツがtrueの時
                if (enemy[Playernum].Head.PartsAvailable == true)
                {
                    //前のHPを保持
                    int oldhp = player[Playernum].Head.HP;

                    //ダメージ分を引く
                    ConsoleTextWriter("Player[" + Playernum + "]の頭パーツに" + damage + "ダメージ");
                    player[Playernum].Head.HP -= damage;

                    //HPバー
                    hpbarctrl.funcCPS(Playernum, Parts, damage);

                    //もしHPが0以下になったら
                    if (player[Playernum].Head.HP <= 0)
                    {
                        //HPを0にする
                        player[Playernum].Head.HP = 0;
                        //パーツをfalseにする
                        player[Playernum].Head.PartsAvailable = false;
                        ConsoleTextWriter("頭パーツが破壊された");

                        Player.SetActive(false);
                        ConsoleTextWriter("機能停止");
                        //敗北
                        PhasetoDefeat();
                    }

                    ConsoleTextWriter("頭パーツのHPは" + oldhp + "→" + player[Playernum].Head.HP);
                }
            }
            //右腕パーツ
            else if (Parts == 1)
            {
                //パーツがtrueの時
                if (player[Playernum].RightArm.PartsAvailable == true)
                {
                    //前のHPを保持
                    int oldhp = player[Playernum].RightArm.HP;

                    //ダメージ分を引く
                    ConsoleTextWriter("Enemy[" + Playernum + "]の右腕パーツに" + damage + "ダメージ");
                    player[Playernum].RightArm.HP -= damage;

                    //HPバー
                    hpbarctrl.funcCPS(Playernum, Parts, damage);

                    //もしHPが0以下になったら
                    if (player[Playernum].RightArm.HP <= 0)
                    {
                        //HPを0にする
                        player[Playernum].RightArm.HP = 0;
                        //パーツをfalseにする
                        player[Playernum].RightArm.PartsAvailable = false;
                        ConsoleTextWriter("右腕パーツが破壊された");
                    }

                    ConsoleTextWriter("右腕パーツのHPは" + oldhp + "→" + player[Playernum].RightArm.HP);
                }
                //パーツがfalseの時　別のパーツをランダムで選ぶ
                else
                {
                    AttacktoParts(Playernum, Random.Range(0, 3), damage);
                }
            }
            //左腕パーツ
            else if (Parts == 2)
            {
                //パーツがtrueの時
                if (player[Playernum].LeftArm.PartsAvailable == true)
                {
                    //前のHPを保持
                    int oldhp = player[Playernum].LeftArm.HP;

                    //ダメージ分を引く
                    ConsoleTextWriter("Enemy[" + Playernum + "]の左腕パーツに" + damage + "ダメージ");
                    player[Playernum].LeftArm.HP -= damage;

                    //HPバー
                    hpbarctrl.funcCPS(Playernum, Parts, damage);

                    //もしHPが0以下になったら
                    if (player[Playernum].LeftArm.HP <= 0)
                    {
                        //HPを0にする
                        player[Playernum].LeftArm.HP = 0;
                        //パーツをfalseにする
                        player[Playernum].LeftArm.PartsAvailable = false;
                        ConsoleTextWriter("左腕パーツが破壊された");
                    }

                    ConsoleTextWriter("左腕パーツのHPは" + oldhp + "→" + player[Playernum].LeftArm.HP);
                }
                //パーツがfalseの時　別のパーツをランダムで選ぶ
                else
                {
                    AttacktoParts(Playernum, Random.Range(0, 3), damage);
                }
            }
            //脚部パーツ
            else if (Parts == 3)
            {
                //パーツがtrueの時
                if (player[Playernum].Leg.PartsAvailable == true)
                {
                    //前のHPを保持
                    int oldhp = enemy[Playernum].Leg.HP;

                    //ダメージ分を引く
                    ConsoleTextWriter("Enemy[" + Playernum + "]の脚部パーツに" + damage + "ダメージ");
                    player[Playernum].Leg.HP -= damage;

                    //HPバー
                    hpbarctrl.funcCPS(Playernum, Parts, damage);

                    //もしHPが0以下になったら
                    if (player[Playernum].Leg.HP <= 0)
                    {
                        //HPを0にする
                        player[Playernum].Leg.HP = 0;
                        //パーツをfalseにする
                        player[Playernum].Leg.PartsAvailable = false;
                        ConsoleTextWriter("脚部パーツが破壊された");
                    }

                    ConsoleTextWriter("脚部パーツのHPは" + oldhp + "→" + player[Playernum].Leg.HP);
                }
                //パーツがfalseの時　別のパーツをランダムで選ぶ
                else
                {
                    AttacktoParts(Playernum, Random.Range(0, 3), damage);
                }
            }
        }

    }


    //フェーズを移行
    //Move
    public void PhasetoMove()
    {
        //もし初めてフェーズがMoveに移動したら
        if (phase == Phase.None)
        {
            ConsoleTextWriter("ゲームがスタートされました");
        }
        //フェーズをMoveに移行する
        phase = Phase.Move;
        ConsoleTextWriter("フェーズが[Move]に移行しました");
    }
    //Command
    public void PhasetoCommand()
    {
        //フェーズをAttackに移行する
        phase = Phase.Command;
        ConsoleTextWriter("フェーズが[Command]へ移行しました");
    }
    //Attack
    public void PhasetoAttack()
    {
        //フェーズをAttackに移行する
        phase = Phase.Attack;
        ConsoleTextWriter("フェーズが[Attack]へ移行しました");
    }
    //Defence
    public void PhasetoDefence()
    {
        //フェーズをDefenceに移行する
        phase = Phase.Defence;
        ConsoleTextWriter("フェーズ[Defence]へ移行しました");
    }
    //Winning
    public void PhasetoWinning()
    {
        //フェーズをWinningに移行する
        phase = Phase.Winning;
        ConsoleTextWriter("フェーズが[Winning]へ移行しました");
    }
    //Defeat
    public void PhasetoDefeat()
    {
        //フェーズをDefeatに移行する
        phase = Phase.Defeat;
        ConsoleTextWriter("フェーズが[Defeat]へ移行しました");
    }

    //コンソールにテキストを描画する
    public void ConsoleTextWriter( string str )
    {
        ConsoleText.text = ConsoleText.text + str + "\n";
    }
}
