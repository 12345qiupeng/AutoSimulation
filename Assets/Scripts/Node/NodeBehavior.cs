using UnityEngine;

namespace Node {
	public class NodeBehavior : MonoBehaviour {
		private void OnMouseDown() => Destroy(gameObject);

		private void OnMouseOver() {
			if (Input.GetMouseButton(0)) Destroy(gameObject);
		}
	}
}