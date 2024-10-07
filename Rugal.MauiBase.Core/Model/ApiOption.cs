using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace Rugal.MauiBase.Core.Model;
public abstract class ApiOptionBase<TParam, TResult, TOption>
    where TOption : ApiOptionBase<TParam, TResult, TOption>
    where TParam : class
{
    public event Action OnCalling;
    public event Action<HttpResponseMessage> OnError;
    public event Action<TResult> OnSuccess;
    public event Action OnComplete;
    public event Action<Exception> OnThrow;
    public ConcurrentDictionary<string, string> Query { get; set; }
    public TParam Param { get; protected set; }
    public List<FormFile> Files { get; protected set; }
    public bool HasQuery => !Query.IsEmpty;
    public bool HasParam => Param is not null;
    public bool HasFiles => Files?.Count > 0;
    public ApiOptionBase()
    {
        Query = [];
        Files = [];
    }

    #region With Method
    public virtual TOption WithOption(TOption Option, bool ClearEvent = true)
    {
        Query = Option.Query;

        if (ClearEvent)
            ClearEvents();
        OnCalling += Option.OnCalling;
        OnSuccess += Option.OnSuccess;
        OnError += Option.OnError;
        OnComplete += Option.OnComplete;
        return This();
    }
    public virtual TOption WithQuery(object Query)
    {
        if (Query is null)
            return This();

        if (Query is string QueryString)
        {
            if (string.IsNullOrWhiteSpace(QueryString))
                return This();

            this.Query[QueryString] = null;
            return This();
        }

        var AllProperty = Query
            .GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToArray();

        foreach (var Property in AllProperty)
        {
            var Value = Property.GetValue(Query);
            WithQuery(Property.Name, Value);
        }

        return This();
    }
    public virtual TOption WithQuery<TValue>(string Key, TValue Value)
    {
        if (Value is string StringValue)
        {
            Query[Key] = StringValue;
            return This();
        }
        var JsonString = JsonSerializer.Serialize(Value);
        Query[Key] = JsonString;
        return This();
    }
    public virtual TOption WithParam(TParam Param)
    {
        this.Param = Param;
        return This();
    }
    public virtual TOption WithCalling(Action CallingFunc)
    {
        OnCalling += CallingFunc;
        return This();
    }
    public virtual TOption WithSuccess(Action<TResult> SuccessFunc)
    {
        OnSuccess += SuccessFunc;
        return This();
    }
    public virtual TOption WithError(Action<HttpResponseMessage> ErrorFunc)
    {
        OnError += ErrorFunc;
        return This();
    }
    public virtual TOption WithError(Action<Exception> ThrowFunc)
    {
        OnThrow += ThrowFunc;
        return This();
    }
    public virtual TOption WithComplete(Action CompleteFunc)
    {
        OnComplete += CompleteFunc;
        return This();
    }
    #endregion

    #region Add File
    public virtual TOption AddFile(FormFile FormFile)
    {
        Files.Add(FormFile);
        return This();
    }
    public virtual TOption AddFile(string Key, string FilePath, string ContentType = null)
    {
        Files.Add(new FormFile(Key, FilePath, ContentType));
        return This();
    }
    public virtual TOption AddFile(string Key, byte[] Buffer, string ContentType = null)
    {
        Files.Add(new FormFile(Key, Buffer, ContentType));
        return This();
    }
    public virtual TOption AddFile(string Key, Stream Stream, string ContentType = null)
    {
        Files.Add(new FormFile(Key, Stream, ContentType));
        return This();
    }
    public virtual TOption AddFiles(IEnumerable<FormFile> FormFile)
    {
        Files.AddRange(FormFile);
        return This();
    }
    public virtual TOption AddFiles(string Key, IEnumerable<string> FilePaths, string ContentType = null)
    {
        Files.AddRange(FilePaths.Select(Item => new FormFile(Key, Item, ContentType)));
        return This();
    }
    public virtual TOption AddFiles(string Key, IEnumerable<byte[]> Buffers, string ContentType = null)
    {
        Files.AddRange(Buffers.Select(Item => new FormFile(Key, Item, ContentType)));
        return This();
    }
    public virtual TOption AddFiles(string Key, IEnumerable<Stream> Streams, string ContentType = null)
    {
        Files.AddRange(Streams.Select(Item => new FormFile(Key, Item, ContentType)));
        return This();
    }
    #endregion

    #region Clear Method
    public virtual TOption ClearQuery()
    {
        Query.Clear();
        return This();
    }
    public virtual TOption ClearParam()
    {
        Param = null;
        return This();
    }
    public virtual TOption ClearFiles()
    {
        foreach (var File in Files)
            File.Dispose();
        Files.Clear();
        return This();
    }
    public virtual TOption ClearEvents()
    {
        OnCalling = null;
        OnError = null;
        OnSuccess = null;
        OnComplete = null;

        return This();
    }
    #endregion

    #region Event Trigger
    public void Calling()
    {
        OnCalling?.Invoke();
    }
    public void Success(TResult ApiResult)
    {
        OnSuccess?.Invoke(ApiResult);
    }
    public void Error(HttpResponseMessage ApiResponse)
    {
        OnError?.Invoke(ApiResponse);
    }
    public void Complete()
    {
        OnComplete?.Invoke();
    }
    public void Throw(Exception Ex)
    {
        OnThrow?.Invoke(Ex);
    }
    #endregion

    #region Generate Method
    public string GenerateQueryString()
    {
        if (!HasQuery)
            return null;

        var Querys = new List<string>();
        foreach (var Item in Query)
        {
            var AddQuery = Item.Key;
            if (!string.IsNullOrWhiteSpace(Item.Value))
                AddQuery += $"={Item.Value}";

            Querys.Add(AddQuery);
        }
        var QueryResult = string.Join('&', Querys);
        return QueryResult;
    }
    #endregion

    private TOption This()
    {
        return (TOption)this;
    }
}
public class ApiOption<TParam, TResult> : ApiOptionBase<TParam, TResult, ApiOption<TParam, TResult>>
    where TParam : class
{
    public ApiOption<TParam, TResult> WithQuery<TQuery>(TQuery Query)
    {
        base.WithQuery(Query);
        return this;
    }
    public ApiOption<TParam, TResult> WithQuery(TParam Query)
    {
        base.WithQuery(Query);
        return this;
    }
}
public class ApiOption<TResult> : ApiOptionBase<object, TResult, ApiOption<TResult>>
{
    public ApiOption<TResult> WithQuery<TQuery>(TQuery Query)
    {
        base.WithQuery(Query);
        return this;
    }
}
public class ApiOption : ApiOption<object>
{
    public ApiOption WithOption<TParam, TResult, TOption>(TOption Option, bool IsClearEvent = true)
        where TOption : ApiOptionBase<TParam, TResult, TOption>
        where TParam : class
    {
        Query = Option.Query;
        Param = Option.Param;
        Files = Option.Files;

        if (IsClearEvent)
            ClearEvents();

        OnCalling += Option.Calling;
        OnSuccess += (Result) => Option.Success((TResult)Result);
        OnError += Option.Error;
        OnComplete += Option.Complete;

        return this;
    }
}