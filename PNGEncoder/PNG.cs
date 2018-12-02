using System;
using System.Collections.Generic;
using System.Text;

namespace PNGEncoder
{
    class PNG
    {
        public const byte PNG_COLOR_TYPE_RGB = 2;

        public const byte PNG_COMPRESSION_METHOD_BASE = 0;
        
        public const byte PNG_INTERLACE_METHOD_BASE = 0;
        public const byte PNG_INTERLACE_NONE = 0;
        public const byte PNG_INTERLACE_ADAM7 = 1;
        public const byte PNG_INTERLACE_LAST = 2;

        
        public const byte PNG_FILTER_METHOD_BASE = 0;

        public const byte FILTER_VALUE_NONE = 0;
        public const byte FILTER_VALUE_SUB = 1;
        public const byte FILTER_VALUE_UP = 2;
        public const byte FILTER_VALUE_AVG = 3;
        public const byte FILTER_VALUE_PAETH = 4;
        public const byte FILTER_VALUE_LAST = 5;
    }
}
