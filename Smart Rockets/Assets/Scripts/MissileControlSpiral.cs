using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissileControlSpiral : MonoBehaviour {
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
        passedMileStones = new bool[70];
        currentAtMilestone = new int[70];
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
        if (crashed) {
            finished = true;
        }
        if (!finished) {
            calcuteFitness();
        }
    }
    void getFitnessLevel() {
        if (!passedMileStones[0] //0
            && mileStones[0].position.y < transform.position.y) {
            passedMileStones[0] = true;
            fitnessLevel = 2;
            if (mileStones[0].position.x < transform.position.x
             && mileStones[0].position.x + 1 > transform.position.x) {
                fitnessLevel += 1;
            }
            currentAtMilestone[0] = current;
        } else if (passedMileStones[0]//1
            && !passedMileStones[1]
            && mileStones[1].position.y < transform.position.y) {
            passedMileStones[1] = true;
            fitnessLevel = 3;
            if (mileStones[1].position.x < transform.position.x
             && mileStones[1].position.x + 1 > transform.position.x) {
                fitnessLevel += 2;
            }
            currentAtMilestone[1] = current;
        } else if (passedMileStones[0]//2
            && passedMileStones[1]
            && !passedMileStones[2]
            && mileStones[2].position.y < transform.position.y) {
            passedMileStones[2] = true;
            fitnessLevel = 4;
            if (mileStones[2].position.x < transform.position.x
             && mileStones[2].position.x + 1 > transform.position.x) {
                fitnessLevel += 2;
            }
            currentAtMilestone[2] = current;
        } else if (passedMileStones[0] //3
            && passedMileStones[1]
            && passedMileStones[2]
            && !passedMileStones[3]
            && mileStones[3].position.y < transform.position.y) {
            passedMileStones[3] = true;
            fitnessLevel = 5;
            if (mileStones[3].position.x < transform.position.x
             && mileStones[3].position.x + 1 > transform.position.x) {
                fitnessLevel += 3;
            }
            currentAtMilestone[3] = current;
        } else if (passedMileStones[0] //4
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && !passedMileStones[4]
            && mileStones[4].position.y < transform.position.y) {
            passedMileStones[4] = true;
            fitnessLevel = 6;
            if (mileStones[4].position.x < transform.position.x
             && mileStones[4].position.x + 1 > transform.position.x) {
                fitnessLevel += 3;
            }
            currentAtMilestone[4] = current;
        } else if (passedMileStones[0] // 5
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && !passedMileStones[5]
            && mileStones[5].position.y < transform.position.y) {
            passedMileStones[5] = true;
            fitnessLevel = 7;
            if (mileStones[5].position.x < transform.position.x
             && mileStones[5].position.x + 1 > transform.position.x) {
                fitnessLevel += 3;
            }
            currentAtMilestone[5] = current;
        } else if (passedMileStones[0] //6
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && !passedMileStones[6]
            && mileStones[6].position.y < transform.position.y) {
            passedMileStones[6] = true;
            fitnessLevel = 8;
            if (mileStones[6].position.x < transform.position.x
             && mileStones[6].position.x + 1 > transform.position.x) {
                fitnessLevel += 4;
            }
            currentAtMilestone[6] = current;
        } else if (passedMileStones[0] //7
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && !passedMileStones[7]
            && mileStones[7].position.y < transform.position.y
            && mileStones[7].position.x + 1 > transform.position.x) {
            passedMileStones[7] = true;
            fitnessLevel = 13;
            if (mileStones[7].position.y + 1 < transform.position.y
             && mileStones[7].position.x < transform.position.x) {
                fitnessLevel += 6;
            }
            currentAtMilestone[7] = current;
        } else if (passedMileStones[0] //8
            && passedMileStones[1]
            && passedMileStones[2]
            && passedMileStones[3]
            && passedMileStones[4]
            && passedMileStones[5]
            && passedMileStones[6]
            && passedMileStones[7]
            && !passedMileStones[8]
            && mileStones[8].position.x > transform.position.x) {
            passedMileStones[8] = true;
            fitnessLevel = 17;
            if (mileStones[8].position.y < transform.position.y
             && mileStones[8].position.y + 1 > transform.position.y) {
                fitnessLevel += 9;
            }
            currentAtMilestone[8] = current;
        } else if (passedMileStones[0] //9
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
            fitnessLevel = 21;
            if (mileStones[9].position.y < transform.position.y
             && mileStones[9].position.y + 1 > transform.position.y) {
                fitnessLevel += 10;
            }
            currentAtMilestone[9] = current;
        } else if (passedMileStones[0] //10
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
            fitnessLevel = 25;
            if (mileStones[10].position.y < transform.position.y
             && mileStones[10].position.y + 1 > transform.position.y) {
                fitnessLevel += 12;
            }
            currentAtMilestone[10] = current;
        } else if (passedMileStones[0] //11
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
            && mileStones[11].position.x > transform.position.x) {
            passedMileStones[11] = true;
            fitnessLevel = 29;
            if (mileStones[11].position.y < transform.position.y
             && mileStones[11].position.y + 1 > transform.position.y) {
                fitnessLevel += 15;
            }
            currentAtMilestone[0] = current;
        } else if (passedMileStones[11] //12
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
            && mileStones[12].position.x > transform.position.x) {
            passedMileStones[12] = true;
            fitnessLevel = 33;
            if (mileStones[12].position.y < transform.position.y
             && mileStones[12].position.y + 1 > transform.position.y) {
                fitnessLevel += 16;
            }
            currentAtMilestone[12] = current;
        } else if (passedMileStones[0] //13
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
            && mileStones[13].position.x > transform.position.x) {
            passedMileStones[13] = true;
            fitnessLevel = 37;
            if (mileStones[13].position.y < transform.position.y
             && mileStones[13].position.y + 1 > transform.position.y) {
                fitnessLevel += 18;
            }
            currentAtMilestone[13] = current;
        } else if (passedMileStones[0] //14
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
            && mileStones[14].position.x > transform.position.x) {
            passedMileStones[14] = true;
            fitnessLevel = 41;
            if (mileStones[14].position.y < transform.position.y
             && mileStones[14].position.y + 1 > transform.position.y) {
                fitnessLevel += 20;
            }
            currentAtMilestone[0] = current;
        } else if (passedMileStones[0] //15
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
            && mileStones[15].position.x > transform.position.x) {
            passedMileStones[15] = true;
            fitnessLevel = 45;
            if (mileStones[15].position.y < transform.position.y
             && mileStones[15].position.y + 1 > transform.position.y) {
                fitnessLevel += 22;
            }
            currentAtMilestone[15] = current;
        } else if (passedMileStones[0] //16
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
            && mileStones[16].position.x > transform.position.x) {
            passedMileStones[16] = true;
            fitnessLevel = 49;
            if (mileStones[16].position.y < transform.position.y
             && mileStones[16].position.y + 1 > transform.position.y) {
                fitnessLevel += 24;
            }
            currentAtMilestone[16] = current;
        } else if (passedMileStones[0] //17
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
            && mileStones[17].position.x > transform.position.x) {
            passedMileStones[17] = true;
            fitnessLevel = 53;
            if (mileStones[17].position.y < transform.position.y
             && mileStones[17].position.y + 1 > transform.position.y) {
                fitnessLevel += 26;
            }
            currentAtMilestone[17] = current;
        } else if (passedMileStones[0] //18
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
            && mileStones[18].position.x + 1 > transform.position.x
            && mileStones[18].position.y + 1 > transform.position.y) {
            passedMileStones[18] = true;
            fitnessLevel = 60;
            if (mileStones[18].position.x < transform.position.x
             && mileStones[18].position.y < transform.position.y) {
                fitnessLevel += 30;
            }
            currentAtMilestone[18] = current;
        } else if (passedMileStones[0] //19
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
            && mileStones[19].position.y + 1 > transform.position.y
            && mileStones[19].position.x + 1 > transform.position.x) {
            passedMileStones[19] = true;
            fitnessLevel = 67;
            if (mileStones[19].position.x > transform.position.x
             && mileStones[19].position.y > transform.position.y) {
                fitnessLevel += 32;
            }
            currentAtMilestone[19] = current;
        } else if (passedMileStones[0] //20
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
            && mileStones[20].position.y > transform.position.y) {
            passedMileStones[20] = true;
            fitnessLevel = 74;
            if (mileStones[20].position.x < transform.position.x
             && mileStones[20].position.x + 1 > transform.position.x) {
                fitnessLevel += 37;
            }
            currentAtMilestone[20] = current;
        } else if (passedMileStones[0] //21
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
            && mileStones[21].position.y > transform.position.y) {
            passedMileStones[21] = true;
            fitnessLevel = 80;
            if (mileStones[21].position.x < transform.position.x
             && mileStones[21].position.x + 1 > transform.position.x) {
                fitnessLevel += 40;
            }
            currentAtMilestone[21] = current;
        } else if (passedMileStones[0] //22
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
            && mileStones[22].position.y > transform.position.y) {
            passedMileStones[22] = true;
            fitnessLevel = 88;
            if (mileStones[22].position.x < transform.position.x
             && mileStones[22].position.x + 1 > transform.position.x) {
                fitnessLevel += 44;
            }
            currentAtMilestone[22] = current;
        } else if (passedMileStones[0] //23
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
            && mileStones[23].position.y > transform.position.y) {
            passedMileStones[23] = true;
            fitnessLevel = 96;
            if (mileStones[23].position.x < transform.position.x
             && mileStones[23].position.x + 1 > transform.position.x) {
                fitnessLevel += 48;
            }
            currentAtMilestone[23] = current;
        } else if (passedMileStones[0] //24
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
            && mileStones[24].position.y > transform.position.y) {
            passedMileStones[24] = true;
            fitnessLevel = 104;
            if (mileStones[24].position.x < transform.position.x
             && mileStones[24].position.x + 1 > transform.position.x) {
                fitnessLevel += 52;
            }
            currentAtMilestone[24] = current;
        } else if (passedMileStones[0] //25
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
            && mileStones[25].position.y > transform.position.y) {
            passedMileStones[25] = true;
            fitnessLevel = 112;
            if (mileStones[25].position.x < transform.position.x
             && mileStones[25].position.x + 1 > transform.position.x) {
                fitnessLevel += 56;
            }
            currentAtMilestone[25] = current;
        } else if (passedMileStones[0] //26
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
            && mileStones[26].position.y > transform.position.y) {
            passedMileStones[26] = true;
            fitnessLevel = 120;
            if (mileStones[26].position.x < transform.position.x
             && mileStones[26].position.x + 1 > transform.position.x) {
                fitnessLevel += 60;
            }
            currentAtMilestone[26] = current;
        } else if (passedMileStones[0] //27
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
            && mileStones[27].position.y > transform.position.y) {
            passedMileStones[27] = true;
            fitnessLevel = 128;
            if (mileStones[27].position.x < transform.position.x
             && mileStones[27].position.x + 1 > transform.position.x) {
                fitnessLevel += 64;
            }
            currentAtMilestone[27] = current;
        } else if (passedMileStones[0] //28
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
            && mileStones[28].position.y > transform.position.y) {
            passedMileStones[28] = true;
            fitnessLevel = 136;
            if (mileStones[28].position.x < transform.position.x
             && mileStones[28].position.x + 1 > transform.position.x) {
                fitnessLevel += 68;
            }
            currentAtMilestone[28] = current;
        } else if (passedMileStones[0] //29
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
            && mileStones[29].position.y > transform.position.y) {
            passedMileStones[29] = true;
            fitnessLevel = 144;
            if (mileStones[29].position.x < transform.position.x
             && mileStones[29].position.x + 1 > transform.position.x) {
                fitnessLevel += 72;
            }
            currentAtMilestone[29] = current;
        } else if (passedMileStones[0] //30
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
            && mileStones[30].position.y + 1 > transform.position.y
            && mileStones[30].position.x < transform.position.x) {
            passedMileStones[30] = true;
            fitnessLevel = 154;
            if (mileStones[30].position.y < transform.position.y
             && mileStones[30].position.x + 1 > transform.position.x) {
                fitnessLevel += 77;
            }
            currentAtMilestone[30] = current;
        } else if (passedMileStones[0] //31
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
            && mileStones[31].position.y + 1 > transform.position.y
            && mileStones[31].position.x < transform.position.x) {
            passedMileStones[31] = true;
            fitnessLevel = 170;
            if (mileStones[31].position.y < transform.position.y
             && mileStones[31].position.x + 1 > transform.position.x) {
                fitnessLevel += 85;
            }
            currentAtMilestone[31] = current;
        } else if (passedMileStones[0] //32
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
            && mileStones[32].position.x < transform.position.x) {
            passedMileStones[32] = true;
            fitnessLevel = 180;
            if (mileStones[32].position.y > transform.position.y
             && mileStones[32].position.y + 1 < transform.position.y) {
                fitnessLevel += 90;
            }
            currentAtMilestone[32] = current;
        } else if (passedMileStones[0] //33
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
            && mileStones[33].position.x < transform.position.x) {
            passedMileStones[33] = true;
            fitnessLevel = 190;
            if (mileStones[33].position.y > transform.position.y
             && mileStones[33].position.y + 1 < transform.position.y) {
                fitnessLevel += 95;
            }
            currentAtMilestone[33] = current;
        } else if (passedMileStones[0] //34
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
            && passedMileStones[33]
            && !passedMileStones[34]
            && mileStones[34].position.x < transform.position.x) {
            passedMileStones[34] = true;
            fitnessLevel = 300;
            if (mileStones[34].position.y > transform.position.y
             && mileStones[34].position.y + 1 < transform.position.y) {
                fitnessLevel += 200;
            }
            currentAtMilestone[34] = current;
        } else if (passedMileStones[0] //35
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
            && passedMileStones[33]
            && passedMileStones[34]
            && !passedMileStones[35]
            && mileStones[35].position.x < transform.position.x) {
            passedMileStones[35] = true;
            fitnessLevel = 310;
            if (mileStones[35].position.y > transform.position.y
             && mileStones[35].position.y + 1 < transform.position.y) {
                fitnessLevel += 205;
            }
            currentAtMilestone[35] = current;
        } else if (passedMileStones[0] //36
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && !passedMileStones[36]
            && mileStones[36].position.x < transform.position.x
            && mileStones[36].position.y < transform.position.y) {
            passedMileStones[36] = true;
            fitnessLevel = 320;
            if (mileStones[36].position.x + 1 > transform.position.x
             && mileStones[36].position.y + 1 > transform.position.y) {
                fitnessLevel += 210;
            }
            currentAtMilestone[36] = current;
        } else if (passedMileStones[0] //37
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && !passedMileStones[37]
            && mileStones[37].position.x < transform.position.x
            && mileStones[37].position.y < transform.position.y) {
            passedMileStones[37] = true;
            fitnessLevel = 335;
            if (mileStones[37].position.x + 1 > transform.position.x
             && mileStones[37].position.y + 1 > transform.position.y) {
                fitnessLevel += 217;
            }
            currentAtMilestone[37] = current;
        } else if (passedMileStones[0] //38
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && !passedMileStones[38]
            && mileStones[38].position.y < transform.position.y) {
            passedMileStones[38] = true;
            fitnessLevel = 350;
            if (mileStones[38].position.x + 1 > transform.position.x
             && mileStones[38].position.y + 1 > transform.position.y) {
                fitnessLevel += 225;
            }
            currentAtMilestone[38] = current;
        } else if (passedMileStones[0] //39
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && !passedMileStones[39]
            && mileStones[39].position.y < transform.position.y) {
            passedMileStones[39] = true;
            fitnessLevel = 365;
            if (mileStones[39].position.x < transform.position.x
             && mileStones[39].position.x + 1 > transform.position.x) {
                fitnessLevel += 232;
            }
            currentAtMilestone[39] = current;
        } else if (passedMileStones[0] //40
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && !passedMileStones[40]
            && mileStones[40].position.y < transform.position.y) {
            passedMileStones[40] = true;
            fitnessLevel = 380;
            if (mileStones[40].position.x < transform.position.x
             && mileStones[40].position.x + 1 > transform.position.x) {
                fitnessLevel += 240;
            }
            currentAtMilestone[40] = current;
        } else if (passedMileStones[0] //41
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && !passedMileStones[41]
            && mileStones[41].position.y < transform.position.y) {
            passedMileStones[41] = true;
            fitnessLevel = 395;
            if (mileStones[41].position.x < transform.position.x
             && mileStones[41].position.x + 1 > transform.position.x) {
                fitnessLevel += 250;
            }
            currentAtMilestone[41] = current;
        } else if (passedMileStones[0] //42
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && !passedMileStones[42]
            && mileStones[42].position.y < transform.position.y) {
            passedMileStones[42] = true;
            fitnessLevel = 410;
            if (mileStones[42].position.x < transform.position.x
             && mileStones[42].position.x + 1 > transform.position.x) {
                fitnessLevel += 260;
            }
            currentAtMilestone[42] = current;
        } else if (passedMileStones[0] //43
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && !passedMileStones[43]
            && mileStones[43].position.y < transform.position.y) {
            passedMileStones[43] = true;
            fitnessLevel = 425;
            if (mileStones[43].position.x < transform.position.x
             && mileStones[43].position.x + 1 > transform.position.x) {
                fitnessLevel += 262;
            }
            currentAtMilestone[43] = current;
        } else if (passedMileStones[0] //44
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && !passedMileStones[44]
            && mileStones[44].position.y < transform.position.y) {
            passedMileStones[44] = true;
            fitnessLevel = 440;
            if (mileStones[44].position.x < transform.position.x
             && mileStones[44].position.x + 1 > transform.position.x) {
                fitnessLevel += 270;
            }
            currentAtMilestone[44] = current;
        } else if (passedMileStones[0] //45
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && !passedMileStones[45]
            && mileStones[45].position.y < transform.position.y) {
            passedMileStones[45] = true;
            fitnessLevel = 455;
            if (mileStones[45].position.x < transform.position.x
             && mileStones[45].position.x + 1 > transform.position.x) {
                fitnessLevel += 277;
            }
            currentAtMilestone[45] = current;
        } else if (passedMileStones[0] //46
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && !passedMileStones[46]
            && mileStones[46].position.y < transform.position.y) {
            passedMileStones[46] = true;
            fitnessLevel = 470;
            if (mileStones[46].position.x < transform.position.x
             && mileStones[46].position.x + 1 > transform.position.x) {
                fitnessLevel += 285;
            }
            currentAtMilestone[46] = current;
        } else if (passedMileStones[0] //47
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && !passedMileStones[47]
            && mileStones[47].position.y < transform.position.y
            && mileStones[47].position.x + 1 > transform.position.x) {
            passedMileStones[47] = true;
            fitnessLevel = 490;
            if (mileStones[47].position.x < transform.position.x
             && mileStones[47].position.y + 1 > transform.position.y) {
                fitnessLevel += 277;
            }
            currentAtMilestone[47] = current;
        } else if (passedMileStones[0] //48
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && !passedMileStones[48]
            && mileStones[48].position.x > transform.position.x) {
            passedMileStones[48] = true;
            fitnessLevel = 500;
            if (mileStones[48].position.y < transform.position.y
             && mileStones[48].position.y + 1 > transform.position.y) {
                fitnessLevel += 300;
            }
            currentAtMilestone[48] = current;
        } else if (passedMileStones[0] //49
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && !passedMileStones[49]
            && mileStones[49].position.x > transform.position.x) {
            passedMileStones[49] = true;
            fitnessLevel = 515;
            if (mileStones[49].position.y < transform.position.y
             && mileStones[49].position.y + 1 > transform.position.y) {
                fitnessLevel += 308;
            }
            currentAtMilestone[49] = current;
        } else if (passedMileStones[0] //50
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && !passedMileStones[50]
            && mileStones[50].position.x > transform.position.x) {
            passedMileStones[50] = true;
            fitnessLevel = 540;
            if (mileStones[50].position.y < transform.position.y
             && mileStones[50].position.y + 1 > transform.position.y) {
                fitnessLevel += 320;
            }
            currentAtMilestone[50] = current;
        } else if (passedMileStones[0] //51
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && !passedMileStones[51]
            && mileStones[51].position.x + 1 > transform.position.x
            && mileStones[51].position.y + 1 > transform.position.y) {
            passedMileStones[51] = true;
            fitnessLevel = 565;
            if (mileStones[51].position.x < transform.position.x
             && mileStones[51].position.y < transform.position.y) {
                fitnessLevel += 332;
            }
            currentAtMilestone[51] = current;
        } else if (passedMileStones[0] //52
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && !passedMileStones[52]
            && mileStones[52].position.y + 1 > transform.position.y
            && mileStones[52].position.x + 1 > transform.position.x) {
            passedMileStones[52] = true;
            fitnessLevel = 595;
            if (mileStones[52].position.x < transform.position.x
             && mileStones[52].position.y < transform.position.y) {
                fitnessLevel += 332;
            }
            currentAtMilestone[52] = current;
        } else if (passedMileStones[0] //53
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && !passedMileStones[53]
            && mileStones[53].position.y > transform.position.y) {
            passedMileStones[53] = true;
            fitnessLevel = 630;
            if (mileStones[53].position.x < transform.position.x
             && mileStones[53].position.x + 1 > transform.position.x) {
                fitnessLevel += 365;
            }
            currentAtMilestone[53] = current;
        } else if (passedMileStones[0] //54
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && !passedMileStones[54]
            && mileStones[54].position.y > transform.position.y) {
            passedMileStones[54] = true;
            fitnessLevel = 660;
            if (mileStones[54].position.x < transform.position.x
             && mileStones[54].position.x + 1 > transform.position.x) {
                fitnessLevel += 380;
            }
            currentAtMilestone[54] = current;
        } else if (passedMileStones[0] //55
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && !passedMileStones[55]
            && mileStones[55].position.y > transform.position.y) {
            passedMileStones[55] = true;
            fitnessLevel = 690;
            if (mileStones[55].position.x < transform.position.x
             && mileStones[55].position.x + 1 > transform.position.x) {
                fitnessLevel += 395;
            }
            currentAtMilestone[55] = current;
        } else if (passedMileStones[0] //56
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && !passedMileStones[56]
            && mileStones[56].position.y > transform.position.y) {
            passedMileStones[56] = true;
            fitnessLevel = 720;
            if (mileStones[56].position.x < transform.position.x
             && mileStones[56].position.x + 1 > transform.position.x) {
                fitnessLevel += 410;
            }
            currentAtMilestone[56] = current;
        } else if (passedMileStones[0] //57
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && !passedMileStones[57]
            && mileStones[57].position.y > transform.position.y) {
            passedMileStones[57] = true;
            fitnessLevel = 750;
            if (mileStones[57].position.x < transform.position.x
             && mileStones[57].position.x + 1 > transform.position.x) {
                fitnessLevel += 425;
            }
            currentAtMilestone[57] = current;
        } else if (passedMileStones[0] //58
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && !passedMileStones[58]
            && mileStones[58].position.y > transform.position.y) {
            passedMileStones[58] = true;
            fitnessLevel = 780;
            if (mileStones[58].position.x < transform.position.x
             && mileStones[58].position.x + 1 > transform.position.x) {
                fitnessLevel += 440;
            }
            currentAtMilestone[58] = current;
        } else if (passedMileStones[0] //59
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && !passedMileStones[59]
            && mileStones[59].position.y + 1 > transform.position.y
            && mileStones[59].position.x < transform.position.x) {
            passedMileStones[59] = true;
            fitnessLevel = 820;
            if (mileStones[59].position.y < transform.position.y
             && mileStones[59].position.x + 1 > transform.position.x) {
                fitnessLevel += 460;
            }
            currentAtMilestone[59] = current;
        } else if (passedMileStones[0] //60
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && !passedMileStones[60]
            && mileStones[60].position.x < transform.position.x) {
            passedMileStones[60] = true;
            fitnessLevel = 860;
            if (mileStones[60].position.y < transform.position.y
             && mileStones[60].position.y + 1 > transform.position.y) {
                fitnessLevel += 470;
            }
            currentAtMilestone[60] = current;
        } else if (passedMileStones[0] //61
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && !passedMileStones[61]
            && mileStones[61].position.x < transform.position.x) {
            passedMileStones[61] = true;
            fitnessLevel = 900;
            if (mileStones[61].position.y < transform.position.y
             && mileStones[61].position.y + 1 > transform.position.y) {
                fitnessLevel += 500;
            }
            currentAtMilestone[61] = current;
        } else if (passedMileStones[0] //62
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && passedMileStones[61]
            && !passedMileStones[62]
            && mileStones[62].position.x < transform.position.x
            && mileStones[62].position.y < transform.position.y) {
            passedMileStones[62] = true;
            fitnessLevel = 940;
            if (mileStones[62].position.y + 1 > transform.position.y
             && mileStones[62].position.x + 1 > transform.position.x) {
                fitnessLevel += 520;
            }
            currentAtMilestone[62] = current;
        } else if (passedMileStones[0] //63
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && passedMileStones[61]
            && passedMileStones[62]
            && !passedMileStones[63]
            && mileStones[63].position.y < transform.position.y) {
            passedMileStones[63] = true;
            fitnessLevel = 980;
            if (mileStones[63].position.x < transform.position.x
             && mileStones[63].position.x + 1 > transform.position.x) {
                fitnessLevel += 540;
            }
            currentAtMilestone[63] = current;
        } else if (passedMileStones[0] //64
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && passedMileStones[61]
            && passedMileStones[62]
            && passedMileStones[63]
            && !passedMileStones[64]
            && mileStones[64].position.y < transform.position.y) {
            passedMileStones[64] = true;
            fitnessLevel = 1020;
            if (mileStones[64].position.x < transform.position.x
             && mileStones[64].position.x + 1 > transform.position.x) {
                fitnessLevel += 570;
            }
            currentAtMilestone[64] = current;
        } else if (passedMileStones[0] //65
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && passedMileStones[61]
            && passedMileStones[62]
            && passedMileStones[63]
            && passedMileStones[64]
            && !passedMileStones[65]
            && mileStones[65].position.y < transform.position.y) {
            passedMileStones[65] = true;
            fitnessLevel = 1060;
            if (mileStones[65].position.x < transform.position.x
             && mileStones[65].position.x + 1 > transform.position.x) {
                fitnessLevel += 580;
            }
            currentAtMilestone[65] = current;
        } else if (passedMileStones[0] //66
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && passedMileStones[61]
            && passedMileStones[62]
            && passedMileStones[63]
            && passedMileStones[64]
            && passedMileStones[65]
            && !passedMileStones[66]
            && mileStones[66].position.y < transform.position.y) {
            passedMileStones[66] = true;
            fitnessLevel = 1100;
            if (mileStones[66].position.x < transform.position.x
             && mileStones[66].position.x + 1 > transform.position.x) {
                fitnessLevel += 600;
            }
            currentAtMilestone[66] = current;
        } else if (passedMileStones[0] //67
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && passedMileStones[61]
            && passedMileStones[62]
            && passedMileStones[63]
            && passedMileStones[64]
            && passedMileStones[65]
            && passedMileStones[66]
            && !passedMileStones[67]
            && mileStones[67].position.y < transform.position.y) {
            passedMileStones[67] = true;
            fitnessLevel = 1140;
            if (mileStones[67].position.x < transform.position.x
             && mileStones[67].position.x + 1 > transform.position.x) {
                fitnessLevel += 620;
            }
            currentAtMilestone[67] = current;
        } else if (passedMileStones[0] //68
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && passedMileStones[61]
            && passedMileStones[62]
            && passedMileStones[63]
            && passedMileStones[64]
            && passedMileStones[65]
            && passedMileStones[66]
            && passedMileStones[67]
            && !passedMileStones[68]
            && mileStones[68].position.y < transform.position.y) {
            passedMileStones[68] = true;
            fitnessLevel = 1180;
            if (mileStones[68].position.x < transform.position.x
             && mileStones[68].position.x + 1 > transform.position.x) {
                fitnessLevel += 640;
            }
            currentAtMilestone[68] = current;
        } else if (passedMileStones[0] //69
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
            && passedMileStones[33]
            && passedMileStones[34]
            && passedMileStones[35]
            && passedMileStones[36]
            && passedMileStones[37]
            && passedMileStones[38]
            && passedMileStones[39]
            && passedMileStones[40]
            && passedMileStones[41]
            && passedMileStones[42]
            && passedMileStones[43]
            && passedMileStones[44]
            && passedMileStones[45]
            && passedMileStones[46]
            && passedMileStones[47]
            && passedMileStones[48]
            && passedMileStones[49]
            && passedMileStones[50]
            && passedMileStones[51]
            && passedMileStones[52]
            && passedMileStones[53]
            && passedMileStones[54]
            && passedMileStones[55]
            && passedMileStones[56]
            && passedMileStones[57]
            && passedMileStones[58]
            && passedMileStones[59]
            && passedMileStones[60]
            && passedMileStones[61]
            && passedMileStones[62]
            && passedMileStones[63]
            && passedMileStones[64]
            && passedMileStones[65]
            && passedMileStones[66]
            && passedMileStones[67]
            && passedMileStones[68]
            && !passedMileStones[69]
            && mileStones[69].position.y < transform.position.y) {
            passedMileStones[69] = true;
            fitnessLevel = 1220;
            if (mileStones[69].position.x < transform.position.x
             && mileStones[69].position.x + 1 > transform.position.x) {
                fitnessLevel += 700;
            }
            currentAtMilestone[69] = current;
        }
    }
    void calcuteFitness() {
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
