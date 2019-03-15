


    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;

    public class MapSpawn : MonoBehaviour
    {


        public Camera MainCamera;

        public GameObject DetectcedPlanePrefab;

        public GameObject PlanePrefab;

        public GameObject PointPrefab;

        private const float ModelRotation = 180.0f;

        private bool AreWeKillingIt = false;

        private bool spawned = false;

    private ButtonManager buttonManager;

        private void Start()
        {
        buttonManager = GameObject.Find("Butooon").GetComponent<ButtonManager>();
        PlanePrefab = buttonManager.Selection;        
        }
    void Update()
        {
            AppLifecycle();

            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                if ((hit.Trackable is DetectedPlane) && Vector3.Dot(MainCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    GameObject prefab;
                    if (hit.Trackable is FeaturePoint)
                    {
                        prefab = PointPrefab;
                    }
                    else
                    {
                        prefab = PlanePrefab;
                    }
                    if (!spawned)
                    {
                        var Map = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                        Map.transform.Rotate(0, ModelRotation, 0, Space.Self);

                        var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                        Map.transform.parent = anchor.transform;

                        spawned = true;
                    }
                }
            }
        }

        private void AppLifecycle()
        {
            if (Input.GetKey(KeyCode.Escape)) { Application.Quit(); }

            if (Session.Status != SessionStatus.Tracking)
            {
                const int SleepTimeout = 15;
                Screen.sleepTimeout = SleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (AreWeKillingIt) { return; }

            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                AndroidToastMessage("Camera permission is needed to run this application.");
                AreWeKillingIt = true;
                Invoke("KILLITNOW", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                AndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                AreWeKillingIt = true;
                Invoke("KILLITNOW", 0.5f);
            }
        }

        private void KILLITNOW()
        {
            Application.Quit();
        }

        private void AndroidToastMessage(string message)
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject UnityActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (UnityActivity != null)
            {
                AndroidJavaClass ToastClass = new AndroidJavaClass("android.widget.Toast");
                UnityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject ToastObject = ToastClass.CallStatic<AndroidJavaObject>("maketext", UnityActivity, message, 0);
                    ToastObject.Call("show");
                }));
            }
        }
    }

