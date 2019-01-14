using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Window : MonoBehaviour {

    [Header("Buttons")]
    public bool showCloseButton = true;
    public bool showOptionsButton = true;

    public GameObject closeButton;
    public GameObject optionsButton;
    public GameObject settingsMenu;

    private Animator animator;

    private void Start() {
        animator = settingsMenu.GetComponent<Animator>();
    }

    private void OnValidate() {
        if (closeButton != null) closeButton.SetActive(showCloseButton);
        if (optionsButton != null) optionsButton.SetActive(showOptionsButton);
    }

    public void OnSettingsButtonClick() {
        bool isOpen = animator.GetBool("isOpen");
        animator.SetBool("isOpen", !isOpen);
    }

    public void OnCloseButtonClick() {
        gameObject.SetActive(false);
    }
}
