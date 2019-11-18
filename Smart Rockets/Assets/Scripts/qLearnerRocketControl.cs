using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class qLearnerRocketControl : MonoBehaviour {
    [SerializeField]
    public float speed;
    private int count;
    private Vector2 vel;
    Rigidbody2D rb;
    public bool isReady;
    public int current;
    public Transform goalTransform;
    public Transform startPos;
    public bool finished;
    private bool crashed;
    public bool reachedGoal;
    private double maxDist;
    public GameObject explosion;
    private bool exploded;
    private int[,] stateRewards;
    private float[,] V;

    int RewardFunction(int stateX, int stateY) {
        int reward = 0;
        try {
            reward = stateRewards[stateY + 5, stateX + 13];
        } catch (IndexOutOfRangeException e) {
            if (stateY + 5 > 32) {
                if (stateX + 13 > 24) {
                    reward = stateRewards[32, 24];
                } else {
                    reward = stateRewards[32, stateX + 13];
                }
            } else if (stateX + 13 > 24) {
                reward = stateRewards[stateY + 5, 24];
            }
        }
        return reward;
        
    }
    float StateTransitionProbability(int s1X, int s1Y, int s2X, int s2Y, float turn) {
        int s1reward = RewardFunction(s1X, s1Y);
        int s2reward = RewardFunction(s2X, s2Y);
        float angle = transform.rotation.eulerAngles.z + turn;
        float probability = 0;
        if (Math.Abs(s1X - s2X) <= 1 && Math.Abs(s1X - s2X) <= 1) {
            if (s1reward == 1) {
                if (angle <= 71.565 && angle >= 18.4349) {
                    probability = .4f;
                } else if (angle <= 108.4349 && angle >= 71.565) {
                    probability = .1f;
                } else if (angle <= 161.565 && angle >= 108.4349) {
                    probability = 0;
                } else if (angle <= 198.435 && angle >= 161.565) {
                    probability = 0;
                } else if (angle <= 251.5651 && angle >= 198.435) {
                    probability = 0;
                } else if (angle <= 288.435 && angle >= 251.5651) {
                    probability = .1f;
                } else if (angle <= 341.5651 && angle >= 251.5651) {
                    probability = .4f;
                } else {
                    probability = .5f;
                }
            } else if (s1reward == 2) {
                if (angle <= 71.565 && angle >= 18.4349) {
                    probability = .6f;
                } else if (angle <= 108.4349 && angle >= 71.565) {
                    probability = .3f;
                } else if (angle <= 161.565 && angle >= 108.4349) {
                    probability = 0;
                } else if (angle <= 198.435 && angle >= 161.565) {
                    probability = 0;
                } else if (angle <= 251.5651 && angle >= 198.435) {
                    probability = 0;
                } else if (angle <= 288.435 && angle >= 251.5651) {
                    probability = .3f;
                } else if (angle <= 341.5651 && angle >= 251.5651) {
                    probability = .6f;
                } else {
                    probability = .7f;
                }
            } else if (s1reward == 3) {
                if (angle <= 71.565 && angle >= 18.4349) {
                    probability = .75f;
                } else if (angle <= 108.4349 && angle >= 71.565) {
                    probability = .45f;
                } else if (angle <= 161.565 && angle >= 108.4349) {
                    probability = 0;
                } else if (angle <= 198.435 && angle >= 161.565) {
                    probability = 0;
                } else if (angle <= 251.5651 && angle >= 198.435) {
                    probability = 0;
                } else if (angle <= 288.435 && angle >= 251.5651) {
                    probability = .45f;
                } else if (angle <= 341.5651 && angle >= 251.5651) {
                    probability = .75f;
                } else {
                    probability = .85f;
                }
            } else if (s1reward == 5) {
                if (angle <= 71.565 && angle >= 18.4349) {
                    probability = .85f;
                } else if (angle <= 108.4349 && angle >= 71.565) {
                    probability = .55f;
                } else if (angle <= 161.565 && angle >= 108.4349) {
                    probability = 0;
                } else if (angle <= 198.435 && angle >= 161.565) {
                    probability = 0;
                } else if (angle <= 251.5651 && angle >= 198.435) {
                    probability = 0;
                } else if (angle <= 288.435 && angle >= 251.5651) {
                    probability = .55f;
                } else if (angle <= 341.5651 && angle >= 251.5651) {
                    probability = .85f;
                } else {
                    probability = 1f;
                }
            }
        }
        return probability;
    }
    float Qfunction(int currentX, int currentY, float turn) {
        float q = 0;
        if (currentY < -1) {
            for (int i = 0; i < 5; i++) { //y
                for (int j = currentX - 2; j < currentX - 2 + 5; j++) { //x
                    q += StateTransitionProbability(currentX, currentY, j, i - 5, turn) * (RewardFunction(currentX, currentY) + V[i, j + 13]);
                } 
            }
        } else if (currentY > 26) {
            for (int i = 28; i < 33; i++) { //y
                for (int j = currentX - 2; j < currentX - 2 + 5; j++) { //x
                    q += StateTransitionProbability(currentX, currentY, j, i - 5, turn) * (RewardFunction(currentX, currentY) + V[i, j + 13]);
                }
            }
        } else if (currentX > 10) {
            for (int i = currentY - 2; i < currentY - 2 + 5; i++) { //y
                for (int j = 20; j < 25; j++) { //x
                    q += StateTransitionProbability(currentX, currentY, j - 13, i, turn) * (RewardFunction(currentX, currentY) + V[i + 5, j]);
                }
            }
        } else {
            for (int i = currentY - 2; i < currentY - 2 + 5; i++) { //y
                for (int j = currentX - 2; j < currentX - 2 + 5; j++) { //x
                    try {
                        q += StateTransitionProbability(currentX, currentY, j, i, turn) * (RewardFunction(currentX, currentY) + V[i + 5, j + 13]);
                    } catch (IndexOutOfRangeException) {

                    }
                }
            }
        }
        return q;
    }

    float ComputeBestAction(int currentX, int currentY) {
        float angle = 0;
        float q = -99999999999999;
        for (int i = -90; i <= 90; i+=3) {
            float newQ = Qfunction(currentX, currentY, i + transform.rotation.eulerAngles.z);
            if(newQ >= q) {
                angle = i;
                q = newQ;
            }
        }
        return angle;
    }
    void Awake() {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    void Start() {
        current = 0;
        count = 0;
        finished = false;
        maxDist = Math.Sqrt(Math.Pow((double)(transform.position.x - goalTransform.position.x), 2) +
            Math.Pow((double)(transform.position.y - goalTransform.position.y), 2));
        //Physics2D.gravity = Vector2.zero;
        exploded = false;
        Physics2D.Raycast(transform.position, transform.up, 3);
        Physics2D.Raycast(transform.position, transform.up + transform.right, 2.3f);
        Physics2D.Raycast(transform.position, transform.up - transform.right, 2.3f);
        stateRewards = new int[,] {
            {-10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}, //33
            {-10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}, //32
            {-10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10, -10}, //31
            {-10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10}, //30
            {-10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10}, //29
            {-10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  3 ,  5 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10}, //28
            {-10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10}, //27
            {-10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10}, //26
            {-10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10, -10}, //25
            {-10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10, -10}, //24
            {-10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  2 ,  5 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10, -10}, //23
            {-10, -10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10}, //22
            {-10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 ,  1 , -10, -10}, //21
            {-10, -10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 , -10}, //20
            {-10, -10, -10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 ,  1 , -10, -10}, //19
            {-10, -10, -10, -10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 ,  1 , -10, -10}, //18
            {-10, -10, -10, -10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10}, //17
            {-10, -10, -10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10}, //16
            {-10, -10, -10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 , -10, -10, -10, -10, -10}, //15
            {-10, -10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10}, //14
            {-10, -10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10}, //13
            {-10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10}, //12
            {-10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10}, //11
            {-10, -10, -10, -10, -10, -10, -10,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10}, //10
            {-10, -10, -10, -10, -10, -10,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10}, //9
            {-10, -10, -10, -10, -10, -10,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10, -10}, //8
            {-10, -10, -10, -10, -10,  1 ,  1 ,  2 ,  3 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}, //7
            {-10, -10, -10, -10, -10,  1 ,  2 ,  3 ,  5 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}, //6
            {-10, -10, -10, -10,  1 ,  1 ,  2 ,  5 ,  5 ,  5 ,  3 ,  2 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}, //5
            {-10, -10, -10,  1 ,  1 ,  3 ,  5 ,  10 , 5 ,  3 ,  2 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}, //4
            {-10, -10, -10, -10,  1 ,  3 ,  5 ,  10 , 5 ,  3 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}, //3
            {-10, -10, -10, -10, -10,  1 ,  1 ,  1 ,  1 ,  1 ,  1 ,  1 , -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}, //2
            {-10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10}  //1
        };
        V = new float[,] {
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0},
            {  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0}
    };
    }

    void OnCollisionEnter2D(Collision2D collision) {
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
        Destroy(expl, 1);
        transform.position = startPos.position;
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        transform.rotation = rotation;
        //if (collision.gameObject.tag == "wall" && !reachedGoal) { //prevents it from updating fitness after it has finished the stage
        //    crashed = true;
        //    rb.freezeRotation = true;
        //    Physics2D.gravity = new Vector2(0, -9.8f);
        //    GetComponent<MeshRenderer>().enabled = false;
        //    if (!exploded) {
        //        exploded = true;
        //        ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
        //        foreach (ParticleSystem child in ps) {
        //            child.Stop();
        //        }
        //        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
        //        Destroy(expl, 1);
        //    }
        //}
        //if (collision.gameObject.tag == "Goal" && !crashed) {
        //    reachedGoal = true;
        //    crashed = true;
        //    GetComponent<MeshRenderer>().enabled = false;
        //    if (!exploded) {
        //        exploded = true;
        //        ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
        //        foreach (ParticleSystem child in ps) {
        //            child.Stop();
        //        }
        //        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
        //        Destroy(expl, 1);
        //    }
        //}
    }

    // Update is called once per frame
    void Update() {
        float angle = 0;
        if (!crashed) {
            
        }
        rb.velocity = transform.up * speed * 2;
        if (count % 5 == 0) { //change range of forces applied on rockets || remove count %10? 
            int stateX = (int)Math.Floor(transform.position.x);
            int stateY = (int)Math.Floor(transform.position.y);
            angle = ComputeBestAction(stateX, stateY);
            Debug.Log(angle);
            V[stateY + 5, stateX + 13] = Qfunction(stateX, stateY, transform.eulerAngles.z + angle);
           // Debug.Log(transform.rotation.eulerAngles);
            //current++;  //this runs 50 times in total
        }
        count++;
        Quaternion target = Quaternion.Euler(0, 0, transform.eulerAngles.z + (float)angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5f);
        if (current < 50) {
            // float x = -thrusterLeftForces[current] + thrusterRightForces[current];
            // float y = thrusterLeftForces[current] + thrusterRightForces[current];
            // double angle = Math.Atan2(y, x) * (180 / Math.PI) - 90;
            
        }
        //if (current >= 50) {
        //    current++;
        //}
        //if (current >= 200 || crashed) {
        //    finished = true;
        //}
    }
}
