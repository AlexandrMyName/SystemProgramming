using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.VisualScripting;


namespace TestJobSystem.one
{

    public class TestJobSystem_OneTask : MonoBehaviour
    {

        [SerializeField] private int _randomNumbers = 100;
        private NativeArray<int> _intArray;

        
        private void Start()
        {

            Debug.LogWarning("First Task");

            _intArray = new NativeArray<int>(_randomNumbers, Allocator.TempJob);
              
            ArrayRandomIntBuilder arrayRandomIntBuilder = new ArrayRandomIntBuilder(_intArray,_randomNumbers);
            ArraySnipInt arraySnipInt = new ArraySnipInt(_intArray, 10);

            var randomHandle = arrayRandomIntBuilder.Schedule();
            randomHandle.Complete();
            var jobMainSnipper = arraySnipInt.Schedule(randomHandle);
            jobMainSnipper.Complete();

            DebugResultToConsole();

            _intArray.Dispose();
            Debug.LogWarning("_______________");
        }
         

        private void DebugResultToConsole()
        {
            int countSlicable = 0;
            foreach(var number in _intArray)
            {
                if (number == 0)
                {
                    countSlicable++;
                }
                else
                {
                    Debug.Log($" Object default : {number}");
                }
            }
            Debug.Log($"Objects was modified to zero : {countSlicable}");
        }

    }


    [BurstCompile]
    public struct ArraySnipInt : IJob
    {

        public NativeArray<int> _array;
        private int _sliceAfter;
        [WriteOnly]
        public int CountModifier;

        public ArraySnipInt(NativeArray<int> array, int sliceAfter) {
        
            _array = array;
            _sliceAfter = sliceAfter;
            CountModifier = 0;
        }


        public void Execute()
        {
            
            for(int i = 0; i < _array.Length; i++)
            {

                var item = _array[i];
               
                if(_array[i] > _sliceAfter)
                {
                    _array[i] = 0;
                    CountModifier++;
                }    

            }
           
        }
       
    }


    [BurstCompile]
    public struct ArrayRandomIntBuilder : IJob
    {

        public NativeArray<int> _array;
        private int _randomNumbers;

        public ArrayRandomIntBuilder(NativeArray<int> array, int randomNumbers)
        {

            _array = array;
            _randomNumbers = randomNumbers;
        }


        public void Execute()
        {

            for (int i = 0; i < _array.Length; i++)
            {

                var item = _array[i];
                _array[i] = i;
                 
            }
        }
    }
}