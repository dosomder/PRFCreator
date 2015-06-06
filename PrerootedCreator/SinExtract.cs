using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace PRFCreator
{
    static class SinExtract
    {
        public static void ExtractSin(BackgroundWorker sender, string sinfile, string outfile, bool log = true)
        {
            using (FileStream stream = new FileStream(sinfile, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(stream))
            {
                if (log)
                    Logger.WriteLog("Verifying extracted Sin File"); ;
                if (!SinFile.VerifySin(br))
                {
                    sender.CancelAsync();
                    return;
                }

                List<SinFile.BlockInfoHeader> bihs = null;
                int SinVer = SinFile.GetSinVersion(br);
                switch (SinVer)
                {
                    case 2:
                        bihs = SinFileV2.GetBIHs(br);
                        break;
                    case 3:
                        bihs = SinFileV3.GetBIHs(br);
                        break;
                }
                if (log)
                    Logger.WriteLog("Extracting image from Sin File " + Path.GetFileName(sinfile));

                SinExtract.ExtractSinData(sender, br, bihs, outfile, log);
            }
        }

        private static void ExtractSinData(BackgroundWorker sender, BinaryReader br, List<SinFile.BlockInfoHeader> bihs, string destination, bool showProgress = true)
        {
            using (FileStream fsw = new FileStream(destination, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fsw))
            {
                int SinVersion = SinFile.GetSinVersion(br);
                long previousDest = 0, previousLength = 0;
                int dataStart = SinFile.GetDataStart(br);
                br.BaseStream.Position = dataStart;
                for (int i = 0; i < bihs.Count; i++)
                {
                    SinFile.BlockInfoHeader bih = bihs[i];
                    if (previousDest + previousLength < bih.dataDest)
                        FillFF(fsw, (previousDest + previousLength), (bih.dataDest - previousDest - previousLength));

                    if (SinVersion != 2)
                        br.BaseStream.Position = dataStart + bih.dataStart;

                    fsw.Position = bih.dataDest;

                    if (SinFile.isCompressed(bih))
                        DecompressAndCopy(br, bw, bih);
                    else
                        CopyBytes(br, bw, bih.dataLength);

                    previousDest = bih.dataDest;
                    previousLength = SinFile.isCompressed(bih) ? bih.destLength : bih.dataLength;
                    if (showProgress)
                        sender.ReportProgress((int)((float)i / bihs.Count * 100));
                }

                long finallength = SinFile.GetFinalLength(fsw);
                if (finallength > previousDest + previousLength)
                    FillFF(fsw, (previousDest + previousLength), (finallength - previousDest - previousLength));
                else
                    //new sinv3 with compression adds some stuff after ext4 image
                    fsw.SetLength(finallength);

                if (showProgress)
                    sender.ReportProgress(100);
            }
        }

        private static void FillFF(FileStream fs, long start, long length)
        {
            fs.Position = start;
            for (long i = 0; i < length; i++)
            {
                //WriteByte has an internal buffer
                fs.WriteByte(0xFF);
            }
        }

        private static void CopyBytes(BinaryReader _in, BinaryWriter _out, long length)
        {
            //use 4096bytes as buffer
            for (long i = length; i > 0; i -= 4096)
            {
                byte[] readBuf = new byte[i >= 4096 ? 4096 : i];
                _in.Read(readBuf, 0, readBuf.Length);
                _out.Write(readBuf, 0, readBuf.Length);
            }
        }

        private static void DecompressAndCopy(BinaryReader _in, BinaryWriter _out, SinFile.BlockInfoHeader bih)
        {
            if (bih.blockSize != bih.destLength)
                throw new Exception("This decompression was unexpected");

            byte[] compressed = new byte[bih.dataLength];
            _in.Read(compressed, 0, compressed.Length);
            byte[] decompressed = LZ4.LZ4Codec.Decode(compressed, 0, compressed.Length, Convert.ToInt32(bih.blockSize));
            _out.Write(decompressed, 0, decompressed.Length);
        }
    }
}
