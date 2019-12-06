using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMed : MonoBehaviour {
    // Start is called before the first frame update
    private int numRockets = 200;
    public GameObject rocketPrefab;
    private GameObject[] rockets = new GameObject[200];
    private MissileControlMed[] rocketsControl = new MissileControlMed[200];
    public float speed;
    public Transform goalTransform;
    private int numGenes = 200;
    private float geneRange = 40f;
    public Transform startPos;
    public Transform[] mileStones = new Transform[13];
    private bool[] passedMilestone = new bool[13];
    public int currentMilestoneLevel = 0;
    public int currentGen = 1;
    private int currentRange = 0;
    private int prevRange = 0;
    private int numMilestones = 13;

    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        for (int i = 0; i < numRockets; i++) {
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlMed>();
            rocketsControl[i].isReady = true;
            rocketsControl[i].thrusterLeftForces = createForces();
            rocketsControl[i].thrusterRightForces = createForces();
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].mileStones = mileStones;
        }
    }
    float[] createForces() {
        float[] forces = new float[numGenes];
        for (int i = 0; i < numGenes; i++) {
            forces[i] = UnityEngine.Random.Range(0f, geneRange);
        }
        return forces;
    }
    // Update is called once per frame
    void Update() {
        if (finished(rocketsControl)) {
            float[] crashPos = rocketsControl[0].crashPos;
            bool samePos = true;
            for (int i = 0; i < numRockets; i++) {
                samePos = samePos && ((Math.Abs(rocketsControl[i].crashPos[0] - crashPos[0]) <= .1) && (Math.Abs(rocketsControl[i].crashPos[1] - crashPos[1]) <= .1));
            }
            if (samePos) {
                Quaternion rotation = new Quaternion(0, 0, 0, 1);
                for (int i = 0; i < numRockets; i++) {
                    float[][] forces = new float[][] { rocketsControl[i].thrusterLeftForces, rocketsControl[i].thrusterRightForces };
                    Destroy(rockets[i]);
                    rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
                    rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlMed>();
                    rocketsControl[i].thrusterLeftForces = forces[0];
                    rocketsControl[i].thrusterRightForces = forces[1];
                    rocketsControl[i].goalTransform = goalTransform;
                    rocketsControl[i].speed = speed;
                    rocketsControl[i].mileStones = mileStones;
                }
                for (int i = 0; i < numRockets; i++) {
                    rocketsControl[i].isReady = true;
                }
            } else {
                double totalFitness = 0;
                //get total fitness of all rockets
                double maxFitness = 0;
                for (int i = 0; i < rocketsControl.Length; i++) {
                    if (rocketsControl[i].fitness > maxFitness) {
                        maxFitness = rocketsControl[i].fitness;
                    }
                    totalFitness += rocketsControl[i].fitness;
                }
                //normalize rocket fitness and get a fraction of 200
                for (int i = 0; i < rocketsControl.Length; i++) {
                    rocketsControl[i].fitness = Math.Floor((rocketsControl[i].fitness / totalFitness) * 1000);
                }
                List<MissileControlMed> matingpool = new List<MissileControlMed>();
                for (int i = 0; i < numRockets; i++) {
                    for (int j = 0; j < rocketsControl[i].fitness; j++) {
                        matingpool.Add(rocketsControl[i]);
                    }
                }
                mutationRange();
                destroyAndCreate(matingpool);
            }
            currentGen++;
        }
    }
    bool finished(MissileControlMed[] rc) {
        bool finished = true;
        for (int i = 0; i < numRockets; i++) {
            finished = finished && rc[i].finished;
            if (!finished) {
                return finished;
            }
        }
        return finished;
    }
    void destroyAndCreate(List<MissileControlMed> matingpool) {
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        //assign random mass?
        for (int i = 0; i < numRockets; i++) {
            int parent1 = UnityEngine.Random.Range(0, matingpool.Count);
            int parent2 = UnityEngine.Random.Range(0, matingpool.Count);
            float[][] forces = mate(matingpool[parent1], matingpool[parent2]);
            Destroy(rockets[i]);
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlMed>();
            rocketsControl[i].thrusterLeftForces = forces[0];
            rocketsControl[i].thrusterRightForces = forces[1];
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].speed = speed;
            rocketsControl[i].mileStones = mileStones;
        }
        for (int i = 0; i < numRockets; i++) {
            rocketsControl[i].isReady = true;
        }
    }
    void mutationRange() {
        int numPassed = 0;
        int currentAvg = 0;
        int numHitTarget = 0;
        if (currentMilestoneLevel < numMilestones) {
            if (!passedMilestone[currentMilestoneLevel]) {
                for (int i = 0; i < rocketsControl.Length; i++) {
                    if (rocketsControl[i].passedMileStones[currentMilestoneLevel]) {
                        currentAvg += rocketsControl[i].currentAtMilestone[currentMilestoneLevel];
                        numPassed++;
                    }
                    if (rocketsControl[i].reachedGoal) {
                        numHitTarget++;
                    }
                }
            }
        }
        if (((float)numPassed / (float)rocketsControl.Length) > .3) {
            currentMilestoneLevel++;
            if (currentRange != numGenes - 1) {
                prevRange = currentRange;
            }
            currentRange = (currentAvg / (numPassed)) - 1;
            if (currentRange < 0) {
                currentRange = 0;
            }
        }
        if (((float)numHitTarget / (float)rocketsControl.Length) > .3) {
            currentRange = numGenes - 1;
        } else if (currentRange == numGenes - 1) {
            currentRange = prevRange;
        }
    }
    float[] mutate(float[] gene) {
        if (UnityEngine.Random.Range(0, 20) < 1) {
            int r = UnityEngine.Random.Range(5, 8);
            for (int i = 0; i < r; i++) {
                gene[UnityEngine.Random.Range(currentRange, numGenes - 1)] = UnityEngine.Random.Range(0f, geneRange);
            }
        }
        return gene;
    }
    float[][] mate(MissileControlMed parent1, MissileControlMed parent2) {
        float[] childThrusterLeft = new float[numGenes];
        float[] childThrusterRight = new float[numGenes];
        float parent1Percentage = (float)parent1.fitness / (float)(parent1.fitness + parent2.fitness);
        float parent2Percentage = (float)parent2.fitness / (float)(parent1.fitness + parent2.fitness);
        for (int i = 0; i < numGenes; i++) {
            childThrusterLeft[i] = (parent1.thrusterLeftForces[i] * parent1Percentage) + (parent2.thrusterLeftForces[i] * parent2Percentage);
            childThrusterRight[i] = (parent1.thrusterRightForces[i] * parent1Percentage) + (parent2.thrusterRightForces[i] * parent2Percentage);
        }
        return new float[][] {mutate(childThrusterLeft), mutate(childThrusterRight)};
    }
}
