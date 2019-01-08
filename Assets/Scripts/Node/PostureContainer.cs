using MechDancer.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Node {
	public class PostureContainer : MonoBehaviour {
		public GameObject m_ArrowPrefabs;
		public Color      m_Color;
		public Slider     m_HeightSlider;

		public Dropdown m_SelectDropDown;

		// Start is called before the first frame update
		public string m_SelectName;

		public void OnClearButtonResponed() => ClearPose();

		private void ClearPose() {
			for (var i = transform.childCount - 1; i >= 0; --i)
				Destroy(transform.GetChild(i).gameObject);
		}

		public void OnHighChanged() {
			if (m_SelectDropDown.captionText.text == m_SelectName)
				transform
				   .position
				   .Also(it => it.Set(it.x, m_HeightSlider.value, it.z));
		}

		public void AddPose(Vector3 pos, Quaternion rot) {
			pos.y += 0.6f;
			var r1 = Quaternion.Euler(new Vector3(90, 180, 0));
			rot = rot * r1;

			var gm = Instantiate(m_ArrowPrefabs, transform);
			pos.y                 += transform.position.y;
			gm.transform.position =  pos;
			gm.transform.rotation =  rot;
			var render = gm.GetComponentInChildren<MeshRenderer>();

			render.material.color = m_Color;
		}
	}
}