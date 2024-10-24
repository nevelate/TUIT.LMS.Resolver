using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TUIT.LMS.API
{
    public delegate void ProgressHandler(long bytes, long currentBytes, long totalBytes);

    class ProgressStreamContent : StreamContent
    {
        private const int DEFAULT_BUFFER_SIZE = 4096;

        public event ProgressHandler ProgressChanged = delegate { };

        private long currentBytes = 0;
        private long totalBytes = -1;


        public Stream InnerStream { get; }
        public int BufferSize { get; }

        public ProgressStreamContent(Stream innerStream, int bufferSize = DEFAULT_BUFFER_SIZE) :
            base(innerStream, bufferSize)
        {
            InnerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            BufferSize = bufferSize > 0 ? bufferSize : throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        private void ResetInnerStream()
        {
            if (InnerStream.Position != 0)
            {
                // Если внутренний поток нужно считать повторно, то этот внутренний поток должен поддерживать
                // возврат каретки(например FileStream), иначе внутренний поток не может быть считан повторно
                // в целевой поток(например NetworkStream)
                if (InnerStream.CanSeek)
                {
                    InnerStream.Position = 0;
                    currentBytes = 0;
                }
                else
                    throw new InvalidOperationException("The inner stream has already been read!");
            }
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            // Сбрасываем состояние внутреннего потока
            ResetInnerStream();

            // Если общее количество байт еще не получено, то пытаемся получить
            // его из заголовков контента
            if (totalBytes == -1)
                totalBytes = Headers.ContentLength ?? -1;

            // Если общее количество байт еще не найдено, то пытаемся
            // вычислить его из потока
            if (totalBytes == -1 && TryComputeLength(out var computedLength))
                totalBytes = computedLength == 0 ? -1 : computedLength;

            // Если общее количество байт отрицательное значение, то
            // присваеваем ему -1, идентифицирующее о невалидном общем количестве байт
            totalBytes = Math.Max(-1, totalBytes);

            // Начинаем читать внутренний поток
            var buffer = new byte[BufferSize];
            var bytesRead = 0;
            while ((bytesRead = await InnerStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                stream.Write(buffer, 0, bytesRead);
                currentBytes += bytesRead;

                // Генерируем событие ProgressChanged, чтобы оповестить о текущем прогрессе считывания
                ProgressChanged(bytesRead, currentBytes, totalBytes);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            var result = base.TryComputeLength(out length);
            totalBytes = length;
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                InnerStream.Dispose();

            base.Dispose(disposing);
        }
    }
}
