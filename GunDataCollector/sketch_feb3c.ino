#include "I2Cdev.h"
#include "MPU6050.h"
#include "Wire.h"
#include <SPI.h>
#include <HTTPClient.h>
#include <WiFi.h>
#include <WiFiAP.h>
#include <WiFiClient.h>
#include <WiFiGeneric.h>
#include <WiFiMulti.h>
#include <WiFiSTA.h>
#include <WiFiScan.h>
#include <WiFiServer.h>
#include <WiFiType.h>
#include <WiFiUdp.h>
#include <time.h>

MPU6050 accelgyro;

int16_t ax, ay, az;
int16_t gx, gy, gz;
int16_t ix, iy, iz;

int BAM_total = 0;

String server = "http://165.227.237.10";
HTTPClient http;


void setup() {
    Wire.begin();
    Serial.begin(38400);

    WiFi.mode(WIFI_STA);
    WiFi.begin("Aryans hotspot", "password");

    while (WiFi.status() != WL_CONNECTED) {
      Serial.print(".");
      delay(100);
    }
    

  Serial.print("CONNECTED!");

    Serial.println("Initializing I2C devices...");
    accelgyro.initialize();

    Serial.println("Testing device connections...");
    Serial.println(accelgyro.testConnection() ? "MPU6050 connection successful" : "MPU6050 connection failed");

    pinMode(18, INPUT_PULLUP); 

    configTime(0, 0, "pool.ntp.org");
}

void loop() {
    accelgyro.getMotion9(&ax, &ay, &az, &gx, &gy, &gz, &ix, &iy, &iz);
    int button_pressed = digitalRead(18);

    float POW = (ax - 1300) * 90.0/17000;
    float PEW = (az + 200)  * 90.0/16000;

    BAM_total += gy;
    float BAM = BAM_total * 90.0/200000;

    char postData[50];
    sprintf(postData, "{\"POW\": %2.2f, \"PEW\": %2.2f, \"BAM\": %2.2f}", POW, PEW, BAM);

    String path = server + "/upload_gyroscope/";
    http.begin(path);

    http.addHeader("Content-Type", "application/json");
    int respCode = http.POST(postData);
    http.end();

    if (button_pressed == 0 ) {
      time_t current = time(nullptr);
      String t(ctime(&current));
      t = t.substring(0, t.length()-2);
      String postData2 = "{\"time_fired\": \"" + t + "\"}";

      Serial.println(postData2);

      path = server + "/upload_time_fired/";
      http.begin(path);

      http.addHeader("Content-Type", "application/json");
      int respCode = http.POST(postData2);
      Serial.println(http.getString());
      http.end();
    }

    Serial.print("a/g:\t");
    Serial.print(POW); Serial.print("\t");
    Serial.print(BAM); Serial.print("\t");
    Serial.print(PEW); Serial.print("\t");
    Serial.print(respCode); Serial.print("\t");
    Serial.println(button_pressed == 1 ? "Not pressed": "Pressed");
}
