using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MathNet.Filtering;
using System;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Filtering;
using MathNet.Filtering.FIR;
using MathNet.Filtering.IIR;
using MathNet.Filtering.Median;

public class Filtering : MonoBehaviour
{

    public double fs = 968;
    public double fc = 40; //cutoff frequency lpf

    //bandpass filter
    public double fc1 = 0; //low cutoff frequency
    public double fc2 = 10; //high cutoff frequency
    public float LowPassFilterFactor = 0.2f;
    public float IgnoreReadingFactor = 0.4f;

    public double[] yf1_lpf;
    public double[] yf2_bp ;
    public double[] medi;
    //public double[] yf3_bpn;

    double[] y;

    public double[] filteredData;
    private float lowPassValue = 0;
    // Start is called before the first frame update
    void Start()
    {
        //signal + noise
        fs = 968; //sampling rate

        //double fw = 5; //signal frequency
        //double fn = 50; //noise frequency
        //double n = 5; //number of periods to show
        //double A = 10; //signal amplitude
        //double N = 1; //noise amplitude
        //int size = (int)(n * fs / fw); //sample size

        //var t = Enumerable.Range(1, size).Select(p => p * 1 / fs).ToArray();
        //var y = t.Select(p => (A * Math.Sin(2 * Math.PI * fw * p)) + (N * Math.Sin(2 * Math.PI * fn * p))).ToArray(); //Original

        y = new double[]{ 91, 92, 92, 93, 92, 92, 90, 89, 89, 91, 92, 92, 93, 97, 97, 97, 91, 85, 80, 78, 75, 76, 75, 80, 86, 94,
            97, 98, 99, 99, 97, 103, 106, 106, 105, 96, 87, 84, 82, 83, 84, 89, 94, 94, 92, 93, 96, 94, 91, 91, 89, 87, 88,
            91, 93, 90, 93, 94, 98, 97, 95, 94, 93, 91, 89, 87, 85, 87, 89, 90, 92, 87, 145, 47, 77, 144, 72, 134, 196, 72,
            70, 195, 78, 49, 78, 173, 129, 141, 96, 97, 98, 191, 118, 66, 121, 178, 198, 112, 110, 111, 111, 110, 108, 88,
            86, 85, 83, 84, 82, 83, 84, 87, 91, 89, 90, 89, 88, 88, 88, 85, 84, 80, 84, 73, 52, 96, 96, 100, 100, 101, 109,
            113, 109, 112, 110, 111, 109, 104, 94, 96, 94, 87, 85, 84, 87, 86, 86, 86, 87, 92, 88, 89, 87, 89, 87, 86, 84, 82,
            85, 87, 94, 95, 
            //92, 92, 93, 92, 92, 90, 89, 89, 91, 92, 92, 93, 97, 97, 97, 91, 85, 80, 78, 75, 76, 75, 80, 86, 94,
            //97, 98, 99, 99, 97, 103, 106, 106, 105, 96, 87, 84, 82, 83, 84, 89, 94, 94, 92, 93, 96, 94, 91, 91, 89, 87, 88,
            //91, 93, 90, 93, 94, 98, 97, 95, 94, 93, 91, 89, 87, 85, 87, 89, 90, 92, 87, 145, 47, 77, 144, 72, 134, 196, 72,
            //70, 195, 78, 49, 78, 173, 129, 141, 96, 97, 98, 191, 118, 66, 121, 178, 198, 112, 110, 111, 111, 110, 108, 88,
            //86, 85, 83, 84, 82, 83, 84, 87, 91, 89, 90, 89, 88, 88, 88, 85, 84, 80, 84, 73, 52, 96, 96, 100, 100, 101, 109,
            //113, 109, 112, 110, 111, 109, 104, 94, 96, 94, 87, 85, 84, 87, 86, 86, 86, 87, 92, 88, 89, 87, 89, 87, 86, 84, 82,
            //85, 87, 94, 95, 
            //92, 92, 93, 92, 92, 90, 89, 89, 91, 92, 92, 93, 97, 97, 97, 91, 85, 80, 78, 75, 76, 75, 80, 86, 94,
            //97, 98, 99, 99, 97, 103, 106, 106, 105, 96, 87, 84, 82, 83, 84, 89, 94, 94, 92, 93, 96, 94, 91, 91, 89, 87, 88,
            //91, 93, 90, 93, 94, 98, 97, 95, 94, 93, 91, 89, 87, 85, 87, 89, 90, 92, 87, 145, 47, 77, 144, 72, 134, 196, 72,
            //70, 195, 78, 49, 78, 173, 129, 141, 96, 97, 98, 191, 118, 66, 121, 178, 198, 112, 110, 111, 111, 110, 108, 88,
            //86, 85, 83, 84, 82, 83, 84, 87, 91, 89, 90, 89, 88, 88, 88, 85, 84, 80, 84, 73, 52, 96, 96, 100, 100, 101, 109,
            //113, 109, 112, 110, 111, 109, 104, 94, 96, 94, 87, 85, 84, 87, 86, 86, 86, 87, 92, 88, 89, 87, 89, 87, 86, 84, 82,
            //85, 87, 94, 95, 
            //91, 92, 92, 93, 92, 92, 90, 89, 89, 91, 92, 92, 93, 97, 97, 97, 91, 85, 80, 78, 75, 76, 75, 80, 86, 94,
            //97, 98, 99, 99, 97, 103, 106, 106, 105, 96, 87, 84, 82, 83, 84, 89, 94, 94, 92, 93, 96, 94, 91, 91, 89, 87, 88,
            //91, 93, 90, 93, 94, 98, 97, 95, 94, 93, 91, 89, 87, 85, 87, 89, 90, 92, 87, 145, 47, 77, 144, 72, 134, 196, 72,
            //70, 195, 78, 49, 78, 173, 129, 141, 96, 97, 98, 191, 118, 66, 121, 178, 198, 112, 110, 111, 111, 110, 108, 88,
            //86, 85, 83, 84, 82, 83, 84, 87, 91, 89, 90, 89, 88, 88, 88, 85, 84, 80, 84, 73, 52, 96, 96, 100, 100, 101, 109,
            //113, 109, 112, 110, 111, 109, 104, 94, 96, 94, 87, 85, 84, 87, 86, 86, 86, 87, 92, 88, 89, 87, 89, 87, 86, 84, 82,
            //85, 87, 94, 95, 
            //92, 92, 93, 92, 92, 90, 89, 89, 91, 92, 92, 93, 97, 97, 97, 91, 85, 80, 78, 75, 76, 75, 80, 86, 94,
            //97, 98, 99, 99, 97, 103, 106, 106, 105, 96, 87, 84, 82, 83, 84, 89, 94, 94, 92, 93, 96, 94, 91, 91, 89, 87, 88,
            //91, 93, 90, 93, 94, 98, 97, 95, 94, 93, 91, 89, 87, 85, 87, 89, 90, 92, 87, 145, 47, 77, 144, 72, 134, 196, 72,
            //70, 195, 78, 49, 78, 173, 129, 141, 96, 97, 98, 191, 118, 66, 121, 178, 198, 112, 110, 111, 111, 110, 108, 88,
            //86, 85, 83, 84, 82, 83, 84, 87, 91, 89, 90, 89, 88, 88, 88, 85, 84, 80, 84, 73, 52, 96, 96, 100, 100, 101, 109,
            //113, 109, 112, 110, 111, 109, 104, 94, 96, 94, 87, 85, 84, 87, 86, 86, 86, 87, 92, 88, 89, 87, 89, 87, 86, 84, 82,
            //85, 87, 94, 95, 
            //92, 92, 93, 92, 92, 90, 89, 89, 91, 92, 92, 93, 97, 97, 97, 91, 85, 80, 78, 75, 76, 75, 80, 86, 94,
            //97, 98, 99, 99, 97, 103, 106, 106, 105, 96, 87, 84, 82, 83, 84, 89, 94, 94, 92, 93, 96, 94, 91, 91, 89, 87, 88,
            //91, 93, 90, 93, 94, 98, 97, 95, 94, 93, 91, 89, 87, 85, 87, 89, 90, 92, 87, 145, 47, 77, 144, 72, 134, 196, 72,
            //70, 195, 78, 49, 78, 173, 129, 141, 96, 97, 98, 191, 118, 66, 121, 178, 198, 112, 110, 111, 111, 110, 108, 88,
            //86, 85, 83, 84, 82, 83, 84, 87, 91, 89, 90, 89, 88, 88, 88, 85, 84, 80, 84, 73, 52, 96, 96, 100, 100, 101, 109,
            //113, 109, 112, 110, 111, 109, 104, 94, 96, 94, 87, 85, 84, 87, 86, 86, 86, 87, 92, 88, 89, 87, 89, 87, 86, 84, 82,
            //85, 87, 94, 95 
        };

        filteredData = new double[y.Length];

    }

