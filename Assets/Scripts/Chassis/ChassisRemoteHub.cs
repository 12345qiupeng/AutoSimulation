using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MechDancer.Common;
using MechDancer.Framework.Dependency;
using MechDancer.Framework.Net.Modules.Multicast;
using MechDancer.Framework.Net.Modules.TcpConnection;
using MechDancer.Framework.Net.Presets;
using MechDancer.Framework.Net.Protocol;
using MechDancer.Framework.Net.Resources;
using UnityEngine;
using static Functions;

namespace Chassis {
	/// <inheritdoc />
	/// <summary>
	///     Unity 底盘通信节点
	/// </summary>
	public class ChassisRemoteHub : RemoteHub {
		// ReSharper disable once InconsistentNaming
		private const string algorithm = nameof(algorithm);
		private const string Chassis   = "- chassis:";

		// 任务中断器
		private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
		private readonly CancellationToken       _token;

		public ChassisRemoteHub(
			Action<float, float>                                    externalCommandReceived,
			Action<float, float, float>                             externalPoseReceived,
			Action<float, float, float, float, float, float, float> externalTransReceived,
			Action<string>                                          commandReceived,
			Action<List<Vector2>>                                   externalListReceived,
			Action<List<Vector2>>                                   externalTargetReceived
		) : base("Unity Chassis",
		         newMemberDetected: it => Debug.Log($"{Chassis} detected {it}"),
		         additions: new IComponent[] {
			                                     new CoreControlProcessor(externalCommandReceived),
			                                     new CorePoseProcessor(externalPoseReceived),
			                                     new CoreTransProcessor(externalTransReceived),
			                                     new CoreCmdProcessor(commandReceived),
			                                     new CoreListProcessor(externalListReceived),
			                                     new CoreTargetListProcessor(externalTargetReceived)
		                                     }
		        ) => _token = _cancellation.Token;

		/// <summary>
		///     广播位姿
		/// </summary>
		public void PublishPose(float x, float y, float θ)
			=> Broadcast((byte) Command.SPose,
			             new MemoryStream(12).Write(x)
			                                 .Write(y)
			                                 .Write(θ)
			                                 .GetBuffer());


		public void Start() {
			Launch(() => _token.IsCancellationRequested, () => Invoke());
			Launch(() => _token.IsCancellationRequested, Accept);
			// 定时刷新，保持连接
			var pacemaker = new Pacemaker();
			Launch(TimeSpan.FromSeconds(2),
			       () => _token.IsCancellationRequested, pacemaker.Activate);
		}

		public void Stop() => _cancellation.Cancel();

		public void Send(string cmd) {
			var temp = Connect(algorithm, (byte) TcpCmd.Mail, I => I.Say(cmd.GetBytes()));
			Debug.Log(temp);
		}

		/// <inheritdoc />
		/// <summary>
		///     响应算法控制指令
		/// </summary>
		private class CoreControlProcessor : IMulticastListener {
			private static readonly byte[] InterestList = {(byte) Command.AVelocity};

			private readonly Action<float, float> _drive;

			public CoreControlProcessor(Action<float, float> drive) => _drive = drive;

			public void Process(RemotePacket remotePacket) {
				var (sender, _, payload) = remotePacket;
				if (sender != algorithm) return;
				var stream = new MemoryStream(payload);
				_drive(stream.ReadFloat(),  // v
				       stream.ReadFloat()); // ω
			}

			public IReadOnlyCollection<byte> Interest => InterestList;
		}

		/// <summary>
		///     响应算法位姿
		/// </summary>
		private class CorePoseProcessor : IMulticastListener {
			private static readonly byte[] InterestList = {(byte) Command.APose};

			private readonly Action<float, float, float> _drive;

			public CorePoseProcessor(Action<float, float, float> drive) => _drive = drive;

			public void Process(RemotePacket remotePacket) {
				var (sender, _, payload) = remotePacket;
				if (sender != algorithm) return;
				var stream = new MemoryStream(payload);
				_drive(stream.ReadFloat(),  // x
				       stream.ReadFloat(),  // y
				       stream.ReadFloat()); // θ
			}

			public IReadOnlyCollection<byte> Interest => InterestList;
		}

		/// <summary>
		///     响应算法
		/// </summary>
		private class CoreListProcessor : IMulticastListener {
			private static readonly byte[] InterestList = {(byte) Command.AList};

			private readonly Action<List<Vector2>> _drive;

			public CoreListProcessor(Action<List<Vector2>> drive) => _drive = drive;

			public void Process(RemotePacket remotePacket) {
				var (sender, _, payload) = remotePacket;
				if (sender != algorithm) return;
				var stream = new MemoryStream(payload);
				var list   = new List<Vector2>();
				while (stream.Available() > 0)
					list.Add(new Vector2(stream.ReadFloat(), stream.ReadFloat()));
				_drive(list);
			}

			public IReadOnlyCollection<byte> Interest => InterestList;
		}

		/// <summary>
		///     响应目标轨迹
		/// </summary>
		private class CoreTargetListProcessor : IMulticastListener {
			private static readonly byte[] InterestList = {(byte) Command.ATargetList};

			private readonly Action<List<Vector2>> _drive;

			public CoreTargetListProcessor(Action<List<Vector2>> drive) => _drive = drive;

			public void Process(RemotePacket remotePacket) {
				var (sender, _, payload) = remotePacket;
				if (sender != algorithm) return;
				var stream = new MemoryStream(payload);
				var list   = new List<Vector2>();
				while (stream.Available() > 0)
					list.Add(new Vector2(stream.ReadFloat(), stream.ReadFloat()));
				_drive(list);
			}

			public IReadOnlyCollection<byte> Interest => InterestList;
		}

		/// <summary>
		///     响应矩阵显示位姿
		/// </summary>
		private class CoreTransProcessor : IMulticastListener {
			private static readonly byte[] InterestList = {(byte) Command.ATrans};

			private readonly Action<float, float, float, float, float, float, float> _drive;

			public CoreTransProcessor(Action<float, float, float, float, float, float, float> drive) => _drive = drive;

			public void Process(RemotePacket remotePacket) {
				var (sender, _, payload) = remotePacket;
				if (sender != algorithm) return;
				var stream = new MemoryStream(payload);
				_drive(stream.ReadFloat(),  // a
				       stream.ReadFloat(),  // b
				       stream.ReadFloat(),  // c
				       stream.ReadFloat(),  // d
				       stream.ReadFloat(),  // e
				       stream.ReadFloat(),  // f
				       stream.ReadFloat()); // det
			}

			public IReadOnlyCollection<byte> Interest => InterestList;
		}

		/// <summary>
		///     响应算法指令
		/// </summary>
		private class CoreCmdProcessor : IMailListener {
			private readonly Action<string> _action;

			public CoreCmdProcessor(Action<string> drive) => _action = drive;

			public void Process(string sender, byte[] payload)
				=> payload.TakeIf(_ => sender == algorithm)
				         ?.GetString()
				          .Also(_action);
		}

		private enum Command : byte {
			// Simulation
			SPose = 64, // 模拟位姿

			// Algorithm
			AVelocity   = 65, // 算法目标速度
			APose       = 66, // 算法位姿
			ATrans      = 67, // 算法变换矩阵
			AList       = 68, // 算法变换矩阵
			ATargetList = 69
		}
	}
}