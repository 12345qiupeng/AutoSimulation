using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour {

    public bool isOn = false;

    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    public void Switch() {
        isOn = !isOn;
        animator.SetBool("isOn", isOn);
    }
}
