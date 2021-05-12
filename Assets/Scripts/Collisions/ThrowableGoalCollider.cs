using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableGoalCollider : MonoBehaviour
{
    [SerializeField] GameObject ball;
    [SerializeField] GameObject player;
    [SerializeField] GameObject gameManager;
    int x = 0;
    bool isColliding;

    void Start() {
        // ball = gameObject.transform.parent.gameObject;
        ball = gameObject;
        gameManager = GameObject.FindWithTag("GameManager");
        player = gameManager.GetComponent<GameLogic>().getPlayerFromBall(ball);
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Goal") {
            if (isColliding) return;
            isColliding = true;
            Debug.Log(x++);
            Debug.Log("Goal!");
            GameObject scoredOnPlayer = gameManager.GetComponent<GameLogic>().getPlayerFromBowl(col.gameObject);
            // check which bowl it is and send that info to the gameManager
            gameManager.GetComponent<GameLogic>().computeChoices(false, player, scoredOnPlayer);
        }
        else if (col.gameObject.tag == "Center") {
            if (isColliding) return;
            isColliding = true;
            Debug.Log("Goal in Center!" + x++);
            GameObject scoredOnPlayer = gameManager.GetComponent<GameLogic>().getPlayerFromBowl(col.gameObject);
            // check which bowl it is and send that info to the gameManager
            gameManager.GetComponent<GameLogic>().computeChoices(true, player, scoredOnPlayer);
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Goal") {
            isColliding = false;
        }
        else if (col.gameObject.tag == "Center") {
            isColliding = false;
        }
    }
}
