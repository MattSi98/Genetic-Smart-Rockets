using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Main : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject rocketPrefab;
    public GameObject[] rockets = new GameObject[50];
    public MissileControl[] rocketsControl = new MissileControl[50];
    public float speed;
    public Transform goalTransform;

    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        Vector3 position = new Vector3(0f, -1f, 43.97672f);
        Quaternion rotation = new Quaternion(-1, 0, 0, 1);
        for (int i = 0; i < 50; i++) {
            rockets[i] = Instantiate(rocketPrefab, position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControl>();
            rocketsControl[i].isReady = true;
            rocketsControl[i].forcesX = createForces(true);
            rocketsControl[i].forcesY = createForces(false);
            rocketsControl[i].goalTransform = goalTransform;
        }
    }
    float[] createForces(bool X) {
        float[] forces = new float[50];
        if(X) {
            for (int i = 0; i < 50; i++) {
                forces[i] = UnityEngine.Random.Range(-50f, 50f);
            }
        } else {
            for (int i = 0; i < 50; i++) {
                forces[i] = UnityEngine.Random.Range(0f, 50f);
            }
        }
        return forces;
    }
    // Update is called once per frame
    void Update() {
        if (finished(rocketsControl)) {
            double totalFitness = 0;
            //get total fitness of all rockets
            for (int i = 0; i < rocketsControl.Length; i++) {
                totalFitness += rocketsControl[i].fitness;
            }
            //normalize rocket fitness and get a fraction of 60
            for (int i = 0; i < rocketsControl.Length; i++) {
                rocketsControl[i].fitness = Math.Floor((rocketsControl[i].fitness / totalFitness) * 200);
            }
            List<float[][]> matingpool = new List<float[][]>();
            for (int i = 0; i < 50; i++) {
                for (int j = 0; j < rocketsControl[i].fitness; j++) {
                    matingpool.Add(new float[][] {rocketsControl[i].forcesX, rocketsControl[i].forcesY});
                }
            }
            destroyAndCreate(matingpool);
        }
    }
    bool finished(MissileControl[] rc) {
        bool finished = true;
        for (int i = 0; i < 50; i++) {
            finished = finished && rc[i].finished;
            if (!finished) {
                return finished;
            }
        }
        return finished;
    }
    void destroyAndCreate(List<float[][]> matingpool) {
        Vector3 position = new Vector3(0f, -1f, 43.97672f);
        Quaternion rotation = new Quaternion(-1, 0, 0, 1);
        Debug.Log(matingpool.Count);
        for (int i = 0; i < 50; i++) {
            int parent1 = UnityEngine.Random.Range(0, matingpool.Count);
            int parent2 = UnityEngine.Random.Range(0, matingpool.Count);
            float[][] forces = mate(matingpool[parent1], matingpool[parent2]);
            Destroy(rockets[i]);
            rockets[i] = Instantiate(rocketPrefab, position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControl>();
            rocketsControl[i].forcesX = forces[0];
            rocketsControl[i].forcesY = forces[1];
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].speed = speed;
        }
        for (int i = 0; i < 50; i++) {
            rocketsControl[i].isReady = true;
        }
    }
    float[] mutate(float[] gene, bool X) {
        if(UnityEngine.Random.Range(0,20) == 10) {
            for (int i = 0; i < UnityEngine.Random.Range(0,5); i++) {
                if (X) {
                    gene[UnityEngine.Random.Range(0, 49)] = UnityEngine.Random.Range(-50f, 50f);
                } else {
                    gene[UnityEngine.Random.Range(0, 49)] = UnityEngine.Random.Range(0f, 50f);
                }
            }
        }
        return gene;
    }
    float[][] mate(float[][] parent1, float[][] parent2) {
        float[] childX = new float[50];
        float[] childY = new float[50];
        for (int i = 0; i < 50; i++) {
            if (i % 2 == 0) {
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
