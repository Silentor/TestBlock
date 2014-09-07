using System;
using System.Linq;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Network.Serialization;
using Random = System.Random;

namespace Common.Tests
{
    [TestFixture]
    public class SerializerTests
    {
        private CommandSerializer _serializer;
        private static bool _isTestContractAdded;

        [TestFixtureSetUp]
        public void Init()
        {
            if (!_isTestContractAdded)
            {
                RuntimeTypeModel.Default.Add(typeof (Message), true).AddSubType(666, typeof (TestMessage));
                _isTestContractAdded = true;
            }

            _serializer = new CommandSerializer();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCompressibleStream(bool isCompressed)
        {
            var data = GenerateCompressibleData(98580);

            var startData = data.ToArray();
            var cmd = new TestMessage() {Data = startData};
            int size;
            var serialized = _serializer.Encode(cmd, out size, isCompressed);

            var cmd2 = (TestMessage)_serializer.Decode(serialized, isCompressed);

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
            var serialized = _serializer.Encode(cmd, out size, isCompressed);

            var cmd2 = (TestMessage)_serializer.Decode(serialized, isCompressed);

            Assert.That(cmd.Data, Is.EqualTo(startData));
            Assert.That(cmd2.Data, Is.EqualTo(startData));
        }

        [Test]
        public void TestBaseClassSerialization()
        {
            var cmd = new LoginData("Test");
            Message baseCmd = cmd;

            int size;
            var serialized = _serializer.Encode(baseCmd, out size);
            var serialized2 = new byte[size];
            Array.Copy(serialized, serialized2, size);

            var cmd2 = (LoginData)_serializer.Decode(serialized2);

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

    [ProtoContract]
    public class TestMessage : Message
    {
        public override Headers Header
        {
            get { return (Headers)255; }
        }

        [ProtoMember(1, IsPacked = true, OverwriteList = true, IsRequired = true)]
        public byte[] Data;
    }
}
