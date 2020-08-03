using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.IO.Ports;


public class ArduinoThreadedRead : MonoBehaviour
{
    SerialPort stream;
    public string port = "COM4";
    public int baudRate = 9600;
    public int timeout = 5000;
    public bool hasError = false;
    public float framesPerPing = 1;


    private Thread thread;
    private Queue outputQueue;  // From Unity to Arduino
    private Queue inputQueue;   // From Arduino to Unity

    private string rXMsg;
    int i = 0, j = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartThread();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % framesPerPing == 0 && !hasError)
        {
            ////Send control character to request data
            SendToArduino("ECHO "+i);
            Debug.Log("ping sent "+i);
            i++;
        }
        //check for receiving data
        rXMsg = ReadFromArduino();
        if(rXMsg != null)
        {
            Debug.Log(rXMsg);
            j++;
        }
    }

    public void StartThread()
    {
        outputQueue = Queue.Synchronized(new Queue());
        inputQueue = Queue.Synchronized(new Queue());
        //create and start the thread
        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    public void ThreadLoop()
    {
        // Opens the connection on the serial port
        stream = new SerialPort(port, baudRate);
    
        stream.ReadTimeout = 50;
        stream.Open();

        // Looping
        while (true)
        {
            // Send to Arduino
            if (outputQueue.Count != 0)
            {
                string command = outputQueue.Dequeue().ToString();
                WriteToArduino(command);
            }

            // Read from Arduino
            string result = ReadFromArduino(timeout);
            if (result != null)
                inputQueue.Enqueue(result);
        }
    }

    //adds a message to the output queue to be sent to the Arduino
    public void SendToArduino(string command)
    {
        outputQueue.Enqueue(command);
    }

    //extracts a received message from the input queue
    public string ReadFromArduino()
    {
        if (inputQueue.Count == 0)
            return null;

        return (string)inputQueue.Dequeue();
    }
    
    //writes and transmits a message to the Arduino via serial
    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    //reads a message from Arduino via serial
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
}
