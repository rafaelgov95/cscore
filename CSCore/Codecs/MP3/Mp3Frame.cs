﻿using System;
using System.IO;

namespace CSCore.Codecs.MP3
{
    public class Mp3Frame
    {
        public const int MaxFrameLength = 0x4000;

        private static readonly int[, ,] BitRates = new int[,,]
        {
            {
                // Version 1
                { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448 },   // Layer 1
                { 0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384 },      // Layer 2
                { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 },       // Layer 3
            },
            {
                // Version 2 & 2.5
                { 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256 },      // Layer 1
                { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 },           // Layer 2
                { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 },           // Layer 3 (same as
                                                                                            // layer 2)
            }
        };

        private static readonly int[,] _samplesPerFrame = new int[,]
        {
            //Version 1
            {
                384,    //Layer 1
                1152,   //Layer 2
                1152    //Layer 3
            },
            //Version 2-2.5
            {
                384,    //Layer 1
                1152,   //Layer 2
                576     //Layer 3
            }
        };

        private static readonly int[,] _sampleRates = new int[,]
        {
            //Version 1
            {
                44100,
                48000,
                32000
            },
            //Version 2
            {
                22050,
                24000,
                16000
            },
            //Version 2.5
            {
                11025,
                12000,
                8000
            }
        };

        private Stream _stream;
        private long _streamPosition, _dataPosition;
        private byte[] _headerBuffer;

        public static Mp3Frame FromStream(Stream stream)
        {
            Mp3Frame frame = new Mp3Frame(stream);
            return frame.FindFrame(stream, true) ? frame : null;
        }

        public static Mp3Frame FromStream(Stream stream, ref byte[] data)
        {
            Mp3Frame frame = new Mp3Frame(stream);
            if (frame.FindFrame(stream, false))
            {
                data = CSCore.Utils.Buffer.BufferUtils.CheckBuffer(data, frame.FrameLength);
                Array.Copy(frame._headerBuffer, 0, data, 0, 4);
                int read = stream.Read(data, 4, frame.FrameLength - 4);
                if (read != frame.FrameLength - 4)
                    return null;

                return frame;
            }

            return null;
        }

        private Mp3Frame(Stream stream)
        {
            _stream = stream;
        }

        private bool FindFrame(Stream stream, bool seek)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (!stream.CanRead) throw new ArgumentException("stream not readable");

            const string loggerLocation = "Mp3Frame.FindFrame(Stream)";
            byte[] buffer = new byte[4];
            int read;

            if ((read = stream.Read(buffer, 0, buffer.Length)) < 4)
            {
                Context.Current.Logger.Fatal("Stream EOF.", loggerLocation);
                return false;
            }

            int totalRead = 0;

            _streamPosition = stream.Position;

            while (!ParseFrame(buffer) || MPEGLayer != MpegLayer.Layer3)
            {
                for (int i = 0; i < 3; i++)
                    buffer[i] = buffer[i + 1];

                if ((read = stream.Read(buffer, 3, 1)) < 1)
                {
                    Context.Current.Logger.Fatal("Stream EOF.", loggerLocation);
                    return false;
                }

                totalRead += read;
                /*if (totalRead > MaxFrameLength)
                {
                    Context.Current.Logger.Error("Could not find a MP3 Frame.", loggerLocation);
                    return false;
                }*/

                _streamPosition = stream.Position;
            }

            _headerBuffer = buffer;
            _dataPosition = stream.Position;

            if (_stream.CanSeek && seek)
                _stream.Position += FrameLength - 4;

            if (FrameLength > MaxFrameLength)
                return false;

            return true;
        }

        public int ReadData(ref byte[] buffer, int offset)
        {
            long currentPosition = _stream.Position;

            buffer = CSCore.Utils.Buffer.BufferUtils.CheckBuffer(buffer, FrameLength + offset);
            _stream.Position = _streamPosition;
            int read = _stream.Read(buffer, offset, FrameLength);

            _stream.Position = currentPosition;

            return read;
        }

        /// <summary>
        /// <remarks>
        /// http://mpgedit.org/mpgedit/mpeg_format/mpeghdr.htm
        /// </remarks>
        /// </summary>
        private bool ParseFrame(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Length < 4) throw new ArgumentException("buffer has to bigger than 3");

