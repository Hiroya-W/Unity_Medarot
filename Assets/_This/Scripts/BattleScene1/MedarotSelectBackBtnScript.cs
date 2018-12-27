using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedarotSelectBackBtnScript : MonoBehaviour {

    //GameController1の参照
    public GameController1 gameController;

    //MedarotSelectの参照
    //RobottleInfoBattleReadyの参照
    public GameObject MedarotSelect;
    public GameObject RobottleInfoBattleReady;

    //戻るボタンの動作はメダロット選択中とそうでないときで分岐
    public void BackScript()
    {
        //選択中である時
        if (gameController.isselecting)
        {
            //関数を呼び出す
            gameController.MedarotSelectReturn();
        }
        //選択中でない時　メダロットが一体も選ばれていない時
        else
        {
            //MedarotSelectをSetActive(false)
            //RobottleInfoBattleReadyをSetActive(true)
            MedarotSelect.SetActive(false);
            RobottleInfoBattleReady.SetActive(true);

        }
    }
}
