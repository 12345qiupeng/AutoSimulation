using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Node {
	public class TrackContainer : MonoBehaviour {
		#region PredictLine

		public float      m_height;
		public string     m_SelectName;
		public GameObject m_DotPrefabs;
		public GameObject m_Car;
		public Slider     m_HeightSlider;
		public Color      m_LineColor;
		public Dropdown   m_SelectDropDown;
		public bool       m_GolbalBased = true;


		public void OnHighChanged() {
			var text = m_SelectDropDown.captionText;
			if (text.text != m_SelectName) return;
			var pos = transform.position;
			pos.y              = m_HeightSlider.value;
			m_height           = m_HeightSlider.value;
			transform.position = pos;
		}

		public void DrawPredictLine(IEnumerable<Vector2> line) {
			ClearPredictLine();
			foreach (var vec in line) {
				var pl = Instantiate(m_DotPrefabs, transform);
				pl.transform.position = !m_GolbalBased
					                        ? TransCarCordToWorldCord(vec, m_height)
					                        : new Vector3(vec.x, m_height, vec.y);
				pl.GetComponent<Renderer>().material.color = m_LineColor;
			}
		}

		private void ClearPredictLine() {
			var count = transform.childCount;
			for (var i = count - 1; i >= 0; i--) {
				Destroy(transform.GetChild(i).gameObject);
			}
		}

		private Vector3 TransCarCordToWorldCord(Vector2 vec, float height) {
			var temp = new Vector3(vec.x, height, vec.y);
			temp = m_Car.transform.rotation * temp;
			temp = temp + m_Car.transform.position;

			return temp;
		}

		#endregion
	}
}