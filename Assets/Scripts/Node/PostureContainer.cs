using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostureContainer : MonoBehaviour
{
    // Start is called before the first frame update
    public string m_SelectName;
    public Dropdown m_SelectDropDown;
    public GameObject m_ArrowPrefabs;
    public Color m_RecordColor;
    public Color m_PlayColor;
    public Slider m_HeightSlider;

    private bool m_IsRecording = true;

    public void OnRecordChanged(bool value)
    {
        m_IsRecording = value;
    }

    public void OnHighChanged()
    {
        Text text = m_SelectDropDown.captionText;
        if (text.text == m_SelectName)
        {
            Vector3 pos = transform.position;
            pos.y = m_HeightSlider.value;
            transform.position = pos;
        }
    }

    public void AddPose(Vector3 pos,Quaternion rot)
    {
        pos.y += 0.6f;
        Quaternion r1 = Quaternion.Euler(new Vector3(90, 180, 0));
        rot = rot * r1;

        GameObject gm = Instantiate(m_ArrowPrefabs, transform);
        pos.y += transform.position.y;
        gm.transform.position = pos;
        gm.transform.rotation = rot;

        MeshRenderer render = gm.GetComponentInChildren<MeshRenderer>();
        if (m_IsRecording)
            render.material.color = m_RecordColor;
        else
            render.material.color = m_PlayColor;

        
    }
}
