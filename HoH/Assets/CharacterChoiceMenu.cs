using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterChoiceMenu : MonoBehaviour
{
    public Button[] characterButtons;
    
    private int currentPlayer = 1;

    public Button p1b;

    public TextMeshProUGUI p1characterNameText;

    public TextMeshProUGUI p2characterNameText;

    public Button startButton;

    


    void Start()
    {
        foreach (Button button in characterButtons)
        {
            button.onClick.AddListener(() => OnCharacterButtonClicked(button));
        }
    }

    void OnCharacterButtonClicked(Button button)
    {
        // Get the character index or identifier from the button
        int characterIndex = System.Array.IndexOf(characterButtons, button);

        // Assign the character to the current player
        if (currentPlayer == 1)
        {
            Debug.Log("Player 1 chose character " + characterIndex);
     
            Transform childP1 = button.transform.Find("P1");
            childP1.gameObject.SetActive(true);
            currentPlayer = 2;
            p1b= button;
            p1characterNameText.text = button.name;

            PlayerPrefs.SetString("Player1Choice",button.name);

        }
        else
        {
            Debug.Log("Player 2 chose character " + characterIndex);
            // TODO: Assign character to Player 2
            // TODO: Update UI to indicate Player 2's choice
            Transform childP1 = button.transform.Find("P2");
            childP1.gameObject.SetActive(true);
            button.Select();
            p2characterNameText.text = button.name;

            PlayerPrefs.SetString("Player2Choice", button.name);

            Debug.Log("1");
            foreach (Button butt in characterButtons)
            {
                Debug.Log("2");
                if (butt!=p1b && butt != button)
                {
                    Debug.Log("3");
                    butt.interactable = false;
                }
            }

            startButton.gameObject.SetActive(true);
        }

        

        
    }

    public void ResetCharacterSelection()
    {
        // Reactivate all character buttons
        foreach (Button button in characterButtons)
        {
            button.interactable = true;
            // Deactivate any child objects indicating player choices
            Transform childP1 = button.transform.Find("P1");
            if (childP1 != null)
            {
                childP1.gameObject.SetActive(false);
            }
            Transform childP2 = button.transform.Find("P2");
            if (childP2 != null)
            {
                childP2.gameObject.SetActive(false);
            }
        }
        // Reset current player to 1
        currentPlayer = 1;

        p1characterNameText.text = "";
        p2characterNameText.text = "";

        startButton.gameObject.SetActive(false);
    }
}
