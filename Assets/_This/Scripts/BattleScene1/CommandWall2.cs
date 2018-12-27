using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandWall2 : MonoBehaviour
{

    public GameController2 gameController;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //物体の衝突があった時
    void OnTriggerEnter(Collider collider)
    {
        //移動を止める
        for (int i = 0; i < gameController.selectedMedarotnum; i++)
        {
            //SelectPlayers[i].PlayerGameObject.transform.position -= PlayerMovement[i];
            gameController.SelectPlayers[i].PlayerGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameController.SelectPlayers[i].PlayerGameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            gameController.SelectPlayers[i].PlayerGameObject.GetComponent<Animator>().SetBool("Running", false);
        }

        //敵
        for (int i = 0; i < gameController.SelectEnemys.Length; i++)
        {
            //SelectEnemys[i].PlayerGameObject.transform.position -= EnemyMovement[i];
            gameController.SelectEnemys[i].PlayerGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameController.SelectEnemys[i].PlayerGameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            gameController.SelectEnemys[i].PlayerGameObject.GetComponent<Animator>().SetBool("Running", false);
        }


        //それのタグがプレイヤーだったら
        if (collider.gameObject.CompareTag("Player1"))
        {
            //フェーズをAttackへ移行
            gameController.phase = GameController2.Phase.Command1;
            gameController.PhaseCommandfunc(0);
        }
        else if (collider.gameObject.CompareTag("Player2"))
        {
            //フェーズをAttackへ移行
            gameController.phase = GameController2.Phase.Command1;
            gameController.PhaseCommandfunc(1);
        }
        else if (collider.gameObject.CompareTag("Player3"))
        {
            //フェーズをAttackへ移行
            gameController.phase = GameController2.Phase.Command1;
            gameController.PhaseCommandfunc(2);
        }

        //それのタグがエネミーだったら
        if (collider.gameObject.CompareTag("Enemy1"))
        {
            //フェーズをDefenceへ移行
            gameController.phase = GameController2.Phase.Command1;
            gameController.PhaseCommandfunc(3);
        }
        else if (collider.gameObject.CompareTag("Enemy2"))
        {
            //フェーズをDefenceへ移行
            gameController.phase = GameController2.Phase.Command1;
            gameController.PhaseCommandfunc(4);
        }
        else if (collider.gameObject.CompareTag("Enemy3"))
        {
            //フェーズをDefenceへ移行
            gameController.phase = GameController2.Phase.Command1;
            gameController.PhaseCommandfunc(5);
        }
    }
}
