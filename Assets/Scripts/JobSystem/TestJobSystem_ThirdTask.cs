using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System.Collections.Generic;
using UnityEngine.Jobs;
using System.Runtime.CompilerServices;

namespace TestJobSystem.one
{

    public class TestJobSystem_ThirdTask : MonoBehaviour
    {

        [SerializeField] private List<Transform> _transformsRotations = new();
         
        [SerializeField] private float _speed = 10f;
        [SerializeField] private Vector3 _axisRotate = Vector3.forward;
        [SerializeField] private GameObject _prefab;
        private TransformAccessArray _transformAccesArray;

        public int CountObjects;


        private void OnDestroy()
        {
            _transformAccesArray.Dispose();
        }


        private void Start()
        {
             
            for(int i = 0; i < CountObjects; i++)
            {
               var sgameobject =  GameObject.Instantiate(_prefab,this.transform);
                Vector3 initialPosition = Vector3.zero;
                initialPosition += Vector3.right * Random.Range(-10, 10);
                initialPosition += Vector3.up * Random.Range(-10, 10);
                initialPosition += Vector3.forward * Random.Range(-10, 10);
                sgameobject.transform.position = initialPosition;
                sgameobject.GetComponent<Collider>().enabled = false;
                _transformsRotations.Add(sgameobject.transform);
            }
            _transformAccesArray = new TransformAccessArray(_transformsRotations.ToArray());

        }


        private void Update()
        {
            JobsSystemRotations();

            //UpdateRotations();
        }


         
        private void JobsSystemRotations()
        {
            RotateJob rotateJob = new RotateJob() { Axis = _axisRotate, Delta = Time.deltaTime };

            var handle = rotateJob.Schedule(_transformAccesArray);
        }


        
        private void UpdateRotations()
        {
            for (int i = 0; i < _transformsRotations.Count; i++)
            {

                Quaternion quaternion = Quaternion.Euler(_axisRotate.x, _axisRotate.y, _axisRotate.z);

                _transformsRotations[i].rotation *= Quaternion.Inverse(_transformsRotations[i].rotation) * quaternion * _transformsRotations[i].rotation;
                _transformsRotations[i].position += Vector3.right / 2 * Time.deltaTime;

            }
        }
    }



    [BurstCompile]
    public struct RotateJob : IJobParallelForTransform
    {
         
        public Vector3 Axis;
        public float Delta;

        public void Execute(int index, TransformAccess transform)
        {
            
            Quaternion quaternion = Quaternion.Euler(Axis.x, Axis.y, Axis.z);

            transform.rotation *= Quaternion.Inverse(transform.rotation) * quaternion * transform.rotation;
            transform.position += Vector3.right/2 * Delta;
        }
    }

}