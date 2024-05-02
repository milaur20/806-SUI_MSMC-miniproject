using UnityEngine;
using TMPro;

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
    Challenge
}

public enum ChallengeStage
{
    ProvideChallenge,
    ListenForCompletion
}

public class QuizSystem : MonoBehaviour
{
    private QuizState currentState = QuizState.Question;
    private ChallengeStage currentChallengeState = ChallengeStage.ProvideChallenge;
    private bool answeredYes;
    public bool skipEvaluation;
    public bool hasCompletedChallenge;
    public int questionIndex;
    public int oldIndex = -1;
    public AudioSource soundCue;
    public float distanceThreshold = 0.1f;
    public float distanceToLeftHand;
    public float distanceToRightHand;

    public TextMeshProUGUI questionText;

    [SerializeField] GameObject ui;
    [SerializeField] GameObject yesZone;
    [SerializeField] GameObject noZone;

    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject headset;
    public GameObject referenceObject;

    public QuizQuestion[] quizQuestions;

    private Vector3 originalPos;

    private void Start()
    {
        originalPos = quizQuestions[questionIndex].answerObj.transform.position;
        soundCue = GetComponent<AudioSource>();
        EnableAnswerObj();
    }

    private void Update()
    {
        switch (currentState)
        {
            case QuizState.Question:
                UpdateQuestionState();
                break;
            case QuizState.Challenge:
                UpdateChallengeState();
                break;
        }
    }

    private void UpdateQuestionState()
    {
        if (skipEvaluation)
        {
            hasCompletedChallenge = true;
            HasUserFormedPistol();
            EvaluateAnswer(answeredYes);
            skipEvaluation = false;
            currentState = QuizState.Question;
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

    private void UpdateChallengeState()
    {
        switch (currentChallengeState)
        {
            case ChallengeStage.ProvideChallenge:
                ProvideChallenge();
                break;
            case ChallengeStage.ListenForCompletion:
                CheckIfChallengeCompleted();
                break;
        }
    }

    private void ProvideChallenge()
    {
        string challengeText = "";

        switch (quizQuestions[questionIndex].challengeType)
        {
            case 1:
                challengeText = "Put your hand close to the spider.";
                break;
            case 2:
                challengeText = "Put your head close to the spider.";
                break;
            case 3:
                challengeText = "Form a pistol with your hand, and aim here.";
                break;
        }

        if (!string.IsNullOrEmpty(challengeText))
        {
            ui.GetComponent<TextMeshProUGUI>().text = challengeText;
        }
        currentChallengeState = ChallengeStage.ListenForCompletion;
    }

    private void CheckIfChallengeCompleted()
    {
        switch (quizQuestions[questionIndex].challengeType)
        {
            case 1:
                if (!hasCompletedChallenge) AreHandsCloseToReferenceObject();
                break;
            case 2:
                if (!hasCompletedChallenge) IsHmdCloseToReferenceObject();
                break;
        }

        if (skipEvaluation)
        {
            hasCompletedChallenge = true;
            HasUserFormedPistol();
            EvaluateAnswer(answeredYes);
            skipEvaluation = false;
            currentState = QuizState.Question;
            return;
        }
    }

    private void GiveQuestion()
    {
        ui.GetComponent<TextMeshProUGUI>().text = quizQuestions[questionIndex].questionText;
    }

    public void EvaluateAnswer(bool answeredYes)
    {
        quizQuestions[questionIndex].answeredYes = answeredYes;
        Debug.Log(quizQuestions[questionIndex].questionText + " was answered " + (answeredYes ? "Yes" : "No"));
        soundCue.Play();
        oldIndex = questionIndex;
        questionIndex++;
        hasCompletedChallenge = false;
        EnableAnswerObj();
    }

    public void LetGo()
    {
        Collider[] childColliders = quizQuestions[questionIndex].answerObj.GetComponentsInChildren<Collider>(true);

        foreach (Collider childCollider in childColliders)
        {
            if (childCollider.gameObject.activeSelf && childCollider.CompareTag("Answer"))
            {
                if (yesZone.GetComponentInChildren<Collider>().bounds.Intersects(childCollider.bounds))
                {
                    answeredYes = true;
                    ResetObjtoOriginalPos(childCollider.gameObject);
                    EvaluateAnswer(answeredYes);
                    break;
                }
                else if (noZone.GetComponentInChildren<Collider>().bounds.Intersects(childCollider.bounds))
                {
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
        if (quizQuestions[questionIndex].provideChallenge && quizQuestions[questionIndex].challengeType == 3)
        {
            hasCompletedChallenge = true;
            quizQuestions[questionIndex].answerObj.SetActive(true);
            currentState = QuizState.Question;
        }
    }
}
