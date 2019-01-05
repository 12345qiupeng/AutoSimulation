using MechDancer.Framework.Dependency.UniqueComponent;
using UnityEngine;
using UnityEngine.UI;

namespace Chassis {
	public class DataDisplay : MonoBehaviour {
		private readonly Hook<float[], object> _data = new Hook<float[], object>();
		public           Text                  a;
		public           Text                  b;
		public           Text                  c;
		public           Text                  d;
		public           Text                  det;
		public           Text                  e;
		public           Text                  f;

		public void Set(float a, float b, float c, float d, float e, float f, float det)
			=> _data.Field = new[] {a, b, c, d, e, f, det};

		private void Update() {
			if (_data.Field == null) return;
			a.text   = _data.Field[0].ToString();
			b.text   = _data.Field[1].ToString();
			c.text   = _data.Field[2].ToString();
			d.text   = _data.Field[3].ToString();
			e.text   = _data.Field[4].ToString();
			f.text   = _data.Field[5].ToString();
			det.text = _data.Field[6].ToString();
		}
	}
}