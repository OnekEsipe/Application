using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Onek.data
{
    public class Signature
    {
        public String Name { get; set; } = "";
        public byte[] Bitmap { get; set; }

        public Signature()
        {

        }

        public Signature(String name, byte[] bitmapByte)
        {
            Name = name;
            Bitmap = bitmapByte;
        }
       
    }
}
