using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.ApplicationModel.Core;

namespace BinaryFiles.Helpers
{
    public static class SaveUtils
    {
        //static public void SaveAndPrint<T>(BinaryWriter writer, T value)
        //{
        //    writer.Write((dynamic)value);
        //    Debug.WriteLine($"Wrote to binary: {value}");
        //}

        /* ----------------------------------------------------------------
         *  We nearly got away with being able to use a templated
         *  SaveAndPrint<>() function, using (dynamic) to force it into
         *  the right Write() overload. But the int/long crossover caught
         *  us out, turning a zero from an 8-byte long into a 4-byte int,
         *  breaking things ... "binarily".
         *
         *  C# probably has a way to explicitly prevent narrowing
         *  conversions, but I am moving on.
         * ---------------------------------------------------------------*/
        static public void SaveAndPrintInt(BinaryWriter writer, int value)
        {
            writer.Write(value);
            Debug.WriteLine($"Wrote to binary: {value}");
        }

        static public void SaveAndPrintLong(BinaryWriter writer, long value)
        {
            writer.Write(value);
            Debug.WriteLine($"Wrote to binary: {value}");
        }

        static public void SaveAndPrintBool(BinaryWriter writer, bool value)
        {
            writer.Write(value);
            Debug.WriteLine($"Wrote to binary: {value}");
        }

        static public void SaveAndPrintString(BinaryWriter writer, string value)
        {
            writer.Write(value);
            Debug.WriteLine($"Wrote to binary: {value}");
        }


        /* ----------------------------------------------------------------
         *  The logic of binary reading means we were never going to avoid
         *  all these overloads.
         * ---------------------------------------------------------------*/ 
        static public int LoadAndPrintInt(BinaryReader reader)
        {
            var value = reader.ReadInt32();
            Debug.WriteLine($"Read int from binary: {value}");
            return value;
        }

        static public long LoadAndPrintLong(BinaryReader reader)
        {
            var value = reader.ReadInt64();
            Debug.WriteLine($"Read long from binary: {value}");
            return value;
        }

        static public bool LoadAndPrintBool(BinaryReader reader)
        {
            var value = reader.ReadBoolean();
            Debug.WriteLine($"Read bool from binary: {value}");
            return value;
        }

        static public string LoadAndPrintString(BinaryReader reader)
        {
            var value = reader.ReadString();
            Debug.WriteLine($"Read string from binary: {value}");
            return value;
        }
    }
}
