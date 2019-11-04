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
    public float[] thrusterLeftForces;
    public float[] thrusterRightForces;
    public float[] forcesY;
    private int current;
    public Transform goalTransform;
    public double fitness;
    public double fitnessTime;
    public bool finished;
    private bool crashed;
    public bool reachedGoal;
    private double maxDist;
    private int endFrame;
    private bool updateFitnessTime1;
    private bool updateFitnessTime2;

    void Awake() {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    void Start() {
        fitness = 0f;
        current = 0;
        count = 0;
        finished = false;
        maxDist = Math.Sqrt(Math.Pow((double)(transform.position.x - goalTransform.position.x), 2) +
            Math.Pow((double)(transform.position.y - goalTransform.position.y), 2));
        Physics2D.gravity = Vector2.zero;
        updateFitnessTime1 = false;
        updateFitnessTime2 = false;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Rocket") {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CompositeCollider2D>(), GetComponent<CompositeCollider2D>());
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CapsuleCollider2D>(), GetComponent<CapsuleCollider2D>());
        }
        if(collision.gameObject.tag == "wall" && !reachedGoal && current < 50) { //prevents it from updating fitness after it has finished the stage
            fitness *= .50;
            crashed = true;
            endFrame = current;
            rb.freezeRotation = true;
            Physics2D.gravity = new Vector2(0, -9.8f);
        }
        if (collision.gameObject.tag == "Goal" && !crashed) {
            Debug.Log("hit goal?");
            fitness *= 4;
            reachedGoal = true;
            crashed = true;
            endFrame = current;
        }
    }

    // Update is called once per frame
    void Update() {
        if (isReady && !crashed) {
            if (current < 50 && count % 10 == 0) { //change range of forces applied on rockets || remove count %10? 

                //rb.AddRelativeForce(thrusterRightForces[current] * new Vector2(speed * .5f, -speed));
                //rb.AddRelativeForce(thrusterLeftForces[current] * new Vector2(-speed * .5f, -speed));
                //rb.AddRelativeForce(forcesX[current] * new Vector2(speed, speed)); //change scalar perhaps? As we improve, we want to be able to adjust the magnitude of  the vectors4
                //rb.AddRelativeForce(forcesY[current] * new Vector2(0, speed)); //mating function, incorperate longest lasting rocket by definition of it taking a long time to crash
                rb.AddRelativeForce(thrusterLeftForces[current] * new Vector2(-speed * .5f, speed * .5f) + 
                                    thrusterRightForces[current] * new Vector2(speed * .5f, speed * .5f));
                rb.AddRelativeForce(forcesY[current] * new Vector2(0, speed));
                current++;  //this runs 50 times in total
            }
            count++;
        }
        if (current < 50) {
            float x = thrusterLeftForces[current] * -speed * .5f + thrusterRightForces[current] * speed * .5f;
            float y = thrusterLeftForces[current] * speed * .5f + thrusterRightForces[current] * speed * .5f;
            double angle = Math.Atan2(y, x) * (180 / Math.PI) - 90;
            Quaternion target = Quaternion.Euler(0, 0, (float)angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5f);
        }
        if (current >= 50 && current <= 200) {
            current++;
        }
        if (current >= 200 || crashed) {
            finished = true;
            updateFitnessTime1 = true;
        }
        if (updateFitnessTime1 && !updateFitnessTime2) {
            Debug.Log("fitnessTime");
            fitness += fitnessTime;
            updateFitnessTime2 = true;
        }
        if (!finished) {
            calcuteFitness();
           // fitness += fitness * (1 + (endFrame) / 200) * (.5);
           // Debug.Log(fitness);
            //Debug.Log(maxDist);
        }
    }
    // fitness +=  fitness*(1 + (200-current)/200)*(.75)
    void calcuteFitness() {
        double dist = Math.Sqrt(Math.Pow((double)(transform.position.x - goalTransform.position.x), 2) +
            Math.Pow((double)(transform.position.y - goalTransform.position.y), 2));

        double currentFitness = maxDist - dist;
        if (dist < 1) {
            currentFitness *= 1.5;
        }
        if (!crashed) {
            fitness = currentFitness;
            fitnessTime += .05;
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
