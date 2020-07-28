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

    private Thread thread;
    private Queue outputQueue;  // From Unity to Arduino
    private Queue inputQueue;   // From Arduino to Unity

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

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

    public void SendToArduino(string command)
    {
        outputQueue.Enqueue(command);
    }

    public string ReadFromArduino()
    {
        if (inputQueue.Count == 0)
            return null;

        return (string)inputQueue.Dequeue();
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
}
