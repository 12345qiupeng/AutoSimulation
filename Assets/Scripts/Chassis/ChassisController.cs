using System;
using System.Collections.Generic;
using System.Linq;
using MechDancer.Common;
using MechDancer.Framework.Dependency.UniqueComponent;
using Node;
using UnityEngine;
using UnityEngine.UI;

namespace Chassis
{
	public class ChassisController : MonoBehaviour
	{
		public Dropdown ctrlMode;


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
		private void Drive(float v, float w)
		{
			transform.Translate(new Vector3(v, 0, 0) * Time.deltaTime);
			transform.Rotate(-new Vector3(0, w / Mathf.PI * 180, 0) * Time.deltaTime);
		}

		private void DriveByRTO(float rho, float theta, float omega)
		{
			var tWVModle = new ThreeWheelVehicle(rho, theta, omega);
			var pose = tWVModle.Trajectory(0.1).Take(1);
			foreach (var p in pose)
			{
				transform.Translate(new Vector3(p.x, 0, p.y));
				transform.Rotate(new Vector3(0, (float) (p.z / Math.PI) * 180, 0));
			}

		}

		#endregion

		#region Remote

		/// <summary>
		///     远程终端
		/// </summary>
		private readonly ChassisRemoteHub _remoteHub;


		/// <summary>
		///     远程终端只能在解析器中构造
		/// </summary>
		public ChassisController()
			=> _remoteHub = new ChassisRemoteHub
			( // -
				(v, w) => _velocity.Field = Tuple.Create(v, w),
				//  -
				(x, y, θ) => _pose.Field = Tuple.Create(x, y, θ),
				Console.WriteLine,
				list => _line.Field = list,
				list => _targetLine.Field = list,
				(rho, theta, omega) => _carCtrlCmd.Field = Tuple.Create(rho, theta, omega)
			);

		public void AddKeyPose()
		{
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

		#endregion

		#region Engine

		private readonly Hook<Tuple<float, float, float>, object> _carCtrlCmd
			= new Hook<Tuple<float, float, float>, object>();

		private readonly Hook<Tuple<float, float>, object> _velocity
			= new Hook<Tuple<float, float>, object>();

		private readonly Hook<Tuple<float, float, float>, object> _pose
			= new Hook<Tuple<float, float, float>, object>();

		public float nodeCreatePeriod = 1.0f;
		private Vector3 _lastPosition = Vector3.zero;

		public GameObject postureContainer;

		private readonly Hook<List<Vector2>, object> _line
			= new Hook<List<Vector2>, object>();

		private readonly Hook<List<Vector2>, object> _targetLine
			= new Hook<List<Vector2>, object>();

		private PostureContainer _postureContainer;

		private void CreateNode(float x, float y, float θ)
		{
			var temp = transform;

			if ((temp.position - _lastPosition).magnitude < nodeCreatePeriod) return;
			_postureContainer
				.AddPose(_lastPosition = temp.position, temp.rotation);
		}

		private void Start()
		{
			_postureContainer = postureContainer
				.GetComponent<PostureContainer>();
			_remoteHub.Start();
		}

		private void OnDestroy() => _remoteHub.Stop();

		private void Update()
		{
			_line.Field
				?.Also(it => trackContainer
					.GetComponent<TrackContainer>()
					.DrawPredictLine(it));
			_line.Field = null;
			_targetLine.Field
				?.Also(it => targetContainer
					.GetComponent<TrackContainer>()
					.DrawPredictLine(it));
			_targetLine.Field = null;
		}

		private void FixedUpdate()
		{
			switch (ctrlMode.captionText.text)
			{
				case "模拟控制":
					if (_carCtrlCmd.Field == null) return;
					var (sr, sθ, so) = _carCtrlCmd.Field;
					DriveByRTO(sr,sθ,so);
					var (sx, sy, scθ) = CurrentPose;
					_remoteHub.PublishPose(sx, sy, scθ);
					CreateNode(sx, sy, scθ);
					break;
				case "键盘控制":
					Drive(Input.GetAxis("Vertical") * 0.5f,
						Input.GetAxis("Horizontal") * (-Mathf.PI / 4));
					var (kx, ky, kθ) = CurrentPose;
					_remoteHub.PublishPose(kx, ky, kθ);
					CreateNode(kx, ky, kθ);
					break;
				case "实时控制":
					if (_pose.Field == null) return;
					var (rx, ry, rθ) = _pose.Field;
					transform.SetPose(rx, ry, null, rθ);
					CreateNode(rx, ry, rθ);
					break;
				default:
					Drive(Input.GetAxis("Vertical") * 0.5f,
						Input.GetAxis("Horizontal") * (-Mathf.PI / 4));
					var (x, y, θ) = CurrentPose;
					_remoteHub.PublishPose(x, y, θ);
					CreateNode(x, y, θ);
					break;
			}
		}


		#endregion
	}
}