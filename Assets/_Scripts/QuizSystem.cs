using UnityEngine;
using TMPro;
using Oculus.Interaction;

[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public bool answeredYes;
    public bool provideChallenge;
    public int challengeType;
    public GameObject answerObj;
}

public enum QuizState
{
    Question,
    Challenge,
}

public class QuizSystem : MonoBehaviour
{
    private ActiveStateSelector gunPose;
    public QuizState currentState;
    public bool answeredYes;
    public int questionIndex;
    public bool skipEvaluation;
    public bool hasCompletedChallenge;
    public int oldIndex = -1;

    public float distanceThreshold = 0.1f;
    public float distanceToLeftHand;
    public float distanceToRightHand;
    public TextMeshProUGUI questionText;

    [SerializeField] GameObject ui;
    [SerializeField] GameObject yesZone;
    [SerializeField] GameObject noZone;
    public Vector3 originalPos;

    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject headset;
    public GameObject referenceObject;

    public QuizQuestion[] quizQuestions;

    

    void Start()
    {
        originalPos = quizQuestions[questionIndex].answerObj.transform.position;
        //Debug.Log(questionIndex);
        EnableAnswerObj();
        currentState = QuizState.Question;
    }

    void Update()
    {
        switch (currentState)
        {
            case QuizState.Question:
                UpdateQuestionState();
                //Debug.Log("Question state");
                break;
            case QuizState.Challenge:
                UpdateChallengeState();
                //Debug.Log("Challenge state");
                break;
        }
    }

    void UpdateQuestionState()
    {
        if (skipEvaluation)
        {
            skipEvaluation = false;
            hasCompletedChallenge = true;
            EvaluateAnswer(answeredYes);
            return;
        }

        if (questionIndex != oldIndex)
        {
            if (!quizQuestions[questionIndex].provideChallenge || hasCompletedChallenge)
            {
                GiveQuestion();
            }
            else
            {
                currentState = QuizState.Challenge;
            }
        }
    }

    void UpdateChallengeState()
    {
        ProvideChallenge();
        CheckIfChallengeCompleted();
    }

    void ProvideChallenge()
    {
        Debug.Log("Providing challenge");
        string challengeText = "";

        if (quizQuestions[questionIndex].challengeType == 1)
        {
            challengeText = "Put your hand close to the spider.";
        }
        else if (quizQuestions[questionIndex].challengeType == 2)
        {
            challengeText = "Put your head close to the spider.";
        }
        else if (quizQuestions[questionIndex].challengeType == 3)
        {
            quizQuestions[questionIndex].answerObj.SetActive(false);
            challengeText = "Form a pistol with your hand, and aim here.";
        }

        if (!string.IsNullOrEmpty(challengeText))
        {
            ui.GetComponent<TextMeshProUGUI>().text = challengeText;
        }
    }

    void CheckIfChallengeCompleted()
    {
        if (quizQuestions[questionIndex].challengeType == 1 && !hasCompletedChallenge)
        {
            AreHandsCloseToReferenceObject();
        }
        else if (quizQuestions[questionIndex].challengeType == 2 && !hasCompletedChallenge)
        {
            IsHmdCloseToReferenceObject();
        }
        /* dont need this since we check it somewhere else
        else if (quizQuestions[questionIndex].challengeType == 3 && !hasCompletedChallenge)
        {
            HasUserFormedPistol();
        }
        */
        else
        {
            currentState = QuizState.Question;
        }
    }

    void GiveQuestion()
    {
        ui.GetComponent<TextMeshProUGUI>().text = quizQuestions[questionIndex].questionText;
    }

    public void EvaluateAnswer(bool answeredYes)
    {
        Debug.Log("Evaluating answer");
        quizQuestions[questionIndex].answeredYes = answeredYes;
        Debug.Log(quizQuestions[questionIndex].questionText + " was answered " + (answeredYes ? "Yes" : "No"));
        oldIndex = questionIndex;
        questionIndex++;
        hasCompletedChallenge = false;
        EnableAnswerObj();
    }

    public void LetGo()
    {
        Debug.Log("Let go");
        Collider objColliders = quizQuestions[questionIndex].answerObj.GetComponent<Collider>();
        Collider[] childColliders = quizQuestions[questionIndex].answerObj.GetComponentsInChildren<Collider>(true);
        Debug.Log(childColliders.Length);

        foreach (Collider childCollider in childColliders)
        {
            if (childCollider.gameObject.activeSelf && childCollider.CompareTag("Answer"))
            {
                Debug.Log("Answer object found");
                if (yesZone.GetComponentInChildren<Collider>().bounds.Intersects(childCollider.bounds))
                {
                    Debug.Log("Answered Yes");
                    answeredYes = true;
                    ResetObjtoOriginalPos(childCollider.gameObject);
                    EvaluateAnswer(answeredYes);
                    break;
                }
                else if (noZone.GetComponentInChildren<Collider>().bounds.Intersects(childCollider.bounds))
                {
                    Debug.Log("Answered No");
                    answeredYes = false;
                    ResetObjtoOriginalPos(childCollider.gameObject);
                    EvaluateAnswer(answeredYes);
                    break;
                }
                else
                {
                    Debug.LogWarning("Answer object is not in the Yes or No zone.");
                }
            }
            else
            {
                Debug.LogWarning("Answer object's child collider is missing tag.");
            }
        }

        Debug.LogWarning("No active child object with collider found on answer object.");
    }

    private void ResetObjtoOriginalPos(GameObject answerChildObj)
    {
        answerChildObj.transform.position = originalPos;
        answerChildObj.transform.rotation = Quaternion.identity;
        Debug.Log("Resetting object to original position");
    }

    private void EnableAnswerObj()
    {
        if (quizQuestions[questionIndex].answerObj.activeSelf == false)
        {
            quizQuestions[questionIndex].answerObj.SetActive(true);
        }
        if (quizQuestions[oldIndex].answerObj.activeSelf == true)
        {
            quizQuestions[oldIndex].answerObj.SetActive(false);
        }
    }

    public void AreHandsCloseToReferenceObject()
    {
        if (referenceObject == null)
        {
            referenceObject = quizQuestions[questionIndex].answerObj;
        }
        distanceToLeftHand = Vector3.Distance(leftHand.transform.position, referenceObject.transform.position);
        distanceToRightHand = Vector3.Distance(rightHand.transform.position, referenceObject.transform.position);

        if (distanceToLeftHand <= distanceThreshold || distanceToRightHand <= distanceThreshold)
        {
            hasCompletedChallenge = true;
            currentState = QuizState.Question;
        }
    }

    public void IsHmdCloseToReferenceObject()
    {
        float distanceToHmd = Vector3.Distance(headset.transform.position, referenceObject.transform.position);

        if (distanceToHmd <= distanceThreshold)
        {
            hasCompletedChallenge = true;
            currentState = QuizState.Question;
        }
    }

    public void HasUserFormedPistol()
    {
        if(quizQuestions[questionIndex].provideChallenge && quizQuestions[questionIndex].challengeType == 3)
        {
            hasCompletedChallenge = true;
            quizQuestions[questionIndex].answerObj.SetActive(true);
            currentState = QuizState.Question;
        }
    }
}
