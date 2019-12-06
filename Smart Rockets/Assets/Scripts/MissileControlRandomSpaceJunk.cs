using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissileControlRandomSpaceJunk : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField]
    public float speed;
    public float slerpRate;
    private int count;
    Rigidbody2D rb;
    public bool isReady;
    public float[] thrusterLeftForces;
    public float[] thrusterRightForces;
    public int current;
    public Transform goalTransform;
    public double fitness;
    public bool finished;
    private bool crashed;
    public bool reachedGoal;
    private double maxDist;
    public Transform[] mileStones;
    public bool[] passedMileStones;
    public int[] currentAtMilestone;
    private int fitnessLevel;
    public GameObject explosion;
    private bool exploded;
    public int numGenes;
    public float[] crashPos;

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
        fitnessLevel = 1;
        passedMileStones = new bool[7];
        currentAtMilestone = new int[7];
        for (int i = 0; i < passedMileStones.Length; i++) {
            passedMileStones[i] = false;
        }
        exploded = false;
        numGenes = 400;
        crashPos = new float[2];
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Rocket") {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CompositeCollider2D>(), GetComponent<CompositeCollider2D>());
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<CapsuleCollider2D>(), GetComponent<CapsuleCollider2D>());
        }
        if (collision.gameObject.tag == "wall" && !reachedGoal) { //prevents it from updating fitness after it has finished the stage
            crashPos[0] = transform.position.x;
            crashPos[1] = transform.position.y;
            fitness *= .50;
            crashed = true;
            rb.freezeRotation = true;
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
            rb.velocity = transform.up * speed;
            if (current < numGenes && count % 5 == 0) { //change range of forces applied on rockets || remove count %10? 
                current++;  //this runs 50 times in total
            }
            count++;
        }
        if (current < numGenes) {
            float x = -thrusterLeftForces[current] + thrusterRightForces[current];
            float y = thrusterLeftForces[current] + thrusterRightForces[current];
            double angle = Math.Atan2(y, x) * (180 / Math.PI) - 90;
            Quaternion target = Quaternion.Euler(0, 0, transform.eulerAngles.z + (float)angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * slerpRate);
        }
        if (current >= numGenes) {
            current++;
        }
        if (crashed || current >= numGenes) {
            finished = true;
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
            currentAtMilestone[0] = current;
        } else if (passedMileStones[0]
                && !passedMileStones[1]
                && mileStones[1].position.y < transform.position.y) {
            passedMileStones[1] = true;
            fitnessLevel = 3;
            currentAtMilestone[1] = current;
        } else if (passedMileStones[0]
                && passedMileStones[1]
                && !passedMileStones[2]
                && mileStones[2].position.y < transform.position.y) {
            passedMileStones[2] = true;
            fitnessLevel = 4;
            currentAtMilestone[2] = current;
        } else if (passedMileStones[0]
                && passedMileStones[1]
                && passedMileStones[2]
                && !passedMileStones[3]
                && mileStones[3].position.y < transform.position.y) {
            passedMileStones[3] = true;
            fitnessLevel = 5;
            currentAtMilestone[3] = current;
        } else if (passedMileStones[0]
                && passedMileStones[1]
                && passedMileStones[2]
                && passedMileStones[3]
                && !passedMileStones[4]
                && mileStones[4].position.y < transform.position.y) {
            passedMileStones[4] = true;
            fitnessLevel = 7;
            currentAtMilestone[4] = current;
        } else if (passedMileStones[0]
                && passedMileStones[1]
                && passedMileStones[2]
                && passedMileStones[3]
                && passedMileStones[4]
                && !passedMileStones[5]
                && mileStones[5].position.y < transform.position.y) {
            passedMileStones[5] = true;
            fitnessLevel = 10;
            currentAtMilestone[5] = current;
        } else if (passedMileStones[0]
                && passedMileStones[1]
                && passedMileStones[2]
                && passedMileStones[3]
                && passedMileStones[4]
                && passedMileStones[5]
                && !passedMileStones[6]
                && mileStones[6].position.y < transform.position.y) {
            passedMileStones[6] = true;
            fitnessLevel = 13;
            currentAtMilestone[6] = current;
        }
    }
    void calcuteFitness() {
        double dist = Math.Sqrt(Math.Pow((double)(transform.position.x - goalTransform.position.x), 2) +
            Math.Pow((double)(transform.position.y - goalTransform.position.y), 2));
        getFitnessLevel();
        double currentFitness = maxDist - dist;
        if (dist < 1) {
            currentFitness *= 1.5;
        }
        if (!crashed) {
            fitness = currentFitness;
            fitness *= fitnessLevel;
        }

    }
}
