using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour {
    [SerializeField]
    private float speed;
    bool started;
    bool gameOver;
    Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }


    void Start() {
        started = false;
        gameOver = false;
    }

    // Update is called once per frame
    void Update() {
    }
}
