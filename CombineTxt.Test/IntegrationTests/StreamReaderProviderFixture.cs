using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace CombineTxt.Test.IntegrationTests
{
    [TestFixture]
    public class StreamReaderProviderFixture
    {
        [Test]
        public void GetStream_Returns_StreamReader_Based_On_File()
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(folderPath, "temp.txt");
            Guid uniqueId = Guid.NewGuid();
            File.WriteAllText(filePath, uniqueId.ToString(), Encoding.UTF8);
            StreamReaderProvider srp = new StreamReaderProvider(filePath, Encoding.UTF8);
            StreamReader sr = srp.GetStreamReader();
            Assert.AreEqual(uniqueId.ToString(), sr.ReadToEnd());
            sr.Close();
            File.Delete(filePath);
        }

        [Test]
        public void GetStream_Returns_StreamReader_Based_On_Stream()
        {
            MemoryStream ms = new MemoryStream();
            StreamReaderProvider srp = new StreamReaderProvider(ms, Encoding.UTF8);
            StreamReader sr = srp.GetStreamReader();
            Assert.AreSame(ms, sr.BaseStream);
            Assert.AreEqual(Encoding.UTF8, sr.CurrentEncoding);
        }
    }
}
