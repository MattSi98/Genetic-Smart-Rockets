using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainEasy : MonoBehaviour {
    // Start is called before the first frame update
    public int numRockets;
    public GameObject rocketPrefab;
    private GameObject[] rockets = new GameObject[200];
    private MissileControlEasy[] rocketsControl = new MissileControlEasy[200];
    public float speed;
    public Transform goalTransform;
    private int numGenes = 1000;
    private float geneRange = 40f;
    public Transform startPos;
    public Transform mileStone;
    private bool passedMilestone = false;
    private int currentMilestoneLevel = 0;
    public int currentGen = 1;
    public int currentRange = 0;
    private int prevRange = 0;
    private GenerationDisplay generationDisplay;
    public bool finishedTraining;
    public float slerpRate;

    public bool startSimulationBool = false;
    private GameObject slider1;
    private GameObject slider2;
    private GameObject slider3;
    private GameObject button1;

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
        for (int i = 0; i < numRockets; i++) {
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlEasy>();
            rocketsControl[i].isReady = true;
            rocketsControl[i].thrusterLeftForces = createForces();
            rocketsControl[i].thrusterRightForces = createForces();
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].mileStone = mileStone;
            rocketsControl[i].numGenes = numGenes;
            rocketsControl[i].speed = speed;
            rocketsControl[i].slerpRate = slerpRate;
        }
        currentGen = 1;
        generationDisplay.genText.text = "Generation: " + currentGen;

        slider1 = GameObject.Find("Slider-speed");
        slider2 = GameObject.Find("Slider-mutate");
        slider3 = GameObject.Find("Slider-population");
        button1 = GameObject.Find("Button - start sim");
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
        if (startSimulationBool == true)
        {

            slider1.SetActive(false);
            slider2.SetActive(false);
            slider3.SetActive(false);
            button1.SetActive(false);


            if (finished(rocketsControl))
            {
                float percentReachedGoal = percentFinished(rocketsControl);
                if (percentReachedGoal >= .95)
                {
                    finishedTraining = true;
                    generationDisplay.genText.text = "Generation: " + currentGen + "\n Finished training in " + currentGen + " generations!";
                }
                if (!finishedTraining)
                {
                    float[] crashPos = rocketsControl[0].crashPos;
                    bool samePos = true;
                    for (int i = 0; i < numRockets; i++)
                    {
                        samePos = samePos && ((Math.Abs(rocketsControl[i].crashPos[0] - crashPos[0]) <= .1) && (Math.Abs(rocketsControl[i].crashPos[1] - crashPos[1]) <= .1));
                    }
                    if (samePos)
                    {
                        Quaternion rotation = new Quaternion(0, 0, 0, 1);
                        for (int i = 0; i < numRockets; i++)
                        {
                            float[][] forces = new float[][] { rocketsControl[i].thrusterLeftForces, rocketsControl[i].thrusterRightForces };
                            Destroy(rockets[i]);
                            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
                            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlEasy>();
                            rocketsControl[i].thrusterLeftForces = forces[0];
                            rocketsControl[i].thrusterRightForces = forces[1];
                            rocketsControl[i].goalTransform = goalTransform;
                            rocketsControl[i].speed = speed;
                            rocketsControl[i].mileStone = mileStone;
                            rocketsControl[i].numGenes = numGenes;
                            rocketsControl[i].slerpRate = slerpRate;
                        }
                        for (int i = 0; i < numRockets; i++)
                        {
                            rocketsControl[i].isReady = true;
                        }
                    }
                    else
                    {
                        double totalFitness = 0;
                        //get total fitness of all rockets
                        double maxFitness = 0;
                        int mostFit = 0;
                        for (int i = 0; i < rocketsControl.Length; i++)
                        {
                            if (rocketsControl[i].fitness > maxFitness)
                            {
                                maxFitness = rocketsControl[i].fitness;
                                mostFit = i;
                            }
                            totalFitness += rocketsControl[i].fitness;
                        }
                        //normalize rocket fitness and get a fraction of 200
                        for (int i = 0; i < rocketsControl.Length; i++)
                        {
                            rocketsControl[i].fitness = Math.Floor((rocketsControl[i].fitness / totalFitness) * 500);
                        }
                        List<MissileControlEasy> matingpool = new List<MissileControlEasy>();
                        for (int i = 0; i < numRockets; i++)
                        {
                            for (int j = 0; j < rocketsControl[i].fitness; j++)
                            {
                                matingpool.Add(rocketsControl[i]);
                            }
                        }
                        mutationRange();
                        destroyAndCreate(matingpool, rocketsControl[mostFit]);
                    }
                    currentGen++;
                    generationDisplay.genText.text = "Generation: " + currentGen;
                }
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
    void destroyAndCreate(List<MissileControlEasy> matingpool, MissileControlEasy mostFit) {
        Quaternion rotation = new Quaternion(0, 0, 0, 1);
        //assign random mass?
        Destroy(rockets[0]);
        rockets[0] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
        rocketsControl[0] = rockets[0].GetComponentInChildren<MissileControlEasy>();
        rocketsControl[0].thrusterLeftForces = mostFit.thrusterLeftForces;
        rocketsControl[0].thrusterRightForces = mostFit.thrusterRightForces;
        rocketsControl[0].goalTransform = goalTransform;
        rocketsControl[0].speed = speed;
        rocketsControl[0].mileStone = mileStone;
        rocketsControl[0].numGenes = numGenes;
        rocketsControl[0].slerpRate = slerpRate;
        Destroy(rockets[1]);
        rockets[1] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
        rocketsControl[1] = rockets[1].GetComponentInChildren<MissileControlEasy>();
        rocketsControl[1].thrusterLeftForces = mostFit.thrusterLeftForces;
        rocketsControl[1].thrusterRightForces = mostFit.thrusterRightForces;
        rocketsControl[1].goalTransform = goalTransform;
        rocketsControl[1].speed = speed;
        rocketsControl[1].mileStone = mileStone;
        rocketsControl[1].numGenes = numGenes;
        rocketsControl[1].slerpRate = slerpRate;
        for (int i = 2; i < numRockets; i++) {
            int parent1 = UnityEngine.Random.Range(0, matingpool.Count);
            int parent2 = UnityEngine.Random.Range(0, matingpool.Count);
            float[][] forces = mate(matingpool[parent1], matingpool[parent2]);
            Destroy(rockets[i]);
            rockets[i] = Instantiate(rocketPrefab, startPos.position, rotation) as GameObject;
            rocketsControl[i] = rockets[i].GetComponentInChildren<MissileControlEasy>();
            rocketsControl[i].thrusterLeftForces = forces[0];
            rocketsControl[i].thrusterRightForces = forces[1];
            rocketsControl[i].goalTransform = goalTransform;
            rocketsControl[i].speed = speed;
            rocketsControl[i].mileStone = mileStone;
            rocketsControl[i].numGenes = numGenes;
            rocketsControl[i].slerpRate = slerpRate;
        }
        for (int i = 0; i < numRockets; i++) {
            rocketsControl[i].isReady = true;
        }
    }

    float percentFinished(MissileControlEasy[] mce) {
        int numReachedGoal = 0;
        for (int i = 0; i < mce.Length; i++) {
            if (mce[i].reachedGoal) {
                numReachedGoal++;
            }
        }
        return (float)numReachedGoal / (float)numRockets;
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
        if (((float)numHitGoal / (float)rocketsControl.Length) > .8) {
            currentRange = 20;
        }
    }
    float[] mutate(float[] gene) {
        if (UnityEngine.Random.Range(0, 20) < 1) {
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
    float[][] mate(MissileControlEasy parent1, MissileControlEasy parent2) {
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

    public void AdjustSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void AdjustMutations(float newMut)
    {
        slerpRate = newMut;
    }
    public void AdjustPop(float newPop)
    {
        numRockets = (int)newPop;
    }


    public void toggleStartSim()
    {
        startSimulationBool = true;
    }
}
