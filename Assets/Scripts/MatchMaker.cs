using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

[System.Serializable]
public class Match{
    public string matchID;
    public SyncListGameObject players = new SyncListGameObject();
    public Match(string matchID, GameObject player) {
        this.matchID = matchID;
        players.Add(player);
    }

    public Match() {}
}

// so its networkable
[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> {}

[System.Serializable]
public class SyncListMatch : SyncList<Match> {}

public class MatchMaker : NetworkBehaviour
{
    public static MatchMaker instance;
    public SyncListMatch matches = new SyncListMatch();
    public SyncList<string> matchIDs = new SyncList<string>();
    [SerializeField] GameObject gameManagerPrefab;

    public List<Transform> playerSpawnSpots;
    public List<Transform> chairSpawnSpots;
    public List<Transform> bowlSpots;

    public GameObject gamePlayerPrefab;
    public GameObject chairPrefab;
    public GameObject bowlPrefab;
    public GameObject ballPrefab;

    void Start() {
        instance = this;
    }

    // try to make a game with this matchID if same id doesn't exist
    public bool HostGame(string matchID, GameObject player, out int playerIndex) {
        playerIndex = -1;
        if (!matchIDs.Contains(matchID)) {
            matches.Add(new Match(matchID, player));
            matchIDs.Add(matchID);
            Debug.Log("Match made");
            playerIndex = 1;
            return true;
        }
        else {
            Debug.Log("Match ID already exists");
            return false;
        }
    }

    // try to join the match at that matchID if exists
    public bool JoinGame(string matchID, GameObject player, out int playerIndex) {
        playerIndex = -1;
        if (matchIDs.Contains(matchID)) {
            for (int x = 0; x < matches.Count; x++) {
                if (matches[x].matchID == matchID) {
                    matches[x].players.Add(player);
                    playerIndex = matches[x].players.Count;
                    break;
                }
            }
            Debug.Log("Match joined");
            return true;
        }
        else {
            Debug.Log("Match ID does not exist");
            return false;
        }
    }

    public void StartGame(string _matchID) {
        GameObject gameManager = Instantiate(gameManagerPrefab);
        NetworkServer.Spawn(gameManager);
        // this turn manager only applies to players with this match id
        gameManager.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();

        int inc = 0;
        int y = 0;
        Quaternion quat = Quaternion.Euler(0,0,0);

        for (int x = 0; x < matches.Count; x++) {
            if (matches[x].matchID == _matchID) {
                int numP = matches[x].players.Count;
                inc = 12 / (numP);
                foreach (var player in matches[x].players) {
                    LobbyPlayer _player = player.GetComponent<LobbyPlayer>();

                    // spawn ingame player
                    GameObject gamePlayer = Instantiate(gamePlayerPrefab, playerSpawnSpots[y].position, playerSpawnSpots[y].rotation);
                    NetworkServer.Spawn(gamePlayer, _player.gameObject);
                    // spawn chair
                    //GameObject chair = Instantiate(chairPrefab, chairSpawnSpots[y].position, chairSpawnSpots[y].rotation);
                    //NetworkServer.Spawn(chair, _player.gameObject);
                    // spawn bowl
                    GameObject bowl = Instantiate(bowlPrefab, bowlSpots[y].position, bowlSpots[y].rotation);
                    NetworkServer.Spawn(bowl, _player.gameObject);
                    // spawn ball
                    GameObject ball = Instantiate(ballPrefab, bowlSpots[y].position, quat);
                    NetworkServer.Spawn(ball, _player.gameObject);

                    gamePlayer.GetComponent<Player>().playerID = _player.playerIndex;
                    //gamePlayer.GetComponent<Player>().chair = chair;
                    gamePlayer.GetComponent<Player>().bowl = bowl;
                    gamePlayer.GetComponent<Player>().ball = ball;

                    // gamePlayer.GetComponent<Player>().RpcPairObjects(chair, bowl, ball);
                    gamePlayer.GetComponent<Player>().RpcPairObjects(null, bowl, ball);

                    //gameManager.GetComponent<GameLogic>().AddPlayer(gamePlayer, chair, bowl, ball);
                    gameManager.GetComponent<GameLogic>().AddPlayer(gamePlayer, null, bowl, ball);
                    gameManager.GetComponent<GameLogic>().numPlayers = numP;
                    gameManager.GetComponent<GameLogic>().numActivePlayers = numP;

                    y += inc;
                    
                    _player.StartGame();
                }
            }
        }
        gameManager.GetComponent<GameLogic>().startGame = true;
    }

    // makes a random matchID as ordered by LobbyPlayer when trying to host a game
    public static string GetRandomMatchID() {
        string id = string.Empty;
        for (int x = 0; x < 5; x++) {
            // numbers and letters
            int r = UnityEngine.Random.Range(0, 36);
            if (r < 26) {
                id += (char) (r + 65);
            }
            else {
                id += (r - 26).ToString();
            }
        }
        Debug.Log("Match ID is: {id}");
        return id;
    }
}

public static class MatchExtensions {
        public static Guid ToGuid(this string id) {
            MD5CryptoServiceProvider prov = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = prov.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
    }
