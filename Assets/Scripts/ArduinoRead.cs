using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

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
            //stream.Write("\r");  // Carriage Return
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
                AsynchronousReadFromArduino
                ((string s) => Debug.Log(s),     // Callback
                    () => Debug.LogError("Error!"), // Error callback
                    10000f                          // Timeout (milliseconds)
                )
            );
        }
    }

}
