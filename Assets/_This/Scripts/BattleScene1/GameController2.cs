using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController2 : MonoBehaviour
{
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

    //ゲームの進行時間
    public float Timer;
    public bool[] MamoruFlag;
    public float[] MamoruStartTime;
    public float[] MamoruEndTime;


    //パーツリスト
    public PartsListScript PartsList;
    //サウンドプレイヤー
    public SoundsPlayer soundPlayer;
    //HPバー
    public HpBarController2 hpbar;

    //テキストオブジェクト
    //メダロットの名前
    public Text MedarotName;
    //メダロットのパーツの名前
    public Text MedarotPartsName;
    //パーツの装甲値
    public Text MedarotPartsArmor;
    //パーツの攻撃方法
    public Text MedarotPartsMethod;
    //パーツのスキル
    public Text MedarotPartsSkill;
    //選ばれたメダロットの名前
    public Text SelectMedarotName;

    //リーダーの名前
    public Text LeaderName;
    //リーダーのコマンド
    public Text LeaderCommand;
    //サブ1の名前
    public Text LeftName;
    //サブ2のコマンド
    public Text LeftCommand;
    //サブ2の名前
    public Text RightName;
    //サブ2のコマンド
    public Text RightCommand;

    //コマンドの表示
    //頭コマンド
    public Text SelHeadCommand;
    //頭パーツの使用回数
    public Text HeadCount;
    //右ウデコマンド
    public Text SelRightCommand;
    //左ウデコマンド
    public Text SelLeftCommand;
    //チャージコマンド
    public Text SelChargeCommand;
    //コマンドテキスト
    public Text CommandText;
    //バトルメッセージ
    public Text BattleMessage;
    //ロボトルファイトテキスト
    public Text BattleStartText;
    //リザルト表示
    public Text ResultText;

    //ゲームオブジェクト
    //一番上のゲームオブジェクト
    public GameObject MedarotTopParent;
    //ロボトルインフォ画面
    public GameObject RobottleInfoWindow;
    //メダロット選択画面
    public GameObject MedarotSelectWindow;
    //戦闘開始 確認画面
    public GameObject BattleReadyWindow;
    //コマンド画面
    public GameObject CommandWindow;
    //コマンド画面表示切り替えボタン
    public GameObject CommandWindowChangeBtn;
    //エンカウントのフラッシュの演出
    public GameObject EncountFlashObj;
    //選択中の黄色い枠　リーダー用
    public GameObject Leader_sel1;
    //選択中の黄色い枠　サブ1用
    public GameObject Left_sel1;
    //選択中の黄色い枠　サブ2用
    public GameObject Right_sel1;
    //カメラ
    public GameObject Camera;
    //ターゲットカーソル青色
    public GameObject TargetCursorBluePre;
    //ターゲットカーソルピンク色
    public GameObject TargetCursorPinkPre;
    //カーソルを一時的にしまう
    GameObject CommandTargetCursor;
    //コマンド選択中のキャラに表示する黄色いやつ
    public GameObject Command_sel1;
    public GameObject Command_sel2;
    public GameObject Command_sel3;

    //イメージオブジェクト
    //白い画像
    public Image EncountFlash;
    //黒い画像
    public Image BlackWall1;
    public Image BlackWall2;

    //フェード用の黒い画像
    public Image BlackImageForFade;

    //生まれてくる敵プレハブ
    public GameObject playerPrefab;
    public GameObject UnityChanPrefab;

    //プレイヤーを格納
    //パーツの構造体
    [System.Serializable]
    public struct _Parts
    {
        //パーツが壊れているか
        public bool PartsAvailable;
        //パーツID
        public int PartsID;
        //パーツの耐久値
        public int HP;
    }

    //プレイヤーステータスの構造体
    [System.Serializable]
    public struct _PlayerStatus
    {
        //キャラ選択で選ばれているか否か
        public bool SelectFrag;
        //選択したメダロットの番号
        public int Playernum;
        //キャラの名前
        public string Name;
        //冷却中か否か
        public bool Cooling;
        //パーツ
        public _Parts[] Parts;
        //頭パーツの使用回数
        public int HeadCount;
        //ターゲット相手
        public int TargetEnemy;
        //ターゲットのパーツ
        public int TargetParts;
        //使用するパーツ
        public int SelectPartnum;
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

    //プレイヤーを格納
    public _ExistPlayers[] ExistPlayers;
    public _ExistPlayers[] ExistEnemys;

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

    //プレイヤーの移動量
    public Vector3[] PlayerMovement;
    public Vector3[] EnemyMovement;
    public Vector3 Movement;

    //プレイヤーの移動量の倍率
    //100倍くらいがちょうどいい　基準
    public int PlayerMovingSpeed;
    public int EnemyMovingSpeed;

    //攻撃力の倍率
    public float DamagePower;

    //選択したメダロットの番号
    public int selectMedarotnum;

    //選択したメダロットの数
    public int selectedMedarotnum;

    //コマンドウィンドウでのコマンド選択のフラグ
    public bool HeadMethodFlag;
    public bool RightMethodFlag;
    public bool LeftMethodFlag;

    //ターゲットカーソルが存在するか
    public bool targetcursorflag;
    //メダロットが選ばれたか
    public bool isselecting = false;
    //選ばれたメダロットの名前
    public string[] SelectMedarotNameText = new string[3];

    //初期化
    void Start()
    {
        //プレイヤーの初期化
        ExistPlayersIni();

        //テキストの初期化
        TextIni();

        //プレイヤーとエネミーの生成
        PlayerGenerate();
        EnemyGenerate();

        //時間は0
        Timer = 0f;

        //守るスキル開始時間と終了時間
        MamoruFlag      = new bool[6];
        MamoruStartTime = new float[6];
        MamoruEndTime   = new float[6];
    }

    // Update is called once per frame
    void Update()
    {
        //フェーズ
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
                //経過時間を蓄積
                Timer += Time.deltaTime;
                //Debug.Log(Timer);
                PlayerMoving();

                for (int i = 0; i < 6; i++)
                {
                    //まもるスキルが発動している時
                    if (MamoruFlag[i])
                    {
                        //プレイヤーなら
                        if (0 <= i && i <= 2)
                        {
                            //存在してて
                            if(SelectPlayers[i].PlayerGameObject.activeInHierarchy == true)
                            {
                                //終了時間になった　もしくは　そのパーツが破壊されていれば
                                if (MamoruEndTime[i] <= Timer || (SelectPlayers[i].PlayerStatus.Parts[GetSelectPartsNum(true, i)].PartsAvailable == false))
                                {
                                    MamoruEndTime[i] = 0;
                                    //冷却に入る
                                    SelectPlayers[i].PlayerStatus.Cooling = true;
                                    MamoruFlag[i] = false;
                                    BattleMessage.text = "ガードが終了した";
                                    //1秒後まだ文字が残っていれば
                                    StartCoroutine(DelayMethod(1f, () =>
                                    {
                                        if (BattleMessage.text == "ガードが終了した")
                                            BattleMessage.text = "";
                                    }));
                                }
                            }
                            //存在していなかったら
                            else
                            {
                                MamoruEndTime[i] = 0;
                                MamoruFlag[i] = false;
                            }
                        }
                        //敵なら
                        else if (3 <= i && i <= 5)
                        {
                            //存在してて
                            if(SelectEnemys[i-3].PlayerGameObject.activeInHierarchy == true)
                            {
                                //終了時間になった　もしくは　そのパーツが破壊されていれば
                                if (MamoruEndTime[i] <= Timer || (SelectEnemys[i - 3].PlayerStatus.Parts[GetSelectPartsNum(false, i - 3)].PartsAvailable == false))
                                {
                                    MamoruEndTime[i] = 0;
                                    //冷却に入る
                                    SelectEnemys[i - 3].PlayerStatus.Cooling = true;
                                    MamoruFlag[i] = false;
                                    BattleMessage.text = "ガードが終了した";
                                    //1秒後まだ文字が残っていれば
                                    StartCoroutine(DelayMethod(1f, () =>
                                    {
                                        if (BattleMessage.text == "ガードが終了した")
                                            BattleMessage.text = "";
                                    }));
                                }
                            }
                            //存在していなかったら
                            else
                            {
                                MamoruEndTime[i] = 0;
                                MamoruFlag[i] = false;
                            }
                        }
                    }
                }

                break;
            //攻撃中
            case Phase.Attack:
                break;
            //防御中
            case Phase.Defence:
                break;
            //勝利
            case Phase.Winning:
                break;
            //敗北
            case Phase.Defeat:
                break;
        }
    }

    //プレイヤーの初期化
    void ExistPlayersIni()
    {
        //プレイヤーの保持数だけ配列確保
        ExistPlayers = new _ExistPlayers[maxPlayer];
        ExistEnemys = new _ExistPlayers[maxEnemy];

        //選ばれたプレイヤーの格納
        SelectPlayers = new _ExistPlayers[3];
        SelectEnemys = new _ExistPlayers[3];

        //パーツの配列の初期化
        //プレイヤー
        for (int i = 0; i < ExistPlayers.Length; i++)
        {
            //パーツは4つ
            ExistPlayers[i].PlayerStatus.Parts = new _Parts[4];
        }
        //敵
        for (int i = 0; i < ExistEnemys.Length; i++)
        {
            //パーツは4つ
            ExistEnemys[i].PlayerStatus.Parts = new _Parts[4];
        }

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

        //プレイヤーの移動速度
        Movement = new Vector3(0, 0, 1) * Time.deltaTime;


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

    //テキストの初期化
    void TextIni()
    {
        //テキストの初期化
        MedarotName.text = "";
        MedarotPartsName.text = "";
        MedarotPartsArmor.text = "";
        MedarotPartsMethod.text = "";
        MedarotPartsSkill.text = "";
        SelectMedarotName.text = "";
    }

    //プレイヤーを生成
    void PlayerGenerate()
    {
        //配置する座標を設定
        Vector3 placePosition;
        //配置する回転角を設定
        Quaternion q;

        //プレイヤー保持数だけ繰り返す
        for (int playerCount = 0; playerCount < maxPlayer; ++playerCount)
        {
            //中身がnullだったら
            if (ExistPlayers[playerCount].PlayerGameObject == null)
            {
                //生成する座標
                placePosition = new Vector3(-100 + playerCount * 10, -0.61f, -9.325f);
                //回転角
                q = new Quaternion();
                //回転角は0
                //q = Quaternion.Euler(0, 180, 0);
                q = Quaternion.identity;

                //プレイヤーを作成する
                //インスタンスの作成
                ExistPlayers[playerCount].PlayerGameObject = Instantiate(UnityChanPrefab, placePosition, q) as GameObject;

                //色変更
                //ExistPlayers[playerCount].PlayerGameObject.GetComponent<Renderer>().material.color = playerAlbedo[playerCount];
                //プレイヤーのパーツID 1,2,3が順に入る
                ExistPlayers[playerCount].PlayerStatus.Parts[0].PartsID = PartsList.CharaList[playerCount + 1].ID;
                ExistPlayers[playerCount].PlayerStatus.Parts[1].PartsID = PartsList.CharaList[playerCount + 1].ID;
                ExistPlayers[playerCount].PlayerStatus.Parts[2].PartsID = PartsList.CharaList[playerCount + 1].ID;
                ExistPlayers[playerCount].PlayerStatus.Parts[3].PartsID = PartsList.CharaList[playerCount + 1].ID;

                //選んだプレイヤーの頭パーツのIDからメダロットの名前を拾ってくる
                ExistPlayers[playerCount].PlayerStatus.Name = PartsList.GetCharaName(GetStatusPartsID(true, playerCount, 0));

                //IDからパーツを拾ってくる
                for (int PartsNum = 0; PartsNum < 4; PartsNum++)
                {
                    //各パーツのステータス
                    ExistPlayers[playerCount].PlayerStatus.Parts[PartsNum].PartsAvailable = true;
                    ExistPlayers[playerCount].PlayerStatus.Parts[PartsNum].HP = PartsList.GetCharaArmor(GetStatusPartsID(true, playerCount, PartsNum), PartsNum);
                }
            }
        }
    }

    //敵を生成
    void EnemyGenerate()
    {
        //配置する座標を設定
        Vector3 placePosition;
        //配置する回転角を設定
        Quaternion q;

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
                ExistEnemys[enemyCount].PlayerGameObject = Instantiate(UnityChanPrefab, placePosition, q) as GameObject;

                //色変更
                //ExistEnemys[enemyCount].PlayerGameObject.GetComponent<Renderer>().material.color = enemyAlbedo[enemyCount];
                //プレイヤーのパーツID 1,2,3が順に入る
                ExistEnemys[enemyCount].PlayerStatus.Parts[0].PartsID = PartsList.CharaList[enemyCount + 1].ID;
                ExistEnemys[enemyCount].PlayerStatus.Parts[1].PartsID = PartsList.CharaList[enemyCount + 1].ID;
                ExistEnemys[enemyCount].PlayerStatus.Parts[2].PartsID = PartsList.CharaList[enemyCount + 1].ID;
                ExistEnemys[enemyCount].PlayerStatus.Parts[3].PartsID = PartsList.CharaList[enemyCount + 1].ID;

                //選んだプレイヤーの頭パーツのIDからメダロットの名前を拾ってくる
                ExistEnemys[enemyCount].PlayerStatus.Name = PartsList.GetCharaName(GetStatusPartsID(false, enemyCount, 0));
                //頭パーツのステータス
                //IDからパーツを拾ってくる
                for (int PartsNum = 0; PartsNum < 4; PartsNum++)
                {
                    //各パーツのステータス
                    ExistEnemys[enemyCount].PlayerStatus.Parts[PartsNum].PartsAvailable = true;
                    ExistEnemys[enemyCount].PlayerStatus.Parts[PartsNum].HP = PartsList.GetCharaArmor(GetStatusPartsID(false, enemyCount, PartsNum), PartsNum);
                }
                //SelectEnemyに格納
                SelectEnemys[enemyCount] = ExistEnemys[enemyCount];

                //MedarotTopParentの子として設定
                SelectEnemys[enemyCount].PlayerGameObject.transform.parent = MedarotTopParent.transform;
                //回転は0
                SelectEnemys[enemyCount].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

                //生成する場所
                switch (enemyCount)
                {
                    case 0:
                        SelectEnemys[enemyCount].PlayerGameObject.transform.position = new Vector3(0.0f, -0.61f, 7.29f);
                        SelectEnemys[enemyCount].PlayerGameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                        SelectEnemys[enemyCount].PlayerGameObject.tag = "Enemy1";
                        break;
                    case 1:
                        SelectEnemys[enemyCount].PlayerGameObject.transform.position = new Vector3(-6.11f, -0.61f, 7.29f);
                        SelectEnemys[enemyCount].PlayerGameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                        SelectEnemys[enemyCount].PlayerGameObject.tag = "Enemy2";
                        break;
                    case 2:
                        SelectEnemys[enemyCount].PlayerGameObject.transform.position = new Vector3(6.11f, -0.61f, 7.29f);
                        SelectEnemys[enemyCount].PlayerGameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                        SelectEnemys[enemyCount].PlayerGameObject.tag = "Enemy3";
                        break;
                }
            }
        }
    }

    //パーツのIDを拾ってくる
    //PlayerNum 配列の添字 PorE プレイヤーならTrue エネミーならfalse
    int GetStatusPartsID(bool PorE, int PlayerNum, int PartsNum)
    {
        //パーツのIDを返す
        int PartsID = 0;

        //プレイヤーなら
        if (PorE)
        {
            //パーツのID
            PartsID = ExistPlayers[PlayerNum].PlayerStatus.Parts[PartsNum].PartsID;
        }
        //エネミーなら
        else
        {
            //パーツのID
            PartsID = ExistEnemys[PlayerNum].PlayerStatus.Parts[PartsNum].PartsID;
        }

        //値返す
        return PartsID;
    }

    //パーツのIDを拾ってくる
    //PlayerNum 配列の添字 PorE プレイヤーならTrue エネミーならfalse
    int GetSelectPlayerPartsID(bool PorE, int PlayerNum, int PartsNum)
    {
        //パーツのIDを返す
        int PartsID = 0;

        //プレイヤーなら
        if (PorE)
        {
            //パーツのID
            PartsID = SelectPlayers[PlayerNum].PlayerStatus.Parts[PartsNum].PartsID;
        }
        //エネミーなら
        else
        {
            //パーツのID
            PartsID = SelectEnemys[PlayerNum].PlayerStatus.Parts[PartsNum].PartsID;
        }

        //値返す
        return PartsID;
    }

    //パーツのIDを拾ってくる
    //PlayerNum 配列の添字 PorE プレイヤーならTrue エネミーならfalse
    int GetSelectPartsID(bool PorE, int PlayerNum)
    {
        //選択したパーツのナンバーを拾ってくる
        int SelectPartsNum = GetSelectPartsNum(PorE, PlayerNum);

        //パーツのIDを返す
        int PartsID = 0;

        //プレイヤーなら
        if (PorE)
        {
            //パーツのID
            PartsID = SelectPlayers[PlayerNum].PlayerStatus.Parts[SelectPartsNum].PartsID;
        }
        //エネミーなら
        else
        {
            //パーツのID
            PartsID = ExistEnemys[PlayerNum].PlayerStatus.Parts[SelectPartsNum].PartsID;
        }

        //値返す
        return PartsID;
    }

    //パーツのHPを拾ってくる
    //PlayerNum 配列の添字 PorE プレイヤーならTrue エネミーならfalse
    int GetSelectPartsHP(bool PorE, int PlayerNum, int PartsNum)
    {
        //パーツのHPを返す
        int PlayerHP = 0;

        //プレイヤーなら
        if (PorE)
        {
            //パーツのID
            PlayerHP = SelectPlayers[PlayerNum].PlayerStatus.Parts[PartsNum].HP;
        }
        //エネミーなら
        else
        {
            //パーツのID
            PlayerHP = SelectEnemys[PlayerNum].PlayerStatus.Parts[PartsNum].HP;
        }

        //値返す
        return PlayerHP;
    }

    //プレイヤーの名前を拾ってくる
    //PlayerNum 配列の添字 PorE プレイヤーならTrue エネミーならfalse
    string GetStatusCharaName(bool PorE, int PlayerNum)
    {
        //プレイヤーの名前を返す
        string PlayerName;

        //プレイヤーなら
        if (PorE)
        {
            //パーツのID
            PlayerName = ExistPlayers[PlayerNum].PlayerStatus.Name;
        }
        //エネミーなら
        else
        {
            //パーツのID
            PlayerName = ExistEnemys[PlayerNum].PlayerStatus.Name;
        }

        //値返す
        return PlayerName;
    }

    //プレイヤーが冷却中か否か
    //PlayerNum 配列の添字 PorE プレイヤーならTrue エネミーならfalse
    bool GetStatusCooling(bool PorE, int PlayerNum)
    {
        //プレイヤーの名前を返す
        bool Cooling;

        //プレイヤーなら
        if (PorE)
        {
            //冷却中か
            Cooling = SelectPlayers[PlayerNum].PlayerStatus.Cooling;
        }
        //エネミーなら
        else
        {
            //冷却中か
            Cooling = SelectEnemys[PlayerNum].PlayerStatus.Cooling;
        }

        //値返す
        return Cooling;
    }

    //プレイヤーが選択したパーツナンバー
    //PlayerNum 配列の添字 PorE プレイヤーならTrue エネミーならfalse
    int GetSelectPartsNum(bool PorE, int PlayerNum)
    {
        //選択したパーツナンバー
        int PartsNum;

        //プレイヤーなら
        if (PorE)
        {
            //冷却中か
            PartsNum = SelectPlayers[PlayerNum].PlayerStatus.SelectPartnum;
        }
        //エネミーなら
        else
        {
            //冷却中か
            PartsNum = SelectEnemys[PlayerNum].PlayerStatus.SelectPartnum;
        }

        //値返す
        return PartsNum;
    }

    //ロボトルインフォフェーズに移動
    public void ToRobottleInfo()
    {
        //ロボトルインフォフェーズに移動
        phase = Phase.Roboinfo;

        StartCoroutine(DelayMethod(2.65f, () =>
        {
            RobottleInfoWindow.SetActive(true);
        }));

        StartCoroutine(EncountFlasher());
    }

    //送らせて実行
    public IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    //エンカウント時のフラッシュの演出
    private IEnumerator EncountFlasher()
    {
        //黒い壁のRectTransform
        RectTransform RectBlackWall1 = BlackWall1.GetComponent<RectTransform>();
        RectTransform RectBlackWall2 = BlackWall2.GetComponent<RectTransform>();

        //画像をとってくる
        Image flashimg = EncountFlash.gameObject.GetComponent<Image>();

        //透明度をいじる
        for (int i = 0; i < 2; i++)
        {
            while (flashimg.color.a < 1)
            {
                //flashimg.color += new Color(0, 0, 0, 0.03f);
                flashimg.color += new Color(0, 0, 0, 5 * Time.deltaTime);
                yield return null;
            }
            while (flashimg.color.a > 0)
            {
                //flashimg.color -= new Color(0, 0, 0, 0.10f);
                flashimg.color -= new Color(0, 0, 0, 10 * Time.deltaTime);
                yield return null;
            }
        }

        //黒い壁の移動
        while (RectBlackWall1.localPosition.x > 200)
        {
            RectBlackWall1.localPosition -= new Vector3(1, 0, 0) * 270 * Time.deltaTime;
            RectBlackWall2.localPosition += new Vector3(1, 0, 0) * 270 * Time.deltaTime;
            yield return null;
        }

        //2秒後に消す
        StartCoroutine(DelayMethod(2f, () =>
        {
            EncountFlashObj.SetActive(false);
        }));
    }

    private IEnumerator FadePlusToCommand()
    {
        //フェードアウト
        while (BlackImageForFade.color.a < 1)
        {
            BlackImageForFade.color += new Color(0, 0, 0, 1f) * Time.deltaTime;
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
                    //座標とタグを変更
                    SelectPlayers[playernum].PlayerGameObject.transform.position = new Vector3(0.0f, -0.61f, -7.58f);
                    SelectPlayers[playernum].PlayerGameObject.tag = "Player1";
                    break;
                case 1:
                    //座標とタグを変更
                    SelectPlayers[playernum].PlayerGameObject.transform.position = new Vector3(-6.11f, -0.61f, -7.58f);
                    SelectPlayers[playernum].PlayerGameObject.tag = "Player2";
                    break;
                case 2:
                    //座標とタグを変更
                    SelectPlayers[playernum].PlayerGameObject.transform.position = new Vector3(6.11f, -0.61f, -7.58f);
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
        //Leader_sel1.SetActive(true);
        //Left_sel1.SetActive(false);
        //Right_sel1.SetActive(false);

        //プレイヤーの名前
        LeaderName.text = SelectPlayers[0].PlayerStatus.Name;
        LeftName.text = SelectPlayers[1].PlayerStatus.Name;
        RightName.text = SelectPlayers[2].PlayerStatus.Name;

        //コマンド
        LeaderCommand.text = "";
        RightCommand.text = "";
        LeftCommand.text = "";

        //最初はリーダーコマンド
        SelHeadCommand.text = PartsList.GetCharaPartName(GetSelectPlayerPartsID(true, 0, 0), 0);
        SelRightCommand.text = PartsList.GetCharaPartName(GetSelectPlayerPartsID(true, 0, 1), 1);
        SelLeftCommand.text = PartsList.GetCharaPartName(GetSelectPlayerPartsID(true, 0, 2), 2);

        HeadMethodFlag = false;
        RightMethodFlag = false;
        LeftMethodFlag = false;

        //送れてBGM開始
        StartCoroutine(DelayMethod(0.7f, () =>
        {
            //PlayBattleMusic
            soundPlayer.PlayBattleMusic();
        }));

        //フェードイン
        StartCoroutine(DelayMethod(0.2f, () =>
        {
            StartCoroutine(FadeinToCommand());
        }));

    }

    //フェードインとコマンドに移動
    private IEnumerator FadeinToCommand()
    {
        while (BlackImageForFade.color.a > 0)
        {
            BlackImageForFade.color -= new Color(0, 0, 0, 2f) * Time.deltaTime;
            yield return null;
        }

        //Fade用GameObjectを非アクティブにする
        BlackImageForFade.gameObject.SetActive(false);

        //コマンド選択をしているキャラ
        Leader_sel1.SetActive(true);
        Left_sel1.SetActive(false);
        Right_sel1.SetActive(false);
    }

    //コマンドフェーズ
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
        StartCoroutine(DelayMethod(0.5f, () =>
        {
            //StopEncountMusic
            soundPlayer.StopEncountMusic();
        }));

        //HPバーの設定
        hpbar.SetUpBar(selectedMedarotnum, maxEnemy);

        //攻撃対象の決定
        //プレイヤー
        for (int i = 0; i < selectedMedarotnum; i++)
        {
            SelectPlayers[i].PlayerStatus.TargetEnemy = UnityEngine.Random.Range(0, maxEnemy);
            SelectPlayers[i].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
        }
        //エネミー
        for (int i = 0; i < maxEnemy; i++)
        {
            SelectEnemys[i].PlayerStatus.TargetEnemy = UnityEngine.Random.Range(0, selectedMedarotnum);
            SelectEnemys[i].PlayerStatus.TargetParts = UnityEngine.Random.Range(0, 4);
            SelectEnemys[i].PlayerStatus.SelectPartnum = UnityEngine.Random.Range(0, 3);
        }
    }

    //攻撃方法の選択
    public void AtMethodSelect(int Parts)
    {
        if (phase == Phase.Command)
        {
            //カーソルが存在すれば破壊
            if (targetcursorflag)
            {
                Destroy(CommandTargetCursor);
                targetcursorflag = false;
            }

            //コマンドの文字列
            String AtMethodText = PartsList.GetCharaAtMethod(GetStatusPartsID(true, selectMedarotnum, Parts), Parts);
            String SkillText = PartsList.GetCharaSkill(GetStatusPartsID(true, selectMedarotnum, Parts), Parts);
            //頭パーツのコマンドが選ばれていて　再び選ばれた時
            if ((Parts == 0 && HeadMethodFlag == true) ||  //頭パーツが選ばれた
                (Parts == 1 && RightMethodFlag == true) ||  //右腕パーツが選ばれた
                (Parts == 2 && LeftMethodFlag == true))    //左腕パーツが選ばれた
            {
                //選んだパーツを記憶しておく
                SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;

                //テキストの設定
                if (selectMedarotnum == 0)
                {
                    LeaderCommand.text = AtMethodText;
                }
                else if (selectMedarotnum == 1)
                {
                    LeftCommand.text = AtMethodText;
                }
                else if (selectMedarotnum == 2)
                {
                    RightCommand.text = AtMethodText;
                }

                //決定音を鳴らす
                soundPlayer.PlayDecisionSound();
                //次のメダロットの選択に移動する
                NextMedarotSelect();
                //パーツ選択のフラグを下げる
                HeadMethodFlag = false;
                RightMethodFlag = false;
                LeftMethodFlag = false;

                return;
            }

            //コマンドテキスト
            CommandText.text = SkillText + "スキル　" + AtMethodText;
            //バトルメッセージテキスト
            BattleMessage.text = SkillText + "スキル　" + AtMethodText;

            //うつ　か　ねらいうち　だったら
            if (SkillText == "うつ" || SkillText == "ねらいうち")
            {
                CommandText.text += "　" + GetStatusCharaName(false, SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy);
                BattleMessage.text += "　" + GetStatusCharaName(false, SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy);

                //カーソルがなかったら生成
                if (!targetcursorflag)
                {
                    //カーソルの生成
                    CommandTargetCursor = Instantiate(TargetCursorBluePre);
                    targetcursorflag = true;
                }

                //敵の位置
                Vector3 Enemypoti = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position;

                //位置の変更
                CommandTargetCursor.transform.position = new Vector3(Enemypoti.x, CommandTargetCursor.transform.position.y, Enemypoti.z);

                //ターゲットは
                int Target = SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts;

                //頭狙い
                if (Target == 0)
                {
                    CommandText.text += "頭パーツ狙い";
                    BattleMessage.text += "頭パーツ狙い";
                }
                //右腕狙い
                else if (Target == 1)
                {
                    CommandText.text += "右腕パーツ狙い";
                    BattleMessage.text += "右腕パーツ狙い";
                }
                //左腕狙い
                else if (Target == 2)
                {
                    CommandText.text += "左腕パーツ狙い";
                    BattleMessage.text += "左腕パーツ狙い";
                }
                else if (Target == 3)
                {
                    CommandText.text += "脚部パーツ狙い";
                    BattleMessage.text += "脚部パーツ狙い";
                }
            }

            //選択のサウンドを鳴らす
            soundPlayer.PlaySelectionSound();

            //一旦全部下げる
            HeadMethodFlag = false;
            RightMethodFlag = false;
            LeftMethodFlag = false;

            //フラグを立てる
            if (Parts == 0)
                //フラグは頭パーツ
                HeadMethodFlag = true;
            else if (Parts == 1)
                //フラグは右腕パーツ
                RightMethodFlag = true;
            else if (Parts == 2)
                //フラグは左腕パーツ
                LeftMethodFlag = true;
        }
    }

    public void AtMethodSelect1(int Parts)
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

            //コマンドの文字列
            String AtMethodText = PartsList.GetCharaAtMethod(GetStatusPartsID(true, selectMedarotnum, Parts), Parts);
            String SkillText = PartsList.GetCharaSkill(GetStatusPartsID(true, selectMedarotnum, Parts), Parts);
            //頭パーツのコマンドが選ばれていて　再び選ばれた時
            if ((Parts == 0 && HeadMethodFlag == true) ||  //頭パーツが選ばれた
                (Parts == 1 && RightMethodFlag == true) ||  //右腕パーツが選ばれた
                (Parts == 2 && LeftMethodFlag == true))    //左腕パーツが選ばれた
            {
                //選んだパーツを記憶しておく
                SelectPlayers[selectMedarotnum].PlayerStatus.SelectPartnum = Parts;

                //テキストの設定
                if (selectMedarotnum == 0)
                {
                    LeaderCommand.text = AtMethodText;
                }
                else if (selectMedarotnum == 1)
                {
                    LeftCommand.text = AtMethodText;
                }
                else if (selectMedarotnum == 2)
                {
                    RightCommand.text = AtMethodText;
                }

                //決定音を鳴らす
                soundPlayer.PlayDecisionSound();
                //充填中
                SelectPlayers[selectMedarotnum].PlayerStatus.Cooling = false;
                //フェーズをMoveに切り替える
                Command1toMove();
                //パーツ選択のフラグを下げる
                HeadMethodFlag = false;
                RightMethodFlag = false;
                LeftMethodFlag = false;

                return;
            }

            //コマンドテキスト
            CommandText.text = SkillText + "スキル　" + AtMethodText;
            //バトルメッセージテキスト
            BattleMessage.text = SkillText + "スキル　" + AtMethodText;

            //うつ　か　ねらいうち　だったら
            if (SkillText == "うつ" || SkillText == "ねらいうち")
            {
                CommandText.text += "　" + GetStatusCharaName(false, SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy);
                BattleMessage.text += "　" + GetStatusCharaName(false, SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy);

                //カーソルがなかったら生成
                if (!targetcursorflag)
                {
                    //カーソルの生成
                    CommandTargetCursor = Instantiate(TargetCursorBluePre);
                    targetcursorflag = true;
                }

                //敵の位置
                Vector3 Enemypoti = SelectEnemys[SelectPlayers[selectMedarotnum].PlayerStatus.TargetEnemy].PlayerGameObject.transform.position;

                //位置の変更
                CommandTargetCursor.transform.position = new Vector3(Enemypoti.x, CommandTargetCursor.transform.position.y, Enemypoti.z);

                //ターゲットは
                int Target = SelectPlayers[selectMedarotnum].PlayerStatus.TargetParts;

                //頭狙い
                if (Target == 0)
                {
                    CommandText.text += "頭パーツ狙い";
                    BattleMessage.text += "頭パーツ狙い";
                }
                //右腕狙い
                else if (Target == 1)
                {
                    CommandText.text += "右腕パーツ狙い";
                    BattleMessage.text += "右腕パーツ狙い";
                }
                //左腕狙い
                else if (Target == 2)
                {
                    CommandText.text += "左腕パーツ狙い";
                    BattleMessage.text += "左腕パーツ狙い";
                }
                else if (Target == 3)
                {
                    CommandText.text += "脚部パーツ狙い";
                    BattleMessage.text += "脚部パーツ狙い";
                }
            }

            //選択のサウンドを鳴らす
            soundPlayer.PlaySelectionSound();

            //一旦全部下げる
            HeadMethodFlag = false;
            RightMethodFlag = false;
            LeftMethodFlag = false;

            //フラグを立てる
            if (Parts == 0)
            {
                if (SelectPlayers[selectMedarotnum].PlayerStatus.Parts[Parts].PartsAvailable)
                {
                    //フラグは頭パーツ
                    HeadMethodFlag = true;
                }
            }
            else if (Parts == 1)
            {
                if (SelectPlayers[selectMedarotnum].PlayerStatus.Parts[Parts].PartsAvailable)
                {
                    //フラグは右腕パーツ
                    RightMethodFlag = true;
                }
            }
            else if (Parts == 2)
            {
                if (SelectPlayers[selectMedarotnum].PlayerStatus.Parts[Parts].PartsAvailable)
                {
                    //フラグは左腕パーツ
                    LeftMethodFlag = true;
                }
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

    //次のメダロットの選択をする
    public void NextMedarotSelect()
    {
        //選択したメダロットの数を増やす
        selectMedarotnum++;

        //コマンドの選択を全て消す
        Command_sel1.SetActive(false);
        Command_sel2.SetActive(false);
        Command_sel3.SetActive(false);

        //現在選択中のメダロットの表示を全て消す
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

            //コマンドウィンドウを閉じる
            CommandWindow.SetActive(false);
            CommandWindowChangeBtn.SetActive(false);
            //ロボトルファイト
            BattleStart();

            return;
        }

        if (selectMedarotnum == 1)
        {
            //コマンド選択をしているキャラ
            Left_sel1.SetActive(true);
        }
        else if (selectMedarotnum == 2)
        {
            Right_sel1.SetActive(true);
        }

        //次のメダロットのステータスを表示する
        SelHeadCommand.text = PartsList.GetCharaPartName(GetStatusPartsID(true, selectMedarotnum, 0), 0);
        SelRightCommand.text = PartsList.GetCharaPartName(GetStatusPartsID(true, selectMedarotnum, 1), 1);
        SelLeftCommand.text = PartsList.GetCharaPartName(GetStatusPartsID(true, selectMedarotnum, 2), 2);

    }

    //ロボトルファイト
    public void BattleStart()
    {
        //ロボトル
        BattleStartText.gameObject.SetActive(true);
        //1秒後にテキストを消す
        StartCoroutine(DelayMethod(1.0f, () =>
        {
            BattleStartText.gameObject.SetActive(false);
        }));
        //更に1秒後にフェーズを移動する
        StartCoroutine(DelayMethod(1.5f, () =>
        {
            phase = Phase.Move;
        }));
    }

    //メダロット選択画面
    public void MedarotSelect()
    {
        //Unitychanのアニメーターコンポーネントを拾ってくる
        Animator ca = ExistPlayers[selectMedarotnum].PlayerGameObject.GetComponent<Animator>();

        selectMedarotnum = 0;

        //前のオブジェクトの位置を変える
        ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(100 + selectMedarotnum * 10,
                                                                        MedarotSelectWindow.transform.position.y + 0.5f,
                                                                        MedarotSelectWindow.transform.position.z);

        //親子関係を変更 回転 位置を移動 
        ExistPlayers[selectMedarotnum].PlayerGameObject.transform.parent = MedarotSelectWindow.transform;
        ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(Camera.transform.position.x, 180, 0));
        ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localPosition = new Vector3(146f, -81.7f, -77f);

        ca.SetBool("Running", true);

        //選択したメダロットはリセット
        SelectMedarotName.text = "";

        //キャラの名前
        MedarotName.text = GetStatusCharaName(true, selectMedarotnum);

        //初期化
        MedarotPartsName.text = "";
        MedarotPartsArmor.text = "";
        MedarotPartsMethod.text = "";
        MedarotPartsSkill.text = "";

        //テキストの設定
        for (int PartsNum = 0; PartsNum < 4; PartsNum++)
        {
            //キャラのパーツ名
            MedarotPartsName.text += PartsList.GetCharaPartName(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
            //キャラのパーツ装甲
            MedarotPartsArmor.text += PartsList.GetCharaArmor(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
            //キャラの攻撃方法
            MedarotPartsMethod.text += PartsList.GetCharaAtMethod(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
            //キャラのスキル
            MedarotPartsSkill.text += PartsList.GetCharaSkill(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
        }
    }

    //次のメダロットを表示
    public void MedarotSelectNext()
    {
        //次のメダロットを表示するのは所持しているプレイヤーの数の一個手前
        if (0 <= selectMedarotnum && selectMedarotnum < maxPlayer - 1)
        {
            //Unitychanのアニメーターコンポーネントを拾ってくる
            Animator ca = ExistPlayers[selectMedarotnum].PlayerGameObject.GetComponent<Animator>();
            ca.SetBool("Running", false);

            //前のオブジェクトの位置を変える
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(100 + selectMedarotnum * 10,
                                                                MedarotSelectWindow.transform.position.y + 0.5f,
                                                                MedarotSelectWindow.transform.position.z);
            //プラス1する
            selectMedarotnum++;
            //親子関係を変更 回転 位置を移動
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.parent = MedarotSelectWindow.transform;
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(Camera.transform.position.x, 180, 0));
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localPosition = new Vector3(146f, -81.7f, -77f);

            //Unitychanのアニメーターコンポーネントを拾ってくる
            ca = ExistPlayers[selectMedarotnum].PlayerGameObject.GetComponent<Animator>();
            ca.SetBool("Running", true);

            //キャラの名前
            MedarotName.text = GetStatusCharaName(true, selectMedarotnum);
            //テキストの設定
            MedarotPartsName.text = "";
            MedarotPartsArmor.text = "";
            MedarotPartsMethod.text = "";
            MedarotPartsSkill.text = "";
            for (int PartsNum = 0; PartsNum < 4; PartsNum++)
            {
                //キャラのパーツ名
                MedarotPartsName.text += PartsList.GetCharaPartName(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
                //キャラのパーツ装甲
                MedarotPartsArmor.text += PartsList.GetCharaArmor(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
                //キャラの攻撃方法
                MedarotPartsMethod.text += PartsList.GetCharaAtMethod(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
                //キャラのスキル
                MedarotPartsSkill.text += PartsList.GetCharaSkill(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
            }
        }
    }

    //前のメダロットを表示
    public void MedarotSelectBack()
    {
        //前のメダロットを表示するのはselectMedarotnumが1の時まで
        if (0 < selectMedarotnum && selectMedarotnum <= maxPlayer - 1)
        {
            //Unitychanのアニメーターコンポーネントを拾ってくる
            Animator ca = ExistPlayers[selectMedarotnum].PlayerGameObject.GetComponent<Animator>();
            ca.SetBool("Running", false);

            //前のオブジェクトの位置を変える
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.position = new Vector3(100 + selectMedarotnum * 10,
                                                                MedarotSelectWindow.transform.position.y + 0.5f,
                                                                MedarotSelectWindow.transform.position.z);
            //マイナス1する
            selectMedarotnum--;
            //親子関係を変更 回転 位置を移動
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.parent = MedarotSelectWindow.transform;
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localRotation = Quaternion.Euler(new Vector3(Camera.transform.position.x, 180, 0));
            ExistPlayers[selectMedarotnum].PlayerGameObject.transform.localPosition = new Vector3(146f, -81.7f, -77f);

            //Unitychanのアニメーターコンポーネントを拾ってくる
            ca = ExistPlayers[selectMedarotnum].PlayerGameObject.GetComponent<Animator>();
            ca.SetBool("Running", true);


            //キャラの名前
            MedarotName.text = GetStatusCharaName(true, selectMedarotnum);
            //テキストの設定
            MedarotPartsName.text = "";
            MedarotPartsArmor.text = "";
            MedarotPartsMethod.text = "";
            MedarotPartsSkill.text = "";
            for (int PartsNum = 0; PartsNum < 4; PartsNum++)
            {
                //キャラのパーツ名
                MedarotPartsName.text += PartsList.GetCharaPartName(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
                //キャラのパーツ装甲
                MedarotPartsArmor.text += PartsList.GetCharaArmor(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
                //キャラの攻撃方法
                MedarotPartsMethod.text += PartsList.GetCharaAtMethod(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
                //キャラのスキル
                MedarotPartsSkill.text += PartsList.GetCharaSkill(ExistPlayers[selectMedarotnum].PlayerStatus.Parts[PartsNum].PartsID, PartsNum) + "\n";
            }
        }
    }

    //メダロットの選択
    public void MedarotSelectEnter()
    {
        //選べるのは3体まで
        if (selectedMedarotnum < maxPlayer)
        {
            //選ばれていなければ
            if (!ExistPlayers[selectMedarotnum].PlayerStatus.SelectFrag)
            {
                isselecting = true;
                //SelectPlayerに格納
                ExistPlayers[selectMedarotnum].PlayerStatus.SelectFrag = true;
                SelectPlayers[selectedMedarotnum] = ExistPlayers[selectMedarotnum];
                SelectPlayers[selectedMedarotnum].PlayerStatus.Playernum = selectMedarotnum;
                //選択したメダロットの名前を表示
                if (selectedMedarotnum == 0)
                {
                    SelectMedarotName.text += "L " + GetStatusCharaName(true, selectMedarotnum) + "\n";
                }
                else
                {
                    SelectMedarotName.text += "S" + selectedMedarotnum + " " + GetStatusCharaName(true, selectMedarotnum) + "\n";
                }
                SelectMedarotNameText[selectedMedarotnum] = SelectMedarotName.text;
                selectedMedarotnum++;
                MedarotSelectNext();
            }
        }
        //3体選んだら
        if (selectedMedarotnum == 3)
        {
            BattleReadyWindow.SetActive(true);
        }
    }

    //バトルレディ画面を表示する
    public void BattleReadyWindowSetActivetrue()
    {
        //一体以上選ばれていれば
        if (selectedMedarotnum != 0)
        {
            //表示する
            BattleReadyWindow.SetActive(true);
        }
    }

    //メダロットの選択を取り消す
    public void MedarotSelectReturn()
    {
        //選択を取り消す
        //最後に選んだメダロットの選択を取り消す Playernumが選んだメダロットの番号を記憶している
        ExistPlayers[SelectPlayers[selectedMedarotnum - 1].PlayerStatus.Playernum].PlayerStatus.SelectFrag = false;
        //名前を消す
        SelectPlayers[selectedMedarotnum - 1].PlayerStatus.Name = "";
        //選んだメダロットの数をへらす
        selectedMedarotnum--;
        //手前のメダロットを表示する
        MedarotSelectBack();

        //選んだメダロットがあれば
        if (selectedMedarotnum != 0)
        {
            //選択を取り消したメダロット以外のメダロットの名前の一覧を表示
            SelectMedarotName.text = SelectMedarotNameText[selectedMedarotnum - 1];
        }
        //選択中のフラグ
        else if (selectedMedarotnum == 0)
        {
            //一体も選んでいなければ
            //何も書かない
            SelectMedarotName.text = "";
            //メダロットが選ばれているフラグを下げる
            isselecting = false;
        }
    }

    //フェーズがMoveの時
    //メダロットを移動させる
    public void PlayerMoving()
    {
        //自機と敵を移動させる
        //選んだメダロット分ループ
        for (int i = 0; i < 2; i++)
        {
            //ループ回数で自機か敵か
            bool playerflag;

            if (i == 0)
                playerflag = true;
            else
                playerflag = false;


            //プレイヤーは最大でも3体
            for (int j = 0; j < 3; j++)
            {
                //プレイヤーを移動させる数
                if (playerflag && (j == selectedMedarotnum))
                    continue;
                //敵を移動させる数
                if (!playerflag && (j == maxEnemy))
                    continue;

                //脚部パーツの移動性能をとってくる
                int Moving = PartsList.GetCharaMoving(GetSelectPlayerPartsID(playerflag, j, 3), 3);

                //選択したパーツナンバーを拾ってくる
                int SPart = GetSelectPartsNum(playerflag, j);

                //まもるスキル発動中なら移動させない
                if (playerflag && MamoruFlag[j])
                {
                    //プレイヤーの3体分
                    PlayerMovement[j] = Vector3.zero;

                    continue;
                }
                else if(!playerflag && MamoruFlag[j + 3])
                {
                    //エネミーの3体分
                    EnemyMovement[j] = Vector3.zero;
                    continue;
                }               
                //プレイヤーが冷却中であれば後退
                else if (GetStatusCooling(playerflag, j))
                {
                    //選んだパーツの冷却性能をとってくる
                    int Cooling = PartsList.GetCharaCooling(GetSelectPartsID(playerflag, j), SPart);

                    //プレイヤーだったら
                    if (playerflag)
                    {
                        //移動量の倍率
                        int MoviengBias = (Moving + Cooling) * PlayerMovingSpeed;

                        //移動量を決定
                        PlayerMovement[j] = Movement * MoviengBias * -1;

                        //冷却中の体は正面を向いている
                        SelectPlayers[j].PlayerGameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    //エネミーだったら
                    else
                    {
                        //移動量の倍率
                        int MoviengBias = (Moving + Cooling) * EnemyMovingSpeed;

                        //移動量を決定
                        EnemyMovement[j] = Movement * MoviengBias;

                        //冷却中の体は背を向けている
                        SelectEnemys[j].PlayerGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
                //プレイヤーが充填中であれば前進
                else
                {
                    //選んだパーツの充填性能をとってくる
                    int Loading = PartsList.GetCharaLoading(GetSelectPartsID(playerflag, j), SPart);

                    //プレイヤーだったら
                    if (playerflag)
                    {
                        //移動量の倍率
                        int MoviengBias = (Moving + Loading) * PlayerMovingSpeed;

                        //移動量を決定
                        PlayerMovement[j] = Movement * MoviengBias;

                        //冷却中の体は背を向けている
                        SelectPlayers[j].PlayerGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    //エネミーだったら
                    else
                    {
                        //移動量の倍率
                        int MoviengBias = (Moving + Loading) * EnemyMovingSpeed;

                        //移動量を決定
                        EnemyMovement[j] = Movement * MoviengBias * -1;

                        //冷却中の体は正面を向いている
                        SelectEnemys[j].PlayerGameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                }
                //プレイヤーだったら
                if (playerflag)
                {
                    //速度ベクトルに適応
                    //SelectPlayers[j].PlayerGameObject.GetComponent<Rigidbody>().velocity = PlayerMovement[j];
                    //速度ベクトルが使えないためAddForceを代用
                    SelectPlayers[j].PlayerGameObject.GetComponent<Rigidbody>().AddForce(PlayerMovement[j]);
                    SelectPlayers[j].PlayerGameObject.GetComponent<Animator>().SetBool("Running", true);
                }
                else
                {
                    //速度ベクトルに適応
                    //SelectEnemys[j].PlayerGameObject.GetComponent<Rigidbody>().velocity = EnemyMovement[j];
                    //速度ベクトルが使えないためAddForceを代用
                    SelectEnemys[j].PlayerGameObject.GetComponent<Rigidbody>().AddForce(EnemyMovement[j]);
                    SelectEnemys[j].PlayerGameObject.GetComponent<Animator>().SetBool("Running", true);
                }

            }
        }

    }

    //真ん中のラインに到達した
    public void PhaseAttackfunc(int num)
    {
        //0,1,2ならPlayer
        //3,4,5ならEnemy

        //自機の攻撃
        if (0 <= num && num <= 2)
        {
            //選択したパーツナンバーを拾ってくる
            int SPart = GetSelectPartsNum(true, num);

            //自分選択したパーツのHPが0ではないことを確認する
            //そのパーツのHPが0なら
            if (GetSelectPartsHP(true, num, SPart) == 0)
            {
                //失敗
                //冷却に入る
                SelectPlayers[num].PlayerStatus.Cooling = true;
                //選んだパーツは破壊されている！
                StartCoroutine(AttacknonPart());
                //攻撃を終了する
                return;
            }

            //攻撃スキルで分岐
            string Skill = PartsList.GetCharaSkill(GetSelectPartsID(true, num), SPart);

            //ターゲットを格納
            int TargetEnemy = SelectPlayers[num].PlayerStatus.TargetEnemy;

            //ターゲットパーツ
            int TargetParts = SelectPlayers[num].PlayerStatus.TargetParts;

            //選択したパーツのスキルが
            //スキル　うつ　ねらいうち
            if ((Skill == "うつ") || (Skill == "ねらいうち"))
            {
                //ターゲットが存在するか確認する
                //存在しなければ
                if (SelectEnemys[TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                {
                    //失敗
                    //冷却に入る
                    SelectPlayers[num].PlayerStatus.Cooling = true;
                    //ターゲットは既に機能停止している！
                    StartCoroutine(AttacknonTarget());
                    //攻撃を終了する
                    return;
                }

                //狙ったパーツは既に破壊されているか
                //破壊されている場合別のパーツを選択し直す
                while (true)
                {
                    //攻撃対象は　TargetEnemy の TargetParts で、それが既に破壊されていたら
                    if (SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable == false)
                    {
                        //ランダムで選び直す
                        TargetParts = UnityEngine.Random.Range(0, 4);
                    }
                    //破壊されていないパーツだったら
                    else break;
                }

                //まもるスキルが発動しているか調べる
                for (int i = 0; i < 3; i++)
                {
                    //まもるスキルが発動しているなら
                    if (MamoruFlag[i + 3])
                    {
                        //まもるスキルを使った敵を対象にする
                        TargetEnemy = i;
                        //ターゲットパーツは相手が選択したパーツ
                        TargetParts = SelectEnemys[i].PlayerStatus.SelectPartnum;
                    }
                }

                //選び直したターゲットパーツを代入
                SelectPlayers[num].PlayerStatus.TargetParts = TargetParts;

                //ダメージ量を計算する
                int Damage = 0;

                //基本ダメージ
                Damage += PartsList.GetCharaPower(GetSelectPartsID(true, num), SPart);

                //脚部の射撃の得意さ
                Damage += PartsList.GetCharaShooting(GetSelectPlayerPartsID(true, num, 3), 3);

                Damage = Mathf.FloorToInt(Damage*DamagePower);
                //TargetEnemy の TargetParts の HPをへらす
                SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].HP -= Damage;

                //ゼロになる　もしくは　ゼロよりも小さくなったら  
                if (GetSelectPartsHP(false, TargetEnemy, TargetParts) <= 0)
                {
                    //ゼロにする
                    SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].HP = 0;
                    //パーツは使えない
                    //SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable = false;
                }

                //ダメージヒットの演出
                StartCoroutine(AttackPlayCol(num, TargetEnemy, TargetParts, Damage, PartsList.GetCharaAtMethod(GetSelectPartsID(true, num), SPart)));

                //冷却に入る
                SelectPlayers[num].PlayerStatus.Cooling = true;
            }
            //スキル　たすける
            else if (Skill == "たすける")
            {
                //未実装
                //たすけるスキルの演出
                StartCoroutine(AttackPlayColTasukeru(num, PartsList.GetCharaAtMethod(GetSelectPartsID(true, num), SPart)));

                //冷却に入る
                SelectPlayers[num].PlayerStatus.Cooling = true;
            }
            //スキル　まもる
            else if (Skill == "まもる")
            {
                //未実装
                //まもるスキルの演出
                StartCoroutine(AttackPlayColMamoru(num, PartsList.GetCharaAtMethod(GetSelectPartsID(true, num), SPart)));
                //まもる　スキル発動中のフラグ
                MamoruFlag[num] = true;
                //発動時間を設定
                MamoruStartTime[num] = Timer;
                MamoruEndTime[num] = Timer + 10f;
            }
            //スキル　なぐる　がむしゃら
            else if ((Skill == "なぐる") || (Skill == "がむしゃら"))
            {
                //ターゲットを決定する
                //ターゲットは一番近い敵
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

                //選んだターゲットを格納
                SelectPlayers[num].PlayerStatus.TargetEnemy = TargetEnemy;

                //狙ったパーツは既に破壊されているか
                //破壊されている場合別のパーツを選択し直す
                while (true)
                {
                    //攻撃対象は　TargetEnemy の TargetParts で、それが既に破壊されていたら
                    if (SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable == false)
                    {
                        //ランダムで選び直す
                        TargetParts = UnityEngine.Random.Range(0, 4);
                    }
                    //破壊されていないパーツだったら
                    else break;
                }

                //まもるスキルが発動しているか調べる
                for (int i = 0; i < 3; i++)
                {
                    //まもるスキルが発動しているなら
                    if (MamoruFlag[i + 3])
                    {
                        //まもるスキルを使った敵を対象にする
                        TargetEnemy = i;
                        //ターゲットパーツは相手が選択したパーツ
                        TargetParts = SelectEnemys[i].PlayerStatus.SelectPartnum;
                    }
                }

                //選び直したターゲットパーツを代入
                SelectPlayers[num].PlayerStatus.TargetParts = TargetParts;

                //ダメージ量を計算する
                int Damage = 0;

                //基本ダメージ
                Damage += PartsList.GetCharaPower(GetSelectPartsID(true, num), SPart);

                //脚部の格闘の得意さ
                Damage += PartsList.GetCharaFighting(GetSelectPlayerPartsID(true, num, 3), 3);
                
                Damage = Mathf.FloorToInt(Damage*DamagePower);

                //TargetEnemy の TargetParts の HPをへらす
                SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].HP -= Damage;

                //ゼロになる　もしくは　ゼロよりも小さくなったら  
                if (GetSelectPartsHP(false, TargetEnemy, TargetParts) <= 0)
                {
                    //ゼロにする
                    SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].HP = 0;
                    //パーツは使えない
                    //SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable = false;
                }
                //ダメージヒットの演出
                StartCoroutine(AttackPlayCol(num, TargetEnemy, TargetParts, Damage, PartsList.GetCharaAtMethod(GetSelectPartsID(true, num), SPart)));

                //冷却に入る
                SelectPlayers[num].PlayerStatus.Cooling = true;

            }

        }
        //敵の攻撃だったら
        else if (3 <= num && num <= 5)
        {
            //添字の調整
            num = num - 3;

            //選択したパーツナンバーを拾ってくる
            int SPart = GetSelectPartsNum(false, num);

            //自分選択したパーツのHPが0ではないことを確認する
            //そのパーツのHPが0なら
            if (GetSelectPartsHP(false, num, SPart) == 0)
            {
                //失敗
                //冷却に入る
                SelectEnemys[num].PlayerStatus.Cooling = true;
                //選んだパーツは破壊されている！
                StartCoroutine(AttacknonPart());
                //攻撃を終了する
                return;
            }

            //攻撃スキルで分岐
            string Skill = PartsList.GetCharaSkill(GetSelectPartsID(false, num), SPart);

            //ターゲットを格納
            int TargetEnemy = SelectEnemys[num].PlayerStatus.TargetEnemy;

            //ターゲットパーツ
            int TargetParts = SelectEnemys[num].PlayerStatus.TargetParts;

            //選択したパーツのスキルが
            //スキル　うつ　ねらいうち
            if ((Skill == "うつ") || (Skill == "ねらいうち"))
            {
                //ターゲットが存在するか確認する
                //存在しなければ
                if (SelectPlayers[TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                {
                    //失敗
                    //冷却に入る
                    SelectEnemys[num].PlayerStatus.Cooling = true;
                    //ターゲットは既に機能停止している！
                    StartCoroutine(AttacknonTarget());
                    //攻撃を終了する
                    return;
                }

                //狙ったパーツは既に破壊されているか
                //破壊されている場合別のパーツを選択し直す
                while (true)
                {
                    //攻撃対象は　TargetEnemy の TargetParts で、それが既に破壊されていたら
                    if (SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable == false)
                    {
                        //ランダムで選び直す
                        TargetParts = UnityEngine.Random.Range(0, 4);
                    }
                    //破壊されていないパーツだったら
                    else break;
                }

                //まもるスキルが発動しているか調べる
                for (int i = 0; i < 3; i++)
                {
                    //まもるスキルが発動しているなら
                    if (MamoruFlag[i])
                    {
                        //まもるスキルを使った敵を対象にする
                        TargetEnemy = i;
                        //ターゲットパーツは相手が選択したパーツ
                        TargetParts = SelectPlayers[i].PlayerStatus.SelectPartnum;
                    }
                }

                //選び直したターゲットパーツを代入
                SelectEnemys[num].PlayerStatus.TargetParts = TargetParts;

                //ダメージ量を計算する
                int Damage = 0;

                //基本ダメージ
                Damage += PartsList.GetCharaPower(GetSelectPartsID(false, num), SPart);

                //脚部の射撃の得意さ
                Damage += PartsList.GetCharaShooting(GetSelectPlayerPartsID(false, num, 3), 3);

                Damage = Mathf.FloorToInt(Damage*DamagePower);

                //TargetEnemy の TargetParts の HPをへらす
                SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].HP -= Damage;

                //ゼロになる　もしくは　ゼロよりも小さくなったら  
                if (GetSelectPartsHP(true, TargetEnemy, TargetParts) <= 0)
                {
                    //ゼロにする
                    SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].HP = 0;
                    //パーツは使えない
                    //SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable = false;
                }

                //ダメージヒットの演出
                StartCoroutine(DefencePlayCol(num, TargetEnemy, TargetParts, Damage, PartsList.GetCharaAtMethod(GetSelectPartsID(false, num), SPart)));

                //冷却に入る
                SelectEnemys[num].PlayerStatus.Cooling = true;
            }
            //スキル　たすける
            else if (Skill == "たすける")
            {
                //未実装
                //たすけるスキルの演出
                StartCoroutine(DefencePlayColTasukeru(num, PartsList.GetCharaAtMethod(GetSelectPartsID(false, num), SPart)));

                //冷却に入る
                SelectEnemys[num].PlayerStatus.Cooling = true;
            }
            //スキル　まもる
            else if (Skill == "まもる")
            {
                //未実装
                //まもるスキルの演出
                StartCoroutine(DefencePlayColMamoru(num, PartsList.GetCharaAtMethod(GetSelectPartsID(false, num), SPart)));
                //まもる　スキル発動中のフラグ
                MamoruFlag[num+3] = true;
                //発動時間を設定
                MamoruStartTime[num+3] = Timer;
                MamoruEndTime[num+3] = Timer + 10f;

            }
            //スキル　なぐる　がむしゃら
            else if ((Skill == "なぐる") || (Skill == "がむしゃら"))
            {
                //ターゲットを決定する
                //ターゲットは一番近い敵
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

                //まもるスキルが発動しているか調べる
                for (int i = 0; i < 3; i++)
                {
                    //まもるスキルが発動しているなら
                    if (MamoruFlag[i])
                    {
                        //まもるスキルを使った敵を対象にする
                        TargetEnemy = i;
                        //ターゲットパーツは相手が選択したパーツ
                        TargetParts = SelectPlayers[i].PlayerStatus.SelectPartnum;
                    }
                }

                //選んだターゲットを格納
                SelectEnemys[num].PlayerStatus.TargetEnemy = TargetEnemy;

                //狙ったパーツは既に破壊されているか
                //破壊されている場合別のパーツを選択し直す
                while (true)
                {
                    //攻撃対象は　TargetEnemy の TargetParts で、それが既に破壊されていたら
                    if (SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable == false)
                    {
                        //ランダムで選び直す
                        TargetParts = UnityEngine.Random.Range(0, 4);
                    }
                    //破壊されていないパーツだったら
                    else break;
                }

                //選び直したターゲットパーツを代入
                SelectEnemys[num].PlayerStatus.TargetParts = TargetParts;

                //ダメージ量を計算する
                int Damage = 0;

                //基本ダメージ
                Damage += PartsList.GetCharaPower(GetSelectPartsID(false, num), SPart);

                //脚部の格闘の得意さ
                Damage += PartsList.GetCharaFighting(GetSelectPlayerPartsID(false, num, 3), 3);

                Damage = Mathf.FloorToInt(Damage*DamagePower);

                //TargetEnemy の TargetParts の HPをへらす
                SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].HP -= Damage;

                //ゼロになる　もしくは　ゼロよりも小さくなったら  
                if (GetSelectPartsHP(true, TargetEnemy, TargetParts) <= 0)
                {
                    //ゼロにする
                    SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].HP = 0;
                    //パーツは使えない
                    //SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable = false;
                }
                //ダメージヒットの演出
                StartCoroutine(DefencePlayCol(num, TargetEnemy, TargetParts, Damage, PartsList.GetCharaAtMethod(GetSelectPartsID(false, num), SPart)));

                //冷却に入る
                SelectEnemys[num].PlayerStatus.Cooling = true;

            }
        }
    }

    IEnumerator AttackPlayCol(int playernum, int targetmeda, int targetpart, int damage, string atmethod)
    {
        //ターゲットを表示
        //傾きはx67.915
        Vector3 player = SelectPlayers[playernum].PlayerGameObject.transform.position;

        //カーソルの生成
        GameObject TargetCursor = Instantiate(TargetCursorBluePre);
        //位置の変更
        TargetCursor.transform.position = new Vector3(player.x, TargetCursor.transform.position.y, player.z);

        //敵の位置
        Vector3 enemy = SelectEnemys[targetmeda].PlayerGameObject.transform.position;

        //ターゲットの座標
        Vector3 TargetEnemypoti = new Vector3(enemy.x, TargetCursor.transform.position.y, enemy.z);

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
        for (int i = 0; i < 120; i++)
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
        //全てが終われば移動に戻る かつ 攻撃後のパーツHPが0ではない
        if (phase == Phase.Attack && SelectEnemys[targetmeda].PlayerStatus.Parts[targetpart].HP != 0)
            phase = Phase.Move;
        else if (SelectEnemys[targetmeda].PlayerStatus.Parts[targetpart].HP == 0)
            StartCoroutine(HPisZero(false, targetmeda, targetpart));
    }

    IEnumerator DefencePlayCol(int playernum, int targetmeda, int targetpart, int damage, string atmethod)
    {
        //ターゲットを表示
        //傾きはx67.915
        Vector3 enemy = SelectEnemys[playernum].PlayerGameObject.transform.position;

        //カーソルの生成
        GameObject TargetCursor = Instantiate(TargetCursorPinkPre);
        //位置の変更
        TargetCursor.transform.position = new Vector3(enemy.x, TargetCursor.transform.position.y, enemy.z);

        //敵の位置
        Vector3 player = SelectPlayers[targetmeda].PlayerGameObject.transform.position;

        //ターゲットの位置
        Vector3 TargetPlayerpoti = new Vector3(player.x, TargetCursor.transform.position.y, player.z);

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

        //全てが終われば移動に戻る かつ 攻撃後のパーツHPが0ではない
        if (phase == Phase.Defence && SelectPlayers[targetmeda].PlayerStatus.Parts[targetpart].HP != 0)
            phase = Phase.Move;
        else if (SelectPlayers[targetmeda].PlayerStatus.Parts[targetpart].HP == 0)
            StartCoroutine(HPisZero(true, targetmeda, targetpart));
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

        //存在するプレイヤー分
        for (int i = 0; i < selectedMedarotnum; i++)
        {
            //そのプレイヤーが存在すれば
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
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < selectedMedarotnum; j++)
            {
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
        if (phase == Phase.Attack)
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
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < selectedMedarotnum; j++)
            {
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
        if (phase == Phase.Attack)
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
        for (int i = 0; i < 120; i++)
        {
            for (int j = 0; j < maxEnemy; j++)
            {
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
        if (phase == Phase.Defence)
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
        if (phase == Phase.Defence)
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
        if (phase == Phase.Command1)
        {
            //自機のコマンド選択
            if (0 <= num && num <= 2)
            {
                selectMedarotnum = num;

                //攻撃対象の決定
                int TargetEnemy = UnityEngine.Random.Range(0, maxEnemy);
                int TargetParts = UnityEngine.Random.Range(0, 4);

                //ターゲットが存在するか確認する
                //存在しなければ
                while (true)
                {
                    if (SelectEnemys[TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                    {
                        //ランダムで選び直す
                        TargetEnemy = UnityEngine.Random.Range(0, maxEnemy);
                    }
                    else break;
                }

                SelectPlayers[num].PlayerStatus.TargetEnemy = TargetEnemy;

                //狙ったパーツは既に破壊されているか
                //破壊されている場合別のパーツを選択し直す
                while (true)
                {
                    //攻撃対象は　TargetEnemy の TargetParts で、それが既に破壊されていたら
                    if (SelectEnemys[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable == false)
                    {
                        //ランダムで選び直す
                        TargetParts = UnityEngine.Random.Range(0, 4);
                    }
                    //破壊されていないパーツだったら
                    else break;
                }

                SelectPlayers[num].PlayerStatus.TargetParts = TargetParts;

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
                SelHeadCommand.text  = PartsList.GetCharaPartName(GetSelectPlayerPartsID(true, num, 0), 0);
                SelRightCommand.text = PartsList.GetCharaPartName(GetSelectPlayerPartsID(true, num, 1), 1);
                SelLeftCommand.text  = PartsList.GetCharaPartName(GetSelectPlayerPartsID(true, num, 2), 2);

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
                int TargetEnemy = UnityEngine.Random.Range(0, selectedMedarotnum);
                int TargetParts = UnityEngine.Random.Range(0, 4);
                int SelectParts = UnityEngine.Random.Range(0, 3);

                //ターゲットが存在するか確認する
                //存在しなければ
                while (true)
                {
                    if (SelectPlayers[TargetEnemy].PlayerGameObject.activeInHierarchy == false)
                    {
                        //ランダムで選び直す
                        TargetEnemy = UnityEngine.Random.Range(0, selectedMedarotnum);
                    }
                    else break;
                }

                SelectEnemys[num].PlayerStatus.TargetEnemy = TargetEnemy;

                //狙ったパーツは既に破壊されているか
                //破壊されている場合別のパーツを選択し直す
                while (true)
                {
                    //攻撃対象は　TargetEnemy の TargetParts で、それが既に破壊されていたら
                    if (SelectPlayers[TargetEnemy].PlayerStatus.Parts[TargetParts].PartsAvailable == false)
                    {
                        //ランダムで選び直す
                        TargetParts = UnityEngine.Random.Range(0, 4);
                    }
                    //破壊されていないパーツだったら
                    else break;
                }

                SelectEnemys[num].PlayerStatus.TargetParts = TargetParts;

                //選んだパーツが既に破壊されていたら
                //破壊されている場合別のパーツを選択し直す
                while (true)
                {
                    //選択したパーツは SelectPartsで それが破壊されていれば
                    if (SelectEnemys[num].PlayerStatus.Parts[SelectParts].PartsAvailable == false)
                    {
                        //ランダムで選び直す
                        SelectParts = UnityEngine.Random.Range(0, 4);
                    }
                    //破壊されていないパーツだったら
                    else break;
                }

                SelectEnemys[num].PlayerStatus.SelectPartnum = SelectParts;

                SelectEnemys[num].PlayerStatus.Cooling = false;
                //フェーズを変更
                phase = Phase.Move;
            }
        }
    }

    //HPがゼロになったら
    IEnumerator HPisZero(bool playerflag, int Target, int TargetParts)
    {
        //パーツを使用できなくする
        //プレイヤーだったら
        if (playerflag)
        {
            SelectPlayers[Target].PlayerStatus.Parts[TargetParts].PartsAvailable = false;
        }
        //敵だったら
        else
        {
            SelectEnemys[Target].PlayerStatus.Parts[TargetParts].PartsAvailable = false;
        }

        //パーツ破壊メッセージ
        switch (TargetParts)
        {
            case 0:
                BattleMessage.text = "頭パーツ";
                break;
            case 1:
                BattleMessage.text = "右腕パーツ";
                break;
            case 2:
                BattleMessage.text = "左腕パーツ";
                break;
            case 3:
                BattleMessage.text = "脚部パーツ";
                break;
        }

        BattleMessage.text += "を破壊した！";

        yield return new WaitForSeconds(1f);

        if(TargetParts == 0)
        {
            BattleMessage.text = "機能停止！";
            if (playerflag)
            {
                //機能停止したら全てのパーツのHPを0にする
                for (int i = 0; i < 4; i++)
                {
                    hpbar.SetValue(Target, i, 0);
                }
                SelectPlayers[Target].PlayerGameObject.SetActive(false);
                //リーダーだったら
                if(Target == 0)
                {
                    //フェーズ移行
                    phase = GameController2.Phase.Defeat;
                    //音楽を止める
                    soundPlayer.StopBattleMusic();
                    //敗北BGM
                    soundPlayer.PlayLoseBGM();

                    //テキストを表示
                    ResultText.gameObject.SetActive(true);
                    ResultText.text = "LOSE";
                    yield break;
                }
            }
            else
            {
                //機能停止したら全てのパーツのHPを0にする
                for (int i = 0; i < 4; i++)
                {
                    hpbar.SetValue(Target + 3, i, 0);
                }

                SelectEnemys[Target].PlayerGameObject.SetActive(false);
                //リーダーだったら
                if(Target == 0)
                {

                    //フェーズ移行
                    phase = GameController2.Phase.Winning;
                    //音楽を止める
                    soundPlayer.StopBattleMusic();
                    //勝利BGM
                    soundPlayer.PlayWinBGM();

                    //テキスト表示
                    ResultText.gameObject.SetActive(true);
                    ResultText.text = "WIN";
                    yield break;
                }
            }
        }

        yield return new WaitForSeconds(1f);
        BattleMessage.text = "";
        phase = Phase.Move;
    }

}