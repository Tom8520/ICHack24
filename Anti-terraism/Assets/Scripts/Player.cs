using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    // Constants
     private float IN_GAME_SCREEN_WIDTH = 14;
     private float IN_GAME_SCREEN_HEIGHT = 7;
     private float BOTTOM_LEFT_IN_GAME_SCREEN_X = -7;
     private float BOTTOM_LEFT_IN_GAME_SCREEN_Y = 2;
     private float IN_GAME_SCREEN_Z = 11;

     private int TOTAL_TARGET_NUMBER = 3;

     // Helper classes
     [System.Serializable]
     public class GunStat {
        public bool fired;
        public float projectedX;
        public float projectedY;

        public bool getFired() {
            return fired;
        }

        public float getProjectedX() {
            return projectedX;
        }

        public float getProjectedY () {
            return projectedY;
        }
     }
     
    // Start of class
    GunStat gunStat;
    int shotsFired;
    int numberHit;

    private void Start()
    {
        gunStat = new GunStat();
        shotsFired = 0;
        Application.targetFrameRate = 7;
    }

    // Get position data
    public void GetStatData() {
        StartCoroutine(FetchStatData());
    }

    public IEnumerator FetchStatData() {
        string URL = "http://165.227.237.10/check_fired/";
        using (UnityWebRequest request = UnityWebRequest.Get(URL)) {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError) {
                Debug.Log(request.error);
            } else {
                gunStat = JsonUtility.FromJson<GunStat>(request.downloadHandler.text);
            }
        }
    }

    // Get position data
    public void PostDistanceData() {
        StartCoroutine(SendDistanceData());
    }

    public IEnumerator SendDistanceData() {
        string URL = "http://165.227.237.10/check_fired/";
        using (UnityWebRequest request = UnityWebRequest.Get(URL)) {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError) {
                Debug.Log(request.error);
            } else {
                gunStat = JsonUtility.FromJson<GunStat>(request.downloadHandler.text);
            }
        }
    }

    void Update()
    {
        GetStatData();

        // projectedX and projectedY are fraction of the screen being returned.
        // Turn into in-game coordinates.
        float gunPointX = gunStat.getProjectedX() * IN_GAME_SCREEN_WIDTH + BOTTOM_LEFT_IN_GAME_SCREEN_X;
        float gunPointY = gunStat.getProjectedY() * IN_GAME_SCREEN_HEIGHT + BOTTOM_LEFT_IN_GAME_SCREEN_Y;
        Vector3 gunPoint = new Vector3(gunPointX, gunPointY, IN_GAME_SCREEN_Z);

        GameObject.Find("RedDot").transform.position = gunPoint;

        if (gunStat.getFired())
        {
            shotsFired++;
            Debug.Log("SHOTS FIRED");

            // Get target position and calculate distance
            Target target = GameObject.Find("Target").GetComponent<Target>();
            double distance = Vector3.Distance(gunPoint, target.transform.position);

            if (distance <= 2) {
                numberHit++;
                if (numberHit >= TOTAL_TARGET_NUMBER) {
                    SceneManager.UnloadSceneAsync("AimLab");
                    SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
                    GameObject.Find("AllObjects").GetComponent<Text>().text = shotsFired.ToString();
                }
                target.Kill();
            } else {
                Debug.Log("Missed");
            }
        }
    }
}
