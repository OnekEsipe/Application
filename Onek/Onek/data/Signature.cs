using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Onek.data
{
    /// <summary>
    /// Data class to store signature information
    /// </summary>
    public class Signature
    {
        //Properties
        public String Name { get; set; } = "";
        public byte[] Bitmap { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Signature()
        {

        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="name">The name of the person who sign</param>
        /// <param name="bitmapByte">A bitmap byte array</param>
        public Signature(String name, byte[] bitmapByte)
        {
            Name = name;
            Bitmap = bitmapByte;
        }
       
    }
}
