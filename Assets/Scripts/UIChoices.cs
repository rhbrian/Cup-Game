using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIChoices : MonoBehaviour
{
    [SerializeField] GameObject ChoiceUI;
    [SerializeField] GameObject ChoiceUIBlock;
    [SerializeField] public GameObject GameManager;
    [SerializeField] public GameObject localPlayer;
    public GameObject opponentPlayer;
    public bool gmSet = false;
    public bool playerSet = false;

    [SerializeField] GameObject myBowlUp1;
    [SerializeField] GameObject myBowlUp2;
    [SerializeField] GameObject myBowlDown1;
    [SerializeField] GameObject myBowlDown2;
    [SerializeField] GameObject oppBowlUp1;
    [SerializeField] GameObject AllOppBowlUp1;
    [SerializeField] GameObject ReversePlay;
    [SerializeField] GameObject SwapBowlSize;

    List<GameObject> spawnedChoices = new List<GameObject>();

    public void setLocalPlayer(SyncList<GameObject> players) {
        foreach (var player in players) {
            if (player.GetComponent<Player>().haveAuthority) {
                localPlayer = player;
                playerSet = true;
                break;
            }
        }
    }

    public void SetChoices(bool isCenter, GameObject player, GameObject opponent) {
        if (player != localPlayer) return;
        opponentPlayer = opponent;
        ResetChoices();
        if (isCenter) {
            GameObject allOpp = Instantiate(AllOppBowlUp1, ChoiceUI.transform);
            allOpp.transform.Find("Button").GetComponent<Button>().onClick.AddListener(IncreaseAllOnClick);
            spawnedChoices.Add(allOpp);

            GameObject swapSize = Instantiate(SwapBowlSize, ChoiceUI.transform);
            swapSize.transform.Find("Button").GetComponent<Button>().onClick.AddListener(SwapSizeOnClick);
            spawnedChoices.Add(swapSize);

            GameObject reversePlay = Instantiate(ReversePlay, ChoiceUI.transform);
            reversePlay.transform.Find("Button").GetComponent<Button>().onClick.AddListener(ReversePlayOnClick);
            spawnedChoices.Add(reversePlay);

            GameObject mbDown2 = Instantiate(myBowlDown2, ChoiceUI.transform);
            mbDown2.transform.Find("Button").GetComponent<Button>().onClick.AddListener(DecreaseMBOnClick2);
            spawnedChoices.Add(mbDown2);

            GameObject mbUp2 = Instantiate(myBowlUp2, ChoiceUI.transform);
            mbUp2.transform.Find("Button").GetComponent<Button>().onClick.AddListener(IncreaseMBOnClick2);
            spawnedChoices.Add(mbUp2);
        }
        else {
            GameObject opUp1 = Instantiate(oppBowlUp1, ChoiceUI.transform);
            opUp1.transform.Find("Button").GetComponent<Button>().onClick.AddListener(IncreaseOppOnClick1);
            spawnedChoices.Add(opUp1);

            GameObject mbDown1 = Instantiate(myBowlDown1, ChoiceUI.transform);
            mbDown1.transform.Find("Button").GetComponent<Button>().onClick.AddListener(DecreaseMBOnClick1);
            spawnedChoices.Add(mbDown1);

            GameObject mbUp1 = Instantiate(myBowlUp1, ChoiceUI.transform);
            mbUp1.transform.Find("Button").GetComponent<Button>().onClick.AddListener(IncreaseMBOnClick1);
            spawnedChoices.Add(mbUp1);
        }

        ChoiceUIBlock.SetActive(true);
    }

    public void ResetChoices () {
        spawnedChoices.Clear();
        foreach (Transform child in ChoiceUI.transform) {
            Destroy(child.gameObject);
        }
    }

    void IncreaseAllOnClick() {
        ChoiceUIBlock.SetActive(false);
        GameManager.GetComponent<GameLogic>().ScaleAll(localPlayer);
    }

    void SwapSizeOnClick() {
        ChoiceUIBlock.SetActive(false);
        GameManager.GetComponent<GameLogic>().SwapBowls(localPlayer, opponentPlayer);
    }

    void ReversePlayOnClick() {
        ChoiceUIBlock.SetActive(false);
        GameManager.GetComponent<GameLogic>().ReverseOrder();
    }

    void IncreaseMBOnClick1() {
        ChoiceUIBlock.SetActive(false);
        GameManager.GetComponent<GameLogic>().ScaleBowl(true, localPlayer, 1);
    }

    void IncreaseMBOnClick2() {
        ChoiceUIBlock.SetActive(false);
        GameManager.GetComponent<GameLogic>().ScaleBowl(true, localPlayer, 2);
    }

    void DecreaseMBOnClick1() {
        ChoiceUIBlock.SetActive(false);
        GameManager.GetComponent<GameLogic>().ScaleBowl(false, localPlayer, 1);
    }

    void DecreaseMBOnClick2() {
        ChoiceUIBlock.SetActive(false);
        GameManager.GetComponent<GameLogic>().ScaleBowl(false, localPlayer, 2);
    }

    void IncreaseOppOnClick1() {
        ChoiceUIBlock.SetActive(false);
        GameManager.GetComponent<GameLogic>().ScaleBowl(true, opponentPlayer, 1);
    }

}
