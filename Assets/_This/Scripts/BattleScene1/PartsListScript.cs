using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsListScript : MonoBehaviour {
    [System.Serializable]
    public struct _PartsList
    {
        public string PartName;
        public int Armor;
        //頭パーツ、右腕、左腕パーツ
        public int Success;
        public int Power;
        public int Loading;
        public int Cooling;
        public string Skill;
        public string AtMethod;
        //頭パーツのみ回数がある
        public int HeadCound;
        //脚部パーツ
        public int Moving;
        public int Avoidance;
        public int Defence;
        public int Fighting;
        public int Shooting;
        public string Type;
    }
    [System.Serializable]
    public struct _CharaList
    {
        public string CharaName;
        public int ID;
        public _PartsList[] PartsList;
    }
    public _CharaList[] CharaList;

    //メダロットの名前を取得
    public void GetCharaStatusName( string Target, int CharaListNum )
    {
        Target = CharaList[CharaListNum].CharaName;
    }
    //メダロットのIDを取得
    public void GetCharaStatusID( int Target, int CharaListNum)
    {
        Target = CharaList[CharaListNum].ID;
    }

    //初期設定
    void Awake()
    {
        CharaList = new _CharaList[4];

        //ID1 メタビー
        CharaList[1].CharaName = "メタビー";
        CharaList[1].ID = 1;
        CharaList[1].PartsList = new _PartsList[4];
        //頭パーツ
        CharaList[1].PartsList[0].PartName = "メタミサイル";
        CharaList[1].PartsList[0].Armor = 140;
        CharaList[1].PartsList[0].Success = 24;
        CharaList[1].PartsList[0].Power = 49;
        CharaList[1].PartsList[0].Loading = 27;
        CharaList[1].PartsList[0].Cooling = 19;
        CharaList[1].PartsList[0].HeadCound = 6;
        CharaList[1].PartsList[0].Skill = "うつ";
        CharaList[1].PartsList[0].AtMethod = "ミサイル";
        //右腕パーツ
        CharaList[1].PartsList[1].PartName = "メタリボルバー";
        CharaList[1].PartsList[1].Armor = 140;
        CharaList[1].PartsList[1].Success = 41;
        CharaList[1].PartsList[1].Power = 27;
        CharaList[1].PartsList[1].Loading = 29;
        CharaList[1].PartsList[1].Cooling = 23;
        CharaList[1].PartsList[1].Skill = "ねらいうち";
        CharaList[1].PartsList[1].AtMethod = "ライフル";
        //左腕パーツ
        CharaList[1].PartsList[2].PartName = "メタマシンガン";
        CharaList[1].PartsList[2].Armor = 125;
        CharaList[1].PartsList[2].Success = 32;
        CharaList[1].PartsList[2].Power = 18;
        CharaList[1].PartsList[2].Loading = 40;
        CharaList[1].PartsList[2].Cooling = 23;
        CharaList[1].PartsList[2].Skill = "うつ";
        CharaList[1].PartsList[2].AtMethod = "ガトリング";
        //脚部パーツ
        CharaList[1].PartsList[3].PartName = "オチツカー";
        CharaList[1].PartsList[3].Armor = 130;
        CharaList[1].PartsList[3].Moving = 16;
        CharaList[1].PartsList[3].Avoidance = 18;
        CharaList[1].PartsList[3].Defence = 26;
        CharaList[1].PartsList[3].Fighting = 15;
        CharaList[1].PartsList[3].Shooting = 30;
        CharaList[1].PartsList[3].Skill = "";
        CharaList[1].PartsList[3].Type = "にきゃく";

        //ID2 ロクショウ
        CharaList[2].CharaName = "ロクショウ";
        CharaList[2].ID = 2;
        CharaList[2].PartsList = new _PartsList[4];
        //頭パーツ
        CharaList[2].PartsList[0].PartName = "アンテナ";
        CharaList[2].PartsList[0].Armor = 100;
        CharaList[2].PartsList[0].Success = 44;
        CharaList[2].PartsList[0].Power = 33;
        CharaList[2].PartsList[0].Loading = 46;
        CharaList[2].PartsList[0].Cooling = 31;
        CharaList[2].PartsList[0].HeadCound = 4;
        CharaList[2].PartsList[0].Skill = "たすける";
        CharaList[2].PartsList[0].AtMethod = "レーダーサイト";
        //右腕パーツ
        CharaList[2].PartsList[1].PartName = "チャンバラソード";
        CharaList[2].PartsList[1].Armor = 140;
        CharaList[2].PartsList[1].Success = 36;
        CharaList[2].PartsList[1].Power = 22;
        CharaList[2].PartsList[1].Loading = 40;
        CharaList[2].PartsList[1].Cooling = 27;
        CharaList[2].PartsList[1].Skill = "なぐる";
        CharaList[2].PartsList[1].AtMethod = "ソード";
        //左腕パーツ
        CharaList[2].PartsList[2].PartName = "ピコペコハンマー";
        CharaList[2].PartsList[2].Armor = 180;
        CharaList[2].PartsList[2].Success = 13;
        CharaList[2].PartsList[2].Power = 44;
        CharaList[2].PartsList[2].Loading = 17;
        CharaList[2].PartsList[2].Cooling = 3;
        CharaList[2].PartsList[2].Skill = "がむしゃら";
        CharaList[2].PartsList[2].AtMethod = "ハンマー";
        //脚部パーツ
        CharaList[2].PartsList[3].PartName = "タタッカー";
        CharaList[2].PartsList[3].Armor = 110;
        CharaList[2].PartsList[3].Moving = 25;
        CharaList[2].PartsList[3].Avoidance = 26;
        CharaList[2].PartsList[3].Defence = 21;
        CharaList[2].PartsList[3].Fighting = 29;
        CharaList[2].PartsList[3].Shooting = 8;
        CharaList[2].PartsList[3].Skill = "";
        CharaList[2].PartsList[3].Type = "にきゃく";

        //ID3 ナイトアーマー
        CharaList[3].CharaName = "ナイトアーマー";
        CharaList[3].ID = 3;
        CharaList[3].PartsList = new _PartsList[4];
        //頭パーツ
        CharaList[3].PartsList[0].PartName = "クリアシールド";
        CharaList[3].PartsList[0].Armor = 255;
        CharaList[3].PartsList[0].Success = 24;
        CharaList[3].PartsList[0].Power = 18;
        CharaList[3].PartsList[0].Loading = 56;
        CharaList[3].PartsList[0].Cooling = 23;
        CharaList[3].PartsList[0].HeadCound = 6;
        CharaList[3].PartsList[0].Skill = "まもる";
        CharaList[3].PartsList[0].AtMethod = "ガード";
        //右腕パーツ
        CharaList[3].PartsList[1].PartName = "ナイトシールド";
        CharaList[3].PartsList[1].Armor = 265;
        CharaList[3].PartsList[1].Success = 24;
        CharaList[3].PartsList[1].Power = 20;
        CharaList[3].PartsList[1].Loading = 30;
        CharaList[3].PartsList[1].Cooling = 10;
        CharaList[3].PartsList[1].Skill = "まもる";
        CharaList[3].PartsList[1].AtMethod = "ガード";
        //左腕パーツ
        CharaList[3].PartsList[2].PartName = "グレートシールド";
        CharaList[3].PartsList[2].Armor = 265;
        CharaList[3].PartsList[2].Success = 24;
        CharaList[3].PartsList[2].Power = 20;
        CharaList[3].PartsList[2].Loading = 30;
        CharaList[3].PartsList[2].Cooling = 10;
        CharaList[3].PartsList[2].Skill = "まもる";
        CharaList[3].PartsList[2].AtMethod = "ガード";
        //脚部パーツ
        CharaList[3].PartsList[3].PartName = "トロイモクバ";
        CharaList[3].PartsList[3].Armor = 265;
        CharaList[3].PartsList[3].Moving = 18;
        CharaList[3].PartsList[3].Avoidance = 10;
        CharaList[3].PartsList[3].Defence = 10;
        CharaList[3].PartsList[3].Fighting = 8;
        CharaList[3].PartsList[3].Shooting = 32;
        CharaList[3].PartsList[3].Skill = "";
        CharaList[3].PartsList[3].Type = "しゃりょう";

    }

    //キャラの名前を拾ってくる
    public string GetCharaName( int CharaID )
    {
        //キャラの名前
        string CharaName;

        CharaName = CharaList[CharaID].CharaName;

        return CharaName;
    }
    
    //キャラのIDを拾ってくる
    public int GetCharaID( int CharaID )
    {
        //キャラのID
        int charaID;

        charaID = CharaList[CharaID].ID;

        return charaID;
    }

    //キャラのパーツ名を拾ってくる
    public string GetCharaPartName( int CharaID, int PartsNum )
    {
        //キャラのパーツの名前
        string CharaPartName;

        CharaPartName = CharaList[CharaID].PartsList[PartsNum].PartName;

        return CharaPartName;
    }

    //キャラのパーツの装甲値を拾ってくる
    public int GetCharaArmor(int CharaID, int PartsNum)
    {
        //キャラの装甲値
        int CharaArmor;

        CharaArmor = CharaList[CharaID].PartsList[PartsNum].Armor;

        return CharaArmor;
    }

    //キャラのパーツの成功性能を拾ってくる
    public int GetCharaSuccess(int CharaID, int PartsNum)
    {
        //キャラの成功性能
        int CharaSuccess;

        CharaSuccess = CharaList[CharaID].PartsList[PartsNum].Success;

        return CharaSuccess;
    }

    //キャラのパーツの威力を拾ってくる
    public int GetCharaPower(int CharaID, int PartsNum)
    {
        //キャラの威力
        int CharaPower;

        CharaPower = CharaList[CharaID].PartsList[PartsNum].Power;

        return CharaPower;
    }

    //キャラのパーツの充填性能を拾ってくる
    public int GetCharaLoading(int CharaID, int PartsNum)
    {
        //キャラの充填
        int CharaLoading;

        CharaLoading = CharaList[CharaID].PartsList[PartsNum].Loading;

        return CharaLoading;
    }

    //キャラのパーツの冷却性能を拾ってくる
    public int GetCharaCooling(int CharaID, int PartsNum)
    {
        //キャラの冷却
        int CharaCooling;

        CharaCooling = CharaList[CharaID].PartsList[PartsNum].Loading;

        return CharaCooling;
    }

    //キャラのパーツのスキルを拾ってくる
    public string GetCharaSkill(int CharaID, int PartsNum)
    {
        //キャラのパーツの名前
        string CharaSkill;

        CharaSkill = CharaList[CharaID].PartsList[PartsNum].Skill;

        return CharaSkill;
    }

    //キャラのパーツの攻撃方法を拾ってくる
    public string GetCharaAtMethod(int CharaID, int PartsNum)
    {
        //キャラのパーツの攻撃方法
        string CharaAtMethod;

        CharaAtMethod = CharaList[CharaID].PartsList[PartsNum].AtMethod;

        return CharaAtMethod;
    }

    //キャラのパーツの移動性能を拾ってくる
    public int GetCharaMoving(int CharaID, int PartsNum)
    {
        //キャラの移動性能
        int CharaMoving;

        CharaMoving = CharaList[CharaID].PartsList[PartsNum].Moving;

        return CharaMoving;
    }

    //キャラのパーツの回避性能を拾ってくる
    public int GetCharaAvoid(int CharaID, int PartsNum)
    {
        //キャラの回避性能
        int CharaAvoid;

        CharaAvoid = CharaList[CharaID].PartsList[PartsNum].Avoidance;

        return CharaAvoid;
    }

    //キャラのパーツの防御性能を拾ってくる
    public int GetCharaDefence(int CharaID, int PartsNum)
    {
        //キャラの防御性能
        int CharaDefence;

        CharaDefence = CharaList[CharaID].PartsList[PartsNum].Defence;

        return CharaDefence;
    }

    //キャラのパーツの格闘性能を拾ってくる
    public int GetCharaFighting(int CharaID, int PartsNum)
    {
        //キャラの防御性能
        int CharaFighting;

        CharaFighting = CharaList[CharaID].PartsList[PartsNum].Fighting;

        return CharaFighting;
    }

    //キャラのパーツの射撃性能を拾ってくる
    public int GetCharaShooting(int CharaID, int PartsNum)
    {
        //キャラの防御性能
        int CharaShooting;

        CharaShooting = CharaList[CharaID].PartsList[PartsNum].Shooting;

        return CharaShooting;
    }

    //キャラのパーツの脚部タイプを拾ってくる
    public string GetCharaType(int CharaID, int PartsNum)
    {
        //キャラの防御性能
        string CharaType;

        CharaType = CharaList[CharaID].PartsList[PartsNum].Type;

        return CharaType;
    }
}
