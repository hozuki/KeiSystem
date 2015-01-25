#region GPLv2

/*
KeiSystem
Copyright (C) 2015 MIC Studio
Developer homepage: https://github.com/GridSciense

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kei.Gui
{
    public sealed class StreamLogger : ILogger, IDisposable
    {

        private Stream _stream;
        private StreamWriter _writer;
        private bool _isDisposed = false;

        private StreamLogger(Stream stream)
        {
            _stream = stream;
            _writer = new StreamWriter(_stream, Encoding.UTF8);
        }

        public static StreamLogger Create(Stream stream)
        {
            if (stream == null || !stream.CanWrite)
            {
                throw new ArgumentException("Invalid stream for StreamLogger.");
            }
            return new StreamLogger(stream);
        }

        public void Log(string log)
        {
            if (!_isDisposed)
            {
                _writer.WriteLine(DateTime.Now.ToString());
                _writer.WriteLine(log);
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _writer.Dispose();
            _stream.Dispose();
        }
    }
}
