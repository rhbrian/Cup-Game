using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScaleReticle : MonoBehaviour
{
    [SerializeField] Image reticle;
    Vector3 initialSize;
    [SerializeField]float rate;
    [SerializeField]float ratio;

    public bool readyToShoot = false;

    void Start() {
        initialSize = reticle.rectTransform.localScale;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void Update()
    {
        if (!IsPointerOverUIObject()) {
            reticle.rectTransform.localScale = initialSize + initialSize * Mathf.PingPong(Time.time * rate, ratio);
        }
        else {
            reticle.rectTransform.localScale = new Vector3 (initialSize.x + initialSize.x * .8f,initialSize.y + initialSize.y * .8f, 1f);
        }
    }
}
