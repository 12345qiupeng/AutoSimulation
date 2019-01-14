using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MechDancer.Common;
using MechDancer.Framework.Dependency.UniqueComponent;
using MechDancer.Framework.Net.Modules.TcpConnection;
using MechDancer.Framework.Net.Resources;
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
		public (float x, float y, float theta) CurrentPose =>
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

        private float chassisTheta = 0;

		/// <summary>
		///     用Rho，Theta，Omega控制车辆运动
		/// </summary>
		/// <param name="rho">  长度    		 </param>
		/// <param name="theta">后轮当前角度 rad </param>
		/// <param name="omega">后轮角速度 rad/s </param>
		private void DriveByRTO(float rho, float theta, float omega,out float carV,out float carW)
		{
			var tWVModle = new ThreeWheelVehicle(rho, chassisTheta, omega);
			var pose = tWVModle.Trajectory(Time.deltaTime).Take(1);
			foreach (var p in pose)
			{
				transform.Translate(new Vector3(p.x, 0, p.y));
				transform.Rotate(new Vector3(0, -(float) (p.z / Math.PI) * 180, 0));
			}
            chassisTheta = (float) tWVModle.Theta;
            carV = (float) tWVModle.Velocity;
			carW = (float) tWVModle.Omega;
			//		_remoteHub.Connect("algorism", (byte) TcpCmd.Mail, stream => stream.Say("123"));

		}

		#endregion

		#region Remote

		/// <summary>
		///     远程终端
		/// </summary>
		[HideInInspector] public readonly ChassisRemoteHub _remoteHub;


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
				(rho, theta, omega) =>
                _carCtrlCmd.Field = Tuple.Create(rho, theta, omega)
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


        public void OnSendSuspendButtonClicked()
        {
            isSendStop = !isSendStop;
        }

        private bool isSendStop = false;
        private DateTime _last = DateTime.Now;
		private void FixedUpdate()
		{
			switch (ctrlMode.captionText.text)
			{
				case "模拟控制":
					var (sr, sθ, so) = _carCtrlCmd.Field??Tuple.Create(0f,0f,0f);
					DriveByRTO(sr,sθ,so, out var carv,out var carw);
					var (sx, sy, scθ) = CurrentPose;
                    var now = DateTime.Now;
                    if (now - _last > TimeSpan.FromSeconds(.1))
                    {
                        _last = now;
                        CreateNode(sx, sy, scθ);
                        if(!isSendStop)
                        {
                            Task.Run(() => _remoteHub.PublishPose(sx, sy, scθ, carv, carw, chassisTheta));
                        }
                    }
					break;
				case "实时控制":
					if (_pose.Field == null) return;
					var (rx, ry, rθ) = _pose.Field;
					transform.SetPose(rx, ry, null, rθ);
					CreateNode(rx, ry, rθ);
					break;
				case "键盘控制":
                default:
					var v = Input.GetAxis("Vertical") * 0.5f;
					var w = Input.GetAxis("Horizontal") * (-Mathf.PI / 4);
					Drive(v,w);
					var (x, y, θ) = CurrentPose;
                    if (w == 0) w += 0.01f;
                    if(!isSendStop) _remoteHub.PublishPose(x, y, θ, v, w, v / w);
					CreateNode(x, y, θ);
					break;
			}
		}


		#endregion
	}
}