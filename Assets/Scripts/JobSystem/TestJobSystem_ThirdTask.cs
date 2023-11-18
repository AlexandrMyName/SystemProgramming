using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System.Collections.Generic;
using UnityEngine.Jobs;

namespace TestJobSystem.one
{

    public class TestJobSystem_ThirdTask : MonoBehaviour
    {

        [SerializeField] private List<Transform> _transformsRotations = new();
        [SerializeField] private float _speed = 10f;
        [SerializeField] private Vector3 _axisRotate = Vector3.forward;

        private TransformAccessArray _transformAccesArray;

       
        private void Start()
        {

            _transformAccesArray = new TransformAccessArray(_transformsRotations.ToArray());
  
        }


        private void Update()
        {

            RotateJob rotateJob = new RotateJob() { Axis = _axisRotate, };
           
            var handle = rotateJob.Schedule(_transformAccesArray);
           
        }

    }


    [BurstCompile]
    public struct RotateJob : IJobParallelForTransform
    {
         
        public Vector3 Axis;


        public void Execute(int index, TransformAccess transform)
        {
            
            Quaternion quaternion = Quaternion.Euler(Axis.x, Axis.y, Axis.z);

            transform.rotation *= Quaternion.Inverse(transform.rotation) * quaternion * transform.rotation;
          
        }
    }

}