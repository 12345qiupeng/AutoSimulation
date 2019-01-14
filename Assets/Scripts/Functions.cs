using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MechDancer.Common;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public static class Functions {
	/// <summary>
	///     从流读取一个（反字节序的）单精度浮点数
	/// </summary>
	/// <param name="receiver">流</param>
	/// <returns>浮点数</returns>
	public static float ReadFloat(this Stream receiver)
		=> BitConverter.ToSingle(receiver.WaitReversed(4), 0);

	public static T Write<T>(this T receiver, float value) where T : Stream
		=> receiver.Also(it => it.WriteReversed(BitConverter.GetBytes(value)));

	

	public static void Launch(Func<bool> cancel, Action action) =>
		new Thread(() => {
			           while (!cancel()) action();
		           }) {IsBackground = true}.Start();

	public static void Launch(TimeSpan period, Func<bool> cancel, Action action) =>
		new Thread(() => {
			           while (!cancel()) {
				           Task.Run(action);
				           Thread.Sleep(period);
			           }
		           }) {IsBackground = true}.Start();

	/// <summary>
	///     直接设置一个物体的位姿
	/// </summary>
	/// <param name="receiver"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="h"></param>
	/// <param name="θ"></param>
	public static void SetPose(this Transform receiver,
	                           float          x,
	                           float          y,
	                           float?         h,
	                           float          θ) {
		θ = float.IsNaN(θ) ? 0f : -θ * 180 / Mathf.PI;
		var rotation = receiver.rotation.eulerAngles;
		receiver.SetPositionAndRotation
			(new Vector3(x, h ?? receiver.position.y, y),
			 Quaternion.Euler(rotation.x, θ, rotation.z));
	}

	public static GameObject LoadObject(string name) =>
		(GameObject) Object.Instantiate(Resources.Load(name));

	/// <summary>
	/// 	删除所有子节点
	/// </summary>
	/// <param name="root"></param>
	public static void KillChildren(this Transform root) {
		foreach (var obj in from it in root.GetComponentsInChildren<Transform>()
		                    where it != root
		                    select it.gameObject)
			Object.Destroy(obj.gameObject);
	}

    public static float AdjustToNormalize(this float value)
    {
        // float result=0;
        //  while (value > Mathf.PI) value -= Mathf.PI * 2;
        //  while (value < Mathf.PI) value -= Mathf.PI * 2;
        return 0f;
    }


}