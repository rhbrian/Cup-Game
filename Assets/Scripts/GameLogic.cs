using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameLogic : NetworkBehaviour
{
    // add a header
    // synclists for tracking networked objects
    public SyncList<GameObject> gamePlayers = new SyncList<GameObject>();
    public SyncList<GameObject> chairs = new SyncList<GameObject>();
    public SyncList<GameObject> bowls = new SyncList<GameObject>();
    public SyncList<GameObject> balls = new SyncList<GameObject>();

    // gameplay specific vars
    [SyncVar] public int numPlayers = 0;
    [SyncVar] public int numActivePlayers = 0;
    private bool turnOpen = true;
    private bool reverse = false;

    // [SyncVar] public int turn = 0;
    public int turn = 0;

    // keeping track of time variables
    float turnTimeCap = 10f;
    float turnStartTime = 0f;
    float currTime = 0f;

    [SyncVar (hook = nameof(PairItems))] public bool startGame = false;
    bool setStartDone = false;
    [SerializeField] GameObject ChoiceUI;
    
    // called on adding a player to the game setting
    public void AddPlayer(GameObject gamePlayer, GameObject chair, GameObject bowl, GameObject ball) {
        gamePlayers.Add(gamePlayer);
        chairs.Add(chair);
        bowls.Add(bowl);
        balls.Add(ball);
    }

    // pairing serializable game objects
    public void PairItems(bool oldVal, bool newVal) {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        for (int x = 0; x < numPlayers; x++) {
            gamePlayers[x].GetComponent<Player>().bowl = bowls[x];
            // gamePlayers[x].GetComponent<Player>().chair = chairs[x];
            //chairs[x].GetComponent<SitOnChair>().Setup(gamePlayers[x]);
            gamePlayers[x].GetComponent<Player>().ball = balls[x];
        }
    }

    // helper function to identify the player the bowl belongs to
    public GameObject getPlayerFromBowl(GameObject scoredBowl) {
        for (int x = 0; x < bowls.Count; x++) {
            if (bowls[x] == scoredBowl) return gamePlayers[x];
        }
        return null;
    }

    // helper function to identify the player the ball belongs to
    public GameObject getPlayerFromBall(GameObject ball) {
        for (int x = 0; x < balls.Count; x++) {
            if (balls[x] == ball) return gamePlayers[x];
        }
        return null;
    }

    // scaling bowl based on params
    public void ScaleBowl(bool UpScale, GameObject player, int scale) {
        GameObject bowl = player.GetComponent<Player>().bowl;
        Vector3 bowlSize = bowl.transform.localScale;
        if ((!UpScale && scale == 1 && (bowlSize.x - .0006) < .0011f) || (!UpScale && scale == 2 && (bowlSize.x - .0012) < .0011f)) {
            PlayerEscaped(bowl);
        }
        else {
            bowl.GetComponent<Bowl>().CmdSizeBowl(UpScale, scale);
        }
        /*int result = bowl.GetComponent<Bowl>().CmdSizeBowl(UpScale, scale);
        if (result == 0) {
            PlayerEscaped(bowl);
        }*/
    }

    // if need to scale all the bowls at once
    public void ScaleAll(GameObject localPlayer) {
        foreach (var player in gamePlayers) {
            if (localPlayer == player) continue;
            ScaleBowl(true, player, 1);
        }
    }

    // called on player achieving self-win condition
    public void PlayerEscaped(GameObject bowl) {
        Debug.Log("bowl exists in the list " + bowls.Contains(bowl));
        GameObject player = getPlayerFromBowl(bowl);
        gamePlayers.Remove(player);
        bowls.Remove(bowl);
        numActivePlayers--;
        NetworkServer.Destroy(bowl);
        GameObject.Find("-- UI").transform.Find("GamePlayerUI").transform.Find("Win/Lose").transform.Find("Escape").gameObject.SetActive(true);
    }

    // get the player who scored and player who got scored on here, computer choices
    // if scored in normal bowl/cup then 2 choices :
        // 1 -- make my cup smaller by 1 size
        // 2 -- make my cup larger by 1 size
        // 3 -- make the opponent who I scored on increase bowl/cup by 1 size
    // if scored in middle cup then 4 choices :
        // 1 -- make my cup smaller by 2 sizes
        // 2 -- make my cup larger by 2 sizes
        // 3 -- make all other players' cup larger by 1 size
        // 4 -- reverse the order of play
        // 5 -- swap my cup with someone elses (in this case just swap sizes)
    public void computeChoices(bool isCenter, GameObject shootingPlayer, GameObject scoredOnPlayer) {
        if (!ChoiceUI.GetComponent<UIChoices>().gmSet) {
            ChoiceUI.GetComponent<UIChoices>().GameManager = this.gameObject;
            ChoiceUI.GetComponent<UIChoices>().gmSet = true;
        }
        if (shootingPlayer != gamePlayers[turn]) return;
        if (isCenter) {
            ChoiceUI.GetComponent<UIChoices>().SetChoices(true, shootingPlayer, scoredOnPlayer);
        }
        else if (!isCenter && (shootingPlayer == scoredOnPlayer)) {
            ScaleBowl(true, shootingPlayer, 1);
        }
        else {
            ChoiceUI.GetComponent<UIChoices>().SetChoices(false, shootingPlayer, scoredOnPlayer);
        }
    }

    // set turn start time to current time
    public void setStartTime(float time) {
        turnStartTime = time;
        setStartDone = true;
        gamePlayers[turn].GetComponent<Player>().StartTurn();
    }

    // turn is passed and need to reset turn time again
    public void PassTurn() {
        Debug.Log("Turn = " + turn);
        gamePlayers[turn].GetComponent<Player>().StopTurn();
        if (!reverse) {
            turn++;
            if (turn >= numActivePlayers) {
                turn = 0;
            }
        }
        else {
            turn--;
            if (turn < 0) {
                turn = numActivePlayers - 1;
            }
        }
        setStartDone = false;
    }

    // reverse the order of play
    public void ReverseOrder() {
        if (reverse) {
            reverse = false;
        }
        else {
            reverse = true;
        }
    }

    // implement end game functionality ##
    public void EndGame() {
        //gamePlayers
    }

    // swap bowl sizes
    public void SwapBowls(GameObject player1, GameObject player2) {
        Vector3 swap = player1.GetComponent<Player>().bowl.transform.localScale;
        player1.GetComponent<Player>().bowl.transform.localScale = player2.GetComponent<Player>().bowl.transform.localScale;
        player2.GetComponent<Player>().bowl.transform.localScale = swap;
    }

    // end the game when only 1 player is left and all other player cups have disappeared
    void Update() {
        currTime += Time.deltaTime;
        if (ChoiceUI == null) {
            ChoiceUI = GameObject.Find("-- UI").transform.Find("GamePlayerUI")
            .transform.Find("ChoicePanel").transform.Find("BluePanel").transform.Find("InnerChoicePanel").gameObject;
        }
        if (startGame) {
            if (numActivePlayers == 1) {
                EndGame();
            }
            if (!ChoiceUI.GetComponent<UIChoices>().playerSet) {
                ChoiceUI.GetComponent<UIChoices>().setLocalPlayer(gamePlayers);
            }
            if (currTime - turnStartTime > turnTimeCap) {
                PassTurn();
                turnOpen = true;
            }
            if (!setStartDone) {
                setStartTime(currTime);   
            }
        }
    }
}
