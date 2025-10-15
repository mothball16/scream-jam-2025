using UnityEngine;

public class CarMove : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;

    //Random to give illusion of different cars passing by
    [Header("Settings")]
    public float minSpeed = 2f;
    public float maxSpeed = 6f;
    public float minWaitTime = 0.5f;
    public float maxWaitTime = 2f;

    private float speed;
    private float waitTimer;
    private float waitDuration;
    private bool isWaiting = false;

    void Start()
    {
        //start at Point A
        transform.position = pointA.position;
        //pick a random speed and wait time
        speed = Random.Range(minSpeed, maxSpeed);
        waitDuration = Random.Range(minWaitTime, maxWaitTime);
    }

    void Update()
    {
        if (isWaiting)
        {
            //count down wait timer
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                //done waiting, reset and start moving again
                isWaiting = false;
                speed = Random.Range(minSpeed, maxSpeed);
            }
            return;
        }

        //move toward Point B
        transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);

        //when reached Point B
        if (Vector3.Distance(transform.position, pointB.position) < 0.05f)
        {
            //instantly teleport back to Point A
            transform.position = pointA.position;

            // start waiting before next move
            isWaiting = true;
            waitTimer = 0f;
            waitDuration = Random.Range(minWaitTime, maxWaitTime);
        }
    }
}