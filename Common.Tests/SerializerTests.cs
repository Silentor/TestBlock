using System;
using System.Linq;
using Lidgren.Network;
using NUnit.Framework;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Tests
{
    [TestFixture]
    public class SerializerTests
    {
        private MessageSerializer _serializer;
        private static bool _isTestContractAdded;

        [TestFixtureSetUp]
        public void Init()
        {
            _serializer = new MessageSerializer();
            _serializer.AddMessageHeader<TestMessage>(Headers.Test);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCompressibleStream(bool isCompressed)
        {
            var data = GenerateCompressibleData(98580);

            var startData = data.ToArray();
            var cmd = new TestMessage() {Data = startData};
            int size;
            var serialized = _serializer.Serialize(cmd, out size);
            Array.Resize(ref serialized, size);

            var cmd2 = (TestMessage)_serializer.Deserialize(serialized);

            Assert.That(cmd.Data, Is.EqualTo(startData));
            Assert.That(cmd2.Data, Is.EqualTo(startData));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestUncompressibleStream(bool isCompressed)
        {
            var data = GenerateNoncompressibleData(98580);

            var startData = data.ToArray();
            var cmd = new TestMessage() { Data = startData };
            int size;
            var serialized = _serializer.Serialize(cmd, out size);
            Array.Resize(ref serialized, size);

            var cmd2 = (TestMessage)_serializer.Deserialize(serialized);

            Assert.That(cmd.Data, Is.EqualTo(startData));
            Assert.That(cmd2.Data, Is.EqualTo(startData));
        }

        [Test]
        public void TestBaseClassSerialization()
        {
            var cmd = new LoginData("Test");
            Message baseCmd = cmd;

            int size;
            var serialized = _serializer.Serialize(baseCmd, out size);
            var serialized2 = new byte[size];
            Array.Copy(serialized, serialized2, size);

            var cmd2 = (LoginData)_serializer.Deserialize(serialized2);

            Assert.That(cmd.Name, Is.EqualTo(cmd2.Name));
        }

        private byte[] GenerateNoncompressibleData(int size)
        {
            var rnd = new Random();
            var result = new byte[size];
            rnd.NextBytes(result);

            return result;
        }

        private byte[] GenerateCompressibleData(int size)
        {
            var rnd = new Random();
            var result = new byte[size];
            var value = rnd.NextDouble() * 255d;

            for (var i = 0; i < size; i++)
            {
                value += 255 % rnd.NextDouble()/5;
                result[i] = (byte) value;
            }

            return result;
        }
    }

    public class TestMessage : Message
    {
        [Header(Headers.Test)]
        public override Headers Header
        {
            get { return Headers.Test; }
        }

        public byte[] Data;

        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);

            buffer.WriteVariableInt32(Data.Length);
            buffer.Write(Data);
        }

        public override void Deserialize(NetBuffer buffer)
        {
            base.Deserialize(buffer);

            var length = buffer.ReadVariableInt32();
            Data = buffer.ReadBytes(length);
        }
    }
}
