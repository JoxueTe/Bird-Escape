using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duck_movement : MonoBehaviour
{
    // Move player with click variables
    public float moveSpeed = 10f;
    Vector2 lastClickPos;
    bool moving;
    bool hasBeenShot;
    bool timeToFall;

    // Walkin or iddle variables
    bool walkEnded = true;
    float walkingSpeed = 0.8f;
    float position_to_moveX;

    //Animation variables
    public Animator animator;
    bool isWalking;
    bool gameStarted;
    bool startFlying;

    //Audio
    public AudioSource AudioSrc;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted) walkingStart();

        // Checking when game starts
        if (Input.GetMouseButton(0) && !gameStarted)
        {
            gameStarted = true;
            StartCoroutine(gameStarter());
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(transform.position.x, transform.position.y + 5),
                walkingSpeed * Time.deltaTime);
        }

        if (gameStarted && startFlying) 
        {
            fly();
            AudioSrc.mute = false;

        }

        if (walkEnded)
        {
            StartCoroutine(walking());
        } 

        // Variables to trigger animation
        animator.SetBool("walking", isWalking);
        animator.SetBool("gameStarted", gameStarted);

        StartCoroutine(isMoving());
    }

    void fly()
    {
        // Move placer with click/tap
        if (Input.GetMouseButton(0))
        {
            lastClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moving = true;
        }

        if (moving && (Vector2)transform.position != lastClickPos)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, lastClickPos, step);
        }
        else
        {
            moving = false;
        }

        //Flip character
        Vector3 characterScale = transform.localScale;
        if (transform.position.x > lastClickPos.x) characterScale.x = 1;
        if (transform.position.x < lastClickPos.x) characterScale.x = -1;
        transform.localScale = characterScale;
    }

    void walkingStart()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector2(position_to_moveX, transform.position.y),
            walkingSpeed * Time.deltaTime);

        

        //Flip character
        Vector3 characterScale = transform.localScale;
        if (transform.position.x >= position_to_moveX) characterScale.x = 1;
        if (transform.position.x <= position_to_moveX) characterScale.x = -1;
        transform.localScale = characterScale;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "target")
        {
            hasBeenShot = true;
            moveSpeed = 0;
            animator.SetBool("hasBeenShot", hasBeenShot);
            StartCoroutine(shot());
        }
    }

    private IEnumerator walking()
    {
        walkEnded = false;
        position_to_moveX = Random.Range(-2, 3);
        yield return new WaitForSeconds(Random.Range(4, 8));
        walkEnded = true;
    }

    private  IEnumerator isMoving()
    {
        Vector3 starPos = transform.position;
        yield return new WaitForSeconds(0.1f);
        Vector3 finalPos = transform.position;

        if (starPos.x != finalPos.x) isWalking = true;
        else isWalking = false;
    }

    private IEnumerator gameStarter()
    {
        yield return new WaitForSeconds(0.3f);
        startFlying = true;
        animator.SetBool("startFlying", startFlying);
        transform.position = Vector3.MoveTowards(transform.position, lastClickPos, 3f * Time.deltaTime);
    }

    private IEnumerator shot()
    {
        yield return new WaitForSeconds(0.7f);
        timeToFall = true;
        animator.SetBool("timeToFall", timeToFall);
        rb.gravityScale = 1;
    }
}
