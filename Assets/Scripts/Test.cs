using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chassis;
using Node;
using UnityEngine;
using UnityEngine.Serialization;

public class Test : MonoBehaviour
{
    
    [FormerlySerializedAs("Line")] public GameObject line;
    [FormerlySerializedAs("Car")] public GameObject car;
    public GameObject arrow;

    // Start is called before the first frame update
    public void Click()
    {
        
        var twVehicle=new ThreeWheelVehicle(1,0,0.1);

        var pose = twVehicle.Trajectory(0.1).Take(200);

        foreach (var p in pose)
        {
            var position = new Vector3(p.x,0,p.y);
            var lastPost= Quaternion.AngleAxis((float)(p.z/Math.PI)*180, Vector3.down);
            
            

            car.GetComponent<Transform>().transform.position = position;
            car.GetComponent<Transform>().transform.rotation = lastPost;
            arrow.GetComponent<PostureContainer>().AddPose(position,lastPost);
        }
        //Line.GetComponent<TrackContainer>().DrawPredictLine(tLine);

        

    }

}
