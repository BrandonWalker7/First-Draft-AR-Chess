
//namespace is used to know where to find the scripts
namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

     
    // Controls the HelloAR example.
  
    public class ARChess: MonoBehaviour
    {
         
        // The first-person camera being used to render the passthrough camera image (i.e. AR
        // background). 
        public Camera FirstPersonCamera;

        //used to errase renders when a gameobject is detected
        public Material MatTransparent;
        public MeshRenderer pointcloud;

        // A prefab to place when a raycast from a user touch hits a vertical plane.
        public GameObject ChessBoard;

        // Is is to know if the navmesh has been already isntanciated
        public bool spawn = false;
        
                
        // The rotation in degrees need to apply to prefab when it is placed.
        private const float k_PrefabRotation = 180.0f;

         
        // True if the app is in the process of quitting due to an ARCore connection error,
        // otherwise false.      
        private bool m_IsQuitting = false;

         
        // The Unity Awake() method.
      
        public void Awake()
        {
            // Enable ARCore to target 60fps camera capture frame rate on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;
        }

         
        // The Unity Update() method.
      
        public void Update()
        {
            _UpdateApplicationLifecycle();

            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began || spawn==true)
            {
                return;
            }

            // Should not handle input if the player is pointing on UI.
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    // Choose the prefab using the navmeshprefab
                    GameObject prefab= ChessBoard;

                    // Instantiate prefab at the hit pose.
                    //var gameObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);
                    var gameObject = prefab;
                    gameObject.transform.position = hit.Pose.position;
                    gameObject.transform.rotation = hit.Pose.rotation;

                    // Compensate for the hitPose rotation facing away from the raycast (i.e.
                    // camera).
                    gameObject.transform.Rotate(0, k_PrefabRotation, 0, Space.Self);

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of
                    // the physical world evolves.
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    // Make game object a child of the anchor.
                    gameObject.transform.parent = anchor.transform;

                    //set the spawn to true and close the cycle
                    spawn = true;

                    GameObject[] gos=GameObject.FindGameObjectsWithTag("planeVisual");

                    foreach(GameObject go in gos)
                    {
                        go.GetComponent<MeshRenderer>().material = MatTransparent;
                    }

                    pointcloud.enabled = false;

                    gameObject.SetActive(true);
                }
            }
        }

         
        // Check and update the application lifecycle.
      
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to
            // appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

         
        // Actually quit the application.
      
        private void _DoQuit()
        {
            Application.Quit();
        }

         
        // Show an Android toast message.
      
        // <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject =
                        toastClass.CallStatic<AndroidJavaObject>(
                            "makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
