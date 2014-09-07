using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Silentor.TB.Client.Tools;

namespace Silentor.TB.Client.Tests
{
    [TestFixture]
    public class WorkerTests
    {
        [Test]
        public void TestEmptyWorker()
        {
            //Arrange
            var sut = new Worker<int, string>(i => i.ToString());

            //Assert
            string result;
            sut.GetResult(out result).Should().BeFalse();
        }

        [Test]
        public void TestProcessing()
        {
            //Arrange
            var sut = new Worker<int, string>(i => i.ToString());

            //Act
            sut.Add(1);
            sut.Add(2);
            sut.Add(3);

            while(sut.ProcessedCount != 3)
                Thread.Sleep(10);

            //Assert
            string result;
            var results = new List<string>();
            for (int i = 0; i < 3; i++)
                if (sut.GetResult(out result))
                    results.Add(result);

            results.Should().Equal(new[] { "1", "2", "3" });
        }
    }
}
