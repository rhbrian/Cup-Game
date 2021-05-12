using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyPlayer : NetworkBehaviour
{
    public static LobbyPlayer localPlayer;  // local player instance
    NetworkMatchChecker networkMatchChecker; // match checker
    [SyncVar] public string matchID;    // the unique match ID to identify each game match
    [SyncVar] public int playerIndex;   // keeps track of number of players and also playerID for all clients + host

    void Start()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
        if (isLocalPlayer) {
            localPlayer = this;
        }
        else{
            // not local player joins lobby triggers ui player spawn
            UILobby.instance.SpawnPlayerUIPrefab(this);
        }
    }

    // --- Hosting a match --- //

    // make id and try to host a game with that id
    public void HostGame() {
        // making a random id for lobby
        string ID = MatchMaker.GetRandomMatchID();
        CmdHostGame(ID);
    }

    // hosts a instance of a game match with a match ID tied to it
    [Command]
    void CmdHostGame(string id) {
        matchID = id;
        // register a new game and add player to that list
        if (MatchMaker.instance.HostGame(id, gameObject, out playerIndex)) {
            Debug.Log("Game hosted!");
            networkMatchChecker.matchId = id.ToGuid();
            TargetHostGame(true, id, playerIndex);
        }
        else {
            Debug.Log("Game host failed!");
            TargetHostGame(false, id, playerIndex);
        }
    }

    // called to run functionality for either successful or unsuccessful hosting
    [TargetRpc]
    void TargetHostGame(bool success, string _matchID, int playerIndex) {
        this.playerIndex = playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID : {matchID} == {_matchID}");
        UILobby.instance.HostSuccess(success, _matchID);
    }

    // --- Joining a match --- //

    // trying to join game with this ID
    public void JoinGame(string id) {
        CmdJoinGame(id);
    }

    // client uses matchmaker to check if joining this matchID is possible
    [Command]
    void CmdJoinGame(string id) {
        matchID = id;
        // register a new game and add player to that list
        if (MatchMaker.instance.JoinGame(id, gameObject, out playerIndex)) {
            Debug.Log("Game joined!");
            networkMatchChecker.matchId = id.ToGuid();
            TargetJoinGame(true, id, playerIndex);
        }
        else {
            Debug.Log("Game join failed!");
            TargetJoinGame(false, id, playerIndex);
        }
    }

    // run functionality for successful/unsuccessful join
    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID, int playerIndex) {
        this.playerIndex = playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID : {matchID} == {_matchID}");
        UILobby.instance.JoinSuccess(success, matchID);
    }

    // -- Begin a game --- //
    public void InitiateGame() {
        CmdInitGame();
    }

    [Command]
    void CmdInitGame() {
        MatchMaker.instance.StartGame(matchID);
        Debug.Log("Game Initiated!");
    }

    public void StartGame() {
        CmdStartGame();
    }

    // 
    [Command]
    void CmdStartGame() {
        Debug.Log("Game Started!");
        RpcStartGame();
    }

    [ClientRpc]
    void RpcStartGame() {
        Debug.Log($"MatchID : {matchID} | Starting");
        UILobby.instance.ActivateGameUI();
    }
}
