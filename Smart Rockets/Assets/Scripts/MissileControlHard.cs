using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissileControlHard : MonoBehaviour {
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
    public int current;
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
    public Transform[] mileStones;
    public bool[] passedMileStones;
    private int fitnessLevel;
    public GameObject explosion;
    private bool exploded;
    public int numGenes;
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
        maxDist = 200;
        Physics2D.gravity = Vector2.zero;
        updateFitnessTime1 = false;
        updateFitnessTime2 = false;
        fitnessLevel = 1;
        passedMileStones = new bool[17];
        for (int i = 0; i < passedMileStones.Length; i++) {
            passedMileStones[i] = false;
        }
        exploded = false;
        numGenes = 100;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Rocket") {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CompositeCollider2D>(), GetComponent<CompositeCollider2D>());
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CapsuleCollider2D>(), GetComponent<CapsuleCollider2D>());
        }
        if (collision.gameObject.tag == "wall" && !reachedGoal) { //prevents it from updating fitness after it has finished the stage
            fitness *= .50;
            crashed = true;
            endFrame = current;
            rb.freezeRotation = true;
            Physics2D.gravity = new Vector2(0, -9.8f);
            GetComponent<MeshRenderer>().enabled = false;
            if (!exploded) {
                exploded = true;
                ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem child in ps) {
                    child.Stop();
                }
                GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
                Destroy(expl, 1);
            }
        }
        if (collision.gameObject.tag == "Goal" && !crashed) {
            fitness *= 4;
            reachedGoal = true;
            crashed = true;
            endFrame = current;
            GetComponent<MeshRenderer>().enabled = false;
            if (!exploded) {
                exploded = true;
                ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem child in ps) {
                    child.Stop();
                }
                GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
                Destroy(expl, 1);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (isReady && !crashed) {
            rb.velocity = transform.up * speed * 2;
            if (current < numGenes && count % 5 == 0) { //change range of forces applied on rockets || remove count %10? 

                //rb.AddRelativeForce(thrusterRightForces[current] * new Vector2(speed * .5f, -speed));
                //rb.AddRelativeForce(thrusterLeftForces[current] * new Vector2(-speed * .5f, -speed));
                //rb.AddRelativeForce(forcesX[current] * new Vector2(speed, speed)); //change scalar perhaps? As we improve, we want to be able to adjust the magnitude of  the vectors4
                //rb.AddRelativeForce(forcesY[current] * new Vector2(0, speed)); //mating function, incorperate longest lasting rocket by definition of it taking a long time to crash
            //    rb.AddRelativeForce(thrusterLeftForces[current] * new Vector2(-speed * .5f, speed * .5f) +
              //                      thrusterRightForces[current] * new Vector2(speed * .5f, speed * .5f));
                //rb.AddRelativeForce(forcesY[current] * new Vector2(0, speed));
                current++;  //this runs 50 times in total
            }
            count++;
        }
        if (current < numGenes) {
            float x = thrusterLeftForces[current] * -speed * .5f + thrusterRightForces[current] * speed * .5f;
            float y = thrusterLeftForces[current] * speed * .5f + thrusterRightForces[current] * speed * .5f;
            double angle = Math.Atan2(y, x) * (180 / Math.PI) - 90;
            Quaternion target = Quaternion.Euler(0, 0, transform.eulerAngles.z + (float)angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5f);
        }
        if (current >= numGenes) {
            current++;
        }
        if (current >= 200 || crashed) {
            finished = true;
            updateFitnessTime1 = true;
        }
        if (updateFitnessTime1 && !updateFitnessTime2) {
            fitness += fitnessTime;
            updateFitnessTime2 = true;
        }
        if (!finished) {
            calcuteFitness();
        }
    }
    void getFitnessLevel() {
        if (!passedMileStones[0] 
            && mileStones[0].position.y < transform.position.y) {
            passedMileStones[0] = true;
            fitnessLevel = 2;
        } else if (passedMileStones[0] 
            && !passedMileStones[1] 
            && mileStones[1].position.y < transform.position.y) {
            passedMileStones[1] = true;
            fitnessLevel = 3;
        } else if (passedMileStones[0] 
            && passedMileStones[1] 
            && !passedMileStones[2] 
            && mileStones[2].position.y < transform.position.y) {
            passedMileStones[2] = true;
            fitnessLevel = 4;
        } else if (passedMileStones[0] 
            && passedMileStones[1] 
            && passedMileStones[2] 
            && !passedMileStones[3] 
            && mileStones[3].position.x > transform.position.x) {
            passedMileStones[3] = true;
            fitnessLevel = 5;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && !passedMileStones[4]
            && mileStones[4].position.x > transform.position.x) {
            passedMileStones[4] = true;
            fitnessLevel = 6;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && !passedMileStones[5]
            && mileStones[5].position.y > transform.position.y) {
            passedMileStones[5] = true;
            fitnessLevel = 7;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && !passedMileStones[6]
            && mileStones[6].position.y > transform.position.y) {
            passedMileStones[6] = true;
            fitnessLevel = 8;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && !passedMileStones[7]
            && mileStones[7].position.y > transform.position.y) {
            passedMileStones[7] = true;
            fitnessLevel = 9;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && !passedMileStones[8]
            && mileStones[8].position.y > transform.position.y) {
            passedMileStones[8] = true;
            fitnessLevel = 10;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && passedMileStones[8]
            && !passedMileStones[9]
            && mileStones[9].position.x > transform.position.x) {
            passedMileStones[9] = true;
            fitnessLevel = 11;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && passedMileStones[8]
            && passedMileStones[9]
            && !passedMileStones[10]
            && mileStones[10].position.x > transform.position.x) {
            passedMileStones[10] = true;
            fitnessLevel = 15;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && passedMileStones[8]
            && passedMileStones[9]
            && passedMileStones[10]
            && !passedMileStones[11]
            && mileStones[11].position.y < transform.position.y) {
            passedMileStones[11] = true;
            fitnessLevel = 18;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && passedMileStones[8]
            && passedMileStones[9]
            && passedMileStones[10]
            && passedMileStones[11]
            && !passedMileStones[12]
            && mileStones[12].position.y < transform.position.y) {
            passedMileStones[12] = true;
            fitnessLevel = 19;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && passedMileStones[8]
            && passedMileStones[9]
            && passedMileStones[10]
            && passedMileStones[11]
            && passedMileStones[12]
            && !passedMileStones[13]
            && mileStones[13].position.y < transform.position.y) {
            passedMileStones[13] = true;
            fitnessLevel = 22;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && passedMileStones[8]
            && passedMileStones[9]
            && passedMileStones[10]
            && passedMileStones[11]
            && passedMileStones[12]
            && passedMileStones[13]
            && !passedMileStones[14]
            && mileStones[14].position.y < transform.position.y) {
            passedMileStones[14] = true;
            fitnessLevel = 23;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && passedMileStones[8]
            && passedMileStones[9]
            && passedMileStones[10]
            && passedMileStones[11]
            && passedMileStones[12]
            && passedMileStones[13]
            && passedMileStones[14]
            && !passedMileStones[15]
            && mileStones[15].position.x < transform.position.x) {
            passedMileStones[15] = true;
            fitnessLevel = 24;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && passedMileStones[8]
            && passedMileStones[9]
            && passedMileStones[10]
            && passedMileStones[11]
            && passedMileStones[12]
            && passedMileStones[13]
            && passedMileStones[14]
            && passedMileStones[15]
            && !passedMileStones[16]
            && mileStones[16].position.x < transform.position.x) {
            passedMileStones[16] = true;
            fitnessLevel = 27;
        }
    }
    void calcuteFitness() {
        //instead of taking the distance to the goal, take distance to milestones
        double dist = Math.Sqrt(Math.Pow((double)(transform.position.x - goalTransform.position.x), 2) +
            Math.Pow((double)(transform.position.y - goalTransform.position.y), 2));
        getFitnessLevel();
        double currentFitness = maxDist - dist;
        if (dist < 1) {
            Debug.Log("here");
            currentFitness *= 1.5;
        }
        if (!crashed) {
            //fitness = currentFitness;
            fitnessTime += .05;
            fitness +=  fitnessLevel;
            //fitness = Math.Pow(1.1, current);
            //fitness *= fitnessLevel;
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
