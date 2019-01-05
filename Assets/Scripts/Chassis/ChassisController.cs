using System;
using System.Collections.Generic;
using System.Linq;
using MechDancer.Common;
using MechDancer.Framework.Dependency.UniqueComponent;
using UnityEngine;
using UnityEngine.UI;


namespace Chassis {
	public class ChassisController : MonoBehaviour {
		public DataDisplay data;
		public Toggle      displayMode;

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

        private List<Vector2> pline;
        private bool islinechanged=false;

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
                    (list)=> {
                        pline = list;
                        islinechanged = true;
                    });

		public void AddKeyPose() {
			_remoteHub.Send($"save key pose {Guid.NewGuid().ToString()}");
			var (x, y, θ) = CurrentPose;
			Functions.LoadObject("KeyPose")
			         .transform
			         .SetPose(x, y, 0, θ + Mathf.PI / 2);
		}

        #endregion



        #region PredictLine

        public GameObject m_TrackContainer;
        
        public void OnPredictLineBtnClicked()
        {    
            List<Vector2> tline = new List<Vector2>();
            tline.Add(new Vector2(0, 1));
            tline.Add(new Vector2(0, 2));
            tline.Add(new Vector2(0, 3));
            m_TrackContainer.GetComponent<TrackContainer>().DrawPredictLine(tline);
         }
               

        #endregion


        #region Engine

        private readonly Hook<Tuple<float, float>, object> _velocity
			= new Hook<Tuple<float, float>, object>();

		private readonly Hook<Tuple<float, float, float>, object> _pose
			= new Hook<Tuple<float, float, float>, object>();

		private readonly Hook<Tuple<float, float, float, float, float, float, float>, object> _trans
			= new Hook<Tuple<float, float, float, float, float, float, float>, object>();

		public  float   nodeCreatePeriod = 1.0f;
		private Vector3 _lastPosition    = Vector3.zero;
        public GameObject m_PostureContainer;

    
        private void CreateNode(float x, float y, float θ) {
			if ((transform.position - _lastPosition).magnitude > nodeCreatePeriod)
            //Functions.LoadObject(float.IsNaN(θ) ? "Node" : "Cone")
            //         .Also(_ => _lastPosition = transform.position)
            //         .transform
            //         .SetPose(x, y, 0.5f, θ + Mathf.PI / 2);
            {
                m_PostureContainer.GetComponent<PostureContainer>().AddPose(transform.position, transform.rotation);
                _lastPosition = transform.position;
            }
		}

		private void Start() => _remoteHub.Start();

		private void OnDestroy() => _remoteHub.Stop();

		private void Update() {
            if (islinechanged)
            {
                islinechanged = false;
                m_TrackContainer.GetComponent<TrackContainer>().DrawPredictLine(pline);
            }
            if (_trans.Field == null) return;
			var (a, b, c, d, e, f, det) = _trans.Field;
			data.Set(a, b, c, d, e, f, det);
            
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