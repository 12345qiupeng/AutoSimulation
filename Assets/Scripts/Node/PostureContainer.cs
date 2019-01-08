using UnityEngine;
using UnityEngine.UI;

namespace Node {
	public class PostureContainer : MonoBehaviour {
		// Resources
		public GameObject arrowPrefabs;
		public Color      color;

		// Controls
		public Slider   heightSlider;
		public Dropdown selectDropDown;

		public string selectName;

		public void OnClearButtonResponed() => ClearPose();

		private void ClearPose() => transform.KillChildren();

		public void OnHighChanged() {
			if (selectDropDown.captionText.text != selectName) return;
			var pos = transform.position;
			transform.position = new Vector3(pos.x, heightSlider.value, pos.z);
		}

		public void AddPose(Vector3 pos, Quaternion rot) {
			var gm = Instantiate(arrowPrefabs, transform);
			gm.transform.position = new Vector3(pos.x, pos.y + .6f + transform.position.y, pos.z);
			gm.transform.rotation = rot * Quaternion.Euler(new Vector3(90, 180, 0));

			gm.GetComponentInChildren<MeshRenderer>().material.color = color;
		}
	}
}