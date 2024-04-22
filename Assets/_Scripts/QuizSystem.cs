// Importing necessary libraries
using UnityEngine;
using TMPro;
using Oculus.Voice.Core.Bindings.Android;

// Serializable class to hold quiz question data
[System.Serializable]
public class QuizQuestion
{
    public string questionText; // Text of the question
    public bool answeredYes; // Whether the question was answered with 'Yes'
    public bool provideChallenge; // Whether the question provides a challenge
    public int challengeType; // Type of challenge: 1 = put hand close to object, 2 = put head close to object
    public GameObject answerObj; // The object that represents the answer
}

// Main class for the quiz system
public class QuizSystem : MonoBehaviour
{
    public ChallengeManager challengeManager; // Reference to the challenge manager
    public int questionIndex; // Index of the current question
    public bool skipEvaluation; // Whether to skip evaluation
    public bool hasCompletedChallenge; // Whether the challenge has been completed
    public int oldIndex = -1; // Index of the previous question

    public TextMeshProUGUI questionText; // UI element to display the question text

    // Serialized fields for various UI elements
    [SerializeField] GameObject ui;
    [SerializeField] GameObject yesZone;
    [SerializeField] GameObject noZone;
    public Vector3 originalPos; // Original position of the answer object


    public bool skipQuestion = false;
    private bool answeredYes = false;
    public QuizQuestion[] quizQuestions; // Array of quiz questions

    // Method called at the start of the game
    void Start()
    {
        challengeManager = FindObjectOfType<ChallengeManager>(); // Find the challenge manager in the scene
        originalPos = quizQuestions[questionIndex].answerObj.transform.position; // Save the original position of the answer object
        Debug.Log(questionIndex);
        EnableAnswerObj(); // Enable the answer object
    }

    // Method called every frame
    void Update()
    {
        if(skipQuestion)
        {
            skipQuestion = false;
            hasCompletedChallenge = true;
            EvaluateAnswer(answeredYes);
        }
        // If the current question does not provide a challenge and it's not the same as the old question, update the question
        if(!quizQuestions[questionIndex].provideChallenge && questionIndex != oldIndex)
        {
            UpdateQuestion();
        }
        
        // If the current question provides a challenge, the challenge has been completed, and it's the same as the old question, update the question
        if(quizQuestions[questionIndex].provideChallenge && hasCompletedChallenge && questionIndex == oldIndex)
        {
            UpdateQuestion();
        }
        
        // If the current question provides a challenge, the challenge has not been completed, and it's not the same as the old question, provide the challenge
        if (quizQuestions[questionIndex].provideChallenge && !hasCompletedChallenge && questionIndex != oldIndex)
        {
            if (quizQuestions[questionIndex].provideChallenge)
            {
                string challengeText = "";

                // Set the challenge text based on the challenge type
                if (quizQuestions[questionIndex].challengeType == 1)
                {
                    challengeText = "Put your hand close to the spider.";
                }
                else if (quizQuestions[questionIndex].challengeType == 2)
                {
                    challengeText = "Put your head close to the spider.";
                }

                // If there is a challenge text, update the question text with it, update the question, and then restore the original question text
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
        }
    }

    // Method to update the question text in the UI
    void UpdateQuestion()
    {
        ui.GetComponent<TextMeshProUGUI>().text = quizQuestions[questionIndex].questionText;
        hasCompletedChallenge = false;
        //Debug.Log("Question: " + quizQuestions[questionIndex].questionText);
    }

    // Method to evaluate the answer to a question
    void EvaluateAnswer(bool answeredYes)
    {
        quizQuestions[questionIndex].answeredYes = answeredYes;
        Debug.Log(quizQuestions[questionIndex].questionText + " was answered " + (answeredYes ? "Yes" : "No"));
        oldIndex = questionIndex;
        questionIndex++;
        hasCompletedChallenge = false;
        EnableAnswerObj();
    }

    // Method called when the answer object is let go
    public void LetGo()
    {

        // Get all child colliders of the answer object
        Collider[] childColliders = quizQuestions[questionIndex].answerObj.GetComponentsInChildren<Collider>(true);

        // Loop through all child colliders
        foreach (Collider childCollider in childColliders)
        {
            // If the child collider is active and tagged as 'Answer'
            if (childCollider.gameObject.activeSelf && childCollider.CompareTag("Answer"))
            {
                // If the child collider intersects with the 'Yes' zone, evaluate the answer as 'Yes'
                if (yesZone.GetComponent<Collider>().bounds.Intersects(childCollider.bounds))
                {
                    Debug.Log("Answered Yes");
                    answeredYes = true;
                    ResetObjtoOriginalPos(childCollider.gameObject);
                    EvaluateAnswer(answeredYes);
                }
                // If the child collider intersects with the 'No' zone, evaluate the answer as 'No'
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

        Debug.LogWarning("No active child object with collider found on answer object.");
    }

    // Method to reset the position of the answer object to its original position
    private void ResetObjtoOriginalPos(GameObject answerChildObj)
    {
        answerChildObj.transform.position = originalPos;
        answerChildObj.transform.rotation = Quaternion.identity;
        Debug.Log("Resetting object to original position");
    }

    // Method to enable the answer object for the current question
    private void EnableAnswerObj()
    {
        
        if(quizQuestions[questionIndex].answerObj.activeSelf == false)
        {
            quizQuestions[questionIndex].answerObj.SetActive(true);
        }
        if(quizQuestions[oldIndex].answerObj.activeSelf == true)
        {
            quizQuestions[oldIndex].answerObj.SetActive(false);
        }
    }
}