            //11111111                    //111
            if ((buffer[0] == 0xFF) && (buffer[1] & 0xE0) == 0xE0 /*11100...*/)
            {
                //Wenn ersten 11 Bits auf 1 sind

                /*
                 * 2. Byte:
                 * 1111 1111 (1. Byte --> frame sync)
                 * 1110 0000 = frame sync (11 bits)
                 * 0001 1000 = mpeg vers        --> 11000 = 0x18 --> 000...     = >> 3
                 * 0000 0110 = layer index      --> 110   = 0x06 --> 00000...   = >>
                 * 0000 0001 = protection bit  (--> 1     = 0x1) --> 0000000...
                 *
                 */

                MPEGVersion = (MpegVersion)((buffer[1] & 0x18) >> 3);
                if (MPEGVersion == MpegVersion.Reserved)
                {
                    Context.Current.Logger.Error("MPEGVERSION = MpegVersion.Reserved", "Mp3Header.ParseBytes()");
                    return false;
                }

                MPEGLayer = (MpegLayer)((buffer[1] & 0x6) >> 1);
                if (MPEGLayer == MpegLayer.Reserved)
                {
                    Context.Current.Logger.Error("MPEGLayer == MpegLayer.Reserved", "Mp3Header.ParseBytes()");
                    return false;
                }

                int mpegLayerIndex = 0;
                if (MPEGLayer == MpegLayer.Layer1)
                    mpegLayerIndex = 0;
                else if (MPEGLayer == MpegLayer.Layer2)
                    mpegLayerIndex = 1;
                else
                    mpegLayerIndex = 2;

                CrcEnabled = (buffer[1] & 0x01) == 0x00;

                /*
                 * Byte 3:
                 *
                 * 1111 0000 = Bitrate --> 0xF0 --> >> 4
                 * 0000 1100 = Sampling rate ferquency --> 0xC --> >> 2
                 * 0000 0010 = Padding bit --> 0x02 -->
                 * 0000 0001 = Private bit --> 0x01 -->
                 */

                /*
                 * BitRate
                 *
                 */
                int bitrateIndex = (buffer[2] & 0xF0) >> 4;
                if (bitrateIndex >= 0x0F)
                {
                    //Reserved --> 1111
                    return false;
                }
                else
                {
                    BitRate = BitRates[(MPEGVersion == MpegVersion.Version1) ? 0 : 1, mpegLayerIndex, bitrateIndex] * 1000;
                    if (BitRate == 0) return false;
                }

                /*
                 * SamplingFrequenzy
                 *
                 */
                int samplingIndex = (buffer[2] & 0xC) >> 2;
                if (samplingIndex == 0x3 /* 11 */)
                    return false; //reserved

                if (MPEGVersion == MpegVersion.Version25)
                    SampleRate = _sampleRates[2, samplingIndex];
                else if (MPEGVersion == MpegVersion.Version2)
                    SampleRate = _sampleRates[1, samplingIndex];
                else //version 1
                    SampleRate = _sampleRates[0, samplingIndex];

                SampleCount = _samplesPerFrame[MPEGVersion == MpegVersion.Version1 ? 0 : 1, mpegLayerIndex];

                Padding = ((buffer[2] & 0x02) == 0x02);
                bool privateBit = ((buffer[2] & 0x01) == 0x01);

                /*
                 *
                 * Byte 4
                 * 1100 0000 = ChannelMode --> 0xC0 --> >> 6
                 * 0011 0000 = Mode Extension --> 0x30 --> >> 4
                 * 0000 1000 = CopyRight --> 0x08 >> bool
                 * 0000 0100 = Original --> 0x4 >> bool
                 * 0000 0011 = Emphasis --> 0x3 >> nothing
                 *
                 */
                ChannelMode = (MP3ChannelMode)((buffer[3] & 0xC0) >> 6);
                ChannelExtension = ((buffer[3] & 0x30) >> 4); //Leistung sparen
                CopyRight = (buffer[3] & 0x08) == 0x08; //Leistung sparen
                Original = (buffer[3] & 0x04) == 0x04;
                Emphasis = (buffer[3] & 0x03);

                int koeffizient = SampleCount / 8; //Coefficient
                int tmp = 0;
                tmp = (koeffizient * BitRate / SampleRate + (Padding ? 1 : 0));
                tmp *= (MPEGLayer == MpegLayer.Layer1) ? 4 : 1;
                FrameLength = tmp;

                return (FrameLength <= MaxFrameLength);
            }
            return false;
        }

        public MpegVersion MPEGVersion { get; private set; }

        public MpegLayer MPEGLayer { get; private set; }

        public int BitRate { get; private set; }

        public int SampleRate { get; private set; }

        public MP3ChannelMode ChannelMode { get; private set; }

        public int SampleCount { get; private set; }

        public int FrameLength { get; private set; }

        public int ChannelExtension { get; private set; }

        public bool CopyRight { get; private set; }

        public bool Original { get; private set; }

        public int Emphasis { get; private set; }

        public bool Padding { get; private set; }

        public bool CrcEnabled { get; private set; }
    }

    public enum MP3ChannelMode
    {
        Stereo,
        JointStereo,
        DualChannel,
        Mono
    }

    public enum MpegVersion
    {
        Version25,
        Reserved,
        Version2,
        Version1
    }

    public enum MpegLayer
    {
        Reserved,
        Layer3,
        Layer2,
        Layer1
    }
}