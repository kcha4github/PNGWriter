using System;
using System.Collections.Generic;
using System.Text;

namespace PNGEncoder
{
    class PNGInfo
    {
        uint width;
        uint height;
        
        byte bitDepth;
        byte colorType;
        byte compressionMethod;
        byte filterMethod;
        byte interlaceMethod;

        // property section
        public uint Width { get { return width; } set { width = value; } }
        public uint Height { get { return height; } set { height = value; } }



    }

}
