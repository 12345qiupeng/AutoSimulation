using UnityEngine;

namespace Node {
	public class ArrowManager : MonoBehaviour {
		// Start is called before the first frame update
		[HideInInspector] public GameObject m_Instance;

		private void OnMouseDown() => Destroy(gameObject);

		private void OnMouseOver() {
			if (Input.GetMouseButton(0)) Destroy(gameObject);
		}
	}
}