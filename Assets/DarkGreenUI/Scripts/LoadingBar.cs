using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LoadingBar : MonoBehaviour {

    public RectTransform progressBar;

    [Range(0, 100)]
    public float progress;

    private void OnValidate() {
        if(progressBar != null) {
            float width = GetComponent<RectTransform>().sizeDelta.x;
            progressBar.sizeDelta = new Vector2(width * progress / 100, progressBar.sizeDelta.y);
        }
    }
}
