using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainSpiral : MonoBehaviour {
    // Start is called before the first frame update
    private int numRockets = 250;
    public GameObject rocketPrefab;
    private GameObject[] rockets = new GameObject[250];
    private MissileControlSpiral[] rocketsControl = new MissileControlSpiral[250];
    public float speed;
    public Transform goalTransform;
    private int numGenes = 400;
    private float geneRange = 40f;
    public Transform startPos;
    public Transform[] mileStones = new Transform[70];
    private bool[] passedMilestone = new bool[70];
    public int currentMilestoneLevel = 0;
    public int currentGen = 0;
    public int currentRange = 0;
    private int prevRange = 0;
    private int numMilestones = 70;

    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        for (int i = 0; i < numRockets; i++) {
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlSpiral>();
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
                    rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlSpiral>();
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
                int mostFit = 0;
                for (int i = 0; i < rocketsControl.Length; i++) {
                    if (rocketsControl[i].fitness > maxFitness) {
                        maxFitness = rocketsControl[i].fitness;
                        mostFit = i;
                    }
                    totalFitness += rocketsControl[i].fitness;
                }
                //normalize rocket fitness and get a fraction of 200
                for (int i = 0; i < rocketsControl.Length; i++) {
                    rocketsControl[i].fitness = Math.Floor((rocketsControl[i].fitness / totalFitness) * 1000);
                }
                List<MissileControlSpiral> matingpool = new List<MissileControlSpiral>();
                for (int i = 0; i < numRockets; i++) {
                    for (int j = 0; j < rocketsControl[i].fitness; j++) {
                        matingpool.Add(rocketsControl[i]);
                    }
                }
                mutationRange();
                destroyAndCreate(matingpool, rocketsControl[mostFit]);
            }
            currentGen++;
        }
    }
    bool finished(MissileControlSpiral[] rc) {
        bool finished = true;
        for (int i = 0; i < numRockets; i++) {
            finished = finished && rc[i].finished;
            if (!finished) {
                return finished;
            }
        }
        return finished;
    }
    void destroyAndCreate(List<MissileControlSpiral> matingpool, MissileControlSpiral mostFit) {
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        //assign random mass?
        Destroy(rockets[0]);
        rockets[0] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
        rocketsControl[0] = rockets[0].GetComponentInChildren<MissileControlSpiral>();
        rocketsControl[0].thrusterLeftForces = mostFit.thrusterLeftForces;
        rocketsControl[0].thrusterRightForces = mostFit.thrusterRightForces;
        rocketsControl[0].goalTransform = goalTransform;
        rocketsControl[0].speed = speed;
        rocketsControl[0].mileStones = mileStones;
        for (int i = 2; i < numRockets; i++) {
            int parent1 = UnityEngine.Random.Range(0, matingpool.Count);
            int parent2 = UnityEngine.Random.Range(0, matingpool.Count);
            float[][] forces = mate(matingpool[parent1], matingpool[parent2]);
            Destroy(rockets[i]);
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlSpiral>();
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
        if (((float)numPassed / (float)rocketsControl.Length) > .2) {
            currentMilestoneLevel++;
            if (currentRange != numGenes - 1) {
                prevRange = currentRange;
            }
            if (!((currentAvg / (numPassed)) - 10 < 0)) {
                currentRange = (currentAvg / (numPassed)) - 10;
            }
        }
        if (((float)numHitTarget / (float)rocketsControl.Length) > .2) {
            currentRange = numGenes - 1;
        } else if (currentRange == numGenes - 1) {
            currentRange = prevRange;
        }
    }
    float[] mutate(float[] gene) {
        if (UnityEngine.Random.Range(0, 20) < 5 && currentMilestoneLevel < 70) {
            int r = UnityEngine.Random.Range(5, 15);
            int end = currentRange + 50;
            if (end > numGenes - 1) {
                end = numGenes - 1;
            }
            for (int i = 0; i < r; i++) {
                gene[UnityEngine.Random.Range(currentRange, end)] = UnityEngine.Random.Range(0f, geneRange);
            }
        }
        return gene;
    }
    float[][] mate(MissileControlSpiral parent1, MissileControlSpiral parent2) {
        float[] childThrusterLeft = new float[numGenes];
        float[] childThrusterRight = new float[numGenes];
        float parent1Percentage = (float)parent1.fitness / (float)(parent1.fitness + parent2.fitness);
        float parent2Percentage = (float)parent2.fitness / (float)(parent1.fitness + parent2.fitness);
        for (int i = 0; i < numGenes; i++) {
            childThrusterLeft[i] = (parent1.thrusterLeftForces[i] * parent1Percentage) + (parent2.thrusterLeftForces[i] * parent2Percentage);
            childThrusterRight[i] = (parent1.thrusterRightForces[i] * parent1Percentage) + (parent2.thrusterRightForces[i] * parent2Percentage);
        }
        return new float[][] { mutate(childThrusterLeft), mutate(childThrusterRight) };
    }
}
