

using System;
using System.Collections.Generic;
using MechDancer.Common;
using UnityEngine;

namespace Chassis
{

    public class ThreeWheelVehicle : IChassisModel
    {

        private const double MaxWheelSpeed = 2.0;

        private const double MaxV = MaxWheelSpeed;

        private const double MaxW = 2 * MaxWheelSpeed / Track;

        private const double WheelBase = 0.280;

        private const double Track = 0.543;

        private const double InternalPeriod = 0.01;    //s

        private readonly bool _isVw=true;


        public ThreeWheelVehicle(double v, double w)
        {
            Velocity = v;
            Omega = w;
            Rho = 0;
            Theta = 0;
            BackOmega = 0;
            _isVw = true;
        }
        
        public ThreeWheelVehicle(double rho, double theta, double omega)
        {
            Rho = rho;
            Theta = theta;
            BackOmega = omega;

            var rTheta = -WheelBase / Math.Tan(Theta);
            var sign = rTheta >= 0 ? 1 : -1;
            var thetaRho = Math.Atan(MaxW / MaxV * rTheta);
            
            Velocity = MaxV * sign * Rho * Math.Sin(thetaRho);
            Omega = MaxW * sign * Rho * Math.Cos(thetaRho);
            _isVw = false;
        }

        
        private double BackOmega { get; }
        public double Rho { get; private set; }
        public double Theta { get; private set; }

        public double Velocity { get; private set; }

        public double Omega { get; private set; }

        /// <summary>
        ///     将Rho，Theta，Omega转换到车辆线速度以及角速度
        /// </summary>
        private void ChangeRTOToVW()
        {
            Theta +=  InternalPeriod * BackOmega;
            var rTheta = -WheelBase / Math.Tan(Theta);
            var sign = rTheta >= 0 ? 1 : -1;
            var thetaRho = Math.Atan(MaxW / MaxV * rTheta);

            Velocity = MaxV * sign * Rho * Math.Sin(thetaRho);
            Omega = MaxW * sign * Rho * Math.Cos(thetaRho);

        }
        /// <summary>
        ///     将车辆线速度以及角速度转换到Rho，Theta，Omega
        /// </summary>
        private void ChangeVWToRT()
        {
            var theta = Math.Atan(-WheelBase / (Velocity / Omega));
            var rTheta = -WheelBase / Math.Tan(theta);
            var sign = rTheta >= 0 ? 1 : -1;
            var thetaRho = Math.Atan(MaxW / MaxV * rTheta);
            var rho = Velocity / MaxV / sign / Math.Sin(thetaRho);

            Theta = theta;
            Rho = rho;
        }
        
        /// </summary>
        ///     按照给定周期输出以当前位置为原点的运动
        /// </summary>
        public IEnumerable<Vector3> Trajectory(double period)
        {
            var carCoordinates = Vector3.zero;
            var num = Math.Floor(period / InternalPeriod);
            while(true)
            {
                for (int i = 0; i < num; i++)
                {
                    double radius=0;
                    if (!_isVw)
                        ChangeRTOToVW();
                    else
                        ChangeVWToRT();
                    if (Math.Abs(Omega) < 0.01)
                    {
                        radius = Double.PositiveInfinity;
                    }
                    else
                    {
                        radius = Velocity / Omega;
                    }
                
                    if (!double.IsPositiveInfinity(radius))
                    {
                        var deltaOmega = InternalPeriod * Omega;
                        var carX = radius * Math.Sin(deltaOmega);
                        var carY = radius * (1 - Math.Cos(deltaOmega));
                    
                        var currentPose = carCoordinates.z;
                        var wrdX = carX * Math.Cos(currentPose) - carY * Math.Sin(currentPose);
                        var wrdY = carX * Math.Sin(currentPose) + carY * Math.Cos(currentPose);
                    
                        carCoordinates.x += (float) wrdX;
                        carCoordinates.y += (float) wrdY;
                        carCoordinates.z += (float) deltaOmega;

                    }
                    else
                    {
                        var temps = Velocity * InternalPeriod;
                        var tempx = temps * Math.Cos(carCoordinates.z);
                        var tempy = temps * Math.Sin(carCoordinates.z);

                        carCoordinates.x += (float) tempx;
                        carCoordinates.y += (float) tempy;
                    
                    }
                }                

                yield return carCoordinates;
            }


        }
    }
}