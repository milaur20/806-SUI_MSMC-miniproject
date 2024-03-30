using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    // Slider
    public Slider slider; 
    public TextMeshProUGUI sliderText;

    // Environment button
    public Button environmentButton;

    public Sprite cat;
    public Sprite happy;

    public Sprite tommy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sliderText.text = "Spider Design Level: " + slider.value.ToString();
    }

    public void PanicButtonQuit()
    {
        Application.Quit();
        Debug.Log("Panic Mode: Activated");
    }

    public void EnvironmentButtonPressed(){
    
        // Check which sprite is currently assigned to the environmentButton
        if (environmentButton.image.sprite == cat)
        {
            // Load CatScene if the button is displaying the cat sprite
            //SceneManager.LoadScene("CatScene");
            environmentButton.image.sprite = happy;
            Debug.Log("CAT");
        }

        else if (environmentButton.image.sprite == happy)
        {
            // Load HappyScene if the button is displaying the happy sprite
            //SceneManager.LoadScene("HappyScene");
            environmentButton.image.sprite = tommy;
            Debug.Log("HAPPY");
        }

        else if (environmentButton.image.sprite == tommy)
        {
            // Load TommyScene if the button is displaying the tommy sprite
            //SceneManager.LoadScene("TommyScene");
            environmentButton.image.sprite = cat;
            Debug.Log("TOMMY");
        }

        else
        {
            // If none of the above cases match, you can just load a default scene
            //SceneManager.LoadScene("DefaultScene");
            environmentButton.image.sprite = cat;
            Debug.Log("DEFAULT");
        }
    }
}
