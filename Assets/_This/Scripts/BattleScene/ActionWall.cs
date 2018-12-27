using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionWall : MonoBehaviour {

    //GameControllerの参照
    public GameController gameController;

	void Start () {
		
	}
	
	void Update () {
		
	}

    //物体の衝突があった時
    void OnCollisionEnter(Collision collision)
    {
        //それのタグがプレイヤーだったら
        if (collision.gameObject.CompareTag("Player"))
        {
            //フェーズをAttackへ移行
            gameController.PhasetoAttack();
        }
        //それのタグがエネミーだったら
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //フェーズをDefenceへ移行
            gameController.PhasetoDefence();
        }
    }

    public void funcplusdelay()
    {
        Coroutine coroutine = StartCoroutine(DelayMethod(0.5f, () => {
            //動作
        }));
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}

