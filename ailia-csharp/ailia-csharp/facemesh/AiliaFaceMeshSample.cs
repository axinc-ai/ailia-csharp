/* AILIA C# FaceMesh Sample */
/* Copyright 2023 AXELL CORPORATION */

using System.Collections.Generic;
using System;
using System.Drawing;

using ailiaSDK;
using UnityEngine;
using ailia;

namespace ailia_csharp
{
	public class AiliaFaceMeshSample {
		private AiliaModel ailia_face_detector = new AiliaModel();
		private AiliaModel ailia_face_recognizer = new AiliaModel();

		private AiliaBlazeface blaze_face = new AiliaBlazeface();
		private AiliaFaceMesh face_mesh = new AiliaFaceMesh();

		public bool Infer(Color32[] camera, Bitmap bmp, int tex_width, int tex_height, int channels, string asset_path)
		{
			bool gpu_mode = false;
			if (gpu_mode)
			{
				ailia_face_detector.Environment(Ailia.AILIA_ENVIRONMENT_TYPE_GPU);
				ailia_face_recognizer.Environment(Ailia.AILIA_ENVIRONMENT_TYPE_GPU);
			}
			bool status = ailia_face_detector.OpenFile(null, asset_path + "/assets/blazeface.opt.onnx");
			if (!status)
			{
				Console.WriteLine("Model open error blazeface");
				return false;
			}
			status = ailia_face_recognizer.OpenFile(null, asset_path + "/assets/facemesh_constantpad2d.opt.onnx");
			if (!status)
			{
				Console.WriteLine("Model open error facemesh");
				return false;
			}

			//BlazeFace
			long start_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			List<AiliaBlazeface.FaceInfo> result_detections = blaze_face.Detection(ailia_face_detector, camera, tex_width, tex_height);
			long end_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			long detection_time = (end_time - start_time);

			//Compute facemesh
			Graphics g = Graphics.FromImage(bmp);
			long recognition_time = 0;
			//Compute
			long rec_start_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
			bool debug = false;
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
				DrawAffine2D(g, fx, fy, fw, fh, tex_width, tex_height, face.theta);

				float scale = 1.0f * fw / AiliaFaceMesh.DETECTION_WIDTH;

				float ss=(float)System.Math.Sin(face.theta);
				float cs=(float)System.Math.Cos(face.theta);

				for (int k = 0; k < AiliaFaceMesh.NUM_KEYPOINTS; k++)
				{
					int x = (int)(face.center.x * tex_width  + ((face.keypoints[k].x - AiliaFaceMesh.DETECTION_WIDTH/2) * cs + (face.keypoints[k].y - AiliaFaceMesh.DETECTION_HEIGHT/2) * -ss)* scale);
					int y = (int)(face.center.y * tex_height + ((face.keypoints[k].x - AiliaFaceMesh.DETECTION_WIDTH/2) * ss + (face.keypoints[k].y - AiliaFaceMesh.DETECTION_HEIGHT/2) *  cs)* scale);
					DrawRect2D(g, x, y, 1, 1, tex_width, tex_height);
				}
			}

			Console.WriteLine("" + detection_time + "ms + " + recognition_time + "ms\n" + ailia_face_detector.EnvironmentName());

			ailia_face_detector.Close();
			ailia_face_recognizer.Close();

			return true;
		}

		public void DrawRect2D(Graphics g, int x, int y, int w, int h, int tex_width, int tex_height)
		{
			Rectangle rect = new Rectangle(x, y, w, h);
			Pen whitePen = new Pen(Brushes.White);
			whitePen.Width = 2.0F;
			g.DrawRectangle(whitePen, rect);
		}

		public void DrawAffine2D(Graphics g, int x, int y, int w, int h, int tex_width, int tex_height, float theta)
		{
			float cs = (float)System.Math.Cos(-theta);
			float ss = (float)System.Math.Sin(-theta);

			PointF p1 = new PointF((int)(x + w/2 + w/2 * cs + h/2 * ss), (int)(y + h/2 + w/2 * -ss + h/2 * cs));
			PointF p2 = new PointF((int)(x + w/2 - w/2 * cs + h/2 * ss), (int)(y + h/2 - w/2 * -ss + h/2 * cs));
			PointF p3 = new PointF((int)(x + w/2 - w/2 * cs - h/2 * ss), (int)(y + h/2 - w/2 * -ss - h/2 * cs));
			PointF p4 = new PointF((int)(x + w/2 + w/2 * cs - h/2 * ss), (int)(y + h/2 + w/2 * -ss - h/2 * cs));

			Pen whitePen = new Pen(Brushes.White);
			whitePen.Width = 2.0F;

			g.DrawLine(whitePen, p1, p2);
			g.DrawLine(whitePen, p2, p3);
			g.DrawLine(whitePen, p3, p4);
			g.DrawLine(whitePen, p4, p1);
		}
	}
}
