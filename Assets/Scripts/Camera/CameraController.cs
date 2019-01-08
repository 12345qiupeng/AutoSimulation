using MechDancer.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Camera {
	public class CameraController : MonoBehaviour {
		private static readonly Quaternion Q90   = Quaternion.Euler(90, 0, 0); // 俯角90°
		private static readonly Quaternion Q45   = Quaternion.Euler(45, 0, 0); // 俯角45°
		private static readonly float      Sqrt2 = Mathf.Sqrt(2) / 2;          // cos(45°)
		private                 bool       _lookDown;                          // 俯视视角

		private Vector2? _mouse; // 鼠标之前位置

		public GameObject chassis;    // 底盘引用
		public float      height = 8; // 视角高度
		public Toggle     @lock;      // 锁定视角

		/// <summary>
		///     获取或设置相机关注点
		/// </summary>
		private Vector2 Focus {
			// 从摄像机计算关键点
			get => transform
			      .position
			      .Let(it => new Vector2(it.x, it.z + (_lookDown ? 0 : Sqrt2 * height)));
			// 从关键点设置摄像机
			set {
				if (_lookDown)
					transform.SetPositionAndRotation
						(new Vector3(value.x, height, value.y), Q90);
				else
					transform.SetPositionAndRotation
						(new Vector3(value.x, Sqrt2 * height, value.y - Sqrt2 * height), Q45);
			}
		}

		/// <summary>
		///     俯视视角切换
		/// </summary>
		public void OnLookDownChanged(bool value)
			=> Focus = Focus.Also(_ => _lookDown = value);

		private void LateUpdate() {
			if (@lock.isOn) {
				height += Input.mouseScrollDelta.y;
				Focus = chassis
				       .transform
				       .position
				       .Let(it => new Vector2(it.x, it.z));
			} else {
				var focus = Focus;

				_mouse =
					!Input.GetMouseButton(0)
						? (Vector2?) null
						: Input.mousePosition
						       .Let(it => new Vector2(it.x, it.y))
						       .Also(it => focus -= 0.01f * (it - _mouse ?? Vector2.zero));

				height += Input.mouseScrollDelta.y;
				Focus  =  focus;
			}
		}
	}
}