using System;
using System.Collections.Generic;
using System.Text;

namespace PNGEncoder
{
    public class PNGUtil
    {
        // afterword, CREATE utility class and move to it
        public static void GetBigEndian32(byte[] output, uint input)
        {
            output[0] = (byte)((input >> 24) & 0x000000ff);
            output[1] = (byte)((input >> 16) & 0x000000ff);
            output[2] = (byte)((input >> 8) & 0x000000ff);
            output[3] = (byte)(input & 0x000000ff);
        }

        public static readonly byte[] chunkNameIHDR = new byte[] { 73, 72, 68, 82 };
        public static readonly byte[] chunkNameIDAT = new byte[] { 73, 68, 65, 84 };
        public static readonly byte[] chunkNameIEND = new byte[] { 73, 69, 78, 68 };

        public static readonly int[] passStart = new int[7] { 0, 4, 0, 2, 0, 1, 0 };
        public static readonly int[] passIncrement = new int[7] { 8, 8, 4, 4, 2, 2, 1 };
        public static readonly int[] passYStart = new int[7] { 0, 0, 4, 0, 2, 0, 1 };
        public static readonly int[] passYIncrement = new int[7] { 8, 8, 8, 4, 4, 2, 2 };

    }
}
