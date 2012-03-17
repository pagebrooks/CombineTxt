using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CombineTxt
{
    public class StreamWriterProvider
    {
        private readonly string _fileName;
        private readonly Stream _stream;
        private readonly Encoding _encoding;
        private readonly bool _keepStreamOpen;

        public StreamWriterProvider(string fileName, Encoding encoding)
        {
            _fileName = fileName;
            _encoding = encoding;
            _keepStreamOpen = false;
        }

        public StreamWriterProvider(Stream stream, Encoding encoding, bool keepStreamOpen)
        {
            _stream = stream;
            _encoding = encoding;
            _keepStreamOpen = keepStreamOpen;
        }

        public StreamWriter GetStreamWriter()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                return new StreamWriter(_fileName, false, _encoding);
            }

            return new StreamWriter(_stream, _encoding);
        }

        public bool KeepStreamOpen
        {
            get { return _keepStreamOpen; }
        }
    }
}
