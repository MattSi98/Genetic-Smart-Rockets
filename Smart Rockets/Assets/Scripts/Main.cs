using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Main : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject rocket1;
    public GameObject rocket2;
    public GameObject rocket3;
    public GameObject rocket4;
    public GameObject rocket5;
    public GameObject rocket6;
    public Transform barrierTransform;
    public Transform goalTransform;


    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Barrier") {
                barrierTransform = child;
            }
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        float[] rocket1ForcesX = new float[50];
        float[] rocket2ForcesX = new float[50];
        float[] rocket3ForcesX = new float[50];
        float[] rocket1ForcesY = new float[50];
        float[] rocket2ForcesY = new float[50];
        float[] rocket3ForcesY = new float[50];
        float[] rocket4ForcesX = new float[50];
        float[] rocket5ForcesX = new float[50];
        float[] rocket6ForcesX = new float[50];
        float[] rocket4ForcesY = new float[50];
        float[] rocket5ForcesY = new float[50];
        float[] rocket6ForcesY = new float[50];
        MissileControl mc1;
        MissileControl mc2;
        MissileControl mc3;
        MissileControl mc4;
        MissileControl mc5;
        MissileControl mc6;
        mc1 = rocket1.GetComponentInChildren<MissileControl>();
        mc2 = rocket2.GetComponentInChildren<MissileControl>();
        mc3 = rocket3.GetComponentInChildren<MissileControl>();
        mc4 = rocket4.GetComponentInChildren<MissileControl>();
        mc5 = rocket5.GetComponentInChildren<MissileControl>();
        mc6 = rocket6.GetComponentInChildren<MissileControl>();
        for (int i = 0; i < 50; i++) {
            rocket1ForcesX[i] = Random.Range(0f, 50f);
            rocket2ForcesX[i] = Random.Range(0f, 50f);
            rocket3ForcesX[i] = Random.Range(0f, 50f);
            rocket1ForcesY[i] = Random.Range(-50f, 50f);
            rocket2ForcesY[i] = Random.Range(-50f, 50f);
            rocket3ForcesY[i] = Random.Range(-50f, 50f);
            rocket4ForcesX[i] = Random.Range(0f, 50f);
            rocket5ForcesX[i] = Random.Range(0f, 50f);
            rocket6ForcesX[i] = Random.Range(0f, 50f);
            rocket4ForcesY[i] = Random.Range(-50f, 50f);
            rocket5ForcesY[i] = Random.Range(-50f, 50f);
            rocket6ForcesY[i] = Random.Range(-50f, 50f);
        }
        mc1.isReady = true;
        mc1.forcesX = rocket1ForcesX;
        mc1.forcesY = rocket1ForcesY;
        mc2.isReady = true;
        mc2.forcesX = rocket2ForcesX;
        mc2.forcesY = rocket2ForcesY;
        mc3.isReady = true;
        mc3.forcesX = rocket3ForcesX;
        mc3.forcesY = rocket3ForcesY;
        mc4.isReady = true;
        mc4.forcesX = rocket4ForcesX;
        mc4.forcesY = rocket4ForcesY;
        mc5.isReady = true;
        mc5.forcesX = rocket5ForcesX;
        mc5.forcesY = rocket5ForcesY;
        mc6.isReady = true;
        mc6.forcesX = rocket6ForcesX;
        mc6.forcesY = rocket6ForcesY;
        mc1.goalTransform = goalTransform;
        mc2.goalTransform = goalTransform;
        mc3.goalTransform = goalTransform;
        mc4.goalTransform = goalTransform;
        mc5.goalTransform = goalTransform;
        mc6.goalTransform = goalTransform;
    }

    // Update is called once per frame
    void Update() {
        if (mc1.finished && mc2.finished && mc3.finished && mc4.finished && mc5.finished && mc6.finished) {
            double[] fitnesses = new double[] {mc1.fitness,
                                               mc2.fitness,
                                               mc3.fitness,
                                               mc4.fitness,
                                               mc5.fitness,
                                               mc6.fitness};
            double totalFitness = 0;
            for (int i = 0; i < fitnesses.Length; i++) {
                totalFitness += fitnesses[i];
            }
            for (int i = 0; i < fitnesses.Length; i++) {
                fitnesses[i] = Math.Floor((fitnesses[i] / totalFitness) * 60);
            }
            float[][] matingpool = new float[60][50];

        }
    }

}