    // Update is called once per frame
    void Update()
    {

        var coeffs = IirCoefficients.LowPass(fs, fc, 10);
        var lpf = new OnlineIirFilter(coeffs);
        yf1_lpf = lpf.ProcessSamples(y);

        //lowpass filter
        var lowpass = OnlineIirFilter.CreateLowpass(ImpulseResponse.Infinite, fs, fc);
        var lowassMed = OnlineMedianFilter.CreateLowpass(ImpulseResponse.Infinite, fs, fc);

        //bandpass filter
        var bandpass = OnlineIirFilter.CreateBandpass(ImpulseResponse.Infinite, fs, fc1, fc2);

        ////narrow bandpass filter
        //fc1 = 3; //low cutoff frequency
        //fc2 = 7; //high cutoff frequency
        //var bandpassnarrow = OnlineFirFilter.CreateBandpass(ImpulseResponse.Finite, fs, fc1, fc2);

        //yf1_lpf = lowpass.ProcessSamples(y); //Lowpass
        yf2_bp = bandpass.ProcessSamples(y); //Bandpass
        medi = lowassMed.ProcessSamples(y);
        //yf3_bpn = bandpassnarrow.ProcessSamples(y); //Bandpass Narrow

        if (Input.GetKeyDown(KeyCode.Space))
        {
            filterData();

        }
    }

    public void filterData()
    {
        lowPassValue = (float)y[0];
        filteredData[0] = lowPassValue;
        for(int i = 1; i<y.Length; i++)
        {
            filteredData[i] = lerpFilter((float)y[i]);
        }
        //CSVManager.AppendFilteredData(filteredData);
    }

    float lerpFilter(float newReading)
    {
        //if the new reading is unreasonably high ignore the new reading
        if (newReading > (lowPassValue * (1 + IgnoreReadingFactor))) 
        {
            Debug.Log("current bpm is: " + lowPassValue + " ignored value: " + newReading);
            return lowPassValue; 
        }
        //if the new reading is unreasonably low ignore the new reading
        else if (newReading < (lowPassValue * (1 - IgnoreReadingFactor))) 
        { 
            Debug.Log("current bpm is: " + lowPassValue + " ignored value: " + newReading);
            return lowPassValue; 
        }

        //lerp between last and new value according to some defined factor
        //hold new lerped value
        lowPassValue = Mathf.Lerp(lowPassValue, newReading, LowPassFilterFactor);
        //return the new value
        return lowPassValue;

    }
}
