using System;
using System.Collections.Generic;
using MechDancer.Framework.Dependency.UniqueComponent;
using Node;
using UnityEngine;
using UnityEngine.UI;

namespace Chassis {
	public class ChassisController : MonoBehaviour {
		public Toggle displayMode;

		#region Chassis

		/// <summary>
		///     获取自身二维位姿
		/// </summary>
		private (float x, float y, float theta) CurrentPose =>
			(transform.position.x,
			 transform.position.z,
			 Mathf.Atan2(transform.right.z, transform.right.x));

		/// <summary>
		///     开车
		/// </summary>
		/// <param name="v">速度    m /s </param>
		/// <param name="w">角速度 rad/s </param>
		private void Drive(float v, float w) {
			transform.Translate(new Vector3(v, 0, 0)                * Time.deltaTime);
			transform.Rotate(-new Vector3(0, w / Mathf.PI * 180, 0) * Time.deltaTime);
		}

		#endregion

		#region Remote

		/// <summary>
		///     远程终端
		/// </summary>
		private readonly ChassisRemoteHub _remoteHub;

		private List<Vector2>
			pline,
			targetLine;

		private bool _isLineChanged,
		             _isTargetLineChanged;

		/// <summary>
		///     远程终端只能在解析器中构造
		/// </summary>
		public ChassisController()
			=> _remoteHub = new ChassisRemoteHub
				   ( // -
				    (v, w) => _velocity.Field = Tuple.Create(v, w),
				    //  -
				    (x, y, θ) => _pose.Field = Tuple.Create(x, y, θ),
				    //  -
				    (a, b, c, d, e, f, det) => _trans.Field = Tuple.Create(a, b, c, d, e, f, det),
				    //  -
				    Console.WriteLine,
				    //
				    (list) => {
					    pline          = list;
					    _isLineChanged = true;
				    },
				    (list) => {
					    targetLine           = list;
					    _isTargetLineChanged = true;
				    }
				   );

		public void AddKeyPose() {
			_remoteHub.Send($"save key pose {Guid.NewGuid().ToString()}");
			var (x, y, θ) = CurrentPose;
			Functions.LoadObject("KeyPose")
			         .transform
			         .SetPose(x, y, 0, θ + Mathf.PI / 2);
		}

		#endregion

		#region PredictLine

		public GameObject trackContainer;
		public GameObject targetContainer;

		public void OnPredictLineBtnClicked() =>
			trackContainer
			   .GetComponent<TrackContainer>()
			   .DrawPredictLine
					(new List<Vector2> {
						                   new Vector2(0, 1),
						                   new Vector2(0, 2),
						                   new Vector2(0, 3)
					                   });

		#endregion


		#region Engine

		private readonly Hook<Tuple<float, float>, object> _velocity
			= new Hook<Tuple<float, float>, object>();

		private readonly Hook<Tuple<float, float, float>, object> _pose
			= new Hook<Tuple<float, float, float>, object>();

		private readonly Hook<Tuple<float, float, float, float, float, float, float>, object> _trans
			= new Hook<Tuple<float, float, float, float, float, float, float>, object>();

		public  float      nodeCreatePeriod = 1.0f;
		private Vector3    _lastPosition    = Vector3.zero;
		public  GameObject postureContainer;


		private void CreateNode(float x, float y, float θ) {
			var temp = transform;

			if ((temp.position - _lastPosition).magnitude < nodeCreatePeriod) return;
			postureContainer
			   .GetComponent<PostureContainer>()
			   .AddPose(_lastPosition = temp.position, temp.rotation);
		}

		private void Start() => _remoteHub.Start();

		private void OnDestroy() => _remoteHub.Stop();

		private void Update() {
			if (_isLineChanged) {
				_isLineChanged = false;
				trackContainer.GetComponent<TrackContainer>().DrawPredictLine(pline);
			}

			if (!_isTargetLineChanged) return;
			_isTargetLineChanged = false;
			targetContainer.GetComponent<TrackContainer>().DrawPredictLine(targetLine);
		}

		private void FixedUpdate() {
			if (displayMode.isOn) {
				if (_pose.Field == null) return;
				var (x, y, θ) = _pose.Field;
				transform.SetPose(x, y, null, θ);
				CreateNode(x, y, θ);
			} else {
				Drive(Input.GetAxis("Vertical")   * 0.5f,
				      Input.GetAxis("Horizontal") * (-Mathf.PI / 4));

				var (x, y, θ) = CurrentPose;
				_remoteHub.PublishPose(x, y, θ);
				CreateNode(x, y, θ);
			}
		}

		#endregion
	}
}