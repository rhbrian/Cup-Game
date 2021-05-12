using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    // --- Script for handling how UI reacts to Joining/Hosting --- //
    public static UILobby instance;
    [SerializeField] InputField joinMatchID;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;
    [SerializeField] GameObject lobbyCanvas;

    [SerializeField] Transform UIPlayerParent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] Text matchIDText;
    [SerializeField] GameObject startButton;

    [SerializeField] GameObject UILobbyObj;
    [SerializeField] public GameObject UIGameObj;

    void Start() {
        instance = this;
    }

    public void Host() {
        joinMatchID.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        LobbyPlayer.localPlayer.HostGame();
    }
    public void Join() {
        joinMatchID.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        LobbyPlayer.localPlayer.JoinGame(joinMatchID.text.ToUpper());
    }

    public void HostSuccess(bool success, string matchID) {
        if (success) {
            lobbyCanvas.SetActive(true);
            SpawnPlayerUIPrefab(LobbyPlayer.localPlayer);
            matchIDText.text = matchID;
            startButton.SetActive(true);
        }
        else {
            joinMatchID.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void JoinSuccess(bool success, string matchID) {
        if (success) {
            lobbyCanvas.SetActive(true);
            SpawnPlayerUIPrefab(LobbyPlayer.localPlayer);
            matchIDText.text = matchID;
        }
        else {
            joinMatchID.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void ActivateGameUI() {
        UILobbyObj.SetActive(false);
        UIGameObj.SetActive(true);
    }

    public void SpawnPlayerUIPrefab(LobbyPlayer player) {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
        newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
    }

    public void StartGame() {
        startButton.SetActive(false);
        LobbyPlayer.localPlayer.InitiateGame();
    }
}
