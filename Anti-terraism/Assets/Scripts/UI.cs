using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    Text heartRateText;
    Text accuracyText;
    HeartrateData heartRate;

    private int TOTAL_TARGET_NUMBER = 3;

   [System.Serializable]
    public class HeartrateData {
        public float value = -10;

        public float getValue() {
            return value;
        }

        public void setValue(float value) {
            this.value = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
       heartRate = new HeartrateData();

       heartRateText = GameObject.Find("HeartRateText").GetComponent<Text>();

       accuracyText = GameObject.Find("AccuracyText").GetComponent<Text>();
       int shotsFired = Int32.Parse(GameObject.Find("AllObjects").GetComponent<Text>().text);
       accuracyText.text = (TOTAL_TARGET_NUMBER * 100 / shotsFired).ToString() + "%";

       Application.targetFrameRate = 1;
    }

    public void GetData() {
        StartCoroutine(FetchData());
    }

    public IEnumerator FetchData() {
        string URL = "http://165.227.237.10/get_average_heartrate/";
        using (UnityWebRequest request = UnityWebRequest.Get(URL)) {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError) {
                Debug.Log(request.error);
            } else {
                try {
                    Debug.Log(request.downloadHandler.text);
                    heartRate = JsonUtility.FromJson<HeartrateData>(request.downloadHandler.text);
                } catch (Exception e) {
                    Debug.Log("error in heart rate");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetData();
        if (heartRateText.text == "101.77") {
            String val = heartRate.getValue().ToString();
            heartRateText.text = val;
        }

        if (Input.GetKeyDown("space")) {
            SceneManager.UnloadSceneAsync("SampleScene");
            SceneManager.LoadScene("AimLab", LoadSceneMode.Additive);
        }
    }
}
