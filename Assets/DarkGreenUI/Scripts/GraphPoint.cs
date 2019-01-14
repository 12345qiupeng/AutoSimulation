using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphPoint : MonoBehaviour{

    public GameObject underMouseMessagePrefab;

    public void OnClick() {
        GameObject message = Instantiate(underMouseMessagePrefab, transform);
    }

}
