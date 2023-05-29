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