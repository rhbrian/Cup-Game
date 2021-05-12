using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;
using System;

public class Player : NetworkBehaviour
{

    [SerializeField] public GameObject playerCamera;
    [SerializeField] public GameObject playerCameraLock;
    [SerializeField] public GameObject followTarget;
    [SerializeField] public GameObject followTargetLock;
    [SerializeField] public GameObject centerTarget;
    [SerializeField] public GameLogic gameManager;

    public GameObject chair;
    public GameObject bowl;
    public GameObject ball;

    private bool holdingBall = true;
    [SyncVar] public int playerID = -1;
    public bool haveAuthority = false;

    Animator anim;
    Transform spawnPoint;
    private bool canShoot = false;
    private int switcher = 0;
    public int score;

    void Start() {
        anim = GetComponent<Animator>();
        centerTarget = GameObject.Find("-- Scene").transform.Find("Level").transform.Find("TallRoundTable").transform.Find("Center").gameObject;
    }

    public void SetVcam() {
        if (hasAuthority) {
            spawnPoint = gameObject.transform;
            playerCamera = GameObject.FindGameObjectsWithTag("vcam")[0];
            playerCameraLock = GameObject.FindGameObjectsWithTag("vcam")[1];
            //playerCamera.transform.position = gameObject.transform.position;
            playerCamera.GetComponent<CinemachineVirtualCamera>().Follow = followTarget.transform;
            playerCameraLock.GetComponent<CinemachineVirtualCamera>().Follow = followTargetLock.transform;
            playerCamera.transform.forward = gameObject.transform.forward;
            playerCameraLock.transform.forward = gameObject.transform.forward;
            GetComponent<Swipe>().ball = ball;
            GetComponent<Swipe>().Setup();
            haveAuthority = true;
            gameManager = GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameLogic>();
        }
    }

    public void ResetPosition () {

    }

    [ClientRpc]
    public void RpcPairObjects(GameObject chair, GameObject bowl, GameObject ball) {
        if (hasAuthority) {  
            gameObject.name = "Local";
            this.chair = chair;
            this.bowl = bowl;
            this.ball = ball;
            SetVcam();
        }
    }

    public void StartTurn() {
        if (hasAuthority) {
            GameObject.Find("-- UI").transform.Find("GamePlayerUI").transform.Find("MyTurn").gameObject.SetActive(true);
            Debug.Log("starting my turn");
        }
    }

    public void StopTurn() {
        if (hasAuthority) {
            GameObject.Find("-- UI").transform.Find("GamePlayerUI").transform.Find("MyTurn").gameObject.SetActive(false);
            Debug.Log("stopping my turn");
        }
    }

    void Update() {
        if (hasAuthority) {
            if (Input.GetKeyDown(KeyCode.X)) {
                if (canShoot) {
                    playerCamera.GetComponent<CinemachineVirtualCamera>().Priority = 1;
                    playerCameraLock.GetComponent<CinemachineVirtualCamera>().Priority = 0;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    canShoot = false;
                    GetComponent<Swipe>().shootMode = false;
                }
                else {
                    playerCamera.GetComponent<CinemachineVirtualCamera>().Priority = 0;
                    playerCameraLock.GetComponent<CinemachineVirtualCamera>().Priority = 1;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    canShoot = true;
                    GetComponent<Swipe>().shootMode = true;
                }
            }
            if (canShoot) {
                if (Input.GetKeyDown(KeyCode.Q)) {
                    if (switcher >= gameManager.numActivePlayers) switcher = 0;
                    if (gameManager.gamePlayers[switcher] != gameObject) {
                        playerCameraLock.GetComponent<CinemachineVirtualCamera>().LookAt = gameManager.bowls[switcher++].gameObject.transform;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space)) {
                    playerCameraLock.GetComponent<CinemachineVirtualCamera>().LookAt = centerTarget.transform;
                }
            }
        }
    }

}
