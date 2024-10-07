
namespace Rugal.MauiBase.Core.Model;

public class FormFile : IDisposable
{
    public FormFileType Type { get; set; }
    public string Key { get; private set; }
    public string UploadFileName { get; private set; }
    public string ContentType { get; private set; } = "application/octet-stream";
    public string FilePath { get; private set; }
    public byte[] Buffer { get; private set; }
    public Stream Stream { get; private set; }
    public bool HasFile => GetHasFile();
    protected FormFile(string Key, string ContentType)
    {
        this.Key = Key;

        if (!string.IsNullOrWhiteSpace(ContentType))
            this.ContentType = ContentType;
    }
    public FormFile(string Key, string FilePath, string ContentType = null) : this(Key, ContentType)
    {
        this.FilePath = FilePath;
        Type = FormFileType.FilePath;
    }
    public FormFile(string Key, byte[] Buffer, string ContentType = null) : this(Key, ContentType)
    {
        this.Buffer = Buffer;
        Type = FormFileType.Buffer;
    }
    public FormFile(string Key, Stream Stream, string ContentType = null) : this(Key, ContentType)
    {
        this.Stream = Stream;
        Type = FormFileType.Stream;
    }
    public FormFile WithUploadFileName(string UploadFileName)
    {
        this.UploadFileName = UploadFileName;
        return this;
    }
    public FormFile WithContentType(string ContentType)
    {
        this.ContentType = ContentType;
        return this;
    }
    public FormFile WithImageContent()
    {
        ContentType = "image/*";
        return this;
    }
    public FormFile WithPdfContent()
    {
        ContentType = "application/pdf";
        return this;
    }


    private bool GetHasFile()
    {
        if (Type == FormFileType.FilePath)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                return false;

            if (!File.Exists(FilePath))
                return false;
        }

        if (Type == FormFileType.Buffer)
        {
            if (Buffer is null || Buffer.Length == 0)
                return false;
        }

        if (Type == FormFileType.Stream)
        {
            if (Stream is null || Stream.Length == 0)
                return false;
        }

        return true;
    }
    public void Dispose()
    {
        Stream?.Dispose();
        Buffer = null;
        GC.SuppressFinalize(this);
    }
}
public enum FormFileType
{
    FilePath,
    Buffer,
    Stream
}