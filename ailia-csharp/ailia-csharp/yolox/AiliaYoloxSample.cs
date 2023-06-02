/* AILIA C# YOLOX Sample */
/* Copyright 2023 AXELL CORPORATION */

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
	public partial class AiliaYoloxSample
	{
		private void Infer(Color32[] camera, Bitmap bmp, int width, int height, int channels, string asset_path)
		{
			// Initialize ailia instance
			AiliaDetectorModel ailia_detector = new AiliaDetectorModel();
			bool gpu_mode = false;
			if (gpu_mode){
				ailia_detector.Environment(Ailia.AILIA_ENVIRONMENT_TYPE_GPU);
			}

			// Open yolox model            
			uint category_n = 80;
			ailia_detector.Settings(AiliaFormat.AILIA_NETWORK_IMAGE_FORMAT_BGR, AiliaFormat.AILIA_NETWORK_IMAGE_CHANNEL_FIRST, AiliaFormat.AILIA_NETWORK_IMAGE_RANGE_UNSIGNED_INT8, AiliaDetector.AILIA_DETECTOR_ALGORITHM_YOLOX, category_n, AiliaDetector.AILIA_DETECTOR_FLAG_NORMAL);
			ailia_detector.OpenFile(null, asset_path + "/assets/yolox_tiny.opt.onnx");

			// Inference
			float threshold = 0.2f;
			float iou = 0.25f;
			long start_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			List<AiliaDetector.AILIADetectorObject> list = ailia_detector.ComputeFromImage(camera, width, height, threshold, iou);
			long end_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			Console.WriteLine("Inference Time : " + (end_time - start_time) + " ms");

			//Error
			if (list == null)
			{
				Console.WriteLine("Inference error");
				return;
			}

			// Display detected object
			Graphics g = Graphics.FromImage(bmp);
			foreach (AiliaDetector.AILIADetectorObject obj in list)
			{
				Console.WriteLine("Detected Object category " +obj.category + " prob "+ obj.prob);
				DisplayDetectedObject(obj, g, width, height, channels);
			}

			// Release instance
			ailia_detector.Close();
		}

		private void DisplayDetectedObject(AiliaDetector.AILIADetectorObject box, Graphics g,int tex_width,int tex_height, int tex_channels){
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

			Font fnt = new Font("MS UI Gothic", 20);
			g.DrawString(""+box.category fnt, Brushes.Blue, x1, t1);
			RectangleF rect = new RectangleF(x1, y1, x2, y2);
			g.DrawRectangle(Brushes.White, rect);

			/*
			for (int y = y1; y < y2; y++)
			{
				for (int x = x1; x < x2; x++)
				{
					if (y >= 0 && y < tex_height && x >= 0 && x < tex_width)
					{
						camera[y * tex_channels * tex_width + x*tex_channels + 2] = 255; // fill red
					}
				}
			}
			*/
		}
	}
}
