using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : NetworkBehaviour
{

    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    private Rigidbody _rigidbody;
   
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();
   

    public override void OnStartLocalPlayer()
    {

        _rigidbody = GetComponent<Rigidbody>();

        base.OnStartLocalPlayer();
    }


    void FixedUpdate()
    {

        if(!isLocalPlayer) return;

        IsRunning = canRun && Input.GetKey(runningKey);

        float targetMovingSpeed = IsRunning ? runSpeed : speed;

        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        _rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, _rigidbody.velocity.y, targetVelocity.y);
    }
}