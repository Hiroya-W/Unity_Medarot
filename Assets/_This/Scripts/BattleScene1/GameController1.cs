using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController1 : MonoBehaviour
{
    public PartsListScript PartsList;
    public SoundsPlayer soundPlayer;


    //フェーズを表す数字を文字列としておく
    public enum Phase
    {
        None,               //ゲームスタート待機
        Roboinfo,           //プレイヤー選択
        Command,            //コマンド選択
        Command1,           //コマンド選択　個別
        Move,               //充電　冷却
        Attack,             //プレイヤーの攻撃
        Defence,            //エネミーの攻撃
        Winning,            //勝利
        Defeat,             //敗北
    }

    //Phase型の変数を用意 最初はNone(0)である
    public Phase phase = Phase.None;

    //生まれてくる敵プレハブ
    public GameObject playerPrefab;

    //プレイヤーを格納
    //パーツの構造体
    [System.Serializable]
    public struct _Parts
    {
        public bool PartsAvailable;
        public int PartsID;
        public int HP;
    }
    //プレイヤーステータスの構造体
    [System.Serializable]
    public struct _PlayerStatus
    {
        public bool SelectFrag;
        public int Playernum;
        public string Name;
        public bool Cooling;
        public _Parts Head;
        public int HeadCount;
        public _Parts RightArm;
        public _Parts LeftArm;
        public _Parts Leg;
        public int TargetEnemy;
        public int TargetParts;
        public int SelectPartnum;
        public string defencecommand;
    }

    //存在するプレイヤーの格納
    [System.Serializable]
    public struct _ExistPlayers
    {
        //ゲームオブジェクト
        public GameObject PlayerGameObject;
        //ステータス
        public _PlayerStatus PlayerStatus;
    }
    public _ExistPlayers[] ExistPlayers;
    public _ExistPlayers[] ExistEnemys;

    public Vector3[] PlayerMovement;
    public Vector3[] EnemyMovement;
    public Vector3 Movement;

    public int PlayerMovingSpeed = 100;
    public int EnemyMovingSpeed = 0;

    //選ばれたプレイヤーナンバー
    int[] selectPlayernum;
    //選ばれたプレイヤーの格納
    public _ExistPlayers[] SelectPlayers;
    public _ExistPlayers[] SelectEnemys;
    //プレイヤーの保持数
    public int maxPlayer = 3;
    public int maxEnemy = 3;
    //選択されたプレイヤーの数
    public int selPlayer = 0;

    //プレイヤーのマテリアル
    public Color[] playerAlbedo;
    public Color[] enemyAlbedo;

    //配置する座標を設定
    Vector3 placePosition;
    //配置する回転角を設定
    Quaternion q;

    void Start()
    {
        //配列確保
        //プレイヤー保持数はmaxPlayer
        ExistPlayers = new _ExistPlayers[maxPlayer];
        ExistEnemys = new _ExistPlayers[maxEnemy];

        //テキストの初期化
        MedarotName.text = "";
        MedarotPartsName.text = "";
        MedarotPartsArmor.text = "";
        MedarotPartsMethod.text = "";
        MedarotPartsSkill.text = "";
        SelectMedarotName.text = "";

        //選べるプレイヤーは3体
        selectPlayernum = new int[3];
        SelectPlayers = new _ExistPlayers[3];
        SelectEnemys  = new _ExistPlayers[3];

        //色の設定
        playerAlbedo = new Color[]{
                new Color(255,238,15,255)/255,
                new Color(255,119,255,255)/255,
                new Color(84,119,223,255)/255
        };

        //色の設定
        enemyAlbedo = new Color[]{
                new Color(255,238,15,255)/600,
                new Color(255,119,255,255)/600,
                new Color(84,119,223,255)/600
        };
        //プレイヤーとエネミーの生成
        PlayerGenerate();
        EnemyGenerate();

        Movement = new Vector3(0, 0, 1) * Time.deltaTime;

        //100倍くらいがちょうどいい　基準
        //プレイヤーの移動量
        PlayerMovement = new Vector3[3]{
            Movement ,
            Movement ,
            Movement ,
        };
        //エネミーの移動量
        EnemyMovement = new Vector3[3]{
            Movement * -1,
            Movement * -1,
            Movement * -1,
        };
    }

    void Update()
    {
        switch (phase)
        {
            //何もしていない時
            case Phase.None:
                break;
            //プレイヤー選択
            case Phase.Roboinfo:
                break;
            //コマンド選択中
            case Phase.Command:
                break;
            //充填、冷却中
            case Phase.Move:
                PlayerMoving();
                //PhaseMovefunc();
                break;
            //攻撃中
            case Phase.Attack:
                //PhaseAttackfunc();
                break;
            //防御中
            case Phase.Defence:
                //PhaseDefencefunc();
                break;
            //勝利
            case Phase.Winning:
                /*
                if (Endflag == false)
                {
                    ConsoleTextWriter("勝利しました");
                    ResultText.text = "勝利！";
                    Endflag = true;
                }
                */
                break;
            //敗北
            case Phase.Defeat:
                /*
                if (Endflag == false)
                {
                    ConsoleTextWriter("敗北しました");
                    ResultText.text = "敗北！";
                    Endflag = true;
                }
                */
                break;
        }

    }

    //プレイヤーを生成
    void PlayerGenerate()
    {
        //プレイヤー保持数だけ繰り返す
        for (int playerCount = 0; playerCount < maxPlayer; ++playerCount)
        {
            //中身がnullだったら
            if (ExistPlayers[playerCount].PlayerGameObject == null)
            {
                //生成する座標
                placePosition = new Vector3(-100+playerCount*10, 3f, -9.325f);
                //回転角
                q = new Quaternion();
                //回転角は0
                q = Quaternion.identity;

                //プレイヤーを作成する
                //インスタンスの作成
                ExistPlayers[playerCount].PlayerGameObject = Instantiate(playerPrefab, placePosition, q) as GameObject;

                //色変更
                ExistPlayers[playerCount].PlayerGameObject.GetComponent<Renderer>().material.color = playerAlbedo[playerCount];
                //プレイヤーのパーツID 1,2,3が順に入る
                ExistPlayers[playerCount].PlayerStatus.Head.PartsID     = PartsList.CharaList[playerCount + 1].ID; 
                ExistPlayers[playerCount].PlayerStatus.RightArm.PartsID = PartsList.CharaList[playerCount + 1].ID; 
                ExistPlayers[playerCount].PlayerStatus.LeftArm.PartsID  = PartsList.CharaList[playerCount + 1].ID; 
                ExistPlayers[playerCount].PlayerStatus.Leg.PartsID      = PartsList.CharaList[playerCount + 1].ID;

                //選んだプレイヤーの頭パーツのIDからメダロットの名前を拾ってくる
                ExistPlayers[playerCount].PlayerStatus.Name = PartsList.CharaList[ExistPlayers[playerCount].PlayerStatus.Head.PartsID].CharaName;
                //各パーツのステータス
                ExistPlayers[playerCount].PlayerStatus.Head.PartsAvailable = true;
                ExistPlayers[playerCount].PlayerStatus.Head.HP = PartsList.CharaList[ExistPlayers[playerCount].PlayerStatus.Head.PartsID].PartsList[0].Armor;
                ExistPlayers[playerCount].PlayerStatus.RightArm.PartsAvailable = true;
                ExistPlayers[playerCount].PlayerStatus.RightArm.HP = PartsList.CharaList[ExistPlayers[playerCount].PlayerStatus.RightArm.PartsID].PartsList[1].Armor;
                ExistPlayers[playerCount].PlayerStatus.LeftArm.PartsAvailable = true;
                ExistPlayers[playerCount].PlayerStatus.LeftArm.HP = PartsList.CharaList[ExistPlayers[playerCount].PlayerStatus.LeftArm.PartsID].PartsList[2].Armor;
                ExistPlayers[playerCount].PlayerStatus.Leg.PartsAvailable = true;
                ExistPlayers[playerCount].PlayerStatus.Leg.HP = PartsList.CharaList[ExistPlayers[playerCount].PlayerStatus.Leg.PartsID].PartsList[3].Armor;
            }
        }
    }
    
    //敵を生成
    void EnemyGenerate()
    {
        //プレイヤー保持数だけ繰り返す
        for (int enemyCount = 0; enemyCount < maxEnemy; ++enemyCount)
        {
            //中身がnullだったら
            if (ExistEnemys[enemyCount].PlayerGameObject == null)
            {
                //生成する座標
                placePosition = new Vector3(-100 + enemyCount * 10, 3f, 0.0f);
                //回転角
                q = new Quaternion();
                //回転角は0
                q = Quaternion.identity;

                //プレイヤーを作成する
                //インスタンスの作成
                ExistEnemys[enemyCount].PlayerGameObject = Instantiate(playerPrefab, placePosition, q) as GameObject;

                //色変更
                ExistEnemys[enemyCount].PlayerGameObject.GetComponent<Renderer>().material.color = enemyAlbedo[enemyCount];
                //プレイヤーのパーツID 1,2,3が順に入る
                ExistEnemys[enemyCount].PlayerStatus.Head.PartsID = PartsList.CharaList[enemyCount + 1].ID;
                ExistEnemys[enemyCount].PlayerStatus.RightArm.PartsID = PartsList.CharaList[enemyCount + 1].ID;
                ExistEnemys[enemyCount].PlayerStatus.LeftArm.PartsID = PartsList.CharaList[enemyCount + 1].ID;
                ExistEnemys[enemyCount].PlayerStatus.Leg.PartsID = PartsList.CharaList[enemyCount + 1].ID;

                //選んだプレイヤーの頭パーツのIDからメダロットの名前を拾ってくる
                ExistEnemys[enemyCount].PlayerStatus.Name = PartsList.CharaList[ExistEnemys[enemyCount].PlayerStatus.Head.PartsID].CharaName;
                //頭パーツのステータス
                ExistEnemys[enemyCount].PlayerStatus.Head.PartsAvailable = true;
                ExistEnemys[enemyCount].PlayerStatus.Head.HP = PartsList.CharaList[ExistEnemys[enemyCount].PlayerStatus.Head.PartsID].PartsList[0].Armor;
                ExistEnemys[enemyCount].PlayerStatus.RightArm.PartsAvailable = true;
                ExistEnemys[enemyCount].PlayerStatus.RightArm.HP = PartsList.CharaList[ExistEnemys[enemyCount].PlayerStatus.RightArm.PartsID].PartsList[1].Armor;
                ExistEnemys[enemyCount].PlayerStatus.LeftArm.PartsAvailable = true;
                ExistEnemys[enemyCount].PlayerStatus.LeftArm.HP = PartsList.CharaList[ExistEnemys[enemyCount].PlayerStatus.LeftArm.PartsID].PartsList[2].Armor;
                ExistEnemys[enemyCount].PlayerStatus.Leg.PartsAvailable = true;
                ExistEnemys[enemyCount].PlayerStatus.Leg.HP = PartsList.CharaList[ExistEnemys[enemyCount].PlayerStatus.Leg.PartsID].PartsList[3].Armor;
                SelectEnemys[enemyCount] = ExistEnemys[enemyCount];

                SelectEnemys[enemyCount].PlayerGameObject.transform.parent = MedarotTopParent.transform;
                SelectEnemys[enemyCount].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

                switch (enemyCount)
                {
                    case 0:
                        SelectEnemys[enemyCount].PlayerGameObject.transform.position = new Vector3(0.0f, 0.6f, 9.24f);
                        SelectEnemys[enemyCount].PlayerGameObject.tag = "Enemy1";
                        break;
                    case 1:
                        SelectEnemys[enemyCount].PlayerGameObject.transform.position = new Vector3(-6.11f, 0.6f, 9.24f);
                        SelectEnemys[enemyCount].PlayerGameObject.tag = "Enemy2";
                        break;
                    case 2:
                        SelectEnemys[enemyCount].PlayerGameObject.transform.position = new Vector3(6.11f, 0.6f, 9.24f);
                        SelectEnemys[enemyCount].PlayerGameObject.tag = "Enemy3";
                        break;
                }

            }
        }
    }

    //選ばれたプレイヤーを順にselectPlayersに格納
    void SelectPlayerGenerate()
    {
        for (int playerCount = 0; playerCount < selectPlayernum.Length; ++playerCount)
        {
            if (ExistPlayers[selectPlayernum[playerCount]].PlayerGameObject != null)
            {
                //プレイヤーを作成する
                SelectPlayers[playerCount].PlayerGameObject = Instantiate(playerPrefab, transform.position, transform.rotation) as GameObject;
                return;
            }
        }
    }

    public GameObject RobottleInfoWindow;
    public Image EncountFlash;

    public void ToRobottleInfo()
    {
        //ロボトルインフォフェーズに移動
        phase = Phase.Roboinfo;
        
        Coroutine coroutine = StartCoroutine(DelayMethod(2.65f, () => {
            RobottleInfoWindow.SetActive(true);
        }));

        StartCoroutine(EncountFlasher());
    }

    public Image BlackWall1;
    public Image BlackWall2;
    private RectTransform RectBlackWall1;
    private RectTransform RectBlackWall2;
    public GameObject EncountFlashObj;

    private IEnumerator EncountFlasher()
    {
        Image flashimg = EncountFlash.gameObject.GetComponent<Image>();
        for (int i = 0; i < 2; i++)
        {
            while(flashimg.color.a < 1)
            {
                //flashimg.color = Color.Lerp(flashimg.color, new Color(1,1,1,1), Time.deltaTime*4);
                flashimg.color += new Color(0, 0, 0, 0.03f);
                yield return null;
            }
            while(flashimg.color.a > 0)
            {
                flashimg.color -= new Color(0, 0, 0, 0.10f);
                yield return null;
            }
        }

        RectBlackWall1 = BlackWall1.GetComponent<RectTransform>();
        RectBlackWall2 = BlackWall2.GetComponent<RectTransform>();

        while ( RectBlackWall1.localPosition.x > 202)
        {
            RectBlackWall1.localPosition -= new Vector3(1, 0, 0) * 2;
            RectBlackWall2.localPosition += new Vector3(1, 0, 0) * 2;
            yield return null;
        }

        Coroutine coroutine = StartCoroutine(DelayMethod(2f, () => {
            EncountFlashObj.SetActive(false);
        }));
    }

    public Text LeaderName;
    public Text LeaderCommand;
    public GameObject Leader_sel1;

    public Text LeftName;
    public Text LeftCommand;
    public GameObject Left_sel1;

    public Text RightName;
    public Text RightCommand;
    public GameObject Right_sel1;

    public Text SelHeadCommand;
    public Text HeadCount;

    public Text SelLeftCommand;

    public Text SelRightCommand;

    public Text SelChargeCommand;

    public Text CommandText;

    public GameObject MedarotTopParent;

    public GameObject MedarotSelectWindow;

    public Image BlackImageForFade;

    private IEnumerator FadePlusToCommand()
    {
        //フェードアウト
        while (BlackImageForFade.color.a < 1)
        {
            BlackImageForFade.color += new Color(0, 0, 0, 0.01f);
            yield return null;
        }

        //BattleReadyWindowをfalseに
        BattleReadyWindow.SetActive(false);

        //MedarotSelectWindowをfalseに
        MedarotSelectWindow.SetActive(false);

        //CommandWindowをtrueに
        CommandWindow.SetActive(true);

        //CommandWindowChangeBtnをtrueに
        CommandWindowChangeBtn.SetActive(true);

        //リーダーから
        selectMedarotnum = 0;

        //プレイヤーの生成
        //親子関係を変更 回転 位置を移動 
        for (int playernum = 0; playernum < selectedMedarotnum; playernum++)
        {
            SelectPlayers[playernum].PlayerGameObject.transform.parent = MedarotTopParent.transform;
            SelectPlayers[playernum].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            switch (playernum)
            {
                case 0:
                    SelectPlayers[playernum].PlayerGameObject.transform.position = new Vector3(0.0f, 0.6f, -9.325f);
                    SelectPlayers[playernum].PlayerGameObject.tag = "Player1";
                    break;
                case 1:
                    SelectPlayers[playernum].PlayerGameObject.transform.position = new Vector3(-6.11f, 0.6f, -9.325f);
                    SelectPlayers[playernum].PlayerGameObject.tag = "Player2";
                    break;
                case 2:
                    SelectPlayers[playernum].PlayerGameObject.transform.position = new Vector3(6.11f, 0.6f, -9.325f);
                    SelectPlayers[playernum].PlayerGameObject.tag = "Player3";
                    break;
            }

        }

        //プレイヤーの名前
        LeaderName.text = "";
        RightName.text = "";
        LeftName.text = "";

        //コマンドテキスト
        CommandText.text = "こうどうをせんたくしてください";
        BattleMessage.text = "こうどうをせんたくしてください";

        //コマンド選択をしているキャラ
        Leader_sel1.SetActive(true);
        Left_sel1.SetActive(false);
        Right_sel1.SetActive(false);

        //プレイヤーの名前
        LeaderName.text = SelectPlayers[0].PlayerStatus.Name;
        LeftName.text = SelectPlayers[1].PlayerStatus.Name;
        RightName.text = SelectPlayers[2].PlayerStatus.Name;

        //コマンド
        LeaderCommand.text = "";
        RightCommand.text = "";
        LeftCommand.text = "";

        //最初はリーダーコマンド
        SelHeadCommand.text = PartsList.CharaList[SelectPlayers[0].PlayerStatus.Head.PartsID].PartsList[0].PartName;
        SelRightCommand.text = PartsList.CharaList[SelectPlayers[0].PlayerStatus.LeftArm.PartsID].PartsList[1].PartName;
        SelLeftCommand.text = PartsList.CharaList[SelectPlayers[0].PlayerStatus.RightArm.PartsID].PartsList[2].PartName;

        HeadMethodFlag = false;
        RightMethodFlag = false;
        LeftMethodFlag = false;

        //送れてBGM開始
        StartCoroutine(DelayMethod(0.7f, () => {
            //PlayBattleMusic
            soundPlayer.PlayBattleMusic();
        }));

        //フェードイン
        StartCoroutine(DelayMethod(0.2f, () => {
            StartCoroutine(FadeinToCommand());
        }));

    }

    private IEnumerator FadeinToCommand()
    {
        while (BlackImageForFade.color.a > 0)
        {
            BlackImageForFade.color -= new Color(0, 0, 0, 0.02f);
            yield return null;
        }

        //Fade用GameObjectを非アクティブにする
        BlackImageForFade.gameObject.SetActive(false);
    }

    public HpBarController1 hpbar;

    public void ToCommand()
    {
        //コマンドフェーズに移動
        phase = Phase.Command;

        //Fade用GameObjectをアクティブにする
        BlackImageForFade.gameObject.SetActive(true);

        //コルーチンで擬似的な並列処理
        //フェード
        StartCoroutine(FadePlusToCommand());

        //送れてBGMストップ
        StartCoroutine(DelayMethod(0.5f, () => {
            //StopEncountMusic
            soundPlayer.StopEncountMusic();
        }));

        //HPバーの設定
        hpbar.SetUpBar(selectedMedarotnum, maxEnemy);

        //攻撃対象の決定
        for (int i = 0; i < selectedMedarotnum; i++)
        {
            SelectPlayers[i].PlayerStatus.TargetEnemy = UnityEngine.Random.Range(0, maxEnemy);
            SelectPlayers[i].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
        }

        //攻撃対象の決定
        for (int i = 0; i < maxEnemy; i++)
        {
            SelectEnemys[i].PlayerStatus.TargetEnemy = UnityEngine.Random.Range(0, selectedMedarotnum);
            SelectEnemys[i].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
            SelectEnemys[i].PlayerStatus.SelectPartnum = UnityEngine.Random.Range(0, 3);
        }
    }

    public bool HeadMethodFlag;
    public bool RightMethodFlag;
    public bool LeftMethodFlag;
    public GameObject CommandWindow;
    public GameObject CommandWindowChangeBtn;
    public bool targetcursorflag;
    GameObject CommandTargetCursor;

    //攻撃方法の選択
    public void AtMethodSelect( int Parts )
    {
        if (phase == Phase.Command)
        {
            //カーソルが存在すれば破壊
            if (targetcursorflag)
            {
                Destroy(CommandTargetCursor);
                targetcursorflag = false;
            }

            //頭パーツ
            if (Parts == 0)
            {
                if (HeadMethodFlag == true)
                {
                    if(selectMedarotnum == 0)
                    {
                        LeaderCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;

                    }
                    else if(selectMedarotnum == 1)
                    {
                        LeftCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if(selectMedarotnum == 2)
                    {
                        RightCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }

                    soundPlayer.PlayDecisionSound();
                    NextMedarotSelect();
                    HeadMethodFlag = false;
                    return;
                }
                CommandText.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;

                BattleMessage.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;


                //うつ　か　ねらいうち　だったら
                if (PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill == "うつ" || PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill == "ねらいうち")
                {
                    CommandText.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";
                    BattleMessage.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";

                    //カーソルがなかったら生成
                    if (!targetcursorflag)
                    {
                        //カーソルの生成
                        CommandTargetCursor = Instantiate(TargetCursorBluePre);
                        targetcursorflag = true;
                    }
                    
                    //敵の位置
                    float enemyx = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.x;
                    float enemyy = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.y;
                    float enemyz = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.z;

                    //位置の変更
                    CommandTargetCursor.transform.position = new Vector3(enemyx, CommandTargetCursor.transform.position.y, enemyz);

                    //頭狙い
                    if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 0)
                    {
                        CommandText.text += "頭パーツ狙い";
                        BattleMessage.text += "頭パーツ狙い";
                    }
                    //右腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 1)
                    {
                        CommandText.text += "右腕パーツ狙い";
                        BattleMessage.text += "右腕パーツ狙い";
                    }
                    //左腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 2)
                    {
                        CommandText.text += "左腕パーツ狙い";
                        BattleMessage.text += "左腕パーツ狙い";
                    }
                    else if(SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 3)
                    {
                        CommandText.text += "脚部パーツ狙い";
                        BattleMessage.text += "脚部パーツ狙い";
                    }

                }

                soundPlayer.PlaySelectionSound();
                HeadMethodFlag = true;
                RightMethodFlag = false;
                LeftMethodFlag = false;
            }
            //右腕パーツ
            else if (Parts == 1)
            {
                if (RightMethodFlag == true)
                {
                    if(selectMedarotnum == 0)
                    {
                        LeaderCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if(selectMedarotnum == 1)
                    {
                        LeftCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if(selectMedarotnum == 2)
                    {
                        RightCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    soundPlayer.PlayDecisionSound();
                    NextMedarotSelect();
                    RightMethodFlag = false;
                    return;
                }
                CommandText.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;

                BattleMessage.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;


                //うつ　か　ねらいうち　だったら
                if (PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[0].Skill == "うつ" || PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[0].Skill == "ねらいうち")
                {
                    CommandText.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";
                    BattleMessage.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";

                    //カーソルがなかったら生成
                    if (!targetcursorflag)
                    {
                        //カーソルの生成
                        CommandTargetCursor = Instantiate(TargetCursorBluePre);
                        targetcursorflag = true;
                    }

                    //敵の位置
                    float enemyx = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.x;
                    float enemyy = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.y;
                    float enemyz = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.z;

                    //位置の変更
                    CommandTargetCursor.transform.position = new Vector3(enemyx, CommandTargetCursor.transform.position.y, enemyz);

                    //頭狙い
                    if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 0)
                    {
                        CommandText.text += "頭パーツ狙い";
                        BattleMessage.text += "頭パーツ狙い";
                    }
                    //右腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 1)
                    {
                        CommandText.text += "右腕パーツ狙い";
                        BattleMessage.text += "右腕パーツ狙い";
                    }
                    //左腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 2)
                    {
                        CommandText.text += "左腕パーツ狙い";
                        BattleMessage.text += "左腕パーツ狙い";
                    }
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 3)
                    {
                        CommandText.text += "脚部パーツ狙い";
                        BattleMessage.text += "脚部パーツ狙い";
                    }

                }

                soundPlayer.PlaySelectionSound();
                HeadMethodFlag = false;
                RightMethodFlag = true;
                LeftMethodFlag = false;
            }
            //左腕パーツ
            else if (Parts == 2)
            {
                if (LeftMethodFlag == true)
                {
                    if(selectMedarotnum == 0)
                    {
                        LeaderCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if(selectMedarotnum == 1)
                    {
                        LeftCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if(selectMedarotnum == 2)
                    {
                        RightCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    soundPlayer.PlayDecisionSound();
                    NextMedarotSelect();
                    LeftMethodFlag = false;
                    return;
                }
                CommandText.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;

                BattleMessage.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;

                //うつ　か　ねらいうち　だったら
                if (PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill == "うつ" || PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill == "ねらいうち")
                {
                    CommandText.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";
                    BattleMessage.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";

                    //カーソルがなかったら生成
                    if (!targetcursorflag)
                    {
                        //カーソルの生成
                        CommandTargetCursor = Instantiate(TargetCursorBluePre);
                        targetcursorflag = true;
                    }

                    //敵の位置
                    float enemyx = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.x;
                    float enemyy = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.y;
                    float enemyz = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.z;

                    //位置の変更
                    CommandTargetCursor.transform.position = new Vector3(enemyx, CommandTargetCursor.transform.position.y, enemyz);

                    //頭狙い
                    if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 0)
                    {
                        CommandText.text += "頭パーツ狙い";
                        BattleMessage.text += "頭パーツ狙い";
                    }
                    //右腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 1)
                    {
                        CommandText.text += "右腕パーツ狙い";
                        BattleMessage.text += "右腕パーツ狙い";
                    }
                    //左腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 2)
                    {
                        CommandText.text += "左腕パーツ狙い";
                        BattleMessage.text += "左腕パーツ狙い";
                    }
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 3)
                    {
                        CommandText.text += "脚部パーツ狙い";
                        BattleMessage.text += "脚部パーツ狙い";
                    }

                }

                soundPlayer.PlaySelectionSound();
                HeadMethodFlag = false;
                RightMethodFlag = false;
                LeftMethodFlag = true;
            }
        }
    }

    public GameObject Command_sel1;
    public GameObject Command_sel2;
    public GameObject Command_sel3;

    public void NextMedarotSelect()
    {
        selectMedarotnum++;
        Command_sel1.SetActive(false);
        Command_sel2.SetActive(false);
        Command_sel3.SetActive(false);

        Leader_sel1.SetActive(false);
        Left_sel1.SetActive(false);
        Right_sel1.SetActive(false);
        CommandText.text = "こうどうをせんたくしてください";
        BattleMessage.text = "こうどうをせんたくしてください";

        //全員コマンド選択し終われば
        if (selectMedarotnum == selectedMedarotnum)
        {
            //フェーズ移動
            //phase = Phase.Move;

            //テキストをリセット
            CommandText.text = "";
            BattleMessage.text = "";

            CommandWindow.SetActive(false);
            CommandWindowChangeBtn.SetActive(false);
            BattleStart();

            return;
        }

        if(selectMedarotnum == 1)
        {
            //コマンド選択をしているキャラ
            Left_sel1.SetActive(true);
        }else if(selectMedarotnum == 2)
        {
            Right_sel1.SetActive(true);
        }


        //次のメダロットのステータスを表示する
        SelHeadCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].PartName;
        SelRightCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[1].PartName;
        SelLeftCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[2].PartName;

    }

    public Text BattleStartText;

    public void BattleStart()
    {
        BattleStartText.gameObject.SetActive(true);

        StartCoroutine(DelayMethod(1.0f, () => {
            BattleStartText.gameObject.SetActive(false);
        }));

        StartCoroutine(DelayMethod(1.5f, () => {
            phase = Phase.Move;
        }));
    }
    
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    private IEnumerator Delay1frameMethod(Action action)
    {
        yield return null;
        action();
    }

    //メダロット選択画面
    public GameObject MedarotSelectobj;
    public GameObject Camera;
    public int selectMedarotnum;

    public Text MedarotName;
    public Text MedarotPartsName;
    public Text MedarotPartsArmor;
    public Text MedarotPartsMethod;
    public Text MedarotPartsSkill;
    public Text SelectMedarotName;

    public void MedarotSelect()
    {
        //前のオブジェクトの位置を変える
        ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(100 + selectMedarotnum * 10,
                                                            MedarotSelectobj.transform.position.y + 0.5f,
                                                            MedarotSelectobj.transform.position.z);

        selectMedarotnum = 0;
        //親子関係を変更 回転 位置を移動 
        ExistPlayers[selectMedarotnum].PlayerGameObject.transform.parent   = MedarotSelectobj.transform;
        ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(Camera.transform.position.x, 0, 0));
        ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(MedarotSelectobj.transform.position.x + 5,
                                                                        MedarotSelectobj.transform.position.y + 0.5f,
                                                                        MedarotSelectobj.transform.position.z);
        //選択したメダロットはリセット
        SelectMedarotName.text = "";

        //キャラの名前
        MedarotName.text = ExistPlayers[selectMedarotnum].PlayerStatus.Name;
        //キャラのパーツ名
        MedarotPartsName.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].PartName + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].PartName + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].PartName + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].PartName + "\n";
        //キャラのパーツ装甲
        MedarotPartsArmor.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Armor + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Armor + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Armor + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Armor + "\n";
        //キャラの攻撃方法
        MedarotPartsMethod.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Type + "\n";
        //キャラのスキル
        MedarotPartsSkill.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Skill + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill + "\n"
                              + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Skill + "\n";


    }
    //次のメダロットを表示
    public void MedarotSelectNext()
    {
        if(0 <= selectMedarotnum && selectMedarotnum < maxPlayer - 1)
        {
            
            //前のオブジェクトの位置を変える
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(100 + selectMedarotnum * 10,
                                                                MedarotSelectobj.transform.position.y + 0.5f,
                                                                MedarotSelectobj.transform.position.z);
            //プラス1する
            selectMedarotnum++;
            //親子関係を変更 回転 位置を移動
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.parent = MedarotSelectobj.transform;
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(Camera.transform.position.x, 0, 0));
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(MedarotSelectobj.transform.position.x + 5,
                                                                            MedarotSelectobj.transform.position.y + 0.5f,
                                                                            MedarotSelectobj.transform.position.z);

            //キャラの名前
            MedarotName.text = ExistPlayers[selectMedarotnum].PlayerStatus.Name;
            //キャラのパーツ名
            MedarotPartsName.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].PartName + "\n"
                                  + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].PartName + "\n"
                                  + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].PartName + "\n"
                                  + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].PartName + "\n";
            //キャラのパーツ装甲
            MedarotPartsArmor.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Armor + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Armor + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Armor + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Armor + "\n";
            //キャラの攻撃方法
            MedarotPartsMethod.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod + "\n"
                                    + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod + "\n"
                                    + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod + "\n"
                                    + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Type + "\n";
            //キャラのスキル
            MedarotPartsSkill.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Skill + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Skill + "\n";
        }
    }

    //前のメダロットを表示
    public void MedarotSelectBack()
    {
        if (0 < selectMedarotnum && selectMedarotnum <= maxPlayer - 1)
        {
            
            //前のオブジェクトの位置を変える
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(100 + selectMedarotnum * 10,
                                                                MedarotSelectobj.transform.position.y + 0.5f,
                                                                MedarotSelectobj.transform.position.z);
            //マイナス1する
            selectMedarotnum--;
            //親子関係を変更 回転 位置を移動
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.parent = MedarotSelectobj.transform;
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(Camera.transform.position.x, 0, 0));
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(MedarotSelectobj.transform.position.x + 5,
                                                                            MedarotSelectobj.transform.position.y + 0.5f,
                                                                            MedarotSelectobj.transform.position.z);

            //キャラの名前
            MedarotName.text = ExistPlayers[selectMedarotnum].PlayerStatus.Name;
            //キャラのパーツ名
            MedarotPartsName.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].PartName + "\n"
                                  + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].PartName + "\n"
                                  + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].PartName + "\n"
                                  + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].PartName + "\n";
            //キャラのパーツ装甲
            MedarotPartsArmor.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Armor + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Armor + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Armor + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Armor + "\n";
            //キャラの攻撃方法
            MedarotPartsMethod.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod + "\n"
                                    + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod + "\n"
                                    + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod + "\n"
                                    + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Type + "\n";
            //キャラのスキル
            MedarotPartsSkill.text = PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Skill + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill + "\n"
                                   + PartsList.CharaList[ExistPlayers[selectMedarotnum].PlayerStatus.Leg.PartsID].PartsList[3].Skill + "\n";
        }
    }
    //メダロットの選択
    public int selectedMedarotnum = 0;
    public string[] SelectMedarotNameText = new string[3];
    public bool isselecting = false;
    public GameObject BattleReadyWindow;
    public void MedarotSelectEnter()
    {
        //選べるのは3体まで
        if(selectedMedarotnum < maxPlayer)
        {
            //選ばれていなければ
            if(ExistPlayers[selectMedarotnum].PlayerStatus.SelectFrag == false)
            {
                isselecting = true;
                //SelectPlayerに格納
                ExistPlayers[selectMedarotnum].PlayerStatus.SelectFrag = true;
                SelectPlayers[selectedMedarotnum] = ExistPlayers[selectMedarotnum];
                SelectPlayers[selectedMedarotnum].PlayerStatus.Playernum = selectMedarotnum;
                //選択したメダロットの名前を表示
                if(selectedMedarotnum == 0)
                {
                    SelectMedarotName.text += "L " + ExistPlayers[selectMedarotnum].PlayerStatus.Name + "\n";
                }else
                {
                    SelectMedarotName.text += "S" + selectedMedarotnum + " " + ExistPlayers[selectMedarotnum].PlayerStatus.Name + "\n";
                }
                SelectMedarotNameText[selectedMedarotnum] = SelectMedarotName.text;
                selectedMedarotnum++;
                MedarotSelectNext();
            }
        }
        if(selectedMedarotnum == 3)
        {
            BattleReadyWindow.SetActive(true);
        }
    }

    public void BattleReadyWindowSetActivetrue()
    {
        if(selectedMedarotnum != 0)
        {
            BattleReadyWindow.SetActive(true);
        }
    }

    public void MedarotSelectReturn()
    {
        //選択を取り消す
        ExistPlayers[SelectPlayers[selectedMedarotnum - 1].PlayerStatus.Playernum].PlayerStatus.SelectFrag = false;
        selectedMedarotnum--;
        MedarotSelectBack();
        if (selectedMedarotnum != 0)
        {
            SelectMedarotName.text = SelectMedarotNameText[selectedMedarotnum - 1];
        }
        //選択中のフラグ
        else if (selectedMedarotnum == 0)
        {
            SelectMedarotName.text = "";
            isselecting = false;
        }
    }

    //フェーズがMoveの時
    public void PlayerMoving()
    {
        //自機を移動させる
        for (int i = 0; i < selectedMedarotnum; i++)
        {
            //プレイヤーが冷却中であれば後退
            if (SelectPlayers[i].PlayerStatus.Cooling)
            {
                //脚部パーツの移動性能をとってくる
                int Moving = PartsList.CharaList[SelectPlayers[i].PlayerStatus.Leg.PartsID].PartsList[3].Moving;

                //選択したパーツの冷却性能をとってくる
                int Cooling = 0;
                //選択したパーツナンバーを拾ってくる
                int SPart = SelectPlayers[i].PlayerStatus.SelectPartnum;
                //頭パーツ
                if (SPart == 0)
                {
                    Cooling = PartsList.CharaList[SelectPlayers[i].PlayerStatus.Head.PartsID].PartsList[0].Cooling;
                }
                //右腕パーツ
                else if(SPart == 1)
                {
                    Cooling = PartsList.CharaList[SelectPlayers[i].PlayerStatus.RightArm.PartsID].PartsList[1].Cooling;
                }
                //左腕パーツ
                else if(SPart == 2)
                {
                    Cooling = PartsList.CharaList[SelectPlayers[i].PlayerStatus.RightArm.PartsID].PartsList[2].Cooling;
                }

                //移動量の倍率
                int MoviengBias = Moving + Cooling + PlayerMovingSpeed;

                //移動量を決定
                PlayerMovement[i] = Movement * MoviengBias * -1;

                //速度ベクトルに適応
                SelectPlayers[i].PlayerGameObject.GetComponent<Rigidbody>().velocity = PlayerMovement[i];
            }
            //プレイヤーは冷却中でなければ前進　充填
            else
            {
                //脚部パーツの移動性能をとってくる
                int Moving = PartsList.CharaList[SelectPlayers[i].PlayerStatus.Leg.PartsID].PartsList[3].Moving;

                //選択したパーツの充填性能をとってくる
                int Loading = 0;
                //選択したパーツナンバーを拾ってくる
                int SPart = SelectPlayers[i].PlayerStatus.SelectPartnum;
                //頭パーツ
                if (SPart == 0)
                {
                    Loading = PartsList.CharaList[SelectPlayers[i].PlayerStatus.Head.PartsID].PartsList[0].Loading;
                }
                //右腕パーツ
                else if (SPart == 1)
                {
                    Loading = PartsList.CharaList[SelectPlayers[i].PlayerStatus.RightArm.PartsID].PartsList[1].Loading;
                }
                //左腕パーツ
                else if (SPart == 2)
                {
                    Loading = PartsList.CharaList[SelectPlayers[i].PlayerStatus.RightArm.PartsID].PartsList[2].Loading;
                }

                //移動量の倍率
                int MoviengBias = Moving + Loading + PlayerMovingSpeed;

                //移動量を決定
                PlayerMovement[i] = Movement * MoviengBias;

                //速度ベクトルに適応
                SelectPlayers[i].PlayerGameObject.GetComponent<Rigidbody>().velocity = PlayerMovement[i];

            }
        }

        //敵を移動させる
        for (int i = 0; i < SelectEnemys.Length; i++)
        {
            //エネミーが冷却中であれば後退
            if (SelectEnemys[i].PlayerStatus.Cooling)
            {
                //脚部パーツの移動性能をとってくる
                int Moving = PartsList.CharaList[SelectEnemys[i].PlayerStatus.Leg.PartsID].PartsList[3].Moving;

                //選択したパーツの冷却性能をとってくる
                int Cooling = 0;
                //選択したパーツナンバーを拾ってくる
                int SPart = SelectEnemys[i].PlayerStatus.SelectPartnum;
                //頭パーツ
                if (SPart == 0)
                {
                    Cooling = PartsList.CharaList[SelectEnemys[i].PlayerStatus.Head.PartsID].PartsList[0].Cooling;
                }
                //右腕パーツ
                else if (SPart == 1)
                {
                    Cooling = PartsList.CharaList[SelectEnemys[i].PlayerStatus.RightArm.PartsID].PartsList[1].Cooling;
                }
                //左腕パーツ
                else if (SPart == 2)
                {
                    Cooling = PartsList.CharaList[SelectEnemys[i].PlayerStatus.RightArm.PartsID].PartsList[2].Cooling;
                }

                //移動量の倍率
                int MoviengBias = Moving + Cooling + EnemyMovingSpeed;

                //移動量を決定
                EnemyMovement[i] = Movement * MoviengBias * -1 * -1;

                //速度ベクトルに適応
                SelectEnemys[i].PlayerGameObject.GetComponent<Rigidbody>().velocity = EnemyMovement[i];
            }
            //エネミーは冷却中でなければ前進　充填
            else
            {
                //脚部パーツの移動性能をとってくる
                int Moving = PartsList.CharaList[SelectEnemys[i].PlayerStatus.Leg.PartsID].PartsList[3].Moving;

                //選択したパーツの充填性能をとってくる
                int Loading = 0;
                //選択したパーツナンバーを拾ってくる
                int SPart = SelectEnemys[i].PlayerStatus.SelectPartnum;
                //頭パーツ
                if (SPart == 0)
                {
                    Loading = PartsList.CharaList[SelectEnemys[i].PlayerStatus.Head.PartsID].PartsList[0].Loading;
                }
                //右腕パーツ
                else if (SPart == 1)
                {
                    Loading = PartsList.CharaList[SelectEnemys[i].PlayerStatus.RightArm.PartsID].PartsList[1].Loading;
                }
                //左腕パーツ
                else if (SPart == 2)
                {
                    Loading = PartsList.CharaList[SelectEnemys[i].PlayerStatus.RightArm.PartsID].PartsList[2].Loading;
                }

                //移動量の倍率
                int MoviengBias = Moving + Loading + EnemyMovingSpeed;

                //移動量を決定
                EnemyMovement[i] = Movement * MoviengBias * -1;

                //速度ベクトルに適応
                SelectEnemys[i].PlayerGameObject.GetComponent<Rigidbody>().velocity = EnemyMovement[i];
            }
        }
    }

    public void PhaseAttackfunc( int num )
    {
        //0,1,2ならPlayer
        //3,4,5ならEnemy

        //自機の攻撃
        if (0 <= num && num <= 2)
        {
            //選択したパーツナンバーを拾ってくる
            int SPart = SelectPlayers[num].PlayerStatus.SelectPartnum;

            //自分選択したパーツのHPが0ではないことを確認する
            //選択したパーツが頭パーツ
            if (SPart == 0)
            {
                //ゼロなら
                if(SelectPlayers[num].PlayerStatus.Head.HP == 0)
                {
                    //失敗
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    StartCoroutine(AttacknonPart());

                    //phase = Phase.Move;
                    return;
                }
            }
            //選択したパーツが右腕パーツ
            else if (SPart == 1)
            {
                //ゼロなら
                if (SelectPlayers[num].PlayerStatus.RightArm.HP == 0)
                {
                    //失敗
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    StartCoroutine(AttacknonPart());

                    //phase = Phase.Move;
                    return;
                }
            }
            //選択したパーツが左腕パーツ
            else if(SPart == 2)
            {
                //ゼロなら
                if (SelectPlayers[num].PlayerStatus.LeftArm.HP == 0)
                {
                    //失敗
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    StartCoroutine(AttacknonPart());

                    //phase = Phase.Move;
                    return;
                }
            }

            //攻撃スキルで分岐
            //選択したパーツが頭パーツ
            if (SPart == 0)
            {
                //スキル　うつ　ねらいうち
                if((PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "うつ") || (PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "ねらいうち"))
                {
                    //ターゲットが存在するか確認する
                    //存在しなければ
                    if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                    {
                        //失敗
                        //冷却に入る
                        SelectPlayers[num].PlayerStatus.Cooling = true;
                        StartCoroutine(AttacknonTarget());

                        //phase = Phase.Move;
                        return;
                    }

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                //SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                                //失敗
                                StartCoroutine(AttacknonTarget());
                                SelectPlayers[num].PlayerStatus.Cooling = true;
                                return;
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の右腕パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Power;
                    //脚部の射撃の得意さ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Leg.PartsID].PartsList[3].Shooting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }

                    /////////////
                    //AttackPlay(num, SelectPlayers[num].PlayerStatus.TargetEnemy, SelectPlayers[num].PlayerStatus.TargetParts, Damage);
                    StartCoroutine(AttackPlayCol(num, SelectPlayers[num].PlayerStatus.TargetEnemy, SelectPlayers[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].AtMethod));


                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　たすける
                else if(PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "たすける")
                {
                    //未実装

                    StartCoroutine(AttackPlayColTasukeru(num,PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].AtMethod)); 

                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　まもる
                else if(PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "まもる")
                {
                    //未実装

                    StartCoroutine(AttackPlayColMamoru(num, PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].AtMethod));
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　なぐる　がむしゃら
                else if ((PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "なぐる") || (PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "がむしゃら"))
                {
                    //ターゲットを決定する
                    //ターゲットは一番近い敵
                    int TargetEnemy = 0;
                    float Enemyz = 100;
                    for (int i = 0; i < maxEnemy; i++)
                    {
                        //生き残ってる敵だけ
                        if (SelectEnemys[i].PlayerGameObject.activeInHierarchy == true)
                        {
                            //もし近ければ
                            if (Enemyz > Mathf.Abs(SelectEnemys[i].PlayerGameObject.transform.position.z))
                            {
                                //代入
                                Enemyz = Mathf.Abs(SelectEnemys[i].PlayerGameObject.transform.position.z);

                                //敵の番号を覚えておく
                                TargetEnemy = i;
                            }
                        }
                    }

                    SelectPlayers[num].PlayerStatus.TargetEnemy = TargetEnemy;

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の右腕パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Power;
                    //脚部の格闘の得意さ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Leg.PartsID].PartsList[3].Fighting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(AttackPlayCol(num, SelectPlayers[num].PlayerStatus.TargetEnemy, SelectPlayers[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));


                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;

                }
            }
            //選択したパーツが右腕パーツ
            else if (SPart == 1)
            {
                //スキル　うつ　ねらいうち
                if ((PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "うつ") || (PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "ねらいうち"))
                {
                    //ターゲットが存在するか確認する
                    //存在しなければ
                    if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                    {
                        //失敗
                        //冷却に入る
                        SelectPlayers[num].PlayerStatus.Cooling = true;

                        StartCoroutine(AttacknonTarget());
                        //phase = Phase.Move;
                        return;
                    }

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                //SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                                StartCoroutine(AttacknonTarget());
                                SelectPlayers[num].PlayerStatus.Cooling = true;
                                return;

                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Power;
                    //脚部の射撃の得意さ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Leg.PartsID].PartsList[3].Shooting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }

                    StartCoroutine(AttackPlayCol(num, SelectPlayers[num].PlayerStatus.TargetEnemy, SelectPlayers[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));

                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　たすける
                else if (PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "たすける")
                {
                    //未実装
                    
                    StartCoroutine(AttackPlayColTasukeru(num,PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　まもる
                else if (PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "まもる")
                {
                    //未実装
                    StartCoroutine(AttackPlayColMamoru(num, PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　なぐる　がむしゃら
                else if((PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "なぐる") || (PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "がむしゃら"))
                {
                    //ターゲットを決定する
                    //ターゲットは一番近い敵
                    int TargetEnemy = 0;
                    float Enemyz = 100;
                    for (int i = 0; i < maxEnemy; i++)
                    {
                        //生き残ってる敵だけ
                        if(SelectEnemys[i].PlayerGameObject.activeInHierarchy == true)
                        {
                            //もし近ければ
                            if(Enemyz > Mathf.Abs(SelectEnemys[i].PlayerGameObject.transform.position.z))
                            {
                                //代入
                                Enemyz = Mathf.Abs(SelectEnemys[i].PlayerGameObject.transform.position.z);

                                //敵の番号を覚えておく
                                TargetEnemy = i;
                            }
                        }
                    }

                    SelectPlayers[num].PlayerStatus.TargetEnemy = TargetEnemy;

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Power;
                    //脚部の格闘の得意さ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Leg.PartsID].PartsList[3].Fighting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(AttackPlayCol(num, SelectPlayers[num].PlayerStatus.TargetEnemy, SelectPlayers[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));


                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;

                }
            }

            //選択したパーツが左腕パーツ
            else if (SPart == 2)
            {
                //スキル　うつ　ねらいうち
                if ((PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "うつ") || (PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "ねらいうち"))
                {
                    //ターゲットが存在するか確認する
                    //存在しなければ
                    if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                    {
                        //失敗
                        //冷却に入る
                        SelectPlayers[num].PlayerStatus.Cooling = true;

                        StartCoroutine(AttacknonTarget());
                        //phase = Phase.Move;
                        return;
                    }

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                //SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                                SelectPlayers[num].PlayerStatus.Cooling = true;

                                StartCoroutine(AttacknonTarget());
                                return;
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Power;
                    //脚部の射撃の得意さ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Leg.PartsID].PartsList[3].Shooting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(AttackPlayCol(num, SelectPlayers[num].PlayerStatus.TargetEnemy, SelectPlayers[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod));


                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　たすける
                else if (PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "たすける")
                {
                    //未実装
                    
                    StartCoroutine(AttackPlayColTasukeru(num, PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod));
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　まもる
                else if (PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "まもる")
                {
                    //未実装
                    StartCoroutine(AttackPlayColMamoru(num, PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod));
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　なぐる　がむしゃら
                else if ((PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "なぐる") || (PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "がむしゃら"))
                {
                    //ターゲットを決定する
                    //ターゲットは一番近い敵
                    int TargetEnemy = 0;
                    float Enemyz = 100;
                    for (int i = 0; i < maxEnemy; i++)
                    {
                        //生き残ってる敵だけ
                        if (SelectEnemys[i].PlayerGameObject.activeInHierarchy == true)
                        {
                            //もし近ければ
                            if (Enemyz > Mathf.Abs(SelectEnemys[i].PlayerGameObject.transform.position.z))
                            {
                                //代入
                                Enemyz = Mathf.Abs(SelectEnemys[i].PlayerGameObject.transform.position.z);

                                //敵の番号を覚えておく
                                TargetEnemy = i;
                            }
                        }
                    }

                    SelectPlayers[num].PlayerStatus.TargetEnemy = TargetEnemy;

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Power;
                    //脚部の格闘の得意さ
                    Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Leg.PartsID].PartsList[3].Fighting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectPlayers[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectPlayers[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(AttackPlayCol(num, SelectPlayers[num].PlayerStatus.TargetEnemy, SelectPlayers[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod));

                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;

                }
            }

        }
        //敵の攻撃だったら
        else if (3 <= num && num <= 5)
        {
            //添字の調整
            num = num -3;

            //選択したパーツナンバーを拾ってくる
            int SPart = SelectEnemys[num].PlayerStatus.SelectPartnum;

            //自分選択したパーツのHPが0ではないことを確認する
            //選択したパーツが頭パーツ
            if (SPart == 0)
            {
                //ゼロなら
                if (SelectEnemys[num].PlayerStatus.Head.HP == 0)
                {
                    //失敗
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    StartCoroutine(AttacknonPart());

                    //phase = Phase.Move;
                    return;
                }
            }
            //選択したパーツが右腕パーツ
            else if (SPart == 1)
            {
                //ゼロなら
                if (SelectEnemys[num].PlayerStatus.RightArm.HP == 0)
                {
                    //失敗
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    StartCoroutine(AttacknonPart());

                    //phase = Phase.Move;
                    return;
                }
            }
            //選択したパーツが左腕パーツ
            else if (SPart == 2)
            {
                //ゼロなら
                if (SelectEnemys[num].PlayerStatus.LeftArm.HP == 0)
                {
                    //失敗
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    StartCoroutine(AttacknonPart());

                    //phase = Phase.Move;
                    return;
                }
            }

            //攻撃スキルで分岐
            //選択したパーツが頭パーツ
            if (SPart == 0)
            {
                //スキル　うつ　ねらいうち
                if ((PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "うつ") || (PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "ねらいうち"))
                {
                    //ターゲットが存在するか確認する
                    //存在しなければ
                    if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                    {
                        //失敗
                        //冷却に入る
                        SelectEnemys[num].PlayerStatus.Cooling = true;

                        StartCoroutine(AttacknonTarget());
                        //phase = Phase.Move;
                        return;
                    }

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                //SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);

                                StartCoroutine(AttacknonTarget());
                                SelectEnemys[num].PlayerStatus.Cooling = true;
                                return;
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].Power;
                    //脚部の射撃の得意さ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.Leg.PartsID].PartsList[3].Shooting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(DefencePlayCol(num , SelectEnemys[num].PlayerStatus.TargetEnemy, SelectEnemys[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].AtMethod));


                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　たすける
                else if (PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "たすける")
                {
                    //未実装
                    StartCoroutine(DefencePlayColTasukeru(num, PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].AtMethod));
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　まもる
                else if (PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "まもる")
                {
                    //未実装
                    StartCoroutine(DefencePlayColMamoru(num, PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].AtMethod));
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　なぐる　がむしゃら
                else if ((PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "なぐる") || (PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Skill == "がむしゃら"))
                {
                    //ターゲットを決定する
                    //ターゲットは一番近い敵
                    int TargetEnemy = 0;
                    float Enemyz = 100;
                    for (int i = 0; i < selectedMedarotnum; i++)
                    {
                        //生き残ってる敵だけ
                        if (SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
                        {
                            //もし近ければ
                            if (Enemyz > Mathf.Abs(SelectPlayers[i].PlayerGameObject.transform.position.z))
                            {
                                //代入
                                Enemyz = Mathf.Abs(SelectPlayers[i].PlayerGameObject.transform.position.z);

                                //敵の番号を覚えておく
                                TargetEnemy = i;
                            }
                        }
                    }

                    SelectEnemys[num].PlayerStatus.TargetEnemy = TargetEnemy;

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].Power;
                    //脚部の格闘の得意さ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.Leg.PartsID].PartsList[3].Fighting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(DefencePlayCol(num, SelectEnemys[num].PlayerStatus.TargetEnemy, SelectEnemys[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));


                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;

                }
            }
            //選択したパーツが右腕パーツ
            else if (SPart == 1)
            {
                //スキル　うつ　ねらいうち
                if ((PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "うつ") || (PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "ねらいうち"))
                {
                    //ターゲットが存在するか確認する
                    //存在しなければ
                    if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                    {
                        //失敗
                        //冷却に入る
                        SelectEnemys[num].PlayerStatus.Cooling = true;

                        StartCoroutine(AttacknonTarget());
                        //phase = Phase.Move;
                        return;
                    }

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                //SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);

                                StartCoroutine(AttacknonTarget());
                                SelectEnemys[num].PlayerStatus.Cooling = true;
                                return;
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].Power;
                    //脚部の射撃の得意さ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.Leg.PartsID].PartsList[3].Shooting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(DefencePlayCol(num, SelectEnemys[num].PlayerStatus.TargetEnemy, SelectEnemys[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));


                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　たすける
                else if (PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "たすける")
                {
                    //未実装
                    StartCoroutine(DefencePlayColTasukeru(num, PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　まもる
                else if (PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "まもる")
                {
                    //未実装
                    StartCoroutine(DefencePlayColMamoru(num, PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　なぐる　がむしゃら
                else if ((PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "なぐる") || (PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill == "がむしゃら"))
                {
                    //ターゲットを決定する
                    //ターゲットは一番近い敵
                    int TargetEnemy = 0;
                    float Enemyz = 100;
                    for (int i = 0; i < selectedMedarotnum; i++)
                    {
                        //生き残ってる敵だけ
                        if (SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
                        {
                            //もし近ければ
                            if (Enemyz > Mathf.Abs(SelectPlayers[i].PlayerGameObject.transform.position.z))
                            {
                                //代入
                                Enemyz = Mathf.Abs(SelectPlayers[i].PlayerGameObject.transform.position.z);

                                //敵の番号を覚えておく
                                TargetEnemy = i;
                            }
                        }
                    }

                    SelectEnemys[num].PlayerStatus.TargetEnemy = TargetEnemy;

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].Power;
                    //脚部の格闘の得意さ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.Leg.PartsID].PartsList[3].Fighting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(DefencePlayCol(num, SelectEnemys[num].PlayerStatus.TargetEnemy, SelectEnemys[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod));


                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;

                }
            }

            //選択したパーツが左腕パーツ
            else if (SPart == 2)
            {
                //スキル　うつ　ねらいうち
                if ((PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "うつ") || (PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "ねらいうち"))
                {
                    //ターゲットが存在するか確認する
                    //存在しなければ
                    if (SelectEnemys[SelectPlayers[num].PlayerStatus.TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                    {
                        //失敗
                        //冷却に入る
                        SelectEnemys[num].PlayerStatus.Cooling = true;

                        StartCoroutine(AttacknonTarget());
                        //phase = Phase.Move;
                        return;
                    }

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                //SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                                SelectEnemys[num].PlayerStatus.Cooling = true;

                                StartCoroutine(AttacknonTarget());
                                return;

                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Power;
                    //脚部の射撃の得意さ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.Leg.PartsID].PartsList[3].Shooting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(DefencePlayCol(num, SelectEnemys[num].PlayerStatus.TargetEnemy, SelectEnemys[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod));

                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　たすける
                else if (PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "たすける")
                {
                    //未実装
                    StartCoroutine(DefencePlayColTasukeru(num, PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod));
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　まもる
                else if (PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "まもる")
                {
                    //未実装
                    StartCoroutine(DefencePlayColMamoru(num, PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod));
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;
                }
                //スキル　なぐる　がむしゃら
                else if ((PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "なぐる") || (PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill == "がむしゃら"))
                {
                    //ターゲットを決定する
                    //ターゲットは一番近い敵
                    int TargetEnemy = 0;
                    float Enemyz = 100;
                    for (int i = 0; i < selectedMedarotnum; i++)
                    {
                        //生き残ってる敵だけ
                        if (SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
                        {
                            //もし近ければ
                            if (Enemyz > Mathf.Abs(SelectPlayers[i].PlayerGameObject.transform.position.z))
                            {
                                //代入
                                Enemyz = Mathf.Abs(SelectPlayers[i].PlayerGameObject.transform.position.z);

                                //敵の番号を覚えておく
                                TargetEnemy = i;
                            }
                        }
                    }

                    SelectEnemys[num].PlayerStatus.TargetEnemy = TargetEnemy;

                    //狙ったパーツは既に破壊されているか
                    //破壊されている場合別のパーツを選択し直す
                    while (true)
                    {
                        //ターゲットのパーツは
                        //相手の頭パーツ
                        if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の頭パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の左腕パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                        //相手の脚部パーツ
                        else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                        {
                            //壊れている時
                            if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable == false)
                            {
                                //ランダムで選び直す
                                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                            }
                            //壊れていなければ
                            else
                            {
                                break;
                            }
                        }
                    }

                    //ダメージ量を計算する
                    int Damage = 0;
                    //基本ダメージ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Power;
                    //脚部の格闘の得意さ
                    Damage += PartsList.CharaList[SelectEnemys[num].PlayerStatus.Leg.PartsID].PartsList[3].Fighting;

                    //HPをへらす
                    //相手の頭パーツに攻撃
                    if (SelectEnemys[num].PlayerStatus.TargetParts == 0)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Head.PartsAvailable = false;
                            //頭パーツが使えなくなれば
                            //機能停止にする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerGameObject.SetActive(false);
                        }
                    }
                    //相手の右腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 1)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.RightArm.PartsAvailable = false;
                        }
                    }
                    //相手の左腕パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 2)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.LeftArm.PartsAvailable = false;
                        }
                    }
                    //相手の脚部パーツに攻撃
                    else if (SelectEnemys[num].PlayerStatus.TargetParts == 3)
                    {
                        SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP -= Damage;

                        //ゼロになる　もしくは　ゼロよりも小さくなったら
                        if (SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP <= 0)
                        {
                            //ゼロにする
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.HP = 0;
                            //パーツは使えない
                            SelectPlayers[SelectEnemys[num].PlayerStatus.TargetEnemy].PlayerStatus.Leg.PartsAvailable = false;
                        }
                    }
                    StartCoroutine(DefencePlayCol(num, SelectEnemys[num].PlayerStatus.TargetEnemy, SelectEnemys[num].PlayerStatus.TargetParts, Damage, PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod));


                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //phase = Phase.Move;

                }
            }

        }

    }

    public GameObject TargetCursorBluePre;
    public GameObject TargetCursorPinkPre;
    public Text BattleMessage;

    IEnumerator AttackPlayCol( int playernum, int targetmeda, int targetpart, int damage, string atmethod)
    {
        //ターゲットを表示
        //傾きはx67.915
        float playerx = SelectPlayers[playernum].PlayerGameObject.transform.position.x;
        float playery = SelectPlayers[playernum].PlayerGameObject.transform.position.y;
        float playerz = SelectPlayers[playernum].PlayerGameObject.transform.position.z;

        //カーソルの生成
        GameObject TargetCursor = Instantiate(TargetCursorBluePre);
        //位置の変更
        TargetCursor.transform.position = new Vector3(playerx, TargetCursor.transform.position.y, playerz);

        //敵の位置
        float enemyx = SelectEnemys[targetmeda].PlayerGameObject.transform.position.x;
        float enemyy = SelectEnemys[targetmeda].PlayerGameObject.transform.position.y;
        float enemyz = SelectEnemys[targetmeda].PlayerGameObject.transform.position.z;

        //Vector3 TargetEnemypoti = new Vector3(enemyx + offsetx, TargetCursor.transform.position.y, enemyz + offsetz);
        Vector3 TargetEnemypoti = new Vector3(enemyx, TargetCursor.transform.position.y, enemyz);

        //ターゲットパーツで分岐
        if(targetpart == 0)
        {
            BattleMessage.text = "頭パーツに";
        }else if (targetpart == 1)
        {
            BattleMessage.text = "右腕パーツに";
        }else if (targetpart == 2)
        {
            BattleMessage.text = "左腕パーツに";
        }else if(targetpart == 3)
        {
            BattleMessage.text = "脚部パーツに";
        }

        BattleMessage.text += atmethod + "でこうげき！";

        yield return new WaitForSeconds(1.0f);

        //警告音
        soundPlayer.PlayCursorWarningSound();

        //同じ位置に移動するまでループ
        //120回ループさせる
        //while (!(TargetCursor.transform.position == TargetEnemypoti))
        for(int i=0; i < 120; i++)
        {
            TargetCursor.transform.position = Vector3.Lerp(TargetCursor.transform.position, TargetEnemypoti, 5.0f * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        BattleMessage.text = "";
        //ターゲットパーツで分岐
        if (targetpart == 0)
        {
            BattleMessage.text = "頭パーツに";
        }
        else if (targetpart == 1)
        {
            BattleMessage.text = "右腕パーツに";
        }
        else if (targetpart == 2)
        {
            BattleMessage.text = "左腕パーツに";
        }
        else if (targetpart == 3)
        {
            BattleMessage.text = "脚部パーツに";
        }

        BattleMessage.text += damage + "ダメージ！";

        //ダメージヒット音
        soundPlayer.PlayAttackSound();

        //HPバーの演出
        for (int i = 0; i < damage; i++)
        {
            hpbar.ChangeValue(targetmeda + 3, targetpart, 1);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        BattleMessage.text = "";

        Destroy(TargetCursor);
        //全てが終われば移動に戻る
        phase = Phase.Move;

    }

    IEnumerator DefencePlayCol(int playernum, int targetmeda, int targetpart, int damage, string atmethod)
    {
        //ターゲットを表示
        //傾きはx67.915
        float enemyx = SelectEnemys[playernum].PlayerGameObject.transform.position.x;
        float enemyy = SelectEnemys[playernum].PlayerGameObject.transform.position.y;
        float enemyz = SelectEnemys[playernum].PlayerGameObject.transform.position.z;

        //カーソルの生成
        GameObject TargetCursor = Instantiate(TargetCursorPinkPre);
        //位置の変更
        TargetCursor.transform.position = new Vector3(enemyx, TargetCursor.transform.position.y, enemyz);

        //敵の位置
        float playerx = SelectPlayers[targetmeda].PlayerGameObject.transform.position.x;
        float playery = SelectPlayers[targetmeda].PlayerGameObject.transform.position.y;
        float playerz = SelectPlayers[targetmeda].PlayerGameObject.transform.position.z;

        //Vector3 TargetEnemypoti = new Vector3(enemyx + offsetx, TargetCursor.transform.position.y, enemyz + offsetz);
        Vector3 TargetPlayerpoti = new Vector3(playerx, TargetCursor.transform.position.y, playerz);

        //ターゲットパーツで分岐
        if (targetpart == 0)
        {
            BattleMessage.text = "頭パーツに";
        }
        else if (targetpart == 1)
        {
            BattleMessage.text = "右腕パーツに";
        }
        else if (targetpart == 2)
        {
            BattleMessage.text = "左腕パーツに";
        }
        else if (targetpart == 3)
        {
            BattleMessage.text = "脚部パーツに";
        }

        BattleMessage.text += atmethod + "でこうげき！";

        yield return new WaitForSeconds(1.0f);
        //警告音
        soundPlayer.PlayCursorWarningSound();
        //同じ位置に移動するまでループ
        //120回ループさせる
        //while (!(TargetCursor.transform.position == TargetEnemypoti))
        for (int i = 0; i < 120; i++)
        {
            TargetCursor.transform.position = Vector3.Lerp(TargetCursor.transform.position, TargetPlayerpoti, 5.0f * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        BattleMessage.text = "";
        //ターゲットパーツで分岐
        if (targetpart == 0)
        {
            BattleMessage.text = "頭パーツに";
        }
        else if (targetpart == 1)
        {
            BattleMessage.text = "右腕パーツに";
        }
        else if (targetpart == 2)
        {
            BattleMessage.text = "左腕パーツに";
        }
        else if (targetpart == 3)
        {
            BattleMessage.text = "脚部パーツに";
        }

        BattleMessage.text += damage + "ダメージ！";

        //ダメージヒット音
        soundPlayer.PlayAttackSound();

        //HPバーの演出
        for (int i = 0; i < damage; i++)
        {
            hpbar.ChangeValue(targetmeda, targetpart, 1);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        BattleMessage.text = "";

        Destroy(TargetCursor);
        //全てが終われば移動に戻る
        phase = Phase.Move;

    }

    IEnumerator AttackPlayColTasukeru(int playernum, string atmethod)
    {
        //ターゲットを表示
        //傾きはx67.915
        Vector3[] Player = new Vector3[3];
        Vector3[] Target = new Vector3[3];
        GameObject[] TargetCursor = new GameObject[3];

        //基準の座標
        Vector3 Player1 = SelectPlayers[playernum].PlayerGameObject.transform.position;

        for (int i = 0; i < selectedMedarotnum; i++)
        {
            if (SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
            {
                //カーソルの生成
                TargetCursor[i] = Instantiate(TargetCursorBluePre);
                //プレイヤーの座標
                Player[i] = SelectPlayers[i].PlayerGameObject.transform.position;
                //カーソルの位置をプレイヤーに合わせる
                TargetCursor[i].transform.position = new Vector3(Player1.x, TargetCursor[i].transform.position.y, Player1.z);
                Target[i] = new Vector3(Player[i].x, TargetCursor[i].transform.position.y, Player[i].z);
            }
        }

        BattleMessage.text = "たすけるスキル　" + atmethod + "！";

        yield return new WaitForSeconds(1.0f);

        //警告音
        soundPlayer.PlayCursorWarningSound();

        //同じ位置に移動するまでループ
        //120回ループさせる
        //while (!(TargetCursor.transform.position == TargetEnemypoti))
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < selectedMedarotnum; j++)
            {
                //TargetCursor.transform.position = Vector3.Lerp(TargetCursor.transform.position, TargetEnemypoti, 5.0f * Time.deltaTime);
                if (SelectPlayers[j].PlayerGameObject.activeInHierarchy == true)
                {
                    TargetCursor[j].transform.position = Vector3.Lerp(TargetCursor[j].transform.position, Target[j], 5.0f * Time.deltaTime);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        BattleMessage.text = "みかたチームの せいこうがアップ！";

        yield return new WaitForSeconds(2.0f);

        BattleMessage.text = "";

        for (int i = 0; i < selectedMedarotnum; i++)
        {
            if (SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
            {
                Destroy(TargetCursor[i]);
            }
        }

        //全てが終われば移動に戻る
        phase = Phase.Move;

    }

    IEnumerator AttackPlayColMamoru(int playernum, string atmethod)
    {
        //ターゲットを表示
        //傾きはx67.915
        Vector3[] Player = new Vector3[3];
        Vector3[] Target = new Vector3[3];
        GameObject[] TargetCursor = new GameObject[3];

        //基準の座標
        Vector3 Player1 = SelectPlayers[playernum].PlayerGameObject.transform.position;

        for (int i = 0; i < selectedMedarotnum; i++)
        {
            if (SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
            {
                //カーソルの生成
                TargetCursor[i] = Instantiate(TargetCursorBluePre);
                //プレイヤーの座標
                Player[i] = SelectPlayers[i].PlayerGameObject.transform.position;
                //カーソルの位置をプレイヤーに合わせる
                TargetCursor[i].transform.position = new Vector3(Player1.x, TargetCursor[i].transform.position.y, Player1.z);
                Target[i] = new Vector3(Player[i].x, TargetCursor[i].transform.position.y, Player[i].z);
            }
        }

        BattleMessage.text = "まもるスキル　" + atmethod + "！";

        yield return new WaitForSeconds(1.0f);

        //警告音
        soundPlayer.PlayCursorWarningSound();

        //同じ位置に移動するまでループ
        //120回ループさせる
        //while (!(TargetCursor.transform.position == TargetEnemypoti))
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < selectedMedarotnum; j++)
            {
                //TargetCursor.transform.position = Vector3.Lerp(TargetCursor.transform.position, TargetEnemypoti, 5.0f * Time.deltaTime);
                if (SelectPlayers[j].PlayerGameObject.activeInHierarchy == true)
                {
                    TargetCursor[j].transform.position = Vector3.Lerp(TargetCursor[j].transform.position, Target[j], 5.0f * Time.deltaTime);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        BattleMessage.text = "みがわりに こうげきを うける";

        yield return new WaitForSeconds(2.0f);

        BattleMessage.text = "";

        for (int i = 0; i < selectedMedarotnum; i++)
        {
            if (SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
            {
                Destroy(TargetCursor[i]);
            }
        }

        //全てが終われば移動に戻る
        phase = Phase.Move;

    }

    IEnumerator DefencePlayColTasukeru(int playernum, string atmethod)
    {
        //ターゲットを表示
        //傾きはx67.915
        Vector3[] Enemy = new Vector3[3];
        Vector3[] Target = new Vector3[3];
        GameObject[] TargetCursor = new GameObject[3];

        //基準の座標
        Vector3 Enemy1 = SelectEnemys[playernum].PlayerGameObject.transform.position;

        for (int i = 0; i < maxEnemy; i++)
        {
            if (SelectEnemys[i].PlayerGameObject.activeInHierarchy == true)
            {
                //カーソルの生成
                TargetCursor[i] = Instantiate(TargetCursorPinkPre);
                //プレイヤーの座標
                Enemy[i] = SelectEnemys[i].PlayerGameObject.transform.position;
                //カーソルの位置をプレイヤーに合わせる
                TargetCursor[i].transform.position = new Vector3(Enemy1.x, TargetCursor[i].transform.position.y, Enemy1.z);
                Target[i] = new Vector3(Enemy[i].x, TargetCursor[i].transform.position.y, Enemy[i].z);
            }
        }

        BattleMessage.text = "たすけるスキル　" + atmethod + "！";

        yield return new WaitForSeconds(1.0f);

        //警告音
        soundPlayer.PlayCursorWarningSound();

        //同じ位置に移動するまでループ
        //120回ループさせる
        //while (!(TargetCursor.transform.position == TargetEnemypoti))
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < maxEnemy; j++)
            {
                //TargetCursor.transform.position = Vector3.Lerp(TargetCursor.transform.position, TargetEnemypoti, 5.0f * Time.deltaTime);
                if (SelectEnemys[j].PlayerGameObject.activeInHierarchy == true)
                {
                    TargetCursor[j].transform.position = Vector3.Lerp(TargetCursor[j].transform.position, Target[j], 5.0f * Time.deltaTime);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        BattleMessage.text = "みかたチームの せいこうがアップ！";

        yield return new WaitForSeconds(2.0f);

        BattleMessage.text = "";

        for (int i = 0; i < maxEnemy; i++)
        {
            if (SelectEnemys[i].PlayerGameObject.activeInHierarchy == true)
            {
                Destroy(TargetCursor[i]);
            }
        }

        //全てが終われば移動に戻る
        phase = Phase.Move;

    }

    IEnumerator DefencePlayColMamoru(int playernum, string atmethod)
    {
        //ターゲットを表示
        //傾きはx67.915
        Vector3[] Enemy = new Vector3[3];
        Vector3[] Target = new Vector3[3];
        GameObject[] TargetCursor = new GameObject[3];

        //基準の座標
        Vector3 Enemy1 = SelectEnemys[playernum].PlayerGameObject.transform.position;

        for (int i = 0; i < maxEnemy; i++)
        {
            if (SelectEnemys[i].PlayerGameObject.activeInHierarchy == true)
            {
                //カーソルの生成
                TargetCursor[i] = Instantiate(TargetCursorPinkPre);
                //プレイヤーの座標
                Enemy[i] = SelectEnemys[i].PlayerGameObject.transform.position;
                //カーソルの位置をプレイヤーに合わせる
                TargetCursor[i].transform.position = new Vector3(Enemy1.x, TargetCursor[i].transform.position.y, Enemy1.z);
                Target[i] = new Vector3(Enemy[i].x, TargetCursor[i].transform.position.y, Enemy[i].z);
            }
        }

        BattleMessage.text = "まもるスキル　" + atmethod + "！";

        yield return new WaitForSeconds(1.0f);

        //警告音
        soundPlayer.PlayCursorWarningSound();

        //同じ位置に移動するまでループ
        //120回ループさせる
        //while (!(TargetCursor.transform.position == TargetEnemypoti))
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < maxEnemy; j++)
            {
                //TargetCursor.transform.position = Vector3.Lerp(TargetCursor.transform.position, TargetEnemypoti, 5.0f * Time.deltaTime);
                if (SelectEnemys[j].PlayerGameObject.activeInHierarchy == true)
                {
                    TargetCursor[j].transform.position = Vector3.Lerp(TargetCursor[j].transform.position, Target[j], 5.0f * Time.deltaTime);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        BattleMessage.text = "みがわりに こうげきを うける";

        yield return new WaitForSeconds(2.0f);

        BattleMessage.text = "";

        for (int i = 0; i < maxEnemy; i++)
        {
            if (SelectEnemys[i].PlayerGameObject.activeInHierarchy == true)
            {
                Destroy(TargetCursor[i]);
            }
        }

        //全てが終われば移動に戻る
        phase = Phase.Move;

    }

    IEnumerator AttacknonTarget()
    {
        BattleMessage.text = "ターゲットはすでに きのうていし している！";
        yield return new WaitForSeconds(2.0f);
        BattleMessage.text = "";
        //全てが終われば移動に戻る
        phase = Phase.Move;
    }

    IEnumerator AttacknonPart()
    {
        BattleMessage.text = "えらんだパーツは はかい されている！";
        yield return new WaitForSeconds(2.0f);
        BattleMessage.text = "";
        //全てが終われば移動に戻る
        phase = Phase.Move;
    }

    public void PhaseCommandfunc(int num)
    {
        //個別選択のコマンドなら
        if(phase == Phase.Command1)
        {

            //自機のコマンド選択
            if (0 <= num && num <= 2)
            {
                selectMedarotnum = num;

                //攻撃対象の決定
                SelectPlayers[num].PlayerStatus.TargetEnemy = UnityEngine.Random.Range(0, selectedMedarotnum);
                SelectPlayers[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);

                //CommandWindowをtrueに
                CommandWindow.SetActive(true);

                //CommandWindowChangeBtnをtrueに
                CommandWindowChangeBtn.SetActive(true);

                //コマンドテキスト
                CommandText.text = "こうどうをせんたくしてください";
                BattleMessage.text = "こうどうをせんたくしてください";

                //コマンド選択キャラ
                switch (num)
                {
                    case 0:
                        //コマンド選択をしているキャラ
                        Leader_sel1.SetActive(true);
                        //コマンド
                        LeaderCommand.text = "";
                        break;
                    case 1:
                        //コマンド選択をしているキャラ
                        Left_sel1.SetActive(true);
                        //コマンド
                        LeftCommand.text = "";
                        break;
                    case 2:
                        //コマンド選択をしているキャラ
                        Right_sel1.SetActive(true);
                        //コマンド
                        RightCommand.text = "";
                        break;
                }

                //帰ってきたメダロットのパーツコマンドを表示する
                SelHeadCommand.text = PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].PartName;
                SelRightCommand.text = PartsList.CharaList[SelectPlayers[num].PlayerStatus.LeftArm.PartsID].PartsList[1].PartName;
                SelLeftCommand.text = PartsList.CharaList[SelectPlayers[num].PlayerStatus.RightArm.PartsID].PartsList[2].PartName;

                //何も選ばれていない状態にリセット
                HeadMethodFlag = false;
                RightMethodFlag = false;
                LeftMethodFlag = false;

            }
            //敵のコマンド選択
            else if (3 <= num && num <= 5)
            {
                num = num - 3;
                //攻撃対象の決定
                SelectEnemys[num].PlayerStatus.TargetEnemy = UnityEngine.Random.Range(0, selectedMedarotnum);
                SelectEnemys[num].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
                SelectEnemys[num].PlayerStatus.SelectPartnum = UnityEngine.Random.Range(0, 3);

                SelectEnemys[num].PlayerStatus.Cooling = false;
                //フェーズを変更
                phase = Phase.Move;
            }
        }
    }

    public void AtMethodSelect1( int Parts )
    {
        //個別選択のコマンドなら
        if (phase == Phase.Command1)
        {
            //カーソルが存在すれば破壊
            if (targetcursorflag)
            {
                Destroy(CommandTargetCursor);
                targetcursorflag = false;
            }

            //頭パーツ
            if (Parts == 0)
            {
                if (HeadMethodFlag == true)
                {
                    if (selectMedarotnum == 0)
                    {
                        LeaderCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;

                    }
                    else if (selectMedarotnum == 1)
                    {
                        LeftCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if (selectMedarotnum == 2)
                    {
                        RightCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }

                    soundPlayer.PlayDecisionSound();
                    //NextMedarotSelect();
                    SelectPlayers[selectMedarotnum].PlayerStatus.Cooling = false;
                    Command1toMove();
                    HeadMethodFlag = false;
                    return;
                }
                CommandText.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;

                BattleMessage.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].AtMethod;


                //うつ　か　ねらいうち　だったら
                if (PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill == "うつ" || PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill == "ねらいうち")
                {
                    CommandText.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";
                    BattleMessage.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";

                    //カーソルがなかったら生成
                    if (!targetcursorflag)
                    {
                        //カーソルの生成
                        CommandTargetCursor = Instantiate(TargetCursorBluePre);
                        targetcursorflag = true;
                    }

                    //敵の位置
                    float enemyx = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.x;
                    float enemyy = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.y;
                    float enemyz = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.z;

                    //位置の変更
                    CommandTargetCursor.transform.position = new Vector3(enemyx, CommandTargetCursor.transform.position.y, enemyz);

                    //頭狙い
                    if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 0)
                    {
                        CommandText.text += "頭パーツ狙い";
                        BattleMessage.text += "頭パーツ狙い";
                    }
                    //右腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 1)
                    {
                        CommandText.text += "右腕パーツ狙い";
                        BattleMessage.text += "右腕パーツ狙い";
                    }
                    //左腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 2)
                    {
                        CommandText.text += "左腕パーツ狙い";
                        BattleMessage.text += "左腕パーツ狙い";
                    }
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 3)
                    {
                        CommandText.text += "脚部パーツ狙い";
                        BattleMessage.text += "脚部パーツ狙い";
                    }

                }

                soundPlayer.PlaySelectionSound();
                HeadMethodFlag = true;
                RightMethodFlag = false;
                LeftMethodFlag = false;
            }
            //右腕パーツ
            else if (Parts == 1)
            {
                if (RightMethodFlag == true)
                {
                    if (selectMedarotnum == 0)
                    {
                        LeaderCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if (selectMedarotnum == 1)
                    {
                        LeftCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if (selectMedarotnum == 2)
                    {
                        RightCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    soundPlayer.PlayDecisionSound();
                    //NextMedarotSelect();
                    SelectPlayers[selectMedarotnum].PlayerStatus.Cooling = false;
                    Command1toMove();
                    RightMethodFlag = false;
                    return;
                }
                CommandText.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;

                BattleMessage.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[1].AtMethod;


                //うつ　か　ねらいうち　だったら
                if (PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[0].Skill == "うつ" || PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.RightArm.PartsID].PartsList[0].Skill == "ねらいうち")
                {
                    CommandText.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";
                    BattleMessage.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";

                    //カーソルがなかったら生成
                    if (!targetcursorflag)
                    {
                        //カーソルの生成
                        CommandTargetCursor = Instantiate(TargetCursorBluePre);
                        targetcursorflag = true;
                    }

                    //敵の位置
                    float enemyx = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.x;
                    float enemyy = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.y;
                    float enemyz = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.z;

                    //位置の変更
                    CommandTargetCursor.transform.position = new Vector3(enemyx, CommandTargetCursor.transform.position.y, enemyz);

                    //頭狙い
                    if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 0)
                    {
                        CommandText.text += "頭パーツ狙い";
                        BattleMessage.text += "頭パーツ狙い";
                    }
                    //右腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 1)
                    {
                        CommandText.text += "右腕パーツ狙い";
                        BattleMessage.text += "右腕パーツ狙い";
                    }
                    //左腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 2)
                    {
                        CommandText.text += "左腕パーツ狙い";
                        BattleMessage.text += "左腕パーツ狙い";
                    }
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 3)
                    {
                        CommandText.text += "脚部パーツ狙い";
                        BattleMessage.text += "脚部パーツ狙い";
                    }

                }

                soundPlayer.PlaySelectionSound();
                HeadMethodFlag = false;
                RightMethodFlag = true;
                LeftMethodFlag = false;
            }
            //左腕パーツ
            else if (Parts == 2)
            {
                if (LeftMethodFlag == true)
                {
                    if (selectMedarotnum == 0)
                    {
                        LeaderCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if (selectMedarotnum == 1)
                    {
                        LeftCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    else if (selectMedarotnum == 2)
                    {
                        RightCommand.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;
                        SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;
                    }
                    soundPlayer.PlayDecisionSound();
                    //NextMedarotSelect();
                    SelectPlayers[selectMedarotnum].PlayerStatus.Cooling = false;
                    Command1toMove();
                    LeftMethodFlag = false;
                    return;
                }
                CommandText.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;

                BattleMessage.text = PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill + "スキル　" +
                                   PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.LeftArm.PartsID].PartsList[2].AtMethod;

                //うつ　か　ねらいうち　だったら
                if (PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill == "うつ" || PartsList.CharaList[SelectPlayers[selectMedarotnum].PlayerStatus.Head.PartsID].PartsList[0].Skill == "ねらいうち")
                {
                    CommandText.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";
                    BattleMessage.text += "　" + SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerStatus.Name + "の";

                    //カーソルがなかったら生成
                    if (!targetcursorflag)
                    {
                        //カーソルの生成
                        CommandTargetCursor = Instantiate(TargetCursorBluePre);
                        targetcursorflag = true;
                    }

                    //敵の位置
                    float enemyx = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.x;
                    float enemyy = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.y;
                    float enemyz = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position.z;

                    //位置の変更
                    CommandTargetCursor.transform.position = new Vector3(enemyx, CommandTargetCursor.transform.position.y, enemyz);

                    //頭狙い
                    if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 0)
                    {
                        CommandText.text += "頭パーツ狙い";
                        BattleMessage.text += "頭パーツ狙い";
                    }
                    //右腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 1)
                    {
                        CommandText.text += "右腕パーツ狙い";
                        BattleMessage.text += "右腕パーツ狙い";
                    }
                    //左腕狙い
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 2)
                    {
                        CommandText.text += "左腕パーツ狙い";
                        BattleMessage.text += "左腕パーツ狙い";
                    }
                    else if (SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts == 3)
                    {
                        CommandText.text += "脚部パーツ狙い";
                        BattleMessage.text += "脚部パーツ狙い";
                    }

                }

                soundPlayer.PlaySelectionSound();
                HeadMethodFlag = false;
                RightMethodFlag = false;
                LeftMethodFlag = true;
            }
        }
    }

    public void Command1toMove()
    {
        //テキストをリセット
        CommandText.text = "";
        BattleMessage.text = "";

        //リセット
        Command_sel1.SetActive(false);
        Command_sel2.SetActive(false);
        Command_sel3.SetActive(false);

        //リセット
        Leader_sel1.SetActive(false);
        Left_sel1.SetActive(false);
        Right_sel1.SetActive(false);


        //コマンドウィンドウを閉じる
        CommandWindow.SetActive(false);
        CommandWindowChangeBtn.SetActive(false);

        //フェーズを変更
        phase = Phase.Move;
    }

}

/*

                    //選択したパーツナンバーを拾ってくる
                int SPart = SelectPlayers[num].PlayerStatus.SelectPartnum;

                //攻撃対象を決定
                int Target     = UnityEngine.Random.Range(0, maxEnemy);
                int TargetPart = UnityEngine.Random.Range(0, 4);
                int Damage = 0;

                //頭パーツ
                if (SPart == 0)
                {
                    //自分のパーツのHPが0ではない
                    if(SelectPlayers[num].PlayerStatus.Head.HP != 0)
                    {
                        //うつ　だったら
                        if (PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].AtMethod == "うつ")
                        {
                            Target = SelectPlayers[num].PlayerStatus.TargetEnemy;
                            TargetPart = SelectPlayers[num].PlayerStatus.TargetParts;

                            //ダメージ量を計算
                            Damage = 0;
                            //基本ダメージ
                            Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Power;
                            //脚部の射撃の得意さ
                            Damage += PartsList.CharaList[SelectPlayers[num].PlayerStatus.Leg.PartsID].PartsList[3].Shooting;

                            //攻撃が成功するか
                            //パーツの成功率 未実装
                            int SuccessRate = PartsList.CharaList[SelectPlayers[num].PlayerStatus.Head.PartsID].PartsList[0].Success;

                            //敵は回避、防御ができるか
                            //冷却中であるか
                            bool isCooling = SelectEnemys[num].PlayerStatus.Cooling;
                            bool canavoidance = true;
                            bool candefence   = true;
                            //敵のディフェンスコマンドを初期化
                            SelectEnemys[Target].PlayerStatus.defencecommand = "";
                            //もし敵が冷却中であれば
                            if (isCooling)
                            {
                                //パーツのスキルで回避、防御ができるかかわる
                                //うつ　　　　回避可能　　防御可能
                                //ねらいうち　回避不可能　防御可能
                                //なぐる　　　回避可能　　防御不可能
                                //がむしゃら　回避不可能　防御不可能
                                //他はなし

                                //敵の選択したパーツは何か
                                string Skill = "";
                                //頭パーツなら
                                if (SelectEnemys[num].PlayerStatus.SelectPartnum == 0)
                                {
                                    Skill = PartsList.CharaList[SelectEnemys[num].PlayerStatus.Head.PartsID].PartsList[0].Skill;
                                }
                                //右腕パーツなら
                                else if(SelectEnemys[num].PlayerStatus.SelectPartnum == 1)
                                {
                                    Skill = PartsList.CharaList[SelectEnemys[num].PlayerStatus.RightArm.PartsID].PartsList[1].Skill;
                                }
                                //左腕パーツなら
                                else if(SelectEnemys[num].PlayerStatus.SelectPartnum == 2)
                                {
                                    Skill = PartsList.CharaList[SelectEnemys[num].PlayerStatus.LeftArm.PartsID].PartsList[2].Skill;
                                }

                                //スキルが　うつ　なら
                                if(Skill == "うつ")
                                {
                                    canavoidance = true;
                                    candefence   = true;
                                }
                                //スキルが　ねらいうち　なら
                                else if(Skill == "ねらいうち")
                                {
                                    canavoidance = false;
                                    candefence   = true;
                                }
                                //スキルが　なぐる　なら
                                else if(Skill == "なぐる")
                                {
                                    canavoidance = true;
                                    candefence   = false;
                                }
                                //スキルが　がむしゃら　なら
                                else if(Skill == "がむしゃら")
                                {
                                    canavoidance = false;
                                    candefence   = false;
                                }
                                //スキルがそれ以外であれば　どちらも可能
                                else
                                {
                                    canavoidance = true;
                                    candefence   = true;
                                }
                            }

                            //敵の回避率
                            int Avoidance = PartsList.CharaList[SelectEnemys[Target].PlayerStatus.Leg.PartsID].PartsList[3].Avoidance;
                            int RAvoidance = UnityEngine.Random.Range(0, Avoidance);
                            //敵の防御率
                            int Defence   = PartsList.CharaList[SelectEnemys[Target].PlayerStatus.Leg.PartsID].PartsList[3].Defence;
                            int RDefence = UnityEngine.Random.Range(0, Defence);

                            //攻撃対象がガードするか、回避するか
                            //それぞれの値を最大値として乱数を生成　大きい方を行動として選択
                            //ガードする
                            if (RAvoidance < RDefence)
                            {
                                //ガードが出来るか
                                if (candefence)
                                {
                                    //ダメージを0.8倍にする
                                    Damage = Mathf.FloorToInt(Damage * 0.8f);
                                    SelectEnemys[Target].PlayerStatus.defencecommand = "ぼうぎょ";
                                }
                            }
                            //回避する
                            else if(RAvoidance > RDefence)
                            {
                                //回避ができるか
                                if (canavoidance)
                                {
                                    //ダメージを0にする
                                    Damage = 0;
                                    SelectEnemys[Target].PlayerStatus.defencecommand = "かいひ";
                                }
                            }
                            
                            //攻撃対象が頭パーツの時
                            if (TargetPart == 0)
                            {
                                //相手の現在のパーツのHPからダメージを引く
                                SelectEnemys[Target].PlayerStatus.Head.HP -= Damage;
                            }
                            //右腕パーツ
                            else if(TargetPart == 1)
                            {
                                SelectEnemys[Target].PlayerStatus.RightArm.HP -= Damage;
                            }
                        }
                    }
                    //パーツのHPがゼロなら
                    else
                    {
                        //失敗する
                        Debug.Log("失敗した\n");
                    }
                }
                //右腕パーツ
                else if(SPart == 1)
                {

                }
                //左腕パーツ
                else if(SPart == 2)
                {

                }

*/
