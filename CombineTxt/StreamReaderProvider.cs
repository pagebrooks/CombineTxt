using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CombineTxt
{
    public class StreamReaderProvider
    {
        private readonly string _fileName;
        private readonly Stream _stream;
        private readonly Encoding _encoding;

        public StreamReaderProvider(string fileName, Encoding encoding)
        {
            _fileName = fileName;
            _encoding = encoding;
        }

        public StreamReaderProvider(Stream stream, Encoding encoding)
        {
            _stream = stream;
            _encoding = encoding;
        }

        public StreamReader GetStreamReader()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                return new StreamReader(_fileName, _encoding);
            }

            return new StreamReader(_stream, _encoding);
        }
    }
}
