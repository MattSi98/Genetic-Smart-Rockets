using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainEasy : MonoBehaviour {
    // Start is called before the first frame update
    private int numRockets = 250;
    public GameObject rocketPrefab;
    public GameObject qrocketPrefab;
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
    public int currentGen = 1;
    public int currentRange = 0;
    private GenerationDisplay generationDisplay;
    public bool finishedTraining;

    void Start() {
        foreach (Transform child in transform) {
            if (child.tag == "Canvas") {
                foreach (Transform grandChild in child) {
                    if (grandChild.tag == "Text") {
                        generationDisplay = grandChild.gameObject.GetComponent<GenerationDisplay>();
                    }
                }
            }
            if (child.tag == "Goal") {
                goalTransform = child;
            }
        }
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        //GameObject go = Instantiate(qrocketPrefab, startPos.position, rotation) as GameObject;
        //qLearnerRocketControl q = go.GetComponentInChildren<qLearnerRocketControl>();
        //q.goalTransform = goalTransform;
        //q.startPos = startPos;
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
        currentGen = 1;
        generationDisplay.genText.text = "Generation: " + currentGen;
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
            float percentReachedGoal = percentFinished(rocketsControl);
            if (percentReachedGoal >= .95) {
                finishedTraining = true;
                generationDisplay.genText.text = "Generation: " + currentGen + "\n Finished training in " + currentGen + " generations!";
            }
            if (!finishedTraining) {
                double totalFitness = 0;
                //get total fitness of all rockets
                for (int i = 0; i < rocketsControl.Length; i++) {
                    totalFitness += rocketsControl[i].fitness;
                }
                //normalize rocket fitness and get a fraction of 200
                for (int i = 0; i < rocketsControl.Length; i++) {
                    rocketsControl[i].fitness = Math.Floor((rocketsControl[i].fitness / totalFitness) * 500);
                }
                List<MissileControlEasy> matingpool = new List<MissileControlEasy>();
                for (int i = 0; i < numRockets; i++) {
                    for (int j = 0; j < rocketsControl[i].fitness; j++) {
                        matingpool.Add(rocketsControl[i]);
                    }
                }
                mutationRange();
                destroyAndCreate(matingpool);
                currentGen++;
                generationDisplay.genText.text = "Generation: " + currentGen;
            }   
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
    void destroyAndCreate(List<MissileControlEasy> matingpool) {
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

    float percentFinished(MissileControlEasy[] mce) {
        int numReachedGoal = 0;
        for (int i = 0; i < mce.Length; i++) {
            if(mce[i].reachedGoal) {
                numReachedGoal++;
            }
        }
        return (float)numReachedGoal/(float)numRockets;
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
    float[][] mate(MissileControlEasy parent1, MissileControlEasy parent2) {
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
