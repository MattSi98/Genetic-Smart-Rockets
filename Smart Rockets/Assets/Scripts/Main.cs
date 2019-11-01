﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Main : MonoBehaviour {
    // Start is called before the first frame update
    private int numRockets = 100;
    public GameObject rocketPrefab;
    private GameObject[] rockets = new GameObject[100];
    private MissileControl[] rocketsControl = new MissileControl[100];
    public float speed;
    public Transform goalTransform;
    private int numGenes = 50;
    private float geneRange = 50f;

    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        Vector3 position = new Vector3(0f, -1f, 43.97672f);
        Quaternion rotation = new Quaternion(-1, 0, 0, 1);
        for (int i = 0; i < numRockets; i++) {
            rockets[i] = Instantiate(rocketPrefab, position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControl>();
            rocketsControl[i].isReady = true;
            rocketsControl[i].forcesX = createForces(true);
            rocketsControl[i].forcesY = createForces(false);
            rocketsControl[i].goalTransform = goalTransform;
        }
    }
    float[] createForces(bool X) {
        float[] forces = new float[numGenes];
        if(X) {
            for (int i = 0; i < numGenes; i++) {
                forces[i] = UnityEngine.Random.Range(-geneRange, geneRange);
            }
        } else {
            for (int i = 0; i < numGenes; i++) {
                forces[i] = UnityEngine.Random.Range(0f, geneRange);
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
            //normalize rocket fitness and get a fraction of 200
            for (int i = 0; i < rocketsControl.Length; i++) {
                rocketsControl[i].fitness = Math.Floor((rocketsControl[i].fitness / totalFitness) * 200);
            }
            List<float[][]> matingpool = new List<float[][]>();
            for (int i = 0; i < numRockets; i++) {
                for (int j = 0; j < rocketsControl[i].fitness; j++) {
                    matingpool.Add(new float[][] {rocketsControl[i].forcesX, rocketsControl[i].forcesY});
                }
            }
            destroyAndCreate(matingpool);
        }
    }
    bool finished(MissileControl[] rc) {
        bool finished = true;
        for (int i = 0; i < numRockets; i++) {
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
        //assign random mass?
        for (int i = 0; i < numRockets; i++) {
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
        for (int i = 0; i < numRockets; i++) {
            rocketsControl[i].isReady = true;
        }
    }
    float[] mutate(float[] gene, bool X) {
        if(UnityEngine.Random.Range(0,20) == 10) {
            for (int i = 0; i < UnityEngine.Random.Range(0,5); i++) {
                if (X) {
                    gene[UnityEngine.Random.Range(0, 49)] = UnityEngine.Random.Range(-geneRange, geneRange);
                } else {
                    gene[UnityEngine.Random.Range(0, 49)] = UnityEngine.Random.Range(0f, geneRange);
                }
            }
        }
        return gene;
    }
    float[][] mate(float[][] parent1, float[][] parent2) {
        float[] childX = new float[numGenes];
        float[] childY = new float[numGenes];
        for (int i = 0; i < numGenes; i++) {
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
