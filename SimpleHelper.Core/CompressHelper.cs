﻿using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;

namespace SimpleHelper
{
    public static class CompressHelper
    {
        /// <summary>
        /// Gs the zip.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GZip(string value)
        {
            //Transform string into byte[]  
            var byteArray = new byte[value.Length];
            int indexBa = 0;
            foreach (char item in value)
            {
                byteArray[indexBa++] = (byte)item;
            }

            //Prepare for compress
            using (var ms = new MemoryStream())
            {
                using (var sw = new GZipStream(ms,
                    CompressionMode.Compress))
                {

                    //Compress
                    sw.Write(byteArray, 0, byteArray.Length);
                }

                //Transform byte[] zip data to string
                byteArray = ms.ToArray();
                var sB = new StringBuilder(byteArray.Length);
                foreach (byte item in byteArray)
                {
                    sB.Append((char)item);
                }
                return sB.ToString();
            }
        }

        /// <summary>
        /// Uns the G zip.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string UnGZip(string value)
        {
            //Transform string into byte[]
            var byteArray = new byte[value.Length];
            int indexBa = 0;
            foreach (char item in value)
            {
                byteArray[indexBa++] = (byte)item;
            }

            //Prepare for decompress
            using (var ms = new MemoryStream(byteArray))
            {
                using (var sr = new GZipStream(ms,
                    CompressionMode.Decompress))
                {

                    //Reset variable to collect uncompressed result
                    byteArray = new byte[byteArray.Length];

                    //Decompress
                    int rByte = sr.Read(byteArray, 0, byteArray.Length);

                    //Transform byte[] unzip data to string
                    var sB = new StringBuilder(rByte);
                    //Read the number of bytes GZipStream red and do not a for each bytes in
                    //resultByteArray;
                    for (int i = 0; i < rByte; i++)
                    {
                        sB.Append((char)byteArray[i]);
                    }
                    return sB.ToString();
                }
            }
        }

        /// <summary>
        /// Compresses the memory byte array.
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <returns></returns>
        public static byte[] CompressMemoryByteArray(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }

                return memory.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the memory byte array.
        /// </summary>
        /// <param name="gzip">The gzip.</param>
        /// <returns></returns>
        public static byte[] DecompressMemoryByteArray(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// Compresses to.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public static void CompressTo(this Stream input, Stream output)
        {
            using (GZipStream gz = new GZipStream(output, CompressionMode.Compress, true))
            {
                input.TransfertTo(gz);
                gz.Flush();
            }
        }

        /// <summary>
        /// Decompresses to.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public static void DecompressTo(this Stream input, Stream output)
        {
            using (GZipStream gz = new GZipStream(input, CompressionMode.Decompress, true))
            {
                gz.TransfertTo(output);
            }
        }
        
        /// <summary>
        /// Serializes as XML and compress.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static byte[] SerializeAsXmlAndCompress(object obj)
        {
            using (MemoryStream compressed = new MemoryStream())
            {
                using (GZipStream gzCompress = new GZipStream(compressed, CompressionMode.Compress, true))
                {
                    XmlSerializer xz = new XmlSerializer(obj.GetType());
                    xz.Serialize(gzCompress, obj);
                }
                return compressed.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the and deserialize as XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static T DecompressAndDeserializeAsXml<T>(byte[] data)
        {
            using (MemoryStream compressed = new MemoryStream(data))
            {
                using (GZipStream gzDecompress = new GZipStream(compressed, CompressionMode.Decompress, true))
                {
                    XmlSerializer xz = new XmlSerializer(typeof(T));
                    object obj = xz.Deserialize(gzDecompress);
                    return (T)obj;
                }
            }
        }

        /// <summary>
        /// Transfert a stream to another
        /// </summary>
        /// <param name="source">Source stream</param>
        /// <param name="destination">Destination stream</param>
        static void TransfertTo(this Stream source, Stream destination)
        {
            TransfertTo(source, destination, 4096);
        }

        /// <summary>
        /// Transfert a stream to another
        /// </summary>
        /// <param name="source">Source stream</param>
        /// <param name="destination">Destination stream</param>
        /// <param name="bufferSize">Buffer size</param>
        static void TransfertTo(this Stream source, Stream destination, int bufferSize)
        {
            int read;
            byte[] buffer = new byte[bufferSize];
            while ((read = source.Read(buffer, 0, bufferSize)) > 0)
            {
                destination.Write(buffer, 0, read);
            }
        }
    }
}
