using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ColdFusion.RestPostProcess {

    public class UTF8SanitizerStream : Stream {

        readonly string _pattern;
        readonly string _substitute;
        readonly Stream _output;
        readonly Decoder _decoder;
        string _buffer;


        public UTF8SanitizerStream(string pattern, string substitute, Stream output) {
            _pattern = pattern;
            _substitute = substitute;
            _output = output;
            _decoder = Encoding.UTF8.GetDecoder();
            _buffer = String.Empty;
        }

        public override void Write(byte[] buffer, int offset, int count) {

            // using decoder to extract full character sequences in case buffer ends with a multi-byte character
            var chars = new char[_decoder.GetCharCount(buffer, offset, count)];
            _decoder.GetChars(buffer, offset, count, chars, 0, false);

            var sb = new StringBuilder(_buffer);
            var writer = new StringWriter(sb);
            writer.Write(chars);
            writer.Flush();

            sb.Replace(_pattern, _substitute);
            _buffer = sb.ToString();

            var size = Math.Max(0, _buffer.Length - _pattern.Length);
            var chunk = _buffer.Substring(0, size);
            _buffer = _buffer.Substring(size);

            var bytes = UTF8Encoding.UTF8.GetBytes(chunk);

            // .NET 2.0 will throw an exception when trying to write empty bytes
            if (bytes.Length > 0)
                _output.Write(bytes, 0, bytes.Length);
        }

        public override bool CanRead {
            get { return false; }
        }

        public override bool CanSeek {
            get { return false; }
        }

        public override bool CanWrite {
            get { return true; }
        }

        public override void Flush() {
            var bytes = UTF8Encoding.UTF8.GetBytes(_buffer);
        
            // .NET 2.0 will throw an exception when trying to write empty bytes
            if (bytes.Length > 0)
                _output.Write(bytes, 0, bytes.Length);

            _output.Flush();
        }

        public override long Length {
            get { throw new NotSupportedException(); }
        }

        public override long Position {
            get {
                throw new NotSupportedException();
            }
            set {
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }
    }
}
