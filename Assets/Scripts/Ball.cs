using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    public GameObject Paddle;
    
    Paddle_Agent paddle_agent_script;
    public Transform paddle_transform;
    public float force = 100.0f;
    bool death_flag = true;
    Rigidbody2D rb;
    public BrickManager brickManager;

    private int score = 0;
    public GameObject youWinPanel;
    int brickCount;

    int lives = 5;
    public GameObject[] livesImage;
    public GameObject gameOverPanel;
    


    // Used to track the score for each environment
    [SerializeField] private TextMeshProUGUI scoreText;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
        brickCount = FindObjectOfType<LevelGenerator>().transform.childCount;
    
    }


// Update is called once per frame
    void Update()
    {
        if (lives<=0){
            GameOver();
        }
        if (transform.position.y < -5)
        {
            rb.velocity = Vector2.zero; // Reset velocity to zero
            transform.localPosition = new Vector2(0f, 15.5f);
            lives--;
            livesImage[lives].SetActive(false);
            gameObject.SetActive(false);
            death_flag = true;
        
        }

    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.CompareTag("Brick"))
        {
            other.gameObject.SetActive(false);
            score += 10;
            scoreText.text = score.ToString("00000");
            brickManager.BrickCount();  
            brickCount--;
            if (brickCount <= 0)
            {
                youWinPanel.SetActive(true);
                Time.timeScale = 0;
                 
            }  
        }
    }

    

    // Shoots the ball up from its current position, puts the ball into play
    public void Shoot()
    {
        gameObject.SetActive(true);
        float randomX = Random.Range(-9f, 9f);
        int randomDirection = Random.Range(0, 2);
        transform.localPosition = new Vector2(randomX, paddle_transform.localPosition.y + 10f);
        rb.velocity = Vector3.zero;
        float forceX = Mathf.Sin(Mathf.Deg2Rad * 45) * force;
        if (randomDirection == 0) forceX *= -1;
        float forceY = Mathf.Cos(Mathf.Deg2Rad * 45) * -force;
        rb.AddForce(new Vector2(forceX, forceY));
        death_flag = false;
    }

    // "Kills" the ball, sets the death_flag so that the ball will attach itself to the paddle
    public void Kill()
    {
        gameObject.SetActive(false);
        death_flag = true;

    }

    // Returns wither of not the ball is "alive" or currently in play
    public bool IsDead()
    {
        return death_flag;
    }

    // Needed for score tracking of each env
    public int GetScore()
    {
        return score;
    }

    void GameOver()
    {
        Debug.Log("Game Over");
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        Destroy(gameObject);
        
    }


}
