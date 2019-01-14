using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Graph : MonoBehaviour {
    [Header("Static variables")]
    public Transform GraphLinesContainer;
    public Transform HorizontalLinesContainer;
    public Transform PointsContainer;
    [Space(10)]
    public GameObject CanvasLinePrefab;
    public GameObject horizontalLinePrefab;
    public GameObject pointPrefab;
    [Space(10)]

    [Header("Values")]
    public Vector2[] values;
    [Space(10)]

    [Header("Settings")]
    public float lineWidth = 2;
    public float pointSize = 5;
    public float numberOfHorizontalLines = 3;
    public float horizontalLineHeight = 3;
    [Space(10)]
    public bool isEnabledHeightNormalization = true;
    public bool isEnabledWidthNormalization = true;
    public bool showConnectionPoints = true;
    [Space(10)]

    [Header("ReadOnly variables")]
    public float minValue = Mathf.Infinity;
    public float maxValue = Mathf.NegativeInfinity;
    public float maxX = 0;

    public void OnValidateFunction() {
        CalculateNormalization();
        RemoveChildren(GraphLinesContainer);
        RemoveChildren(HorizontalLinesContainer);
        RemoveChildren(PointsContainer);
        UnityEditor.EditorApplication.delayCall += () => {
            for (int i = 1; i < values.Length; i++) {
                DrawLine(values[i - 1], values[i]);
            }
            DrawHorizontalLine(0, 0);
            for(int i = 1; i < numberOfHorizontalLines; i++) {
                DrawHorizontalLine(i, GetComponent<RectTransform>().sizeDelta.y / (numberOfHorizontalLines - 1) * i);
            }
        };
    }

    private void RemoveChildren(Transform trans) {
        for(int i = 0; i < trans.childCount; i++) {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                try { DestroyImmediate(trans.GetChild(0).gameObject, true); } catch { }
            };
        }
    }

    private void DrawLine(Vector2 pointA, Vector2 pointB) {
        Vector2 beforeNormalization = pointB;
        if (isEnabledHeightNormalization) {
            pointA = new Vector2(pointA.x, Normalize(pointA.y));
            pointB = new Vector2(pointB.x, Normalize(pointB.y));
        }
        if (isEnabledWidthNormalization) {
            pointA = new Vector2(NormalizeWidth(pointA.x), pointA.y);
            pointB = new Vector2(NormalizeWidth(pointB.x), pointB.y);
        }
        GameObject line = Instantiate(CanvasLinePrefab, GraphLinesContainer);
        RectTransform imageRectTransform = line.GetComponent<RectTransform>();
        Vector3 differenceVector = pointB - pointA;

        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        imageRectTransform.pivot = new Vector2(0, 0.5f);
        imageRectTransform.anchoredPosition = pointA;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
        if (showConnectionPoints) {
            DrawPoint(pointA);
            if(beforeNormalization == values[values.Length - 1]) {
                DrawPoint(pointB);
            }
        }
    }

    public void DrawPoint(Vector2 position) {
        GameObject point = Instantiate(pointPrefab, PointsContainer);
        point.GetComponent<RectTransform>().sizeDelta = new Vector2(pointSize, pointSize);
        point.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public void DrawHorizontalLine(int id, float height) {
        GameObject line = Instantiate(horizontalLinePrefab, HorizontalLinesContainer);
        line.GetComponent<RectTransform>().sizeDelta = new Vector2(0, horizontalLineHeight);
        line.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, height);
        line.GetComponent<GraphLine>().number.text = Mathf.Round(maxValue / numberOfHorizontalLines * id).ToString();
    }

    public void CalculateNormalization() {
        minValue = Mathf.Infinity;
        maxValue = Mathf.NegativeInfinity;
        maxX = 0;
        for(int i = 0; i < values.Length; i++) {
            if (values[i].y < minValue)
                minValue = values[i].y;
            if (values[i].y > maxValue)
                maxValue = values[i].y;
            if(values[i].x > maxX) {
                maxX = values[i].x;
            }
        }
    }

    public float Normalize(float value) {
        return value * GetComponent<RectTransform>().sizeDelta.y / (maxValue - minValue) - minValue;
    }

    public float NormalizeWidth(float value) {
        return value * GetComponent<RectTransform>().sizeDelta.x / maxX;
    }
}

[CustomEditor(typeof(Graph))]
public class CanvasLineEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        Graph script = (Graph)target;
        if (GUILayout.Button("Recalculate lines")) {
            script.OnValidateFunction();
        }
    }
}
