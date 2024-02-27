using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle_Player : MonoBehaviour
{
    public GameObject ball;
    public LevelGenerator lg_script;
    public float speed = 20.0f;
    Vector3 velocity = Vector3.zero;
    Ball ball_script;
    Collider2D paddle_collider;
    public float maxBounceAngle = 75f;
    // Start is called before the first frame update
    void Start()
    {
        ball_script = ball.GetComponent<Ball>();
        paddle_collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity.x = Input.GetAxis("Horizontal"); // Horizontal input
        // Apply velocity * speed * delta time to local position of the paddle
        Vector3 moveTo = new Vector3(transform.localPosition.x + velocity.x * speed * Time.deltaTime, transform.localPosition.y, 0);
        if (moveTo.x > -8.7f && moveTo.x < 8.7f) transform.localPosition = moveTo; // Bounds

        if (ball_script.IsDead() && Input.GetButton("Shoot")) 
        {
            ball_script.Shoot(); // Shoot input - only when ball is "dead"
            if (Time.timeScale == 0) Time.timeScale = 1; // resume time, allows the player to start at the same time as the AI
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name == "Ball" && !ball_script.IsDead())
        {
            // Determine Ball angle and velocity
            Vector3 ball_collide_position = other.GetContact(0).point;

            float offset = transform.position.x - ball_collide_position.x;
            float width = paddle_collider.bounds.size.x / 2;

            float currentAngle = Vector2.SignedAngle(Vector2.up, ball.GetComponent<Rigidbody2D>().velocity);
            float bounceAngle = (offset / width) * maxBounceAngle;
            float newBounceAngle = Mathf.Clamp(currentAngle + bounceAngle, -maxBounceAngle, maxBounceAngle);

            Quaternion rotation = Quaternion.AngleAxis(newBounceAngle, Vector3.forward);
            ball.GetComponent<Rigidbody2D>().velocity = rotation * Vector2.up * 10f; //10f feels pretty close here for a velocity
        }
    }
}
