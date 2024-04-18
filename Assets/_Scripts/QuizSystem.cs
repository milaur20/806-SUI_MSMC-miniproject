using UnityEngine;
using TMPro;
using Oculus.Voice.Core.Bindings.Android;

[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public bool answeredYes;
    public bool provideChallenge;
    public int challengeType; // 1 = put hand close to object, 2 put head close to object

    public GameObject answerObj;
}

public class QuizSystem : MonoBehaviour
{
    public ChallengeManager chalManager;
    public int questionIndex;
    public bool skipEvaluation;
    public bool hasCompletedChallenge;
    public int oldIndex = -1;

    public TextMeshProUGUI questionText;

    [SerializeField] GameObject ui;
    [SerializeField] GameObject yesZone;
    [SerializeField] GameObject noZone;
    [SerializeField] GameObject answerObj;
    public Vector3 originalPos;

    public QuizQuestion[] quizQuestions;

    void Start()
    {
        originalPos = answerObj.transform.position;
        EnableAnswerObj();
    }

    void Update()
    {
        if(!quizQuestions[questionIndex].provideChallenge && questionIndex != oldIndex)
        {
            UpdateQuestion();
        }
        else
        {
            
        }
        
        if(quizQuestions[questionIndex].provideChallenge && hasCompletedChallenge && questionIndex == oldIndex)
        {
            UpdateQuestion();
        }
        
        if (quizQuestions[questionIndex].provideChallenge && !hasCompletedChallenge && questionIndex != oldIndex)
        {
            if (quizQuestions[questionIndex].provideChallenge)
            {
                string challengeText = "";

                if (quizQuestions[questionIndex].challengeType == 1)
                {
                    challengeText = "Put your hand close to the spider.";
                }
                else if (quizQuestions[questionIndex].challengeType == 2)
                {
                    challengeText = "Put your head close to the spider.";
                }

                if (!string.IsNullOrEmpty(challengeText))
                {
                    string originalQuestionText = quizQuestions[questionIndex].questionText;
                    quizQuestions[questionIndex].questionText = challengeText;
                    UpdateQuestion();
                    quizQuestions[questionIndex].questionText = originalQuestionText;
                }
            }
            else
            {
                UpdateQuestion();
            }
            oldIndex = questionIndex;

            
        }

    }

    void UpdateQuestion()
    {
        ui.GetComponent<TextMeshProUGUI>().text = quizQuestions[questionIndex].questionText;
        hasCompletedChallenge = false;
        Debug.Log("Question: " + quizQuestions[questionIndex].questionText);
    }

    void EvaluateAnswer(bool answeredYes)
    {
        quizQuestions[questionIndex].answeredYes = answeredYes;
        Debug.Log(quizQuestions[questionIndex].questionText + " was answered " + (answeredYes ? "Yes" : "No"));
        questionIndex++;
        EnableAnswerObj();
    }

    public void LetGo()
    {
        bool answeredYes = false;

        Collider[] childColliders = answerObj.GetComponentsInChildren<Collider>(true);

        foreach (Collider childCollider in childColliders)
        {
            if (childCollider.gameObject.activeSelf)
            {
                if (childCollider.CompareTag("Answer"))
                {
                    if (yesZone.GetComponent<Collider>().bounds.Intersects(childCollider.bounds))
                    {
                        Debug.Log("Answered Yes");
                        answeredYes = true;
                        ResetObjtoOriginalPos(childCollider.gameObject);
                        EvaluateAnswer(answeredYes);
                    }
                    else if (noZone.GetComponent<Collider>().bounds.Intersects(childCollider.bounds))
                    {
                        Debug.Log("Answered No");
                        answeredYes = false;
                        ResetObjtoOriginalPos(childCollider.gameObject);
                        EvaluateAnswer(answeredYes);
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
        if(quizQuestions[questionIndex].answerObj.activeSelf == false)
        {
            quizQuestions[questionIndex].answerObj.SetActive(true);
        }
    }
}