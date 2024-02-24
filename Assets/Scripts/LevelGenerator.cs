using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int size;
    public Vector2 offset;
    public GameObject brickPrefab;
    public Gradient gradient;
    private GameObject[] BrickArray;

    private void Awake()
    {
        BrickArray = new GameObject[size.x * size.y];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                GameObject newBrick = Instantiate(brickPrefab, transform);
                BrickArray[(i * 5) + j] = newBrick;
                // Debug.Log("Array contents: " + string.Join(", ", BrickArray.Where(item => item != null).Select(item => item.transform.localPosition)));
                newBrick.transform.position = transform.position + new Vector3((float)((size.x-1)*0.5f-i) * offset.x, j * offset.y, 0);
                newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j / (size.y - 1));
            }
        }
    }

    public void ReBrick()
    {  
        foreach (GameObject brick in BrickArray) 
        {
            brick.SetActive(true);
            brick.GetComponent<Brick>().SetVisited(0);
        }
    }

    public GameObject[] GetBricks()
    {
        return BrickArray;
    }

    public void Restart()
    {
        Debug.Log("Restart method called");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
