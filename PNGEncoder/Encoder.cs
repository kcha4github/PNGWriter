using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

using zlib;



namespace PNGEncoder
{
    public class Encoder
    {
        int width;
        int height;

        byte bitDepth;
        byte colorType;
        byte compressionMethod;
        byte interlaceMethod;
        byte interlaceType;
        byte filterMethod;

        int pass;
        int passTotal;

        int channels;
        int bytesPerPixel;

        bool interlaced;
        int numRows;
        int userWidth;
        
        MemoryStream dataStream;
        ZStream zStream;
        byte[] zBuf;

        byte[][] image;

        byte[] rowBufferCurrent;
        byte[] rowBufferPrev;
        byte[] rowBufferUp;
        byte[] rowBufferSub;
        
        // yet
        public Encoder(Bitmap bitmap)
        {
            width = bitmap.Width;
            height = bitmap.Height;
            bitDepth = 8;
            colorType = PNG.PNG_COLOR_TYPE_RGB;
            compressionMethod = PNG.PNG_COMPRESSION_METHOD_BASE;

            interlaceMethod = PNG.PNG_INTERLACE_METHOD_BASE;
            interlaced = false;
            passTotal = interlaced ? 7 : 1;
            interlaceType = interlaced ? PNG.PNG_INTERLACE_ADAM7 : PNG.PNG_INTERLACE_NONE;
            
            filterMethod = PNG.PNG_FILTER_METHOD_BASE;

            channels = 3;
            bytesPerPixel = channels * bitDepth;

            image = new byte[height][];
            //image = new byte[height, width * bytesPerPixel];
            for (int y = 0; y < height; y++)
            {
                image[y] = new byte[width * 3];

                for (int x = 0; x < width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    image[y][x * channels + 0] = color.R;
                    image[y][x * channels + 1] = color.G;
                    image[y][x * channels + 2] = color.B;
                }
            }
        }

        // yet
        public void Encode()
        {
            dataStream = new MemoryStream();

            InitZStream();

            WriteSignature();
            WriteChunkIHDR();
            
            // gAMA, sRGB, iCCP, sBIT, cHRM

            // PLTE
            
            // tRNS, bKGD, hIST, oFFs, pCAL, sCAL, pHYs, tIME, sPLT
            // iTXt, zTXt, tEXt

            for (int passIndex = 0; passIndex < passTotal; passIndex++)
            {
                for (int y = 0; y < height; y++)
                {
                    WriteRow(image[y], passIndex);
                }
            }

            WriteFinish();
        }

        private void WriteSignature()
        {
            byte[] signature = new byte[8] { 137, 80, 78, 71, 13, 10, 26, 10 };
            dataStream.Write(signature, 0, signature.Length);
        }

        // yet
        private void WriteChunk(byte[] chunkName, byte[] data)
        {
            byte[] temp4Bytes = new byte[4];

            PNGUtil.GetBigEndian32(temp4Bytes, (uint)data.Length);
            
            dataStream.Write(temp4Bytes, 0, temp4Bytes.Length);
            dataStream.Write(chunkName, 0, chunkName.Length);
            dataStream.Write(data, 0, data.Length);

            uint crc = CRCChecker.CRC(chunkName, data);
            PNGUtil.GetBigEndian32(temp4Bytes, crc);
            dataStream.Write(temp4Bytes, 0, temp4Bytes.Length);
        }

        // yet
        private void WriteChunkIHDR()
        {
            byte[] temp4Bytes = new byte[4];
            byte[] buf = new byte[13];

            switch (colorType)
            {
                case PNG.PNG_COLOR_TYPE_RGB:
                    if (bitDepth != 8 && bitDepth != 16)
                        throw new Exception("Invalid bit depth for RGB image.");
                    break;
                default:
                    throw new Exception("Can't deal with color type without RGB.");
            }

            // actually, need to verify these parameter
            // colorType, bitDepth, compressionMethod, filterMethod, interlaceMethod

            PNGUtil.GetBigEndian32(temp4Bytes, (uint)width);
            Array.Copy(temp4Bytes, 0, buf, 0, temp4Bytes.Length);
            PNGUtil.GetBigEndian32(temp4Bytes, (uint)height);
            Array.Copy(temp4Bytes, 0, buf, 4, temp4Bytes.Length);
            buf[8] = bitDepth;
            buf[9] = colorType;
            buf[10] = compressionMethod;
            buf[11] = filterMethod;
            buf[12] = interlaceMethod;

            WriteChunk(PNGUtil.chunkNameIHDR, buf);
        }

        private void WriteChunkIDAT()
        {
            byte[] buf = new byte[zStream.next_out_index];

            Array.Copy(zBuf, buf, buf.Length);

            WriteChunk(PNGUtil.chunkNameIDAT, buf);

            zBuf.Initialize();
            zStream.avail_out = zBuf.Length;
            zStream.next_out_index = 0;
        }

        private void WriteChunkIEND()
        {
            byte[] buf = new byte[0];

            WriteChunk(PNGUtil.chunkNameIEND, buf);
        }

        // yet
        private void WriteRow(byte[] rowData, int pass )
        {
            rowBufferCurrent = new byte[1 + rowData.Length];

            rowBufferCurrent[0] = PNG.FILTER_VALUE_NONE;
            Array.Copy(rowData, 0, rowBufferCurrent, 1, rowData.Length);

            zStream.next_in = rowBufferCurrent;
            zStream.next_in_index = 0;
            zStream.avail_in = rowBufferCurrent.Length;

            do
            {
                int deflateErr = zStream.deflate(zlib.zlibConst.Z_NO_FLUSH);
                if (deflateErr != zlib.zlibConst.Z_OK && deflateErr != zlib.zlibConst.Z_STREAM_END)
                {
                    throw new Exception("deflate error");
                }

                if (zStream.avail_out == 0)
                {
                    WriteChunkIDAT();
                    zStream.next_out_index = 0;
                    zStream.avail_out = zBuf.Length;
                }

            } while (zStream.avail_in > 0);
        }

        // yet
        private void WriteFinish()
        {
            int deflateErr;
            do
            {
                deflateErr = zStream.deflate(zlib.zlibConst.Z_FINISH);

                if (deflateErr == zlib.zlibConst.Z_OK)
                {
                    if (zStream.avail_out == 0)
                    {
                        WriteChunkIDAT();
                        zStream.next_out_index = 0;
                        zStream.avail_out = zBuf.Length;
                    }
                }
                else if (deflateErr != zlib.zlibConst.Z_STREAM_END)
                {
                    throw new Exception("Write Finish error");
                }
            } while (deflateErr != zlib.zlibConst.Z_STREAM_END);

            WriteChunkIDAT();

            WriteChunkIEND();
        }

        private void InitZStream()
        {
            zStream = new ZStream();
            zStream.deflateInit(zlib.zlibConst.Z_DEFAULT_COMPRESSION);
            zBuf = new byte[8192];
            zStream.next_out = zBuf;
            zStream.avail_out = zBuf.Length;
        }

        public void SavePNG(string path)
        {
            try
            {
                Stream outputStream = File.OpenWrite(path);
                BufferedStream bufferedOutput = new BufferedStream(outputStream);

                byte[] pngByteArray = dataStream.ToArray();

                bufferedOutput.Write(pngByteArray, 0, pngByteArray.Length);
                bufferedOutput.Flush();
                outputStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
