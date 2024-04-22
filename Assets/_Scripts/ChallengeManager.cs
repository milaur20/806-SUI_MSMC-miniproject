using UnityEngine;
using Oculus.Interaction.Input;

public class ChallengeManager : MonoBehaviour
{
    public QuizSystem quizSystem;
    //public QuizQuestion quizQuestion;
    public bool isConditionMet1 = false;
    public bool isConditionMet2 = false;
    public bool isConditionMet3 = false;

public float distanceToLeftHand;
public float distanceToRightHand;
public float distanceToHmd;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject headset;

    
    public GameObject referenceObject; // Reference object to compare distance with hands
    public float distanceThreshold = 1.0f; // Distance threshold for the challenge

    void Awake()
    {
        headset = GameObject.Find("CenterEyeAnchor");
        leftHand = GameObject.Find("LeftHandAnchor");
        rightHand = GameObject.Find("RightHandAnchor");
        referenceObject = GameObject.Find("spider");
        quizSystem = FindObjectOfType<QuizSystem>();
    }

    private bool IsChallengeCompleted(int challengeNumber)
    {
        switch (challengeNumber)
        {
            case 1:
                if (referenceObject == null)
                {
                    Debug.LogError("Reference object is not assigned for Challenge 1!");
                    return false;
                }

                float distanceToLeftHand = Vector3.Distance(leftHand.transform.position, referenceObject.transform.position);
                float distanceToRightHand = Vector3.Distance(rightHand.transform.position, referenceObject.transform.position);

                return distanceToLeftHand <= distanceThreshold || distanceToRightHand <= distanceThreshold;

            case 2:
                if (referenceObject == null)
                {
                    Debug.LogError("Reference object is not assigned for Challenge 2!");
                    return false;
                }

                float distanceToHmd = Vector3.Distance(headset.transform.position, referenceObject.transform.position);

                return distanceToHmd <= distanceThreshold;

            case 3:
                // Add logic for challenge 3 completion
                return false;

            default:
                Debug.LogError("Invalid challenge number: " + challengeNumber);
                return false;
        }
    }

    void Update()
    {
        if(referenceObject == null || quizSystem.questionIndex != quizSystem.oldIndex)
        {
            referenceObject = quizSystem.quizQuestions[quizSystem.questionIndex].answerObj;
        }
        //Debug.Log(quizSystem.quizQuestions[quizSystem.questionIndex].challengeType);
        if(quizSystem.quizQuestions[quizSystem.questionIndex].challengeType == 1)
        {
            AreHandsCloseToReferenceObject();
            if (distanceToLeftHand <= distanceThreshold || distanceToRightHand <= distanceThreshold)
            {
                quizSystem.hasCompletedChallenge = true;
            }
        }
        if(quizSystem.quizQuestions[quizSystem.questionIndex].challengeType == 2)
        {
            IsHmdCloseToReferenceObject();
            if(distanceToHmd <= distanceThreshold)
            {
                quizSystem.hasCompletedChallenge = true;
            }
        }
    }
    public void AreHandsCloseToReferenceObject()
    {
        distanceToLeftHand = Vector3.Distance(referenceObject.transform.position, leftHand.transform.position);
        distanceToRightHand = Vector3.Distance(referenceObject.transform.position, rightHand.transform.position);
        //Debug.Log("Distance to left hand: " + distanceToRightHand);
        
    }
    public void IsHmdCloseToReferenceObject()
    {
        distanceToHmd = Vector3.Distance(referenceObject.transform.position, headset.transform.position);
        Debug.Log("Distance to HMD: " + distanceToHmd);
    }
}