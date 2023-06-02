/* AILIA C# YOLOX Sample */
/* Copyright 2023 AXELL CORPORATION */

using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace ailia_csharp
{
	public partial class AiliaYoloxSample
	{
		public static string[] COCO_CATEGORY = {
		"person", "bicycle", "car", "motorcycle", "airplane", "bus", "train",
		"truck", "boat", "traffic light", "fire hydrant", "stop sign",
		"parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow",
		"elephant", "bear", "zebra", "giraffe", "backpack", "umbrella",
		"handbag", "tie", "suitcase", "frisbee", "skis", "snowboard",
		"sports ball", "kite", "baseball bat", "baseball glove", "skateboard",
		"surfboard", "tennis racket", "bottle", "wine glass", "cup", "fork",
		"knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange",
		"broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair",
		"couch", "potted plant", "bed", "dining table", "toilet", "tv",
		"laptop", "mouse", "remote", "keyboard", "cell phone", "microwave",
		"oven", "toaster", "sink", "refrigerator", "book", "clock", "vase",
		"scissors", "teddy bear", "hair drier", "toothbrush"
		};

		public bool Infer(Color32[] camera, Bitmap bmp, int width, int height, int channels, string asset_path)
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
			bool status = ailia_detector.OpenFile(null, asset_path + "/assets/yolox_tiny.opt.onnx");
			if (!status)
            {
				Console.WriteLine("Model open error");
				return false;
            }

			// Inference
			float threshold = 0.2f;
			float iou = 0.25f;
			long start_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			List<AiliaDetector.AILIADetectorObject> list = ailia_detector.ComputeFromImageB2T(camera, width, height, threshold, iou);
			long end_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			Console.WriteLine("Inference Time : " + (end_time - start_time) + " ms");

			//Error
			if (list == null)
			{
				Console.WriteLine("Inference error");
				return false;
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

			return true;
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
			g.DrawString(""+ COCO_CATEGORY[box.category] + " " + box.prob, fnt, Brushes.White, x1, y1);
			Rectangle rect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
			Pen whitePen = new Pen(Brushes.White);
			whitePen.Width = 2.0F;
			g.DrawRectangle(whitePen, rect);
		}
	}
}
