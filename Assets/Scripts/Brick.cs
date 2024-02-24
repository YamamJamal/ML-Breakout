using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private float[] observations = new float[3]; // [brick_x, brick_y, if_hit_by_ball(0 = not visited | 1 = visited)]

    void Start()
    {
        observations[0] = transform.localPosition.x; 
        observations[1] = transform.localPosition.y;
        observations[2] = 0;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name == "Ball")
        {
            SetVisited(1);
        }
    }

    public void SetVisited(float visited)
    {
        observations[2] = visited;
    }

    public float[] GetObservations()
    {
        return observations;
    }
}
