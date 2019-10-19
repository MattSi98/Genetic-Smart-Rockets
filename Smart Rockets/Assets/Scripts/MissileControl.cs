using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissileControl : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField]
    private float speed;
    private bool move;
    private int count;
    private Vector2 vel;
    Rigidbody2D rb;
    public bool isReady;
    public float[] forcesX;
    public float[] forcesY;
    private int current;
    //private Transform barrierTransform;
    public Transform goalTransform;
    public double fitness;
    public bool finished;

    void Awake() {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    void Start() {
        fitness = 0f;
        move = true;
        current = 0;
        count = 0;
        finished = false;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Rocket") {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    // Update is called once per frame
    void Update() {
        if(isReady) {
            if (current < 50 && count % 10 == 0) {
                rb.AddForce(forcesY[current] * new Vector2(speed, 0));
                rb.AddForce(forcesX[current] * new Vector2(0, speed));
                current++;
            }
            count++;
        }
        if (current > 50) {
            finished = true;
        }
        calcuteFitness();
    }

    void calcuteFitness() {
        double currentFitness = Math.Sqrt(Math.Pow((double)(transform.position.x * goalTransform.position.x), 2) + 
            Math.Pow((double)(transform.position.y * goalTransform.position.y), 2));
        if (currentFitness > fitness) {
            fitness = currentFitness;
        }
    }
}
