using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta;
using Oculus.Interaction;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public bool answeredYes;
}



public class QuizSystem : MonoBehaviour
{
    public int sceneNumber;
    public bool skipEvaluation;

    public Text questionText;

    [SerializeField] GameObject ui;
    [SerializeField] GameObject yesObj;
    [SerializeField] GameObject noObj;
    [SerializeField] GameObject answerObj;
    
    public QuizQuestion[] quizQuestions;

    // Start is called before the first frame update
    void Start()
    {
        updateQuestion();
    }

    void Update()
    {
        updateQuestion();
    }

    void updateQuestion()
    {
        //questionText.text = quizQuestions[sceneNumber].questionText;
        //change the text of the ui object text component to the current question
        ui.GetComponent<TextMeshPro>().text = quizQuestions[sceneNumber].questionText;

        
    }

    void EvaluateAnswer()
    {
        if(skipEvaluation)
        {
            // Get the index of the current scene
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Calculate the index of the next scene in the build order
            int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

            // Load the next scene
            SceneManager.LoadScene(nextSceneIndex);
            return;
        }
        for(int i = 0; i < quizQuestions.Length; i++)
        {
            if(quizQuestions[sceneNumber].answeredYes)
            {
                Debug.Log(quizQuestions[i].questionText + " was answered Yes");
            }
            else
            {
                Debug.Log(quizQuestions[i].questionText + " was answered No");
            }
        }
        
    }

    public void Grabbed()
    {
        Debug.Log("Grabbed");  
    }
    public void LetGo()
    {
        if(answerObj.GetComponent<Collider>().bounds.Intersects(yesObj.GetComponent<Collider>().bounds))
        {
            Debug.Log("Answered Yes");
            quizQuestions[0].answeredYes = true;            
        }
        else if(answerObj.GetComponent<Collider>().bounds.Intersects(noObj.GetComponent<Collider>().bounds))
        {
            Debug.Log("Answered No");
            quizQuestions[0].answeredYes = false;
        }
        EvaluateAnswer();
    }
}
