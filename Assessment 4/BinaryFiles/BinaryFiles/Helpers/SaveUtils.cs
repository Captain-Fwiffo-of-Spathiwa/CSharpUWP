using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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
         *  We very nearly got away with being able to template our
         *  SaveAndPrint<>() function, using (dynamic) to force it into
         *  Write(). But the int/long crossover caught us out, with an
         *  8-byte zero value getting saved in 4-bytes, breaking things.
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
