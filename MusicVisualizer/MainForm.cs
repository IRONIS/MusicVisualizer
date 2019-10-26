using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO.Ports;
using NAudio.Wave;
using System.Collections.Generic;

namespace MusicVisualizer
{
    public partial class MainForm : Form
    {
        private SerialPort arduinoPort;
        private const int baudRate = 9600;
        private const int sampleRate = 100;
        private const string handShake = "ArduinoHandShake";

        private WasapiLoopbackCapture captureInstance = null;

        private class Item
        {
            public string Name;
            public string Value;

            public Item(string name, string value)
            {
                Name = name; Value = value;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private class WaveSample {
            private float Left;
            private float Right;

            public WaveSample(float left, float right) {
                Left = left;
                Right = right;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            
            waveChart.Series.Add("LeftChannel");
            waveChart.Series["LeftChannel"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            waveChart.Series["LeftChannel"].ChartArea = "LeftChannelArea";

            waveChart.Series.Add("RightChannel");
            waveChart.Series["RightChannel"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            waveChart.Series["RightChannel"].ChartArea = "RightChannelArea";

            waveChart.ChartAreas[0].AxisY.Maximum = 1;
            waveChart.ChartAreas[0].AxisY.Minimum = -1;

            waveChart.ChartAreas[0].AxisX.Minimum = 0;
            waveChart.ChartAreas[0].AxisX.Maximum = 100;

            waveChart.ChartAreas[1].AxisY.Maximum = 1;
            waveChart.ChartAreas[1].AxisY.Minimum = -1;

            waveChart.ChartAreas[1].AxisX.Minimum = 0;
            waveChart.ChartAreas[1].AxisX.Maximum = 100;

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cbSerialPort.Items.Add(new Item(port, port));
            }

            // Queue<WaveSample> sampleQueue = new Queue<WaveSample>();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            bool ArduinoPortFound = false;

            try
            {
                string port = cbSerialPort.SelectedItem.ToString();

                Debug.WriteLine("Try connect to port: " + port);

                arduinoPort = new SerialPort(port, baudRate);
                if (ArduinoDetected())
                {
                    ArduinoPortFound = true;
                }
                else
                {
                    ArduinoPortFound = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (ArduinoPortFound == false) return;

            System.Threading.Thread.Sleep(500);

            arduinoPort.BaudRate = baudRate;
            arduinoPort.DtrEnable = true;
            arduinoPort.ReadTimeout = 1000;

            try
            {
                arduinoPort.Open();
                
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private bool ArduinoDetected()
        {
            try
            {
                Debug.WriteLine("OpenPort");
                arduinoPort.Open();

                System.Threading.Thread.Sleep(1000);

                Debug.WriteLine("HandShake");
                string returnMessage = arduinoPort.ReadLine();

                Debug.WriteLine("ClosePort");
                arduinoPort.Close();

                if (returnMessage.Contains("ArduinoHandShake"))
                {
                    Debug.WriteLine("Arduino is detected");                   return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                arduinoPort.Close();

                btnDisconnect.Enabled = false;
                btnConnect.Enabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void btnStartGrab_Click(object sender, EventArgs e)
        {
            // Redefine the capturer instance with a new instance of the LoopbackCapture class
            captureInstance = new WasapiLoopbackCapture();

            // When the capturer receives audio, start writing the buffer into the mentioned file
            captureInstance.DataAvailable += (s, a) =>
            {
                BeginInvoke((Action)(() => {
                    float maxLeft = 0;
                    float maxRight = 0;

                    float currMaxLeft = 0;
                    float currMaxRight = 0;

                    int j = 0;
                    for (int i = 0; i < a.BytesRecorded / 4; i+=8)
                    {
                        float leftSample = BitConverter.ToSingle(a.Buffer, i);
                        float rightSample = BitConverter.ToSingle(a.Buffer, i + 4);

                        if (j > sampleRate) {
                            j = 0;

                            waveChart.Series["LeftChannel"].Points.Add(maxLeft);
                            if (waveChart.Series["LeftChannel"].Points.Count > 100)
                                waveChart.Series["LeftChannel"].Points.RemoveAt(0);

                            waveChart.Series["RightChannel"].Points.Add(maxRight);
                            if (waveChart.Series["RightChannel"].Points.Count > 100)
                                waveChart.Series["RightChannel"].Points.RemoveAt(0);

                            // Rewrite on BackgroundWorker
                            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.1
                            if (arduinoPort != null && arduinoPort.IsOpen)
                            {
                                try
                                {
                                    currMaxLeft = maxLeft * 1023;
                                    currMaxRight = maxRight * 1023;

                                    arduinoPort.Write("<"+ currMaxLeft.ToString("F0")+","+ currMaxRight.ToString("F0") + ">");

                                    Debug.WriteLine("<" + currMaxLeft.ToString("F0") + "," + currMaxRight.ToString("F0") + ">");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("Error while write to arduino: " + ex.Message);
                                }
                            }
                        }
                        else
                        {
                            maxLeft = Math.Max(maxLeft, leftSample);
                            maxRight = Math.Max(maxRight, rightSample);
                            j++;
                        }
                    }
                }));
            };

            // When the Capturer Stops
            captureInstance.RecordingStopped += (s, a) =>
            {
                captureInstance.Dispose();
            };
            
            // Start recording !
            captureInstance.StartRecording();

            // Enable "Stop button" and disable "Start Button"
            btnStartGrab.Enabled = false;
            btnStopGrab.Enabled = true;
        }

        private void btnStopGrab_Click(object sender, EventArgs e)
        {
            // Stop recording !
            captureInstance.StopRecording();

            // Enable "Start button" and disable "Stop Button"
            btnStartGrab.Enabled = true;
            btnStopGrab.Enabled = false;
        }
    }
}
