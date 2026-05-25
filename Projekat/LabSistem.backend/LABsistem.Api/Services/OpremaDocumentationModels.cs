using System;
using System.IO;

namespace LABsistem.Api.Services
{
    public sealed class OpremaDokumentacijaUpload
    {
        public OpremaDokumentacijaUpload(string fileName, string contentType, Func<Stream> openReadStream, long length)
        {
            FileName = fileName;
            ContentType = contentType;
            OpenReadStream = openReadStream;
            Length = length;
        }

        public string FileName { get; }
        public string ContentType { get; }
        public Func<Stream> OpenReadStream { get; }
        public long Length { get; }
    }

    public sealed record OpremaDokumentacijaFile(string FilePath, string FileName);
}
