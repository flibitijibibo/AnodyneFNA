using AnodyneSharp.Logging;
using AnodyneSharp.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AnodyneSharp.Resources
{
    public class ContentWriter : IDisposable
    {
        public string FilePath { get; protected set; }

        private StreamWriter _writer;
        private MemoryStream _stream;
        private int _lineNumber;


        public ContentWriter(string filePath)
        {
            _lineNumber = 0;
            FilePath = filePath;
            SetStreamreader();
        }

        public virtual void Dispose()
        {
            _writer.Flush();
            Storage.SaveFile(FilePath, _stream.ToArray());

            _writer.Dispose();
            _stream.Dispose();
        }

        protected void SetStreamreader()
        {
            _stream = new MemoryStream();
            _writer = new StreamWriter(_stream);
        }

        protected void ThrowFileWarning(string message)
        {
            DebugLogger.AddWarning(FormatFileError(message), false);
        }

        protected void ThrowFileError(string message)
        {
            DebugLogger.AddError(FormatFileError(message), false);
        }
        protected void WriteLine()
        {
            _lineNumber++;
            _writer.WriteLine();
        }

        protected void WriteLine(string line)
        {
            _lineNumber++;
            _writer.WriteLine(line);
        }

        protected void WriteLine(char line)
        {
            _lineNumber++;
            _writer.WriteLine(line);
        }

        protected void WriteLine(int value)
        {
            _lineNumber++;
            _writer.WriteLine(value.ToString());
        }

        protected void Write(string line)
        {
            _writer.Write(line);
        }

        private string FormatFileError(string message)
        {
            return $"{message} : {FilePath} {_lineNumber}";
        }
    }
}
