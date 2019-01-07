using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackContainer : MonoBehaviour
{

    #region PredictLine

    public float m_height;
    public string m_SelectName;
    public GameObject m_DotPrefabs;
    public GameObject m_Car;
    public Slider m_HeightSlider;
    public Color m_LineColor;
    public Dropdown m_SelectDropDown;
    public bool m_GolbalBased=true;

    

    public void OnHighChanged()
    {
        Text text = m_SelectDropDown.captionText;
        if (text.text == m_SelectName)
        {
            Vector3 pos = transform.position;
            pos.y = m_HeightSlider.value;
            m_height = m_HeightSlider.value;
            transform.position = pos;
        }    
    }

    public void DrawPredictLine(List<Vector2> line)
    {
        
        ClearPredictLine();
        foreach (Vector2 vec in line)
        {
            GameObject _pl = Instantiate(m_DotPrefabs, transform);
            if (!m_GolbalBased)
                _pl.transform.position = TransCarCordToWorldCord(vec, m_height);
            else
                _pl.transform.position = new Vector3(vec.x, m_height, vec.y);
            Renderer renderer = _pl.GetComponent<Renderer>();
            renderer.material.color = m_LineColor;

        }
    }

    private void ClearPredictLine()
    {
        int count = transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

    }

    private Vector3 TransCarCordToWorldCord(Vector2 vec,float height)
    {
        Vector3 temp = new Vector3(vec.x, height, vec.y);
        temp = m_Car.transform.rotation * temp;
        temp = temp + m_Car.transform.position;

        return temp;
    }

    #endregion

}
