using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Onek.data
{
    public class Signature
    {
        public int IdEvaluation { get; set; }
        public String Name { get; set; } = "";
        public byte[] BitmapByte { get; set; }

        public Signature()
        {

        }

        public Signature(int idEval, String name, byte[] bitmapByte)
        {
            IdEvaluation = idEval;
            Name = name;
            BitmapByte = bitmapByte;
        }
       
    }
}
