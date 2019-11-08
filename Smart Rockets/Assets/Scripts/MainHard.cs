using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainHard : MonoBehaviour {
    // Start is called before the first frame update
    private int numRockets = 500;
    public GameObject rocketPrefab;
    private GameObject[] rockets = new GameObject[500];
    private MissileControlHard[] rocketsControl = new MissileControlHard[500];
    public float speed;
    public Transform goalTransform;
    private int numGenes = 100;
    private float geneRange = 25f;
    public Transform startPos;
    public Transform[] mileStones = new Transform[16];

    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        for (int i = 0; i < numRockets; i++) {
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlHard>();
            rocketsControl[i].isReady = true;
            rocketsControl[i].forcesX = createForces(true);
            rocketsControl[i].forcesY = createForces(false);
            rocketsControl[i].thrusterLeftForces = createForces(false);
            rocketsControl[i].thrusterRightForces = createForces(false);
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].mileStones = mileStones;
        }
    }
    float[] createForces(bool X) {
        float[] forces = new float[numGenes];
        if (X) {
            for (int i = 0; i < numGenes; i++) {
                forces[i] = UnityEngine.Random.Range(-geneRange, geneRange);
            }
        } else {
            for (int i = 0; i < numGenes; i++) {
                forces[i] = UnityEngine.Random.Range(0f, geneRange * 1.5f);
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
                rocketsControl[i].fitness = Math.Floor((rocketsControl[i].fitness / totalFitness) * 1000);
            }
            List<MissileControlHard> matingpool = new List<MissileControlHard>();
            for (int i = 0; i < numRockets; i++) {
                for (int j = 0; j < rocketsControl[i].fitness; j++) {
                    matingpool.Add(rocketsControl[i]);
                }
            }
            destroyAndCreate(matingpool);
        }
    }
    bool finished(MissileControlHard[] rc) {
        bool finished = true;
        for (int i = 0; i < numRockets; i++) {
            finished = finished && rc[i].finished;
            if (!finished) {
                return finished;
            }
        }
        return finished;
    }
    void destroyAndCreate(List<MissileControlHard> matingpool) {
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        //assign random mass?
        for (int i = 0; i < numRockets; i++) {
            int parent1 = UnityEngine.Random.Range(0, matingpool.Count);
            int parent2 = UnityEngine.Random.Range(0, matingpool.Count);
            float[][] forces = mate(matingpool[parent1], matingpool[parent2]);
            Destroy(rockets[i]);
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlHard>();
            rocketsControl[i].forcesX = forces[0];
            rocketsControl[i].forcesY = forces[1];
            rocketsControl[i].thrusterLeftForces = forces[2];
            rocketsControl[i].thrusterRightForces = forces[3];
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].speed = speed;
            rocketsControl[i].mileStones = mileStones;
        }
        for (int i = 0; i < numRockets; i++) {
            rocketsControl[i].isReady = true;
        }
    }
    float[] mutate(float[] gene, bool X) {
        if (UnityEngine.Random.Range(0, 20) < 5) {
            for (int i = 0; i < UnityEngine.Random.Range(3, 6); i++) {
                if (X) {
                    gene[UnityEngine.Random.Range(0, numGenes - 1)] = UnityEngine.Random.Range(-geneRange, geneRange);
                } else {
                    gene[UnityEngine.Random.Range(0, numGenes - 1)] = UnityEngine.Random.Range(0f, geneRange * 1.5f);
                }
            }
        }
        return gene;
    }
    float[][] mate(MissileControlHard parent1, MissileControlHard parent2) {
        float[] childX = new float[numGenes];
        float[] childY = new float[numGenes];
        float[] childThrusterLeft = new float[numGenes];
        float[] childThrusterRight = new float[numGenes];
        float parent1Percentage = (float)parent1.fitness / (float)(parent1.fitness + parent2.fitness);
            float parent2Percentage = (float)parent2.fitness / (float)(parent1.fitness + parent2.fitness);
            for (int i = 0; i < numGenes; i++) {
                childX[i] = (parent1.forcesX[i] * parent1Percentage) + (parent2.forcesX[i] * parent2Percentage);
                childY[i] = (parent1.forcesY[i] * parent1Percentage) + (parent2.forcesY[i] * parent2Percentage);
                childThrusterLeft[i] = (parent1.thrusterLeftForces[i] * parent1Percentage) + (parent2.thrusterLeftForces[i] * parent2Percentage);
                childThrusterRight[i] = (parent1.thrusterRightForces[i] * parent1Percentage) + (parent2.thrusterRightForces[i] * parent2Percentage);
            }
        return new float[][] { mutate(childX, true), mutate(childY, false), mutate(childThrusterLeft, false), mutate(childThrusterRight, false) };
    }
}
