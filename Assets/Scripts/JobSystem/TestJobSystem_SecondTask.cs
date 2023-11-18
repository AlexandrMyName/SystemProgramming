using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;


namespace TestJobSystem.one
{

    public class TestJobSystem_SecondTask : MonoBehaviour
    {
         
        private NativeArray<Vector3> _positions;

        private NativeArray<Vector3> _velocities;

        private NativeArray<Vector3> _finalVector;

        private JobHandle _handle;


        private void Start()
        {

            Debug.LogWarning("Second Task");
            var positions = new Vector3[3];
            var velocities = new Vector3[3];
            var finalVector = new Vector3[3];

            for (int i = 0; i < 3; i++)
            {
                positions[i] = Vector3.one;
                velocities[i] = Vector3.one;
                finalVector[i] = Vector3.zero;
            }

            _positions = new NativeArray<Vector3>(positions, Allocator.TempJob);
            _velocities = new NativeArray<Vector3>(velocities, Allocator.TempJob);
            _finalVector = new NativeArray<Vector3>(finalVector, Allocator.TempJob);

            var  plusVectorJob = new PlusVectorJob()
            {
                Positions = _positions,
                Velocities = _velocities,
                FinalVector = _finalVector,
            };
           
            _handle = plusVectorJob.Schedule(3, 0);
            
            _handle.Complete();
            
            foreach (var result in _finalVector)
            {
                 
                Debug.Log(result);
            }

            _positions.Dispose();
            _velocities.Dispose(); 
            _finalVector.Dispose();
            Debug.LogWarning("_______________");
        }
          
    }


    [BurstCompile]
    public struct PlusVectorJob : IJobParallelFor
    {

        [ReadOnly]
        public NativeArray<Vector3> Positions;
        [ReadOnly]
        public NativeArray<Vector3> Velocities;
        [WriteOnly]
        public NativeArray<Vector3> FinalVector;

        public void Execute(int index)
        {
            
            FinalVector[index] = Velocities[index] + Positions[index];
        }
    }
}