using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using LZ4s;

namespace Silentor.TB.Common.Network.Compression
{
    public static class LZ4Wrapper
    {
        private const int WRAP_OFFSET_0 = 0;
        private const int WRAP_OFFSET_4 = sizeof(int);
        private const int WRAP_OFFSET_8 = 2 * sizeof(int);
        private const int WRAP_LENGTH = WRAP_OFFSET_8;

        private static void Poke4(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }

        private static uint Peek4(byte[] buffer, int offset)
        {
            // NOTE: It's faster than BitConverter.ToUInt32 (suprised? me too)
            return
                ((uint)buffer[offset]) |
                ((uint)buffer[offset + 1] << 8) |
                ((uint)buffer[offset + 2] << 16) |
                ((uint)buffer[offset + 3] << 24);
        }

        public static byte[] Wrap(
            byte[] inputBuffer, int inputOffset = 0, int inputLength = int.MaxValue)
        {
            inputLength = Math.Min(inputBuffer.Length - inputOffset, inputLength);
            if (inputLength < 0)
                throw new ArgumentException("inputBuffer size of inputLength is invalid");
            if (inputLength == 0)
                return new byte[WRAP_LENGTH];

            var outputLength = inputLength; // MaximumOutputLength(inputLength);
            var outputBuffer = new byte[outputLength];
            outputLength = LZ4Codec.Encode32(
                inputBuffer, inputOffset, inputLength, outputBuffer, 0, outputLength);

            byte[] result;

            if (outputLength >= inputLength || outputLength == 0)
            {
                result = new byte[inputLength + WRAP_LENGTH];
                Poke4(result, WRAP_OFFSET_0, (uint)inputLength);
                Poke4(result, WRAP_OFFSET_4, (uint)inputLength);
                Buffer.BlockCopy(inputBuffer, inputOffset, result, WRAP_OFFSET_8, inputLength);
            }
            else
            {
                result = new byte[outputLength + WRAP_LENGTH];
                Poke4(result, WRAP_OFFSET_0, (uint)inputLength);
                Poke4(result, WRAP_OFFSET_4, (uint)outputLength);
                Buffer.BlockCopy(outputBuffer, 0, result, WRAP_OFFSET_8, outputLength);
            }

            return result;
        }

        public static byte[] Unwrap(
            byte[] inputBuffer, int inputOffset = 0)
        {
            var inputLength = inputBuffer.Length - inputOffset;
            if (inputLength < WRAP_LENGTH)
                throw new ArgumentException("inputBuffer size is invalid");

            var outputLength = (int)Peek4(inputBuffer, inputOffset + WRAP_OFFSET_0);
            inputLength = (int)Peek4(inputBuffer, inputOffset + WRAP_OFFSET_4);
            if (inputLength > inputBuffer.Length - inputOffset - WRAP_LENGTH)
                throw new ArgumentException("inputBuffer size is invalid or has been corrupted");

            byte[] result;

            if (inputLength >= outputLength)
            {
                result = new byte[inputLength];
                Buffer.BlockCopy(
                    inputBuffer, inputOffset + WRAP_OFFSET_8,
                    result, 0, inputLength);
            }
            else
            {
                result = new byte[outputLength];
                LZ4Codec.Decode32(
                    inputBuffer, inputOffset + WRAP_OFFSET_8, inputLength,
                    result, 0, outputLength,
                    true);
            }

            return result;
        }
    }
}
