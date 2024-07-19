using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedRobot : MonoBehaviour
{
    Vector2 nearRightSideOfScreen;
    Vector2 farRightSideOfScreen;

    public Vector2 speed = new Vector2(-5.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        nearRightSideOfScreen = new Vector2(10.0f, -3.0f);
        farRightSideOfScreen = new Vector2(30.0f, -3.0f);

        transform.position = new Vector2(Random.Range(nearRightSideOfScreen.x, farRightSideOfScreen.x),
            Random.Range(nearRightSideOfScreen.y, farRightSideOfScreen.y));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)speed * Time.deltaTime;

        if (transform.position.x <= -12.0f)
        {
            Destroy(gameObject);
        }
    }
}
