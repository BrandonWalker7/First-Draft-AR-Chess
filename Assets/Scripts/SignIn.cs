using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignIn : MonoBehaviour

{
    public GameObject email;
    public GameObject password;
    public GameObject warningStrings;
    Firebase.Auth.FirebaseUser user;
    private string Email;
    private string Password;
    private string WarningString;
    public UnityEngine.UI.Button yourButton;
    Firebase.Auth.FirebaseAuth auth;

    // Start is called before the first frame update
    void Start()
    {
        
        UnityEngine.UI.Button btn = yourButton.GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(SignInAuth);
    }


    private void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                InitializeFirebase();
                print(auth.CurrentUser);

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.LogError("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.LogError("Signed in " + user.UserId);
                //displayName = user.DisplayName ?? "";
               // emailAddress = user.Email ?? "";
                
            }
        }
    }

    public void SignInAuth()
    {
        if (Password != "" && Email != "")
        {
            auth.SignInWithEmailAndPasswordAsync(Email, Password).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
           
        }

        else
        {
            print("Fields Must not remain blank");
        }
    }

   


    // Update is called once per frame
    void Update()
    {
        bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (email.GetComponent<InputField>().isFocused)
            {
                password.GetComponent<InputField>().Select();
            }
        }

        //UserName = userName.GetComponent<InputField>().text;
        Email = email.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;

        if (signedIn)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth.SignOut();
        
        
    }
}
