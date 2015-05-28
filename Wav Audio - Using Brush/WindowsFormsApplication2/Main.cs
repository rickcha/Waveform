using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Main : Form
    {
        public string fileDirectory;    // File location as a string
        double maxSample = 0;           // Shows the largest sample size
        int sampleLength = 0;           // Shows the number of samples

        /* The field names in Wave file header are as follows */
        public string ChunkID;      // Marks the file as a riff file. Characters are each 1 byte long.
        public int ChunkSize;       // Size of the overall file - 8 bytes
        public string Format;       // File Type Header. It equals "WAVE".
        public string Subchunk1ID;  // Format chunk marker. It equals "fmt".
        public int Subchunk1Size;   // Length of format data as listed above. 16 bits for our case.
        public int AudioFormat;     // Type of format (1 is PCM) - 2 byte integer
        public int NumChannels;     // Number of Channels - 2 byte integer
        public int SampleRate;      // Sample Rate = Number of Samples per second, or Hertz.
        public int ByteRate;        // (Sample Rate * BitsPerSample * Channels) / 8.
        public int BlockAlign;      // (BitsPerSample * Channels) / 8.1 - 8 bit mono2 - 8 bit stereo/16 bit mono4 - 16 bit stereo
        public int BitPerSample;    // Bits per sample
        public int Subchunk2Size;   // "data" chunk header. Marks the beginning of the data section.
        public string Subchunk2ID;  // Size of the data section.

        public Main()
        {
            InitializeComponent();
            // chart1.Width = 125;
            this.Width = 100;   // Adjust form1 window size
            this.Height = 205;
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileDirectory = openFileDialog1.FileName;       // Save file location
                byte[] buff = File.ReadAllBytes(fileDirectory); // Read Wave file into a byte array
                
                // Get SubChunk2Size from the header (from 40 to 43 byte)
                byte a1 = buff[40];
                byte a2 = buff[41];
                byte a3 = buff[42];
                byte a4 = buff[43];

                byte[] SubChunk2Size = { a1, a2, a3, a4 };                  // Four bytes represents data size and it is little endian
                int dataLength = BitConverter.ToInt32(SubChunk2Size, 0);    // Convert an array of 4 bytes to signed integer

                // Since mono(16 bits per sample), two bytes provide one sample
                sampleLength = dataLength / 2;

                int cntSample = 0;              // Count number of samples
                double[] intSampleArr = new double[sampleLength];
                double[] normSampleArr = new double[sampleLength];

                // Extract information from buff[] and plot points to the chart1
                for (int i = 44; i < buff.Length; i = i + 2)
                {
                    // The size of single sample is 16 bits, two bytes. Combined two bytes into one sample integer
                    byte[] byteSample = { buff[i], buff[i + 1] };
                    double intSample = BitConverter.ToInt16(byteSample, 0);
                    intSampleArr[cntSample] = intSample;
                    normSampleArr[cntSample] = intSample / Math.Pow(2, 15);
                    cntSample++;
                }

                /*
                 * Window customization
                 */
                this.Width = 830; // 1649 is recommended
                this.Height = 382;

                PointF[] gridPts = new PointF[6];
                gridPts[0] = new PointF(200, 50);   // Height is 250
                gridPts[1] = new PointF(750, 300);  // Width is 550
                gridPts[2] = new PointF(200, 50);   // X starts at 200
                gridPts[3] = new PointF(200, 300);  // Y starts at 175
                gridPts[4] = new PointF(200, 175);
                gridPts[5] = new PointF(750, 175);
                // this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 1), gridPts[0], gridPts[1]);
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 1), gridPts[2], gridPts[3]);
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 1), gridPts[4], gridPts[5]);

                double interval = (double)550 / sampleLength;
                // double x = 5;
                // float y = (float)x;
                // float normSample;

                // int countWavePts = 0;
                PointF[] wavePts = new PointF[sampleLength];
                float xWave;
                float yWave;

                for (int j = 0; j < sampleLength; j++)
                {
                    xWave = (float)(interval * j + 200);
                    yWave = (float)(normSampleArr[j] * 125 + 175);
                    wavePts[j] = new PointF(xWave, yWave);
                }

                // System.Diagnostics.Debug.WriteLine("int: " + 8/5);
                // System.Diagnostics.Debug.WriteLine("int: {0}", yWave);


                // Plot points from the "wavePts" array
                // And draw by connecting two adjacent points.
                for (int l = 0; l < sampleLength-1; l++)
                {
                        this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 1), wavePts[l], wavePts[l+1]);
                }


                // Set the value of maximum sample size
                for (int k = 0; k < cntSample; k++) 
                {
                    if (maxSample < intSampleArr[k])
                        maxSample = intSampleArr[k];
                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fileProperties_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("int: {0}", (double)5/8);
            Detail properties = new Detail(sampleLength, maxSample);
            properties.Show();
        }
    }
}
