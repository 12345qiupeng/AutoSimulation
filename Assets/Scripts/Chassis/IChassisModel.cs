using System.Collections.Generic;
using UnityEngine;


namespace Chassis
{
    /// <summary>
    ///     车辆运动模型
    /// </summary>
    /// <remarks>
    ///     属性：控制信号、轴距、轮距
    ///     方法：车辆轨迹预测
    /// </remarks>
    public interface IChassisModel
    {
        double Omega { get; }
        double Velocity { get; }
        IEnumerable<Vector3> Trajectory(double period);
    }
}