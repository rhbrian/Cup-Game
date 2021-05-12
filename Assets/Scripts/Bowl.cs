using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bowl : NetworkBehaviour
{
    Vector3 maxBowlSize = new Vector3(.0049f, .0049f, .004f);
    Vector3 minBowlSize = new Vector3(.0011f, .0011f, .001f);
    Vector3 ogBowlSize = new Vector3(.003f, .003f, .004f);
    int x = 0;

    [Command]
    public void CmdSizeBowl(bool UpScale, int scale) {
        RpcSizeBowl(UpScale, scale);
    }

    [ClientRpc]
    public void RpcSizeBowl(bool UpScale, int scale) {
        Debug.Log("sizin " + UpScale + " scale " + scale);
        //if (!hasAuthority) return -1;
        Vector3 bowlSize = transform.localScale;
        Debug.Log(transform.localScale);
        if (UpScale) {
            bowlSize += (scale * ogBowlSize * .2f);
            bowlSize.z = .004f;

            if (maxBowlSize.x < bowlSize.x) {
                bowlSize = maxBowlSize;
            }

            transform.localScale = bowlSize;
            // return 1;
        }
        else {
            bowlSize -= (scale * ogBowlSize * .2f);
            bowlSize.z = .004f;
            Debug.Log(bowlSize.x);
            /*if (minBowlSize.x > bowlSize.x) {
                Debug.Log("destroying bowl");
                // remove this player from the game
                // return 0;
            }*/
            transform.localScale = bowlSize;
        }
        // return -1;
    }
}
