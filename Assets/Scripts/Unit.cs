using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace AsyncCoroutine
{

    public class Unit : MonoBehaviour
    {

        [SerializeField] private float _health = 100f;
        [SerializeField,Tooltip("Milliseconds")] private int _delayRecievingHealth = 500;
        [SerializeField] private float _gettableHealth = 5f;
        private bool _isProccessing;
        private float _currentSecondsForRecieving;


        private void Start() => RecieveHealing();
        
         
        //initializer
        private async void RecieveHealing()
        {

            if (_isProccessing) return;
            StartCoroutine(RecieveHealthWithDelay(_delayRecievingHealth, _gettableHealth));

            CancellationToken token = GetToken();

            var result = await WhatTaskFasterAsync(token, AsyncSecond(token), Async60Frames(token));

            string resultTaskCompleted = result == false ? "first" : "second";
            Debug.LogWarning($"Task completed first : {resultTaskCompleted}");

        }


        private static CancellationToken GetToken()
        {
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken token = cancellationTokenSource.Token;
            return token;
        }


        //first task
        private IEnumerator RecieveHealthWithDelay(int delayMilliseconds, float gettableHealth)
        {
             
            _isProccessing = true;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while(_currentSecondsForRecieving < 3.0f || _health < 100.00f)
            {
                yield return new WaitForSeconds(delayMilliseconds / 1000);

                _health += gettableHealth;
                _currentSecondsForRecieving += delayMilliseconds / 1000;

                if(_health >= 100f || _currentSecondsForRecieving >= 3.0f)
                {
                    _health = 100f;
                    _currentSecondsForRecieving = 0;
                    break;
                }
            }
           
            _isProccessing = false;
            stopWatch.Stop();
            Debug.Log($"GettableHealth : {gettableHealth} | Current : {_health}  Time : {stopWatch.Elapsed}");
        }


        //Second task
        private async Task<int> AsyncSecond(CancellationToken token)
        {

            int millisecondsDelay = 1000;

            await Task.Delay(millisecondsDelay,token);

            Debug.Log($"Async Second Task is completed!");
            return millisecondsDelay;
        }


        //Second task
        private async Task<int> Async60Frames(CancellationToken token)
        {

            int frames = 0;

            while (frames < 60)
            {
                await Task.Yield();
                frames++;
                if (token.IsCancellationRequested) break;
            }
            Debug.Log($"Async 60 Frames Task is completed!");
            return frames;
        }
        

        //third task
        private async Task<bool> WhatTaskFasterAsync(CancellationToken token, Task firstTask, Task secondTask)
        {

            bool result = false;
            await Task.Factory.StartNew(async _ => {
                Task completedTask = await Task.WhenAny(firstTask, secondTask);
                if(completedTask == firstTask)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            },token); 
            return result;
        }
    }
}