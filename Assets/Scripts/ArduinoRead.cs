using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

//Code mostly from https://www.alanzucconi.com/2015/10/07/how-to-integrate-arduino-with-unity/#more-2979
//This method of communicating with the Arduino uses coroutines to read from the Arduino 
//  after sending a "PING" message to the Arduino over serial port and then prints "PONG" in the Unity debug log (which is the string sent by Arduino)
//  Unity coroutines are not really executed in parallel and is not thread safe.
//TODO change the method to use real threads
public class ArduinoRead : MonoBehaviour
{
    SerialPort stream;
    public bool hasError = false;
    public float framesPerPing = 1;

    // Start is called before the first frame update
    void Start()
    {
        stream = new SerialPort("COM4", 9600);
        stream.ReadTimeout = 5000;
        //stream.
        stream.Open();
    }

    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout = 0)
    {
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException e)
        {
            return null;
        }
    }

    public void RequestArduino()
    {
        try
        {
            stream.WriteLine("PING");
            stream.BaseStream.Flush();
        }
        catch (Exception)
        {
            hasError = true;
        }
    }

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield break; // Terminates the Coroutine
            }
            else
                yield return null; // Wait for next frame

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % framesPerPing == 0 && !hasError)
        {
            //Send control character to request data
            Debug.Log("Request Sent");
            RequestArduino();
            //wait for receiving data
            StartCoroutine
            (
                //TODO change Debug.Log(s) into someFunc(s)
                AsynchronousReadFromArduino
                ((string s) => Debug.Log(s),     // Callback
                    () => Debug.LogError("Error!"), // Error callback
                    10000f                          // Timeout (milliseconds)
                )
            );
        }
    }

}
