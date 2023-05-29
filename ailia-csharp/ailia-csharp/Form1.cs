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
            AiliaModel net = new AiliaModel();
            int count = net.GetEnvironmentCount();
            Console.WriteLine("Environment count " + count);
            string asset_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            Console.WriteLine(asset_path);
            
            AiliaDetectorModel ailia_detector = new AiliaDetectorModel();
            uint category_n = 80;
            //ailia_detector.Environment(Ailia.AILIA_ENVIRONMENT_TYPE_GPU);
            ailia_detector.Settings(AiliaFormat.AILIA_NETWORK_IMAGE_FORMAT_BGR, AiliaFormat.AILIA_NETWORK_IMAGE_CHANNEL_FIRST, AiliaFormat.AILIA_NETWORK_IMAGE_RANGE_UNSIGNED_INT8, AiliaDetector.AILIA_DETECTOR_ALGORITHM_YOLOX, category_n, AiliaDetector.AILIA_DETECTOR_FLAG_NORMAL);
            ailia_detector.OpenFile(null, asset_path + "/assets/yolox_tiny.opt.onnx");

            string fileName = asset_path + "/assets/input.jpg";
            Bitmap bmp;

            using (var fs = new System.IO.FileStream(
            fileName,
            System.IO.FileMode.Open,
            System.IO.FileAccess.Read))
            {
                bmp = new Bitmap(fs);
            }

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            Console.WriteLine("Input Image : " + bmpData.Width+"x"+bmpData.Height+" stride "+bmpData.Stride);

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            pictureBox1.Image = bmp;

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

            //Detection
            float threshold = 0.2f;
            float iou = 0.25f;
            long start_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            List<AiliaDetector.AILIADetectorObject> list = ailia_detector.ComputeFromImage(camera, bmp.Width, bmp.Height, threshold, iou);
            long end_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            //Error
            if (list == null)
            {
                Console.WriteLine("Inference error");
                return;
            }

            //Display detected object
            foreach (AiliaDetector.AILIADetectorObject obj in list)
            {
                Console.WriteLine("Detected Object " + obj.prob);
                DisplayDetectedObject(obj, rgbValues, bmp.Width, bmp.Height, channels);
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);


            ailia_detector.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void DisplayDetectedObject(AiliaDetector.AILIADetectorObject box,byte [] camera,int tex_width,int tex_height, int tex_channels){
            //Convert to pixel domain
            int x1=(int)(box.x*tex_width);
            int y1=(int)(box.y*tex_height);
            int x2=(int)((box.x+box.w)*tex_width);
            int y2=(int)((box.y+box.h)*tex_height);

            int w=(x2-x1);
            int h=(y2-y1);

            if(w<=0 || h<=0){
                return;
            }

            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    if (y >= 0 && y < tex_height && x >= 0 && x < tex_width)
                    {
                        camera[y * tex_channels * tex_width + x*tex_channels + 2] = 255;
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
