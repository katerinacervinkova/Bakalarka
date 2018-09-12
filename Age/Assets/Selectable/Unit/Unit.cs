using Age;
using UnityEngine;

public class Unit : Selectable
{
    public float moveSpeed, rotateSpeed;
    protected bool moving = false, rotating = false;

    private Vector3 destination;
    private Quaternion targetRotation;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {

    }
    protected void FixedUpdate()
    {
        base.Update();
        if (rotating)
            TurnToTarget();
        else if (moving)
            MakeMove();
    }

    private void MakeMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * moveSpeed);
        if (transform.position == destination) moving = false;
    }


    private void TurnToTarget()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
        Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
        if (transform.rotation == targetRotation || transform.rotation == inverseTargetRotation)
        {
            rotating = false;
            moving = true;
        }
    }

    public override void RightMouseClick(GameObject hitObject, Vector3 hitPoint)
    {
        if (hitObject.name == "Map" && hitPoint != GameWindow.InvalidPosition)
        {
            hitPoint.y = 0;
            StartMove(hitPoint);
        }
        else
        {

        }
    }
    private void StartMove(Vector3 destination)
    {
        this.destination = destination;
        targetRotation = Quaternion.LookRotation(destination - transform.position);
        rotating = true;
        moving = false;
    }
}