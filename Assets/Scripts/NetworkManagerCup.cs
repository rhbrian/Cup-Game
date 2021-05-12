using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror {
    public class NetworkManagerCup : NetworkManager
    {
        GameObject ball, chair, bowl;
        public List<Transform> playerSpawnSpots;
        public List<Transform> chairSpawnSpots;
        public List<Transform> bowlSpots;
        public List<GameObject> players;
        [Scene] [SerializeField] private string menuScene = string.Empty;
        [SerializeField] private NetworkRoomPlayer roomPlayer = null;
        [SerializeField] private int minimumPlayers = 2;
        [SerializeField] public GameObject gameLogic;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            int inc = 0;
            int x = 0;
            if ((numPlayers + 1) > 1) {
                inc = 12 / (numPlayers + 1);
                for (int i = 0; i < players.Count; i++) {
                    players[i].transform.position = playerSpawnSpots[x].position;
                    players[i].transform.rotation = playerSpawnSpots[x].rotation;
                    players[i].GetComponent<Player>().chair.transform.position = chairSpawnSpots[x].position;
                    players[i].GetComponent<Player>().chair.transform.rotation = chairSpawnSpots[x].rotation;
                    players[i].GetComponent<Player>().bowl.transform.position = bowlSpots[x].position;
                    x += inc;
                }
            }

            // choose position and spawn players
            GameObject player = Instantiate(playerPrefab, playerSpawnSpots[x].position, playerSpawnSpots[x].rotation);
            NetworkServer.AddPlayerForConnection(conn, player);
            players.Add(player);

            chair = Instantiate(spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "BarStool"), chairSpawnSpots[x].position, chairSpawnSpots[x].rotation);
            NetworkServer.Spawn(chair);
            // spawn bowl
            bowl = Instantiate(spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "NormalSizeBowl"), bowlSpots[x].position, bowlSpots[x].rotation);
            NetworkServer.Spawn(bowl);

            Quaternion quat = Quaternion.Euler(0,0,0);
            ball = Instantiate(spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "BottleCap"), bowlSpots[x].position, quat);
            NetworkServer.Spawn(ball);

            player.GetComponent<Player>().chair = chair;
            player.GetComponent<Player>().bowl = bowl;
            player.GetComponent<Swipe>().ball = ball;
            //gameLogic.GetComponent<GameLogic>().CmdAddPlayer(player, ball, bowl, chair);
            //player.GetComponent<Swipe>().RpcSetup();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // call base functionality (actually destroys the player)
            for (int i = 0; i < players.Count; i++) {
                if (players[i].name == "Local") {
                    GameObject destroy = players[i];
                    players.RemoveAt(i);
                    Destroy(destroy);
                }
            }
            base.OnServerDisconnect(conn);
        }

        public override void Start() {
            
        }

        void Update() {

        }

        // Converts string property into a Scene property in the inspector
        public class SceneAtrribute : PropertyAttribute { }
    }
}
