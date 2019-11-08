using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainEasy : MonoBehaviour {
    // Start is called before the first frame update
    private int numRockets = 250;
    public GameObject rocketPrefab;
    private GameObject[] rockets = new GameObject[250];
    private MissileControlEasy[] rocketsControl = new MissileControlEasy[250];
    public float speed;
    public Transform goalTransform;
    private int numGenes = 50;
    private float geneRange = 25f;
    public Transform startPos;
    public Transform mileStone;
    private bool passedMilestone = false;
    private int currentMilestoneLevel = 0;
    public int currentGen = 0;
    public int currentRange = 0;

    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        for (int i = 0; i < numRockets; i++) {
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlEasy>();
            rocketsControl[i].isReady = true;
            rocketsControl[i].forcesX = createForces(true);
            rocketsControl[i].forcesY = createForces(false);
            rocketsControl[i].thrusterLeftForces = createForces(false);
            rocketsControl[i].thrusterRightForces = createForces(false);
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].mileStone = mileStone;
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
                rocketsControl[i].fitness = Math.Floor((rocketsControl[i].fitness / totalFitness) * 500);
            }
            List<float[][]> matingpool = new List<float[][]>();
            for (int i = 0; i < numRockets; i++) {
                for (int j = 0; j < rocketsControl[i].fitness; j++) {
                    matingpool.Add(new float[][] { rocketsControl[i].forcesX, rocketsControl[i].forcesY, rocketsControl[i].thrusterLeftForces, rocketsControl[i].thrusterRightForces });
                }
            }
            mutationRange();
            destroyAndCreate(matingpool);
            currentGen++;
        }
    }
    bool finished(MissileControlEasy[] rc) {
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
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        //assign random mass?
        for (int i = 0; i < numRockets; i++) {
            int parent1 = UnityEngine.Random.Range(0, matingpool.Count);
            int parent2 = UnityEngine.Random.Range(0, matingpool.Count);
            float[][] forces = mate(matingpool[parent1], matingpool[parent2]);
            Destroy(rockets[i]);
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlEasy>();
            rocketsControl[i].forcesX = forces[0];
            rocketsControl[i].forcesY = forces[1];
            rocketsControl[i].thrusterLeftForces = forces[2];
            rocketsControl[i].thrusterRightForces = forces[3];
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].speed = speed;
            rocketsControl[i].mileStone = mileStone;
        }
        for (int i = 0; i < numRockets; i++) {
            rocketsControl[i].isReady = true;
        }
    }

    void mutationRange() {
        int numPassed = 0;
        if (!passedMilestone) {
            for (int i = 0; i < rocketsControl.Length; i++) {
                if (rocketsControl[i].passedMileStone) {
                    numPassed++;
                }
            }
        }
        int numHitGoal = 0;
        for (int i = 0; i < rocketsControl.Length; i++) {
            if (rocketsControl[i].reachedGoal) {
                numHitGoal++;
            }
        }
        if (((float)numPassed / (float)rocketsControl.Length) > .8) {
            currentMilestoneLevel++;
            currentRange = 7;
        }
        if (((float)numHitGoal/ (float)rocketsControl.Length) > .8) {
            currentRange = 20;
        }
    }
    float[] mutate(float[] gene, bool X) {
        if (UnityEngine.Random.Range(0, 20) < 1) {
            for (int i = 0; i < UnityEngine.Random.Range(0, 5); i++) {
                if (X) {
                    gene[UnityEngine.Random.Range(currentRange, numGenes - 1)] = UnityEngine.Random.Range(-geneRange, geneRange);
                } else {
                    gene[UnityEngine.Random.Range(currentRange, numGenes - 1)] = UnityEngine.Random.Range(0f, geneRange * 1.5f);
                }
            }
        }
        return gene;
    }
    float[][] mate(float[][] parent1, float[][] parent2) {
        float[] childX = new float[numGenes];
        float[] childY = new float[numGenes];
        float[] childThrusterLeft = new float[numGenes];
        float[] childThrusterRight = new float[numGenes];
        for (int i = 0; i < numGenes; i++) {
            childX[i] = (parent1[0][i] + parent2[0][i]) / 2;
            childY[i] = (parent1[1][i] + parent2[1][i]) / 2;
            childThrusterLeft[i] = (parent1[2][i] + parent2[2][i]) / 2;
            childThrusterRight[i] = (parent1[3][i] + parent2[3][i]) / 2;
        }
        return new float[][] { mutate(childX, true), mutate(childY, false), mutate(childThrusterLeft, false), mutate(childThrusterRight, false) };
    }
}
