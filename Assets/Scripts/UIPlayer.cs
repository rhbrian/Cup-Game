using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIPlayer : MonoBehaviour
{
    [SerializeField] Text text;
    LobbyPlayer lobbyPlayer;
    
    public void SetPlayer(LobbyPlayer lobbyPlayer) {
        this.lobbyPlayer = lobbyPlayer;
        text.text = "Player " + lobbyPlayer.playerIndex.ToString();
    }
}
