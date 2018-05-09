using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{

    public GameObject foodProjectile;
    private Sprite foodProjectileSprite;

    Animator animator;
    Animator queenAnimator;
    public float movementSpeed = 100;
    public Vector3 originalPosition;
    private float throwingStartTime = -1;

    private float lifetime = 10;
    // Use this for initialization
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        this.originalPosition = this.transform.position;
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
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
                this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                movementSpeed = -5;
                throwingStartTime = -1;
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
