/* AILIA Unity Plugin Super Resolution Sample */
/* Copyright 2023 AXELL CORPORATION */

using System;
using System.Collections.Generic;
using UnityEngine;

public class AiliaRealEsrganSample
{
    public bool Infer(Color32[] camera, ref Color32[] outputImage, ref int output_width, ref int output_height, int width, int height, int channels, string asset_path, bool is_anime)
    {
        AiliaModel ailiaModel = new AiliaModel();
        bool gpu_mode = false;
        if (gpu_mode)
        {
            ailiaModel.Environment(Ailia.AILIA_ENVIRONMENT_TYPE_GPU);
        }

        uint memory_mode = Ailia.AILIA_MEMORY_REDUCE_CONSTANT | Ailia.AILIA_MEMORY_REDUCE_CONSTANT_WITH_INPUT_INITIALIZER | Ailia.AILIA_MEMORY_REUSE_INTERSTAGE;
        ailiaModel.SetMemoryMode(memory_mode);

        string onnx_name = "RealESRGAN.opt.onnx";
        if (is_anime)
        {
            onnx_name = "RealESRGAN_anime.opt.onnx";
        }

        //Console.Write(asset_path + "/" + onnx_name);
        bool modelPrepared = ailiaModel.OpenFile(null, asset_path + "/assets/" + onnx_name);
        if (!modelPrepared)
        {
            Console.WriteLine("Model open failed.");
            return false;    
        }
        
        // Set input image shape
        Ailia.AILIAShape shape = new Ailia.AILIAShape();
        shape.x = (uint)width;
        shape.y = (uint)height;
        shape.z = 3;
        shape.w = 1;
        shape.dim = 4;
        ailiaModel.SetInputShape(shape);

        // Get output image shape
        shape = ailiaModel.GetOutputShape();
        int OutputWidth = (int)shape.x;
        int OutputHeight = (int)shape.y;
        int OutputChannel = (int)shape.z;

        // Make input data
        float[] input = new float[width * height * 3];
        long start_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        InputDataPocessingCPU(camera, input);
        long end_time = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        Console.WriteLine("InputDataPocessingCPU Time : " + (end_time - start_time) + " ms");

        // Predict
        float[] output = new float[OutputWidth * OutputHeight * OutputChannel];
        long start_time2 = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        bool result = ailiaModel.Predict(output, input);
        long end_time2 = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        Console.WriteLine("Infer Time : " + (end_time2 - start_time2) + " ms");

        // convert result to image
        outputImage = new Color32[OutputWidth * OutputHeight];
        long start_time3 = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        OutputDataProcessingCPU(output, outputImage);
        long end_time3 = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        Console.WriteLine("OutputDataProcessingCPU Time : " + (end_time3 - start_time3) + " ms");

        ailiaModel.Close();

        output_width = OutputWidth;
        output_height = OutputHeight;

        return true;
    }

    void InputDataPocessingCPU(Color32[] inputImage, float[] processedInputBuffer)
    {
        float weight = 1f / 255f;
        float bias = 0;

        // RealESGGAN : Channel First, RGB, /255f

        for (int i = 0; i < inputImage.Length; i++)
        {
            processedInputBuffer[i + inputImage.Length * 0] = (inputImage[i].r) * weight + bias;
            processedInputBuffer[i + inputImage.Length * 1] = (inputImage[i].g) * weight + bias;
            processedInputBuffer[i + inputImage.Length * 2] = (inputImage[i].b) * weight + bias;
        }
    }

    void OutputDataProcessingCPU(float[] outputData, Color32[] pixelBuffer)
    {
        for (int i = 0; i < pixelBuffer.Length; i++)
        {
            pixelBuffer[i].r = (byte)Mathf.Clamp(outputData[i + 0 * pixelBuffer.Length] * 255, 0, 255);
            pixelBuffer[i].g = (byte)Mathf.Clamp(outputData[i + 1 * pixelBuffer.Length] * 255, 0, 255);
            pixelBuffer[i].b = (byte)Mathf.Clamp(outputData[i + 2 * pixelBuffer.Length] * 255, 0, 255);
            pixelBuffer[i].a = 255;
        }
    }
}
