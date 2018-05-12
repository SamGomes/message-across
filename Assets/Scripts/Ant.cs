using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public GameObject myQueen;
    public GameObject mySpawner;


    public GameObject foodProjectile;
    private Sprite foodProjectileSprite;

    Animator animator;
    Animator queenAnimator;
    public float movementSpeed;
    public Vector3 originalPosition;
    private float throwingStartTime = -1;

    private float lifetime = 10;

    float differenceFromQueen;

    // Use this for initialization
    void Start()
    {
        this.movementSpeed = -5;

        animator = this.gameObject.GetComponent<Animator>();
        this.originalPosition = this.transform.position;
        Destroy(gameObject, lifetime);
        differenceFromQueen = myQueen.transform.position.x - originalPosition.x;
        Debug.Log(differenceFromQueen);

        if(differenceFromQueen < 0)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            movementSpeed = -movementSpeed;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(movementSpeed);
        this.transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        if (throwingStartTime < 0)
        {
            return;
        }
        else
        {
            float currenTime = Time.time;
            float difference = currenTime - throwingStartTime;
            if (difference > 4)
            {
                this.animator.SetTrigger("walk");
                gameObject.GetComponents<AudioSource>()[1].Play();
                if (queenAnimator != null)
                {
                    queenAnimator.SetTrigger("rest");
                }
                
                throwingStartTime = -1;
                this.gameObject.GetComponent<SpriteRenderer>().flipX = !this.gameObject.GetComponent<SpriteRenderer>().flipX;

            }
        }
    }

    public void setCargo(string currTargetWord)
    {
        GameObject child = gameObject.transform.GetChild(0).gameObject;
        SpriteRenderer childRenderer = child.GetComponent<SpriteRenderer>();
        childRenderer.sprite = (Sprite)Resources.Load("Textures/FoodItems/" + currTargetWord, typeof(Sprite));
        foodProjectileSprite = childRenderer.sprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != mySpawner.GetComponent<Collider2D>())
        {
            return;
        }

        this.animator.SetTrigger("throw");
        GameObject child = gameObject.transform.GetChild(0).gameObject;
        Destroy(child);
        Instantiate(foodProjectile, gameObject.transform.position, Quaternion.identity).GetComponent<SpriteRenderer>().sprite = foodProjectileSprite;

        if (queenAnimator != null)
        {
            queenAnimator.SetTrigger("beginEating");
            gameObject.GetComponents<AudioSource>()[0].Play();
        }
        movementSpeed = 0;
        throwingStartTime = Time.time;
    }

    public void setQueenAnimator(Animator queen)
    {
        this.queenAnimator = queen;
    }
}
