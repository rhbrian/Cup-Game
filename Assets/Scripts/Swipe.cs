using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Mirror{
    public class Swipe : NetworkBehaviour
    {
        private bool isSwiping = false;
        private bool isSetup = false;
        public bool shootMode = false;
        float startTime, endTime, touchTime;
        Vector2 startPos, endPos, direction;
        Vector3 direction3D;
        Vector3 initialBallPoint;

        float ballDistance = -1f;

        [SerializeField] float forceInX;
        [SerializeField] float forceInY;
        [SerializeField] float forceInZ;

        [SerializeField] public GameObject ball;

        [SerializeField] public Camera mainCamera;

        public Rigidbody rb;
        Animator anim;

        public static Vector3 ScreenToWorld(Camera camera, Vector3 position) {
            position.z = camera.nearClipPlane; // distance object has from cam
            return camera.ScreenToWorldPoint(position);
            // return camera.ScreenPointToRay(position);
        }

        void Start() {
            forceInX = 5f;
            forceInY = 5f;
            forceInZ = 4f;
            mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
            anim = GetComponent<Animator>();
        }

        public void Setup() {
            rb = ball.GetComponent<Rigidbody>();
            initialBallPoint = rb.transform.position;
            isSetup = true;
        }

        void LaunchBall(Vector3 end3D, float forceRatio) {
            anim.SetBool("DiceThrow", true);
            end3D.y = Mathf.Abs(end3D.y);
            rb.AddForce(-end3D.x * forceInX, end3D.y * forceInY * forceRatio, -end3D.z * forceInZ * forceRatio);
            // rb.AddForce(-end3D.x * forceInX, -end3D.z * forceInZ * forceRatio, -end3D.y * forceInY * forceRatio);
            // rb.AddForce(5, 5, 5);
            Debug.Log("x :" + (-end3D.x * forceInX) + " y :" + (end3D.y * forceInY) + " z :" + (-end3D.z * forceInZ));
            //rb.AddForce(-end3D.x * forceInX, end3D.y * forceInY, -end3D.z * forceInZ);
        }

        [Client]
        void Update() 
        {
            if (isSetup) {
                anim.SetBool("DiceThrow", false);
                if (isSwiping) {
                    Vector2 screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
                    ball.transform.position = new Vector3(worldPos.x, worldPos.y, worldPos.z);
                }

                #region Mouse Inputs
                // press mouse click on the screen
                if (Input.GetMouseButtonDown(0)) {
                    ball.transform.position = initialBallPoint;
                    rb.velocity = new Vector3(0,0,0);
                    rb.isKinematic = true;
                    // Debug.Log("mouse down");
                    // currently swiping
                    isSwiping = true;
                    // time the click starts
                    startTime = Time.time;
                    // get the position the mouse click starts
                    startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    ballDistance = ball.transform.position.z;
                }

                // release mouse click on screen
                if (Input.GetMouseButtonUp(0)) {
                    // Debug.Log("mouse up");
                    // no longer swiping
                    isSwiping = false;
                    // release time
                    endTime = Time.time;
                    // swiping time interval
                    touchTime = endTime - startTime;
                    // get the position the mouse click is released
                    endPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    // calculate swipe direction in 2D
                    direction = endPos - startPos;
                    Debug.Log(touchTime);
                    Debug.Log(direction.magnitude);

                    // add force to ball rb in 3D depending on swipe time, direction and throw forces
                    rb.isKinematic = false;

                    Vector3 end3D = mainCamera.ScreenToWorldPoint(new Vector3(endPos.x, endPos.y, mainCamera.nearClipPlane - 50f));
                    float forceRatio = (direction.magnitude / 350f);
                    // Debug.Log(forceRatio);
                    if (forceRatio > 1.3f) forceRatio = 1.3f;
                    else if (forceRatio < .3f) forceRatio = .3f;

                    //CmdLaunchBall(end3D);
                    if (shootMode) LaunchBall(end3D, forceRatio);
                }
                #endregion

                #region Mobile Touch Input
                // touch the screen
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                    ball.transform.position = initialBallPoint;
                    rb.velocity = new Vector3(0,0,0);
                    rb.isKinematic = true;
                    Debug.Log("mobile down");
                    // currently swiping
                    isSwiping = true;
                    // get touch pos and get time
                    startTime = Time.time;
                    startPos = Input.GetTouch(0).position;
                    ballDistance = ball.transform.position.z;
                }

                // release finger
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
                    Debug.Log("mobile up");
                    // no longer swiping
                    isSwiping = false;
                    // release time
                    endTime = Time.time;
                    // swiping time interval
                    touchTime = endTime - startTime;
                    // get release position
                    endPos = Input.GetTouch(0).position;
                    // calculate swipe direction in 2D
                    direction = endPos - startPos;

                    // add force to ball rb in 3D depending on swipe time, direction and throw forces
                    rb.isKinematic = false;

                    Vector3 end3D = mainCamera.ScreenToWorldPoint(new Vector3(endPos.x, endPos.y, mainCamera.nearClipPlane - 50f));
                    float forceRatio = (direction.magnitude / 500f) * (.15f / touchTime);
                    //rb.AddForce(-end3D.x * forceInX, -end3D.y * forceInY, -end3D.z * forceInZ * forceRatio);
                    //rb.AddForce(-end3D.x * forceInX, -end3D.y * forceInY, -end3D.z * forceInZ);

                    LaunchBall(end3D, forceRatio);

                    //Destroy(gameObject, 3f);
                }
                #endregion
            }
        }
    }
}