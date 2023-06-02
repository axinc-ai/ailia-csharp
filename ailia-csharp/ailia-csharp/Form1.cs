using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ailia_csharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Inference();
        }

        private Bitmap LoadBMP(string fileName)
        {
            Bitmap bmp;
            using (var fs = new System.IO.FileStream(
            fileName,
            System.IO.FileMode.Open,
            System.IO.FileAccess.Read))
            {
                bmp = new Bitmap(fs);
            }
            return bmp;
        }

        private System.Drawing.Imaging.BitmapData GetBmpData(Bitmap bmp)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            return bmpData;
        }

        private byte[] GetPixels(System.Drawing.Imaging.BitmapData bmpData, Bitmap bmp)
        {

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            return rgbValues;
        }

        /*
        private void PutPixels(Bitmap bmp, System.Drawing.Imaging.BitmapData bmpData, byte[] rgbValues)
        {
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
        }
        */

        private void FreeBitmapData(Bitmap bmp, System.Drawing.Imaging.BitmapData bmpData) { 
                        // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }

        private Color32[] ConvertBitmapDataToColor32(Bitmap bmp, System.Drawing.Imaging.BitmapData bmpData, byte[] rgbValues)
        {
            Color32[] camera = new Color32[bmp.Width * bmp.Height];
            int channels = bmpData.Stride / bmpData.Width;
            for (int i = 0; i < camera.Length; i++)
            {
                camera[i].b = rgbValues[0 + i * channels];
                camera[i].g = rgbValues[1 + i * channels];
                camera[i].r = rgbValues[2 + i * channels];
                if (channels == 4)
                {
                    camera[i].a = rgbValues[3 + i * channels];
                }
                else
                {
                    camera[i].a = 255;
                }
            }
            return camera;
        }

        private void Inference()
        {
            string asset_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string fileName = asset_path + "/assets/input.jpg";

            // Open test image
            Bitmap bmp = LoadBMP(fileName);
            System.Drawing.Imaging.BitmapData bmpData = GetBmpData(bmp);
            byte[] rgbValues = GetPixels(bmpData, bmp);
            int channels = bmpData.Stride / bmpData.Width;
            Color32[] camera = ConvertBitmapDataToColor32(bmp, bmpData, rgbValues);
            Console.WriteLine("Input Image : " + bmpData.Width + "x" + bmpData.Height + " stride " + bmpData.Stride);
            FreeBitmapData(bmp, bmpData);
            pictureBox1.Image = bmp;

            AiliaYoloxSample yolox = new AiliaYoloxSample();
            yolox.Infer(camera, bmp, width, height, channels);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
