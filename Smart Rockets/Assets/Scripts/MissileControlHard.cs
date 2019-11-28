using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissileControlHard : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField]
    public float speed;
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
        fitnessLevel = 1;
        passedMileStones = new bool[34];
        currentAtMilestone = new int[34];
        for (int i = 0; i < passedMileStones.Length; i++) {
            passedMileStones[i] = false;
        }
        exploded = false;
        numGenes =200;
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
            rb.freezeRotation = true;
            GetComponent<MeshRenderer>().enabled = false;
            if (!exploded) {
                exploded = true;
                ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem child in ps) {
                    child.Stop();
                    child.Clear();
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
                    child.Clear();
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
            if (current < numGenes && count % 5 == 0) {
                current++;  //this runs 50 times in total
            }
            count++;
        }
        if (current < numGenes) {
            float x = -thrusterLeftForces[current] + thrusterRightForces[current];
            float y = thrusterLeftForces[current] + thrusterRightForces[current];
            double angle = Math.Atan2(y, x) * (180 / Math.PI) - 90;
            Quaternion target = Quaternion.Euler(0, 0, transform.eulerAngles.z + (float)angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5f);
        }
        if (current >= numGenes) {
            current++;
        }
        if (crashed) {
            finished = true;
        }
        if (!finished) {
            calculateFitness();
        }
    }
    void getFitnessLevel() {
        Debug.Log(mileStones[10].position.y > transform.position.y);
        if (!passedMileStones[0]
            && mileStones[0].position.y < transform.position.y
            && mileStones[0].position.x < transform.position.x
            && (mileStones[0].position.x + 1) > transform.position.x) {
            passedMileStones[0] = true;
            fitnessLevel = 2;
            currentAtMilestone[0] = current;
        } else if (passedMileStones[0]
            && !passedMileStones[1]
            && mileStones[1].position.y < transform.position.y
            && mileStones[1].position.x < transform.position.x
            && (mileStones[1].position.x + 1) > transform.position.x) {
            passedMileStones[1] = true;
            fitnessLevel = 3;
            currentAtMilestone[1] = current;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && !passedMileStones[2]
            && mileStones[2].position.y < transform.position.y
            && mileStones[2].position.x < transform.position.x
            && (mileStones[2].position.x + 1) > transform.position.x) {
            passedMileStones[2] = true;
            fitnessLevel = 4;
            currentAtMilestone[2] = current;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && !passedMileStones[3]
            && mileStones[3].position.y < transform.position.y
            && mileStones[3].position.x < transform.position.x
            && (mileStones[3].position.x + 1) > transform.position.x) {
            passedMileStones[3] = true;
            fitnessLevel = 5;
            currentAtMilestone[3] = current;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && !passedMileStones[4]
            && mileStones[4].position.y < transform.position.y
            && mileStones[4].position.x < transform.position.x
            && (mileStones[4].position.x + 1) > transform.position.x) {
            passedMileStones[4] = true;
            fitnessLevel = 6;
            currentAtMilestone[4] = current;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && !passedMileStones[5]
            && mileStones[5].position.y < transform.position.y
            && mileStones[5].position.x < transform.position.x
            && (mileStones[5].position.x + 1) > transform.position.x) {
            passedMileStones[5] = true;
            fitnessLevel = 7;
            currentAtMilestone[5] = current;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && !passedMileStones[6]
            && mileStones[6].position.x > transform.position.x
            && mileStones[6].position.y < transform.position.y
            && (mileStones[6].position.y + 1.25) > transform.position.y) {
            passedMileStones[6] = true;
            fitnessLevel = 8;
            currentAtMilestone[6] = current;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && !passedMileStones[7]
            && mileStones[7].position.x > transform.position.x
            && mileStones[7].position.y < transform.position.y
            && (mileStones[7].position.y + 1.15) > transform.position.y) {
            passedMileStones[7] = true;
            fitnessLevel = 9;
            currentAtMilestone[7] = current;
        } else if (passedMileStones[0]
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && !passedMileStones[8]
            && mileStones[8].position.x > transform.position.x
            && mileStones[8].position.y < transform.position.y
            && (mileStones[8].position.y + 1.05) > transform.position.y) {
            passedMileStones[8] = true;
            fitnessLevel = 10;
            currentAtMilestone[8] = current;
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
            && (mileStones[9].position.x + 1) > transform.position.x
            && mileStones[9].position.y > transform.position.y
            && (mileStones[9].position.y + 1) < transform.position.y) {
            passedMileStones[9] = true;
            fitnessLevel = 11;
            currentAtMilestone[9] = current;
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
            && mileStones[10].position.y > transform.position.y
            //&& mileStones[10].position.x < transform.position.x
            //&& (mileStones[10].position.x + 2) > transform.position.x
            ) {
            passedMileStones[10] = true;
            fitnessLevel = 15;
            currentAtMilestone[10] = current;
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
            && mileStones[11].position.y > transform.position.y
            && mileStones[11].position.x < transform.position.x
            && (mileStones[11].position.x + 1) > transform.position.x) {
            passedMileStones[11] = true;
            fitnessLevel = 18;
            currentAtMilestone[11] = current;
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
            && mileStones[12].position.y > transform.position.y
            && mileStones[12].position.x < transform.position.x
            && (mileStones[12].position.x + 1) > transform.position.x) {
            passedMileStones[12] = true;
            fitnessLevel = 19;
            currentAtMilestone[12] = current;
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
            && mileStones[13].position.y > transform.position.y
            && mileStones[13].position.x < transform.position.x
            && (mileStones[13].position.x + 1) > transform.position.x) {
            passedMileStones[13] = true;
            fitnessLevel = 22;
            currentAtMilestone[13] = current;
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
            && mileStones[14].position.y > transform.position.y
            && mileStones[14].position.x < transform.position.x
            && (mileStones[14].position.x + 1) > transform.position.x) {
            passedMileStones[14] = true;
            fitnessLevel = 23;
            currentAtMilestone[14] = current;
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
            && mileStones[15].position.y > transform.position.y
            && mileStones[15].position.x < transform.position.x
            && (mileStones[15].position.x + 1) > transform.position.x) {
            passedMileStones[15] = true;
            fitnessLevel = 24;
            currentAtMilestone[15] = current;
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
            && mileStones[16].position.y > transform.position.y
            && mileStones[16].position.x < transform.position.x
            && (mileStones[16].position.x + 1) > transform.position.x) {
            passedMileStones[16] = true;
            fitnessLevel = 27;
            currentAtMilestone[16] = current;
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
             && passedMileStones[16]
             && !passedMileStones[17]
             && mileStones[17].position.y > transform.position.y
            && mileStones[17].position.x < transform.position.x
            && (mileStones[17].position.x + 1) > transform.position.x) {
            passedMileStones[17] = true;
            fitnessLevel = 35;
            currentAtMilestone[17] = current;
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
              && passedMileStones[16]
              && passedMileStones[17]
              && !passedMileStones[18]
              && mileStones[18].position.y > transform.position.y
              && mileStones[18].position.x < transform.position.x
              && (mileStones[18].position.x + 1) > transform.position.x) {
            passedMileStones[18] = true;
            fitnessLevel = 40;
            currentAtMilestone[18] = current;
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
               && passedMileStones[16]
               && passedMileStones[17]
               && passedMileStones[18]
               && !passedMileStones[19]
               && mileStones[19].position.x > transform.position.x
              && mileStones[19].position.y < transform.position.y
              && (mileStones[19].position.y + 2) > transform.position.y) {
            passedMileStones[19] = true;
            fitnessLevel = 50;
            currentAtMilestone[19] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && !passedMileStones[20]
                && mileStones[20].position.x > transform.position.x
              && mileStones[20].position.y < transform.position.y
              && (mileStones[20].position.y + 2) > transform.position.y) {
            passedMileStones[20] = true;
            fitnessLevel = 60;
            currentAtMilestone[20] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && !passedMileStones[21]
                && mileStones[21].position.y < transform.position.y
              && mileStones[21].position.x < transform.position.x
              && (mileStones[21].position.x + 2) > transform.position.x) {
            passedMileStones[21] = true;
            fitnessLevel = 70;
            currentAtMilestone[21] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && !passedMileStones[22]
                && mileStones[22].position.y < transform.position.y
              && mileStones[22].position.x < transform.position.x
              && (mileStones[22].position.x + 1) > transform.position.x) {
            passedMileStones[22] = true;
            fitnessLevel = 75;
            currentAtMilestone[22] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && !passedMileStones[23]
                && mileStones[23].position.y < transform.position.y
              && mileStones[23].position.x < transform.position.x
              && (mileStones[23].position.x + 1) > transform.position.x) {
            passedMileStones[23] = true;
            fitnessLevel = 80;
            currentAtMilestone[23] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && passedMileStones[23]
                && !passedMileStones[24]
                && mileStones[24].position.y < transform.position.y
              && mileStones[24].position.x < transform.position.x
              && (mileStones[24].position.x + 1) > transform.position.x) {
            passedMileStones[24] = true;
            fitnessLevel = 85;
            currentAtMilestone[24] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && passedMileStones[23]
                && passedMileStones[24]
                && !passedMileStones[25]
                && mileStones[25].position.y < transform.position.y
              && mileStones[25].position.x < transform.position.x
              && (mileStones[25].position.x + 1) > transform.position.x) {
            passedMileStones[25] = true;
            fitnessLevel = 90;
            currentAtMilestone[25] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && passedMileStones[23]
                && passedMileStones[24]
                && passedMileStones[25]
                && !passedMileStones[26]
                && mileStones[26].position.y < transform.position.y
              && mileStones[26].position.x < transform.position.x
              && (mileStones[26].position.x + 1) > transform.position.x) {
            passedMileStones[26] = true;
            fitnessLevel = 95;
            currentAtMilestone[26] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && passedMileStones[23]
                && passedMileStones[24]
                && passedMileStones[25]
                && passedMileStones[26]
                && !passedMileStones[27]
                && mileStones[27].position.y < transform.position.y
              && mileStones[27].position.x < transform.position.x
              && (mileStones[27].position.x + 1) > transform.position.x) {
            passedMileStones[27] = true;
            fitnessLevel = 100;
            currentAtMilestone[27] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && passedMileStones[23]
                && passedMileStones[24]
                && passedMileStones[25]
                && passedMileStones[26]
                && passedMileStones[27]
                && !passedMileStones[28]
                && mileStones[28].position.x < transform.position.x
              && mileStones[28].position.y < transform.position.y
              && (mileStones[28].position.y + 1) > transform.position.y) {
            passedMileStones[28] = true;
            fitnessLevel = 105;
            currentAtMilestone[28] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && passedMileStones[23]
                && passedMileStones[24]
                && passedMileStones[25]
                && passedMileStones[26]
                && passedMileStones[27]
                && passedMileStones[28]
                && !passedMileStones[29]
                && mileStones[29].position.x < transform.position.x
              && mileStones[29].position.y < transform.position.y
              && (mileStones[29].position.y + 1) > transform.position.y) {
            passedMileStones[29] = true;
            fitnessLevel = 110;
            currentAtMilestone[29] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && passedMileStones[23]
                && passedMileStones[24]
                && passedMileStones[25]
                && passedMileStones[26]
                && passedMileStones[27]
                && passedMileStones[28]
                && passedMileStones[29]
                && !passedMileStones[30]
                && mileStones[30].position.x < transform.position.x
              && mileStones[30].position.y < transform.position.y
              && (mileStones[30].position.y + 1) > transform.position.y) {
            passedMileStones[30] = true;
            fitnessLevel = 130;
            currentAtMilestone[30] = current;
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
                && passedMileStones[16]
                && passedMileStones[17]
                && passedMileStones[18]
                && passedMileStones[19]
                && passedMileStones[20]
                && passedMileStones[21]
                && passedMileStones[22]
                && passedMileStones[23]
                && passedMileStones[24]
                && passedMileStones[25]
                && passedMileStones[26]
                && passedMileStones[27]
                && passedMileStones[28]
                && passedMileStones[29]
                && passedMileStones[30]
                && !passedMileStones[31]
                && mileStones[31].position.x < transform.position.x
              && mileStones[31].position.y < transform.position.y
              && (mileStones[31].position.y + 1) > transform.position.y) {
            passedMileStones[31] = true;
            fitnessLevel = 145;
            currentAtMilestone[31] = current;
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
                 && passedMileStones[16]
                 && passedMileStones[17]
                 && passedMileStones[18]
                 && passedMileStones[19]
                 && passedMileStones[20]
                 && passedMileStones[21]
                 && passedMileStones[22]
                 && passedMileStones[23]
                 && passedMileStones[24]
                 && passedMileStones[25]
                 && passedMileStones[26]
                 && passedMileStones[27]
                 && passedMileStones[28]
                 && passedMileStones[29]
                 && passedMileStones[30]
                 && passedMileStones[31]
                 && !passedMileStones[32]
                 && mileStones[32].position.x < transform.position.x
              && mileStones[32].position.y < transform.position.y
              && (mileStones[32].position.y + 1) > transform.position.y) {
            passedMileStones[32] = true;
            fitnessLevel = 160;
            currentAtMilestone[32] = current;
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
                 && passedMileStones[16]
                 && passedMileStones[17]
                 && passedMileStones[18]
                 && passedMileStones[19]
                 && passedMileStones[20]
                 && passedMileStones[21]
                 && passedMileStones[22]
                 && passedMileStones[23]
                 && passedMileStones[24]
                 && passedMileStones[25]
                 && passedMileStones[26]
                 && passedMileStones[27]
                 && passedMileStones[28]
                 && passedMileStones[29]
                 && passedMileStones[30]
                 && passedMileStones[31]
                 && passedMileStones[32]
                 && !passedMileStones[33]
                 && mileStones[33].position.x < transform.position.x
              && mileStones[33].position.y < transform.position.y
              && (mileStones[33].position.y + 1) > transform.position.y) {
            passedMileStones[33] = true;
            fitnessLevel = 170;
            currentAtMilestone[33] = current;
        }
    }
    void calculateFitness() {
        //instead of taking the distance to the goal, take distance to milestones
        double dist = Math.Sqrt(Math.Pow((double)(transform.position.x - goalTransform.position.x), 2) +
            Math.Pow((double)(transform.position.y - goalTransform.position.y), 2));
        getFitnessLevel();
        double currentFitness = maxDist - dist;
        if (dist < 1) {
            currentFitness *= 1.5;
        }
        if (!crashed) {
            fitness += fitnessLevel;
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
