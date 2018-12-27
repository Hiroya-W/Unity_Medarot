using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandWall : MonoBehaviour {

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
            gameController.player[0].Charge = true;
        }
        //それのタグがエネミーだったら
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameController.enemy[0].Charge = true;
        }
    }
}
