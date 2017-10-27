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

	public AnimationCurve animCloseLR;
	public AnimationCurve animFarLR;

    public AnimationCurve animCloseC;
    public AnimationCurve animFarC;


	private double closeValueL, closeValueR,closeValueC = 0f;
    private double farValueL,farValueR, farValueC = 0f;


    double left, straight, right,
          slow,normal,fast;

    double leftMax, straightMax, rightMax,
          slowMax, normalMax, fastMax;


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

    private int turnAmount=201;
    private double turnJump = 0.01;
    private float actualCarSpeed;


    void Start()
	{
        car = GameObject.FindObjectOfType<Car> ();

        distLeftInfo.text = "0";
        
        turnTerms = new double[4, turnAmount];
        speedTerms = new double[4, 151];
        cutTurnTerms = new double[3, turnAmount];
        cutSpeedTerms = new double[3, 151];


        double j = -1;
        for(int i = 0; i < turnAmount; i++)
        {
            //Dziedzina
            turnTerms[0, i] = j;

            //Funkcja skręcania
            //Left
            if (j >= -1 && j <= -0.9)
                turnTerms[1, i] = Math.Round((-0.9-j)/(-0.9-(-1)),2);

            //Straight
            if (j >= -0.2d && j <= 0)
                turnTerms[2, i] = Math.Round((j-(-0.2))/(0-(-0.2)),2);
            else if(j>=0 && j<=0.2d)
                turnTerms[2, i] = Math.Round((0.2 - j) / (0.2 - (0)), 2);

            //Right
            if (j >= 0.9 && j <= 1)
                turnTerms[3, i] = Math.Round((j - (0.9)) / (1 - (0.9)), 2);

            //Iteracja dziedziny
            j = Math.Round (j + turnJump, 2);
        }


        for(int i=0;i<151;i++)
        {
            //Dziedzina
            speedTerms[0, i] = i;

            //Funkcja prędkości
            //Slow
            if (i >= 0 && i <= 60)
                speedTerms[1, i] = (60 - i) / (60 - (0));

            //Normal
            if (i >= 15 && i <= 75)
                speedTerms[2, i] = (i - (15)) / (75 - (15));
            else if (i >= 75 && i <= 135)
                speedTerms[2, i] = (135 - i) / (135 - (75));

            //Fast
            if (i >= 90 && i <= 150)
                speedTerms[3, i] = (i - (90)) / (150 - (90));
        }

    }

     float leftDistValue = 100;
     float rightDistValue = 100;
     float centerDistValue = 100;
    public void EvaluateStatements() {

        ////float leftDistValue = float.Parse(distanceLeft.ToString());
        ////float rightDistValue = float.Parse(distanceRight.ToString());
        //float leftDistValue = float.Parse(distanceLeft.value.ToString())/100;
        //float rightDistValue = float.Parse(distanceRight.value.ToString())/100;


        right = 0;
        straight = 0;
        left = 0;
        slow = 0;
        normal = 0;
        fast = 0;
        rightMax = 0;
        straightMax = 0;
        leftMax = 0;
        slowMax = 0;
        normalMax = 0;
        fastMax = 0;
        //Rozmywanie

        closeValueL = animCloseLR.Evaluate (leftDistValue);
		closeValueR = animCloseLR.Evaluate (rightDistValue);
        closeValueC = animCloseC.Evaluate(centerDistValue);

        farValueL = animFarLR.Evaluate(leftDistValue);
        farValueR = animFarLR.Evaluate(rightDistValue);
        farValueC = animFarC.Evaluate(centerDistValue);

        //Reguły

        //Wyświetlanie reguł
        Boolean[] rulesBool = new Boolean[8];

        //LC-C CC-C RC-C
        if(closeValueL!=0 && closeValueC!=0 && closeValueR!=0)
        {
            right = Math.Min(closeValueL, closeValueC);
            right = Math.Min(right, closeValueR);
            slow = right;
            rightMax = Math.Max(0, right);
            slowMax = Math.Max(0,slow);
            rulesBool[0] = true;
        }

        //LC-C CC-C RC-F
        if (closeValueL != 0 && closeValueC != 0 && farValueR != 0)
        {
            right = Math.Min(closeValueL, closeValueC);
            right = Math.Min(right, farValueR);
            slow = right;
            rightMax = Math.Max(rightMax, right);
            slowMax = Math.Max(slowMax, slow);
            rulesBool[1] = true;
        }

        //LC-C CC-F RC-C
        if (closeValueL != 0 && farValueC != 0 && closeValueR != 0)
        {
            straight = Math.Min(closeValueL, farValueC);
            straight = Math.Min(straight, closeValueR);
            normal = straight;
            straightMax = Math.Max(0, straight);
            normalMax = Math.Max(0, normal);
            rulesBool[2] = true;
        }

        //LC-C CC-F RC-F
        if (closeValueL != 0 && farValueC != 0 && farValueR != 0)
        {
            right = Math.Min(closeValueL, farValueC);
            right = Math.Min(right, farValueR);
            fast = right;
            rightMax = Math.Max(rightMax, right);
            fastMax = Math.Max(0, fast);
            rulesBool[3] = true;
        }

        //LC-F CC-C RC-C
        if (farValueL != 0 && closeValueC != 0 && closeValueR != 0)
        {
            left = Math.Min(farValueL, closeValueC);
            left = Math.Min(left, closeValueR);
            slow = left;
            leftMax = Math.Max(0, left);
            slowMax = Math.Max(slowMax, slow);
            rulesBool[4] = true;
        }

        //LC-F CC-C RC-F
        if (farValueL != 0 && closeValueC != 0 && farValueR != 0)
        {
            right = Math.Min(farValueL, closeValueC);
            right = Math.Min(right, farValueR);
            slow = right;
            rightMax = Math.Max(rightMax, right);
            slowMax = Math.Max(slowMax, slow);
            rulesBool[5] = true;
        }

        //LC-F CC-F RC-C
        if (farValueL != 0 && farValueC != 0 && closeValueR != 0)
        {
            left = Math.Min(farValueL, farValueC);
            left = Math.Min(left, closeValueR);
            fast = left;
            leftMax = Math.Max(leftMax, left);
            fastMax = Math.Max(fastMax, fast);
            rulesBool[6] = true;
        }

        //LC-F CC-F RC-F
        if (farValueL != 0 && farValueC != 0 && farValueR != 0)
        {
            straight = Math.Min(farValueL, farValueC);
            straight = Math.Min(straight, farValueR);
            fast = straight;
            straightMax = Math.Max(straightMax, straight);
            fastMax = Math.Max(fastMax, fast);
            rulesBool[7] = true;
        }


        //Cięcie termów
        //Funkcja skrętu
        for (int i = 0; i < turnAmount; i++)
        {
            //Left
            cutTurnTerms[0, i] = Math.Min (turnTerms[1, i], leftMax);

            //Straight
            cutTurnTerms[1, i] = Math.Min (turnTerms[2, i], straightMax);

            //Right
            cutTurnTerms[2, i] = Math.Min (turnTerms[3, i], rightMax);
        }

        //Funkcja prędkości
        for(int i=0;i<151;i++)
        {
            //Slow
            cutSpeedTerms[0, i] = Math.Min(speedTerms[1, i], slowMax);

            //Normal
            cutSpeedTerms[1, i] = Math.Min(speedTerms[2, i], normalMax);

            //Fast
            cutSpeedTerms[2, i] = Math.Min(speedTerms[3, i], fastMax);
        }

        //Suma termów

        double[] outSumTurn = new double[turnAmount];
        double[] outSumSpeed = new double[151];

        for (int i = 0; i < turnAmount; i++)
        {
            var v = Math.Max (cutTurnTerms[0, i], cutTurnTerms[1, i]);
            v = Math.Max (v, cutTurnTerms[2, i]);

            outSumTurn[i] = v;
        }

        for(int i=0;i<151;i++)
        {
            var v = Math.Max(cutSpeedTerms[0, i], cutSpeedTerms[1, i]);
            v = Math.Max(v, cutSpeedTerms[2, i]);

            outSumSpeed[i] = v;
        }

        double licznikTurn = 0;
        double mianownikTurn = 0;
        double licznikSpeed = 0;
        double mianownikSpeed = 0;

        for (int i = 0; i < turnAmount; i++)
        {
            licznikTurn += outSumTurn[i] * turnTerms[0, i];

            mianownikTurn += outSumTurn[i];
        }

        for (int i = 0; i < 151; i++)
        {
            licznikSpeed += outSumSpeed[i] * speedTerms[0, i];

            mianownikSpeed += outSumSpeed[i];
        }

        
        double turn = licznikTurn / mianownikTurn;
        double speed = licznikSpeed / mianownikSpeed;

        car.SetRotationSpeed((float)turn);
        car.SetSpeed ((float)speed);
        

        distLeftInfo.text = (leftDistValue*100f).ToString();
        distCenterInfo.text = (centerDistValue * 100f).ToString();
        distRightInfo.text = (rightDistValue*100f).ToString();
        turnInfo.text = turn.ToString();
        speedInfo.text = speed.ToString();
        inputFuzzy.text = "cL= " + closeValueL.ToString()+ "\n" + "cS= "+closeValueC.ToString() + "\n"+"cR= "+closeValueR.ToString()+"\n"+
                          "fL= " + farValueL.ToString() + "\n" + "fS= " + farValueC.ToString() + "\n" + "fR= " + farValueR.ToString();

        rules.text = "Left= " + left.ToString() + "\n" + "Straight= " + straight.ToString() + "\n" + "Right= " + right.ToString() +
                     "\n" + "Slow= " + slow.ToString() + "\n" + "Normal= " + normal.ToString() + "\n" + "Fast= " + fast.ToString();

        rulesMax.text = "LeftMAX= " + leftMax.ToString() + "\n" + "StraightMAX= " + straightMax.ToString() + "\n" + "RightMAX= " + rightMax.ToString() +
             "\n" + "SlowMAX= " + slowMax.ToString() + "\n" + "NormalMAX= " + normalMax.ToString() + "\n" + "FastMAX= " + fastMax.ToString();
        rulesType.text = "C-C-C=" + rulesBool[0] + "        C-C-F=" + rulesBool[1] + "        C-F-C=" + rulesBool[2] + "\n" + "C-F-F=" + rulesBool[3] +
                       "        F-C-C=" + rulesBool[4] + "        F-C-F=" + rulesBool[5] + "\n" + "F-F-C=" + rulesBool[6] + "        F-F-F=" + rulesBool[7];
        actualSpeedInfo.text = actualCarSpeed.ToString();


    }

    public void SetDistances(float L, float R, float C)
    {
        leftDistValue = (L)/100;
        rightDistValue = (R)/100;
        centerDistValue = (C)/100;
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
