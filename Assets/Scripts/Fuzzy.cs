using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityStandardAssets.Vehicles.Car;


public class Fuzzy : MonoBehaviour {

    Car car;

    double[,] turnTerms,speedTerms;

    double[,] cutTurnTerms,cutSpeedTerms;

	public AnimationCurve TERM_CLOSE;
	public AnimationCurve TERM_FAR;
    public AnimationCurve TERM_VERY_FAR;

	private double closeValueL, closeValueR,closeValueC = 0f;
    private double farValueL,farValueR, farValueC = 0f;
    private double veryFarValueL, veryFarValueR, veryFarValueC = 0f;

    double left, straight, right,
          forward,backward,fast;

    double leftMax, straightMax, rightMax,
          forwardMax, backwardMax, fastMax;

    float leftDistValue = 25;
    float rightDistValue = 25;
    float centerDistValue = 25;

    private int arraySteps = 201;
    private double step = 0.01;
    private float actualCarSpeed;


    /// <summary>
    /// DEBUG
    /// </summary>
    public Text distLeftInfo;
    public Text distCenterInfo;
    public Text distRightInfo;
    public Text turnInfo;
    public Text speedInfo;
    public Text actualSpeedInfo;
    public Text inputFuzzy;
    public Text rules;
    public Text rulesMax;
    public Text rulesType;


    void Start()
	{
        leftDistValue = 25;
        rightDistValue = 25;
        centerDistValue = 25;

        car = GetComponentInParent<Car> ();
        distLeftInfo.text = "0";
        
        turnTerms = new double[4, arraySteps];
        speedTerms = new double[3, arraySteps];
        cutTurnTerms = new double[3, arraySteps];
        cutSpeedTerms = new double[2, arraySteps];


        //TERM TURN 
        double j = -1;
        for(int i = 0; i < arraySteps; i++)
        {
            //Dziedzina
            turnTerms[0, i] = j;


            //Funkcja skręcania
            //Left
            if (j >= -1 && j <= -0.2)
                turnTerms[1, i] = Math.Round((-0.2-j)/(-0.2-(-1)),2);

            //Straight
            if (j > -0.8 && j <= 0)
                turnTerms[2, i] = Math.Round((j-(-0.8))/(0-(-0.8)),2);
            else if(j > 0 && j<= 0.8)
                turnTerms[2, i] = Math.Round((0.8 - j)/(0.8 - (0)), 2);


            //Right
            if (j >= 0.2 && j <= 1)
                turnTerms[3, i] = Math.Round((j - (0.2)) / (1 - (0.2)), 2);

            //Iteracja dziedziny
            j = Math.Round (j + step, 2);
        }


        //SPEED TERMS
        j = -1;
        for(int i=0; i < arraySteps; i++)
        {
            //Dziedzina
            speedTerms[0, i] = j;

            //Funkcja prędkości
            //Slow
            if (j >= -1 && j <= 0)
                speedTerms[1, i] = -j;

            //Fast
            if (j >= 0 && j <= 1)
                speedTerms[2, i] = j;

            //Iteracja dziedziny
            j = Math.Round (j + step, 2);
        }

    }

    double Minimum(double v1, double v2, double v3)
    {
        var m = Math.Min (v1, v2);
        m = Math.Min (m, v3);
        return m;
    }


