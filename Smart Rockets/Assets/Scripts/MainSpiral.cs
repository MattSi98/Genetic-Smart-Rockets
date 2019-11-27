using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainSpiral : MonoBehaviour {
    // Start is called before the first frame update
    private int numRockets = 300;
    public GameObject rocketPrefab;
    private GameObject[] rockets = new GameObject[300];
    private MissileControlSpiral[] rocketsControl = new MissileControlSpiral[300];
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
        for (int i = 0; i < numRockets; i++) {
            int parent1 = UnityEngine.Random.Range(0, matingpool.Count);
            int parent2 = UnityEngine.Random.Range(0, matingpool.Count);
            float[][] forces = mate(matingpool[parent1], matingpool[parent2], mostFit);
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
        if (((float)numPassed / (float)rocketsControl.Length) > .1) {
            currentMilestoneLevel++;
            if (!((currentAvg / (numPassed + 3)) - 5 < 0)) {
                currentRange = (currentAvg / (numPassed + 3)) - 5;
            }
        }
        if (((float)numHitTarget / (float)rocketsControl.Length) > .1) {
            currentRange = numGenes - 1;
        }
    }
    float[] mutate(float[] gene) {
        if (UnityEngine.Random.Range(0, 20) < 5 && currentMilestoneLevel < 70) {
            int r = UnityEngine.Random.Range(5, 10);
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
    float[][] mate(MissileControlSpiral parent1, MissileControlSpiral parent2, MissileControlSpiral mostFit) {
        float[] childThrusterLeft = new float[numGenes];
        float[] childThrusterRight = new float[numGenes];
        float parent1Percentage = (float)parent1.fitness / (float)(parent1.fitness + parent2.fitness);
        float parent2Percentage = (float)parent2.fitness / (float)(parent1.fitness + parent2.fitness);
        for (int i = 0; i < currentRange; i++) {
            childThrusterLeft[i] = mostFit.thrusterLeftForces[i];
            childThrusterRight[i] = mostFit.thrusterRightForces[i];
        }
        for (int i = currentRange; i < numGenes; i++) {
            childThrusterLeft[i] = (parent1.thrusterLeftForces[i] * parent1Percentage) + (parent2.thrusterLeftForces[i] * parent2Percentage);
            childThrusterRight[i] = (parent1.thrusterRightForces[i] * parent1Percentage) + (parent2.thrusterRightForces[i] * parent2Percentage);
        }
        return new float[][] { mutate(childThrusterLeft), mutate(childThrusterRight) };
    }
}
