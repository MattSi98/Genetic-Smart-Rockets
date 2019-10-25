using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Main : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject rocketPrefab;
    public GameObject rocket1;
    public GameObject rocket2;
    public GameObject rocket3;
    public GameObject rocket4;
    public GameObject rocket5;
    public GameObject rocket6;
    public Transform barrierTransform;
    public Transform goalTransform;
    MissileControl mc1;
    MissileControl mc2;
    MissileControl mc3;
    MissileControl mc4;
    MissileControl mc5;
    MissileControl mc6;

    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Barrier") {
                barrierTransform = child;
            }
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        //todo fix labels
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
        mc1 = rocket1.GetComponentInChildren<MissileControl>();
        mc2 = rocket2.GetComponentInChildren<MissileControl>();
        mc3 = rocket3.GetComponentInChildren<MissileControl>();
        mc4 = rocket4.GetComponentInChildren<MissileControl>();
        mc5 = rocket5.GetComponentInChildren<MissileControl>();
        mc6 = rocket6.GetComponentInChildren<MissileControl>();
        for (int i = 0; i < 50; i++) {
            rocket1ForcesX[i] = UnityEngine.Random.Range(0f, 50f);
            rocket2ForcesX[i] = UnityEngine.Random.Range(0f, 50f);
            rocket3ForcesX[i] = UnityEngine.Random.Range(0f, 50f);
            rocket1ForcesY[i] = UnityEngine.Random.Range(-50f, 50f);
            rocket2ForcesY[i] = UnityEngine.Random.Range(-50f, 50f);
            rocket3ForcesY[i] = UnityEngine.Random.Range(-50f, 50f);
            rocket4ForcesX[i] = UnityEngine.Random.Range(0f, 50f);
            rocket5ForcesX[i] = UnityEngine.Random.Range(0f, 50f);
            rocket6ForcesX[i] = UnityEngine.Random.Range(0f, 50f);
            rocket4ForcesY[i] = UnityEngine.Random.Range(-50f, 50f);
            rocket5ForcesY[i] = UnityEngine.Random.Range(-50f, 50f);
            rocket6ForcesY[i] = UnityEngine.Random.Range(-50f, 50f);
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
            MissileControl[] rockets = new MissileControl[] {mc1, mc2, mc3, mc4, mc5, mc6};
            double totalFitness = 0;
            //get total fitness of all rockets
            for (int i = 0; i < rockets.Length; i++) {
                totalFitness += rockets[i].fitness;
            }
            //normalize rocket fitness and get a fraction of 60
            for (int i = 0; i < rockets.Length; i++) {
                rockets[i].fitness = Math.Floor((rockets[i].fitness / totalFitness) * 60);
            }
            List<float[][]> matingpool = new List<float[][]>();
            for (int i = 0; i < 6; i++) {
                for (int j = 0; j < rockets[i].fitness; j++) {
                    matingpool.Add(new float[][] {rockets[i].forcesX, rockets[i].forcesY});
                }
            }

            destroyAndCreate(matingpool);
        }
    }

    void destroyAndCreate(List<float[][]> matingpool) {
        Destroy(rocket1);
        Destroy(rocket2);
        Destroy(rocket3);
        Destroy(rocket4);
        Destroy(rocket5);
        Destroy(rocket6);
        for (int i = 0; i < 6; i++) {
            Vector3 position = new Vector3(0f , -1f, 43.97672f);
            Quaternion rotation = new Quaternion(-1, 0, 0, 1);
            int parent1 = UnityEngine.Random.Range(0, matingpool.Count);
            int parent2 = UnityEngine.Random.Range(0, matingpool.Count);
            float[][] forces = mate(matingpool[parent1], matingpool[parent2]);
            if (i == 0) {
                rocket1 = Instantiate(rocketPrefab, position, rotation) as GameObject;
                mc1 = rocket1.GetComponentInChildren<MissileControl>();
                mc1.forcesX = forces[0];
                mc1.forcesY = forces[1];
                mc1.goalTransform = goalTransform;
            }
            if (i == 1) {
                rocket2 = Instantiate(rocketPrefab, position, rotation) as GameObject;
                mc2 = rocket2.GetComponentInChildren<MissileControl>();
                mc2.forcesX = forces[0];
                mc2.forcesY = forces[1];
                mc2.goalTransform = goalTransform;
            }
            if (i == 2) {
                rocket3 = Instantiate(rocketPrefab, position, rotation) as GameObject;
                mc3 = rocket3.GetComponentInChildren<MissileControl>();
                mc3.forcesX = forces[0];
                mc3.forcesY = forces[1];
                mc3.goalTransform = goalTransform;
            }
            if (i == 3) {
                rocket4 = Instantiate(rocketPrefab, position, rotation) as GameObject;
                mc4 = rocket4.GetComponentInChildren<MissileControl>();
                mc4.forcesX = forces[0];
                mc4.forcesY = forces[1];
                mc4.goalTransform = goalTransform;
            }
            if (i == 4) {
                rocket5 = Instantiate(rocketPrefab, position, rotation) as GameObject;
                mc5 = rocket5.GetComponentInChildren<MissileControl>();
                mc5.forcesX = forces[0];
                mc5.forcesY = forces[1];
                mc5.goalTransform = goalTransform;
            }
            if (i == 5) {
                rocket6 = Instantiate(rocketPrefab, position, rotation) as GameObject;
                mc6 = rocket6.GetComponentInChildren<MissileControl>();
                mc6.forcesX = forces[0];
                mc6.forcesY = forces[1];
                mc6.goalTransform = goalTransform;
            }
        }
        mc1.isReady = true;
        mc2.isReady = true;
        mc3.isReady = true;
        mc4.isReady = true;
        mc5.isReady = true;
        mc6.isReady = true;
        mc1.speed = 3f;
        mc2.speed = 3f;
        mc3.speed = 3f;
        mc4.speed = 3f;
        mc5.speed = 3f;
        mc6.speed = 3f;

    }
    float[] mutate(float[] gene, bool X) {
        if(UnityEngine.Random.Range(0,20) == 10) {
            for (int i = 0; i < UnityEngine.Random.Range(0,5); i++) {
                if (X) {
                    gene[UnityEngine.Random.Range(0, 49)] = UnityEngine.Random.Range(0f, 50f);
                } else {
                    gene[UnityEngine.Random.Range(0, 49)] = UnityEngine.Random.Range(-50f, 50f);
                }
            }
        }
        return gene;
    }
    float[][] mate(float[][] parent1, float[][] parent2) {
        float[] childX = new float[50];
        float[] childY = new float[50];
        for (int i = 0; i < 50; i++) {
            if (i < 25) {
                childX[i] = parent1[0][i];
                childY[i] = parent1[1][i];
            } else {
                childX[i] = parent2[0][i];
                childY[i] = parent2[1][i];
            }
        }
        return new float[][] { mutate(childX, true), mutate(childY, false) };
    }

}
