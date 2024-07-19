using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private int robotColorIndex;

    private bool direction;

    private float speed = 5.0f;

    public Sprite redSprite;
    public Sprite blueSprite;
    public Sprite greenSprite;
    public Sprite yellowSprite;


    BrobotType typeOfRobot;

    public GameObject[] robotPrefabs;

    public float prefabSpawner;

    public float prefabIndex;

    private bool isTriggered;

    // Start is called before the first frame update
    void Start()
    {
        isTriggered = false;

        spriteRenderer = GetComponent<SpriteRenderer>();

        robotColorIndex = UnityEngine.Random.Range(0, 4);

        prefabSpawner = UnityEngine.Random.Range(1, 3);

        prefabIndex = UnityEngine.Random.Range(0, 4);

        if (robotColorIndex == 0)
        {
            spriteRenderer.sprite = redSprite;
        }

        if (robotColorIndex == 1)
        {
            spriteRenderer.sprite = blueSprite;
        }

        if (robotColorIndex == 2)
        {
            spriteRenderer.sprite = greenSprite;
        }

        if (robotColorIndex == 3)
        {
            spriteRenderer.sprite = yellowSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -10.0f, 10.0f),
            Mathf.Clamp(transform.position.y, -10.0f, 10.0f));

        float horizontalMovement = Input.GetAxis("Horizontal");

        if (horizontalMovement <= -1)
        {
            direction = false;
        }

        if (horizontalMovement >= 1)
        {
            direction = true;
        }

        Vector2 moveHorizontally = new Vector2(horizontalMovement, 0.0f);

        transform.Translate(moveHorizontally * speed * Time.deltaTime);

        prefabSpawner += Time.deltaTime;

        prefabIndex = UnityEngine.Random.Range(0, 4);

        if (prefabSpawner >= 3.0f)
        {
            if (prefabIndex == 0)
            {
                Instantiate(robotPrefabs[0]);
                prefabSpawner = UnityEngine.Random.Range(1, 3);
            }

            if (prefabIndex == 1)
            {
                Instantiate(robotPrefabs[1]);
                prefabSpawner = UnityEngine.Random.Range(1, 3);
            }

            if (prefabIndex == 2)
            {
                Instantiate(robotPrefabs[2]);
                prefabSpawner = UnityEngine.Random.Range(1, 3);
            }

            if (prefabIndex == 3)
            {
                Instantiate(robotPrefabs[3]);
                prefabSpawner = UnityEngine.Random.Range(1, 3);
            }
        }

        // Change the player to a blue sprite
        if (isTriggered == true && Input.GetKeyDown(KeyCode.B) &&
            Vector2.Distance(transform.position, robotPrefabs[0].transform.position) <= 0.1f)
        {
            Destroy(robotPrefabs[0]);

            spriteRenderer.sprite = blueSprite;
        }

        if (isTriggered == true && Input.GetKeyDown(KeyCode.G) &&
            Vector2.Distance(transform.position, robotPrefabs[1].transform.position) <= 0.1f)
        {
            Destroy(robotPrefabs[1]);

            spriteRenderer.sprite = greenSprite;
        }

        if (isTriggered == true && Input.GetKeyDown(KeyCode.R) &&
            Vector2.Distance(transform.position, robotPrefabs[2].transform.position) <= 0.1f)
        {
            Destroy(robotPrefabs[2]);

            spriteRenderer.sprite = redSprite;
        }

        if (isTriggered == true && Input.GetKeyDown(KeyCode.Y) &&
            Vector2.Distance(transform.position, robotPrefabs[3].transform.position) <= 0.1f)
        {
            Destroy(robotPrefabs[3]);

            spriteRenderer.sprite = yellowSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BlueRobot")
        {
            isTriggered = true;
            Debug.Log("Blue");
        }

        if (collision.gameObject.tag == "GreenRobot")
        {
            isTriggered = true;
            Debug.Log("Green");
        }

        if (collision.gameObject.tag == "RedRobot")
        {
            isTriggered = true;
            Debug.Log("Red");
        }

        if (collision.gameObject.tag == "YellowRobot")
        {
            isTriggered = true;
            Debug.Log("Yellow");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BlueRobot")
        {
            isTriggered = true;
            Debug.Log("Blue");
        }

        if (collision.gameObject.tag == "GreenRobot")
        {
            isTriggered = true;
            Debug.Log("Green");
        }

        if (collision.gameObject.tag == "RedRobot")
        {
            isTriggered = true;
            Debug.Log("Red");
        }

        if (collision.gameObject.tag == "YellowRobot")
        {
            isTriggered = true;
            Debug.Log("Yellow");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BlueRobot")
        {
            isTriggered = false;
            Debug.Log("Blue");
        }

        if (collision.gameObject.tag == "GreenRobot")
        {
            isTriggered = false;
            Debug.Log("Green");
        }

        if (collision.gameObject.tag == "RedRobot")
        {
            isTriggered = false;
            Debug.Log("Red");
        }

        if (collision.gameObject.tag == "YellowRobot")
        {
            isTriggered = false;
            Debug.Log("Yellow");
        }
    }

    public enum BrobotType
    {
        yellow,
	    green,
	    blue,
	    red
    }

}
