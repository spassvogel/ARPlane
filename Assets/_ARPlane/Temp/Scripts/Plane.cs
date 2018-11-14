using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Plane : MonoBehaviour
{

    public Transform target;
    public Transform cam;
    public float maxSpeed = 10.0f;
    public float maxSpeedDistance = 10;
    public float rotateSpeed = 1f;
    public float aimingDistance = 1f;

    Rigidbody body;
    Vector3 previousPosition;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        FollowTarget();
        Banking();

        previousPosition = transform.position;
    }

    void FollowTarget()
    {
        if (target == null) return;

        float distance = Vector3.Distance(target.position, transform.position);
        float speed = Mathf.Min(maxSpeed, distance / maxSpeedDistance * maxSpeed);

        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed);

        // Look at target
        Vector3 lookAtTarget = target.position + (target.position - cam.position).normalized * 100 * aimingDistance;
        lookAtTarget.y = cam.rotation.y;

        // The step size is equal to speed times frame time.
        float step = rotateSpeed * 1000 * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookAtTarget), step);

        Debug.DrawLine(transform.position, lookAtTarget, Color.green);
    }

    void Banking()
    {
        Vector3 velocity = transform.InverseTransformDirection(transform.position - previousPosition) * transform.localScale.x / Time.deltaTime;

        // Assumption: velocity.x is max around 2, regardless of max speed
        float banking = -velocity.x * 45;
        Vector3 currentRotation = transform.eulerAngles;


        transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, banking);
    }
}