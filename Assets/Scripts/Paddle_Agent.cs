using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents; 
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;

public class Paddle_Agent : Agent
{
    public GameObject ball;
    public LevelGenerator lg_script;
    public float speed = 20.0f;
    Vector3 velocity = Vector3.zero;
    Ball ball_script;
    Rigidbody2D ball_rb;
    Transform ball_transform;

    float ball_bounce_angle; // angle the ball flys in after hitting the paddle
    Collider2D paddle_collider;
    Vector3 ball_collide_position = Vector3.zero;
    Vector3 brick_position_hit_by_raycast = Vector3.zero;






    //for calculating bounce angle w/0 unity physics
    public float maxBounceAngle = 75f;

    // Start is called before the first frame update
    void Start()
    {
        ball_script = ball.GetComponent<Ball>();
        ball_rb = ball.GetComponent<Rigidbody2D>();
        ball_transform = ball.GetComponent<Transform>();

        paddle_collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {


    }

  


    



    

    public override void OnEpisodeBegin()
    {}

    public override void CollectObservations(VectorSensor sensor)
    {
        // To normalize the observations - clamp values to be between 0 to 1
        float minX = -8.7f;
        float maxX = 8.7f;
        float minY = -5.0f;
        float maxY = 15.0f;

        sensor.AddObservation((ball_transform.localPosition.x - minX) / (maxX - minX)); // Ball position x
        sensor.AddObservation((ball_transform.localPosition.y - minY) / (maxY - minY)); // Ball position y
        sensor.AddObservation(ball_rb.velocity.x / 100.0f); // Ball velocity x
        sensor.AddObservation(ball_rb.velocity.y / 100.0f); // Ball velocity y

        sensor.AddObservation((transform.localPosition.x - minX) / (maxX - minX)); // Agent position x
        sensor.AddObservation((transform.localPosition.y - minY) / (maxY - minY)); // Agent position y
        sensor.AddObservation(velocity.x); // Agent velocity x
        sensor.AddObservation(velocity.y); // Agent velocity x

        sensor.AddObservation(ball_bounce_angle / 180); // Ball bounce angle (should be 360?)
        sensor.AddObservation((ball_collide_position.x - minX) / (maxX - minX)); // Paddle | Ball collision position x
        sensor.AddObservation((ball_collide_position.y - -4.3f) / (-3.7f - -4.3f)); // Paddle | Ball collision position y

        sensor.AddObservation((brick_position_hit_by_raycast.x - minX) / (maxX - minX)); // Hit brick position x
        sensor.AddObservation((brick_position_hit_by_raycast.y - minY) / (maxY - minY)); // Hit brick position y

        float distance_to_left_wall = Mathf.Sqrt(Mathf.Pow(transform.localPosition.x - minX, 2)) / maxX;
        sensor.AddObservation(distance_to_left_wall); // Distance to left wall
        float distance_to_right_wall = Mathf.Sqrt(Mathf.Pow(transform.localPosition.x - maxX, 2)) / maxX;
        sensor.AddObservation(distance_to_right_wall); // Distance to right wall

        float distanceToBallX = Mathf.Sqrt(Mathf.Pow(transform.localPosition.x - ball_transform.localPosition.x, 2));
        sensor.AddObservation((distanceToBallX - minX) / (maxX - minX)); // Distance between paddle x and ball x

        sensor.AddObservation(ball_script.IsDead()); // Is Ball dead?

        foreach (GameObject brick in lg_script.GetBricks())
        {
            sensor.AddObservation((brick.transform.localPosition.x - minX) / (maxX - minX)); // Brick position x
            sensor.AddObservation((brick.transform.localPosition.y - minY) / (maxY - minY)); // Brick position y
            sensor.AddObservation(brick.GetComponent<Brick>().GetObservations()[2]); // If brick has been visited (is active or not)
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Agent Actions
        // Give x-axis movement to Agent
        velocity = Vector3.zero;
        velocity.x = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1.0f, 1.0f);
        Vector3 moveTo = new Vector3(transform.localPosition.x + velocity.x * speed * Time.deltaTime, transform.localPosition.y, 0);
        if (moveTo.x > -8.7f && moveTo.x < 8.7f) transform.localPosition = moveTo;

        // Give "shooting" action to Agent
        int shoot_control = 0;
        shoot_control = actionBuffers.DiscreteActions[0];
        if (shoot_control > 0 && ball_script.IsDead())
        { // if the ball is "dead" (attached to the paddle), allow the ball to be "shot"
            AddReward(0.005f); // encourge the agent to actually shoot the ball
            ball_script.Shoot();
        }

        // if the distance between paddle x and ball x is less then 1, give tha agent a reward
        float distanceToBallX = Mathf.Sqrt(Mathf.Pow(transform.localPosition.x - ball_transform.localPosition.x, 2));
        if (distanceToBallX < 1.0f && !ball_script.IsDead()) 
        {
            AddReward(0.01f);
        }

        if (ball_transform.localPosition.y < -5.0f)
        { // If the ball goes off screen, give a penalty and reset the ball
            
            SetReward(-1.0f);
            ball_script.Kill();
            EndEpisode();
        }


    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name == "Ball" && !ball_script.IsDead())
        { // If the paddle collides with the ball, end episode and give reward
            AddReward(0.5f);

            // Determine Ball angle and velocity
            ball_collide_position = other.GetContact(0).point;

            float offset = transform.position.x - ball_collide_position.x;
            float width = paddle_collider.bounds.size.x / 2;

            float currentAngle = Vector2.SignedAngle(Vector2.up, ball.GetComponent<Rigidbody2D>().velocity);
            float bounceAngle = (offset / width) * maxBounceAngle;
            ball_bounce_angle = Mathf.Clamp(currentAngle + bounceAngle, -maxBounceAngle, maxBounceAngle);

            Quaternion rotation = Quaternion.AngleAxis(ball_bounce_angle, Vector3.forward);
            ball.GetComponent<Rigidbody2D>().velocity = rotation * Vector2.up * 10f; //10f feels pretty close here for a velocity

            // Use ray case to determine if the ball is going to hit a brick
            RaycastHit2D hit = Physics2D.Raycast(ball_transform.position, ball_rb.velocity);
            // uncomment the next 2 lines to turn on raycast debugging
            // Vector2 start = new Vector2(ball_transform.position.x, ball_transform.position.y);
            // Debug.DrawRay(start, hit.point - start, Color.green, 2, false); // show raycast as green line for debugging
            if (hit.collider != null)
            {
                brick_position_hit_by_raycast = hit.transform.localPosition;
                if (hit.collider.tag == "Brick")
                {
                    hit.collider.GetComponent<Brick>().SetVisited(1);
                    SetReward(1.0f);
                }
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allow user to move Agent with keyboard
        // Move with "ad" or the left and right arrows
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");

        // Shoot with the space bar
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetButton("Shoot")) discreteActionsOut[0] = 1;
        else discreteActionsOut[0] = 0;
    }


}
