/**
* \~japanese
* @file
* @brief AILIA  Unity Compatble Class
* @author AXELL Corporation
* @date  May 29, 2023
* 
* \~english
* @file
* @brief AILIA Unity Compatble Class
* @author AXELL Corporation
* @date  May 29, 2023
*/

using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    public class Debug
    {
        public static void Log(string text)
        {
            Console.WriteLine(text);
        }

        public static void LogError(string text)
        {
            Console.WriteLine(text);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Color32
    {
        public byte a;
        public byte r;
        public byte g;
        public byte b;
    }

    public class Vector2
    {
        public float x;
        public float y;

        public Vector2(float nx, float ny)
        {
            this.x = nx;
            this.y = ny;
        }
    }

    public class Mathf
    {
        public static float Clamp(float v, float min, float max)
        {
            if (v < min)
            {
                v = min;
            }
            if (v > max)
            {
                v = max;
            }
            return v;
        }

        public static float Exp(float v)
        {
            return (float)Math.Exp((double)v);
        }
    }
 }
