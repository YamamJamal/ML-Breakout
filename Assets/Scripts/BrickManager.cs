using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    public LevelGenerator levelGenerator; 
    private int DestroyedBricks = 0;
    public Ball ball;

    public void BrickCount()
    {
        ++DestroyedBricks;
        if(DestroyedBricks == (levelGenerator.size.x * levelGenerator.size.y))
        {
            
            RespawnBricks();
            DestroyedBricks = 0;
            ball.Kill();
        }
        // UnityEngine.Debug.Log(DestroyedBricks);
    }

    public void RespawnBricks()
    {
        levelGenerator.ReBrick();
    }
}

