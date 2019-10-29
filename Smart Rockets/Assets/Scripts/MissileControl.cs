using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissileControl : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField]
    public float speed;
    private int count;
    private Vector2 vel;
    Rigidbody2D rb;
    public bool isReady;
    public float[] forcesX;
    public float[] forcesY;
    private int current;
    public Transform goalTransform;
    public double fitness;
    public bool finished;
    private bool crashed;
    private bool reachedGoal;
    private double maxDist;

    void Awake() {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    void Start() {
        //rb.mass = .5f;
        //Debug.Log(rb.mass);
        fitness = 0f;
        current = 0;
        count = 0;
        finished = false;
        maxDist = Math.Sqrt(Math.Pow((double)(transform.position.x - goalTransform.position.x), 2) +
            Math.Pow((double)(transform.position.y - goalTransform.position.y), 2));
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Rocket") {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CapsuleCollider2D>(), GetComponent<CapsuleCollider2D>());
        }
        if(collision.gameObject.tag == "wall" && !reachedGoal && current < 50) { //prevents it from updating fitness after it has finished the stage
            fitness *= .50;
            crashed = true;
        }
        if (collision.gameObject.tag == "Goal") {
            fitness *= 2;
            reachedGoal = true;
            crashed = true;
        }
    }

    // Update is called once per frame
    void Update() {
        if(isReady && !crashed) {
            if (current < 50 && count % 5 == 0) { //change range of forces applied on rockets || remove count %10? 
                rb.AddForce(forcesX[current] * new Vector2(speed, 0)); //change scalar perhaps? As we improve, we want to be able to adjust the magnitude of  the vectors
                rb.AddForce(forcesY[current] * new Vector2(0, speed)); //mating function, incorperate longest lasting rocket by definition of it taking a long time to crash
                current++;  //this runs 50 times in total
            }
            count++;

        
        }
        if (current >= 50 && current <= 200) {
            current++;
        }
        if (current >= 200 || crashed) {
            finished = true;
        }
        if (!finished) {
            calcuteFitness();
        }
    }

    void calcuteFitness() {
        double dist = Math.Sqrt(Math.Pow((double)(transform.position.x - goalTransform.position.x), 2) +
            Math.Pow((double)(transform.position.y - goalTransform.position.y), 2));
        double currentFitness = maxDist - dist;
        if (dist < 1) {
            currentFitness *= 1.5;
        }
        if (currentFitness > fitness) {
            fitness = currentFitness;
        }
        
    }
}



/* Fitness function
 * How will we determine if something is a good fitness?
 * Combine multiple ways to compute fitness:
 * 1) time alive
 * 2) distance from goal
 * 3) ???
 * Add them all up can multiple by some scalar to determine whcich aspects are more important
 * 
 * 
 * 
 */
