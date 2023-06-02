/* AILIA C# FaceMesh Sample */
/* Copyright 2023 AXELL CORPORATION */

using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ailia_csharp
{
	public class AiliaFaceMeshSample {
		private AiliaModel ailia_face_detector = new AiliaModel();
		private AiliaModel ailia_face_recognizer = new AiliaModel();

		private AiliaBlazeface blaze_face = new AiliaBlazeface();
		private AiliaFaceMesh face_mesh = new AiliaFaceMesh();

		private void Infer(Color32[] camera, Bitmap bmp, int tex_width, int tex_height, int channels, string asset_path)
		{
			string asset_path = Application.temporaryCachePath;
			var urlList = new List<ModelDownloadURL>();
			bool gpu_mode = false;
			if (gpu_mode)
			{
				ailia_face_detector.Environment(Ailia.AILIA_ENVIRONMENT_TYPE_GPU);
				ailia_face_recognizer.Environment(Ailia.AILIA_ENVIRONMENT_TYPE_GPU);
			}
			FileOpened = ailia_face_detector.OpenFile(null, asset_path + "/blazeface.opt.onnx");
			FileOpened = ailia_face_recognizer.OpenFile(null, asset_path + "/facemesh_constantpad2d.opt.onnx");

			//BlazeFace
			long start_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			List<AiliaBlazeface.FaceInfo> result_detections = blaze_face.Detection(ailia_face_detector, camera, tex_width, tex_height);
			long end_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			long detection_time = (end_time - start_time);

			//Compute facemesh
			Graphics g = Graphics.FromImage(bmp);
			long recognition_time = 0;
			if(ailiaModelType==FaceDetectorModels.facemesh){
				//Compute
				long rec_start_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
				List<AiliaFaceMesh.FaceMeshInfo> result_facemesh = face_mesh.Detection(ailia_face_recognizer, camera, tex_width, tex_height, result_detections, debug);
				long rec_end_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
				recognition_time = (rec_end_time - rec_start_time);

				//Draw result
				for (int i = 0; i < result_facemesh.Count; i++)
				{
					AiliaFaceMesh.FaceMeshInfo face = result_facemesh[i];

					int fw = (int)(face.width * tex_width);
					int fh = (int)(face.height * tex_height);
					int fx = (int)(face.center.x * tex_width) - fw / 2;
					int fy = (int)(face.center.y * tex_height) - fh / 2;
					DrawAffine2D(g, Color.green, fx, fy, fw, fh, tex_width, tex_height, face.theta);

					float scale = 1.0f * fw / AiliaFaceMesh.DETECTION_WIDTH;

					float ss=(float)System.Math.Sin(face.theta);
					float cs=(float)System.Math.Cos(face.theta);

					for (int k = 0; k < AiliaFaceMesh.NUM_KEYPOINTS; k++)
					{
						int x = (int)(face.center.x * tex_width  + ((face.keypoints[k].x - AiliaFaceMesh.DETECTION_WIDTH/2) * cs + (face.keypoints[k].y - AiliaFaceMesh.DETECTION_HEIGHT/2) * -ss)* scale);
						int y = (int)(face.center.y * tex_height + ((face.keypoints[k].x - AiliaFaceMesh.DETECTION_WIDTH/2) * ss + (face.keypoints[k].y - AiliaFaceMesh.DETECTION_HEIGHT/2) *  cs)* scale);
						DrawRect2D(g, Color.green, x, y, 1, 1, tex_width, tex_height);
					}
				}
			}

			Console.WriteLine("" + detection_time + "ms + " + recognition_time + "ms\n" + ailia_face_detector.EnvironmentName());

			ailia_face_detector.Close();
			ailia_face_recognizer.Close();
		}

		public void DrawRect2D(Graphics g, Color32 color, int x, int y, int w, int h, int tex_width, int tex_height)
		{
			RectangleF rect = new RectangleF(x, y, x + w, y + h);
			g.DrawRectangle(Brushes.White, rect);
		}

		public void DrawAffine2D(Graphics g, Color32 color, int x, int y, int w, int h, int tex_width, int tex_height, float theta)
		{
			float cs = (float)System.Math.Cos(-theta);
			float ss = (float)System.Math.Sin(-theta);

			PointF p1 = new PointF((int)(x + w/2 + w/2 * cs + h/2 * ss), (int)(y + h/2 + w/2 * -ss + h/2 * cs));
			PointF p2 = new PointF((int)(x + w/2 - w/2 * cs + h/2 * ss), (int)(y + h/2 - w/2 * -ss + h/2 * cs));
			PointF p3 = new PointF((int)(x + w/2 - w/2 * cs - h/2 * ss), (int)(y + h/2 - w/2 * -ss - h/2 * cs));
			PointF p4 = new PointF((int)(x + w/2 + w/2 * cs - h/2 * ss), (int)(y + h/2 + w/2 * -ss - h/2 * cs));

			g.DrawLine(Brushes.White, p1, p2);
			g.DrawLine(Brushes.White, p2, p3);
			g.DrawLine(Brushes.White, p3, p4);
			g.DrawLine(Brushes.White, p4, p1);
		}
	}
}