    public void EvaluateStatements() {

        right = 0;
        straight = 0;
        left = 0;

        forward = 0;
        backward = 0;

        rightMax = 0;
        straightMax = 0;
        leftMax = 0;

        forwardMax = 0;
        backwardMax = 0;


        //Rozmywanie
        closeValueL = TERM_CLOSE.Evaluate (leftDistValue);
		closeValueR = TERM_CLOSE.Evaluate (rightDistValue);
        closeValueC = TERM_CLOSE.Evaluate(centerDistValue);

        farValueL = TERM_FAR.Evaluate(leftDistValue);
        farValueR = TERM_FAR.Evaluate(rightDistValue);
        farValueC = TERM_FAR.Evaluate(centerDistValue);

        veryFarValueL = TERM_VERY_FAR.Evaluate (leftDistValue);
        veryFarValueR = TERM_VERY_FAR.Evaluate (rightDistValue);
        veryFarValueC = TERM_VERY_FAR.Evaluate (centerDistValue);

        //Reguły

        //CCC
        if(closeValueL!=0 && closeValueC!=0 && closeValueR!=0)
        {
            straight = Minimum (closeValueL, closeValueC, closeValueR);
            backward = straight; //Minium of terms

            straightMax = Math.Max(0, straight);
            backwardMax = Math.Max(0, backward);

        }

        //CCF
        if (closeValueL != 0 && closeValueC != 0 && farValueR != 0)
        {
            right = Minimum (closeValueL, closeValueC, farValueR);
            forward = right;

            rightMax = Math.Max(0, right);
            forwardMax = Math.Max(0, forward);

        }

        //CCV
        if (closeValueL != 0 && closeValueC != 0 && veryFarValueR != 0)
        {
            right = Minimum (closeValueL, closeValueC, veryFarValueR);
            forward = right;

            rightMax = Math.Max (rightMax, right);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //CFC
        if (closeValueL != 0 && farValueC != 0 && closeValueR != 0)
        {
            straight = Minimum (closeValueL, farValueC, closeValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //CFF
        if (closeValueL != 0 && farValueC != 0 && farValueR != 0)
        {
            right = Minimum (closeValueL, farValueC, farValueR);
            forward = right;

            rightMax = Math.Max (rightMax, right);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //CFV
        if (closeValueL != 0 && farValueC != 0 && veryFarValueR != 0)
        {
            right = Minimum (closeValueL, farValueC, veryFarValueR);
            forward = right;

            rightMax = Math.Max (rightMax, right);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //CVC
        if (closeValueL != 0 && veryFarValueC != 0 && closeValueR != 0)
        {
            straight = Minimum (closeValueL, veryFarValueC, closeValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //CVF
        if (closeValueL != 0 && veryFarValueC != 0 && farValueR != 0)
        {
            straight = Minimum (closeValueL, veryFarValueC, farValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //CVV
        if (closeValueL != 0 && veryFarValueC != 0 && veryFarValueR != 0)
        {
            straight = Minimum (closeValueL, veryFarValueC, veryFarValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FCC
        if (farValueL != 0 && closeValueC != 0 && closeValueR != 0)
        {
            left = Minimum (farValueL, closeValueC, closeValueR);
            forward = left;

            leftMax = Math.Max (0, left);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FCF
        if (farValueL != 0 && closeValueC != 0 && farValueR != 0)
        {
            left = Minimum (farValueL, closeValueC, farValueR);
            forward = left;

            leftMax = Math.Max (leftMax, left);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FCV
        if (farValueL != 0 && closeValueC != 0 && veryFarValueR != 0)
        {
            right = Minimum (farValueL, closeValueC, veryFarValueR);
            forward = right;

            rightMax = Math.Max (rightMax, right);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FFC
        if (farValueL != 0 && farValueC != 0 && closeValueR != 0)
        {
            straight = Minimum (farValueL, farValueC, closeValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FFF
        if (farValueL != 0 && farValueC != 0 && farValueR != 0)
        {
            straight = Minimum (farValueL, farValueC, farValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FFV
        if (farValueL != 0 && farValueC != 0 && veryFarValueR != 0)
        {
            right = Minimum (farValueL, farValueC, veryFarValueR);
            forward = right;

            rightMax = Math.Max (rightMax, right);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FVC
        if (farValueL != 0 && veryFarValueC != 0 && closeValueR != 0)
        {
            straight = Minimum (farValueL, veryFarValueC, closeValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FVF
        if (farValueL != 0 && veryFarValueC != 0 && farValueR != 0)
        {
            straight = Minimum (farValueL, veryFarValueC, farValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //FVV
        if (farValueL != 0 && veryFarValueC != 0 && veryFarValueR != 0)
        {
            right = Minimum (farValueL, veryFarValueC, veryFarValueR);
            forward = right;

            rightMax = Math.Max (rightMax, right);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VCC
        if (veryFarValueL != 0 && closeValueC != 0 && closeValueR != 0)
        {
            left = Minimum (veryFarValueL, closeValueC, closeValueR);
            forward = left;

            leftMax = Math.Max (leftMax, left);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VCF
        if (veryFarValueL != 0 && closeValueC != 0 && farValueR != 0)
        {
            left = Minimum (veryFarValueL, closeValueC, farValueR);
            forward = left;

            leftMax = Math.Max (leftMax, left);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VCV
        if (veryFarValueL != 0 && closeValueC != 0 && veryFarValueR != 0)
        {
            right = Minimum (veryFarValueL, closeValueC, veryFarValueR);
            forward = right;

            rightMax = Math.Max (rightMax, right);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VFC
        if (veryFarValueL != 0 && farValueC != 0 && closeValueR != 0)
        {
            left = Minimum (veryFarValueL, farValueC, closeValueR);
            forward = left;

            leftMax = Math.Max (leftMax, left);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VFF
        if (veryFarValueL != 0 && farValueC != 0 && farValueR != 0)
        {
            left = Minimum (veryFarValueL, farValueC, farValueR);
            forward = left;

            leftMax = Math.Max (leftMax, left);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VFV
        if (veryFarValueL != 0 && farValueC != 0 && veryFarValueR != 0)
        {
            left = Minimum (veryFarValueL, farValueC, veryFarValueR);
            forward = left;

            leftMax = Math.Max (leftMax, left);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VVC
        if (veryFarValueL != 0 && veryFarValueC != 0 && closeValueR != 0)
        {
            straight = Minimum (veryFarValueL, veryFarValueC, closeValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VVF
        if (veryFarValueL != 0 && veryFarValueC != 0 && farValueR != 0)
        {
            straight = Minimum (veryFarValueL, veryFarValueC, farValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }

        //VVV
        if (veryFarValueL != 0 && veryFarValueC != 0 && veryFarValueR != 0)
        {
            straight = Minimum (veryFarValueL, veryFarValueC, veryFarValueR);
            forward = straight;

            straightMax = Math.Max (straightMax, straight);
            forwardMax = Math.Max (forwardMax, forward);
        }


        //Cięcie termów
        //Funkcja skrętu
        for (int i = 0; i < arraySteps; i++)
        {
            //Left
            cutTurnTerms[0, i] = Math.Min (turnTerms[1, i], leftMax);

            //Straight
            cutTurnTerms[1, i] = Math.Min (turnTerms[2, i], straightMax);

            //Right
            cutTurnTerms[2, i] = Math.Min (turnTerms[3, i], rightMax);
        }

        //Funkcja prędkości
        for(int i=0;i< arraySteps; i++)
        {
            //Forward
            cutSpeedTerms[0, i] = Math.Min(speedTerms[1, i], backwardMax);

            //Backward
            cutSpeedTerms[1, i] = Math.Min(speedTerms[2, i], forwardMax);

        }

        //Suma termów
        double[] outSumTurn = new double[arraySteps];
        double[] outSumSpeed = new double[arraySteps];

        for (int i = 0; i < arraySteps; i++)
        {
            var v = Math.Max (cutTurnTerms[0, i], cutTurnTerms[1, i]);
            v = Math.Max (v, cutTurnTerms[2, i]);

            outSumTurn[i] = v;
        }

        for(int i=0;i< arraySteps; i++)
        {
            var v = Math.Max(cutSpeedTerms[0, i], cutSpeedTerms[1, i]);

            outSumSpeed[i] = v;
        }

        double licznikTurn = 0;
        double mianownikTurn = 0;
        double licznikSpeed = 0;
        double mianownikSpeed = 0;

        for (int i = 0; i < arraySteps; i++)
        {
            licznikTurn += outSumTurn[i] * turnTerms[0, i];

            mianownikTurn += outSumTurn[i];
        }

        for (int i = 0; i < arraySteps; i++)
        {
            licznikSpeed += outSumSpeed[i] * speedTerms[0, i];

            mianownikSpeed += outSumSpeed[i];
        }

        
        double turn = licznikTurn / mianownikTurn;
        double speed = licznikSpeed / mianownikSpeed;

        car.SetRotationSpeed((float)turn);
        car.SetSpeed ((float)speed);
        
        
        distLeftInfo.text = (leftDistValue).ToString();
        distCenterInfo.text = (centerDistValue).ToString();
        distRightInfo.text = (rightDistValue).ToString();
        turnInfo.text = turn.ToString();
        speedInfo.text = speed.ToString();
        inputFuzzy.text = "cL= " + closeValueL.ToString()+ "\n" + "cS= "+closeValueC.ToString() + "\n"+"cR= "+closeValueR.ToString()+"\n"+
                          "fL= " + farValueL.ToString() + "\n" + "fS= " + farValueC.ToString() + "\n" + "fR= " + farValueR.ToString();

        rules.text = "Left= " + left.ToString() + "\n" + "Straight= " + straight.ToString() + "\n" + "Right= " + right.ToString() +
                     "\n" + "Slow= " + forward.ToString() + "\n" + "Normal= " + backward.ToString() + "\n" + "Fast= " + fast.ToString();

        rulesMax.text = "LeftMAX= " + leftMax.ToString() + "\n" + "StraightMAX= " + straightMax.ToString() + "\n" + "RightMAX= " + rightMax.ToString() +
             "\n" + "SlowMAX= " + forwardMax.ToString() + "\n" + "NormalMAX= " + backwardMax.ToString() + "\n" + "FastMAX= " + fastMax.ToString();
        actualSpeedInfo.text = actualCarSpeed.ToString();

    }

    public void SetDistances(float L, float R, float C)
    {
        leftDistValue = (L);
        rightDistValue = (R);
        centerDistValue = (C);
    }

    public void CarSpeed(float v)
    {
        actualCarSpeed = v;
    }

	// Update is called once per frame
	void Update () {
	    EvaluateStatements ();
	}

   

}
