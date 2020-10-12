using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    public GameObject email;
    public GameObject password;
    public GameObject confPass;
    

    //private string UserName;
    private string Email;
    private string Password;
    private string ConfPass;
    private string form;
    private bool EmailValid = false;
    Firebase.Auth.FirebaseAuth auth;
    public Button yourButton;
    // Start is called before the first frame update
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(RegisterButton);
    }
    
    public void RegisterButton()
    {
        if (Password != "" && Email != "" && ConfPass != "" && (Password == ConfPass))
        {
           auth.CreateUserWithEmailAndPasswordAsync(Email, Password).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
        }
        else if (Password != ConfPass)
        {
            print("Passwords must match");
        }
        else
        {
            print("Fields Must not remain blank");
        }
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            /*
            if (userName.GetComponent<InputField>().isFocused){
                email.GetComponent<InputField>().Select();
            }
            */
            if (email.GetComponent<InputField>().isFocused)
            {
                password.GetComponent<InputField>().Select();
            }
            if (password.GetComponent<InputField>().isFocused)
            {
                confPass.GetComponent<InputField>().Select();
            }

        }

        //UserName = userName.GetComponent<InputField>().text;
        Email = email.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;
        ConfPass = confPass.GetComponent<InputField>().text;

    }
}
