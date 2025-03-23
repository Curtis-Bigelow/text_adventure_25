using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public Text storyText; // the story 
    public InputField userInput; // the input field object
    public Text inputText; // part of the input field where user enters response
    public Text placeHolderText; // part of the input field for initial placeholder text
    //public Button abutton;
    //first step to creating and using delegates
    public delegate void Restart(); //Create delegate
    public event Restart onRestart;
    
    private string story; // holds the story to display
    private List<string> commands = new List<string>(); //valid user commands

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        commands.Add("go");
        commands.Add("get");
        commands.Add("restart"); //Added with delegate
        commands.Add("save");
        commands.Add("inventory"); //added inventory for viewing inventiry
        commands.Add("commands");// shows commands

        userInput.onEndEdit.AddListener(GetInput);
        //abutton.onClick.AddListener(DoSomething);
        story = storyText.text;
        NavigationManager.instance.onGameOver += EndGame; //function to call when event occurs
    }

    void EndGame() //function to respond to event
    {
        UpdateStory("\nPlease enter 'restart' to play again. ");
    }

    //void DoSomething() //event handler
    //{
        //Debug.Log("Button clicked!");
    //}

    public void UpdateStory(string msg)
    {
        story += "\n" + msg;
        storyText.text = story;
    }

    void GetInput(string msg)
    {
        if (msg != "")
        {
            char[] splitInfo = { ' ' };
            string[] parts = msg.ToLower().Split(splitInfo); //['go', 'north']

            if (commands.Contains(parts[0])) //if valid command
            {
                if (parts[0] == "go") //wants to switch rooms
                {
                    if (NavigationManager.instance.SwitchRooms(parts[1])) //returns true or false
                    {
                        //fill in later
                    }
                    else
                    {
                        //added the "is locked" response
                        UpdateStory("Exit does not exist or is locked. Try again.");
                    }
                }
                else if (parts[0] == "get") //wants to add item to inventory
                {
                    if (NavigationManager.instance.TakeItem(parts[1])) //returns true or false
                    {
                        GameManager.instance.inventory.Add(parts[1]);
                        UpdateStory("You added a(n) " + parts[1] + " to your inventory");
                    }
                    else
                    {
                        UpdateStory("Sorry, " + parts[1] + "does not exist in this room");
                    }
                }
                else if (parts[0] == "restart")
                {
                    if(onRestart != null)// if anyone is listening
                    {
                        onRestart();
                    }
                }
                else if (parts[0] == "save")
                {
                    GameManager.instance.Save();
                }
                else if (parts[0] == "inventory")
                {
                    if(GameManager.instance.inventory.Count == 0) //checks how many items are in inventory
                    {
                        UpdateStory("You have no items in your inventory");
                    }
                    else if (GameManager.instance.inventory.Count == 1)
                    {
                        UpdateStory("You have " + GameManager.instance.inventory[0] + " in your inventory");
                    }
                    else if (GameManager.instance.inventory.Count == 2)
                    {
                        UpdateStory("You have " + GameManager.instance.inventory[0] + " " + GameManager.instance.inventory[1] + " in your inventory");
                    }
                }
                else if (parts[0] == "commands")
                {
                    foreach(string s in commands) //gets each command and prints them on screen
                    {
                        UpdateStory(s);
                    }
                }
            }

        }

        // reset for next input
        userInput.text = ""; //after input from user
        userInput.ActivateInputField();
    }

}
