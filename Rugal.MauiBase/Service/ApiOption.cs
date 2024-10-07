using System.Text.Json;
namespace Rugal.MauiBase.Service;

public abstract class ApiOptionBase<TApiParam, TApiResult>
{
    public object Param { get; protected set; }
    public Dictionary<string, string> Query { get; set; }
    public abstract event Action OnCalling;
    public abstract event Action<HttpResponseMessage> OnError;
    public abstract event Action<TApiResult> OnSuccess;
    public abstract event Action<TApiResult> OnComplete;
    public virtual ApiOptionBase<TApiParam, TApiResult> WithQuery<TQuery>(TQuery Query) where TQuery : class
    {
        if (Query is null)
            return this;

        if (Query is string QueryString)
        {
            if (string.IsNullOrWhiteSpace(QueryString))
                return this;

            this.Query.Add(QueryString, null);
            return this;
        }

        var AllProperty = Query
            .GetType()
            .GetProperties()
            .Where(Item => Item.GetAccessors().Any(Val => Val.IsPublic))
            .ToArray();

        foreach (var Property in AllProperty)
        {
            var Name = Property.Name;
            var GetValue = Property.GetValue(Query);
            WithQuery(Name, GetValue);
        }

        return this;
    }
    public virtual ApiOptionBase<TApiParam, TApiResult> WithQuery<TValue>(string Key, TValue Value)
    {
        if (Value is string StringValue)
        {
            Query.Add(Key, StringValue);
            return this;
        }
        var JsonString = JsonSerializer.Serialize(Value);
        Query.Remove(Key);
        Query.Add(Key, JsonString);
        return this;
    }
    public virtual ApiOptionBase<TApiParam, TApiResult> WithParam(TApiParam Param)
    {
        this.Param = Param;
        return this;
    }
    public virtual ApiOptionBase<TApiParam, TApiResult> WithCalling(Action CallingFunc)
    {
        OnCalling += CallingFunc;
        return this;
    }
    public virtual ApiOptionBase<TApiParam, TApiResult> WithSuccess(Action<TApiResult> SuccessFunc)
    {
        OnSuccess += SuccessFunc;
        return this;
    }
    public virtual ApiOptionBase<TApiParam, TApiResult> WithError(Action<HttpResponseMessage> ErrorFunc)
    {
        OnError += ErrorFunc;
        return this;
    }
    public virtual ApiOptionBase<TApiParam, TApiResult> WithComplete(Action<TApiResult> CompleteFunc)
    {
        OnComplete += CompleteFunc;
        return this;
    }
    public virtual ApiOptionBase<TApiParam, TApiResult> ClearQuery()
    {
        Query.Clear();
        return this;
    }
}
public abstract class ApiOption<TApiParam, TApiResult, TApiOption> : ApiOptionBase<TApiParam, TApiResult>
    where TApiOption : ApiOption<TApiParam, TApiResult, TApiOption>
{
    public override event Action OnCalling;
    public override event Action<HttpResponseMessage> OnError;
    public override event Action<TApiResult> OnSuccess;
    public override event Action<TApiResult> OnComplete;
    public new TApiParam Param
    {
        get => (TApiParam)base.Param;
        protected set => base.Param = value;
    }
    public ApiOption()
    {
        Query = [];
    }
    public TApiOption WithOption(TApiOption Option)
    {
        Query = Option.Query;
        OnCalling += Option.OnCalling;
        OnSuccess += Option.OnSuccess;
        OnError += Option.OnError;
        OnComplete += Option.OnComplete;
        return This();
    }
    public override TApiOption WithParam(TApiParam Param)
    {
        base.WithParam(Param);
        return This();
    }
    public override TApiOption WithQuery<TQuery>(TQuery Query) where TQuery : class
    {
        base.WithQuery(Query);
        return This();
    }
    public override TApiOption WithQuery<TValue>(string Key, TValue Value)
    {
        base.WithQuery(Key, Value);
        return This();
    }
    public override TApiOption WithCalling(Action CallingFunc)
    {
        base.WithCalling(CallingFunc);
        return This();
    }
    public override TApiOption WithSuccess(Action<TApiResult> SuccessFunc)
    {
        base.WithSuccess(SuccessFunc);
        return This();
    }
    public override TApiOption WithError(Action<HttpResponseMessage> ErrorFunc)
    {
        base.WithError(ErrorFunc);
        return This();
    }
    public override TApiOption WithComplete(Action<TApiResult> CompleteFunc)
    {
        base.WithComplete(CompleteFunc);
        return This();
    }
    public override TApiOption ClearQuery()
    {
        base.ClearQuery();
        return This();
    }
    private TApiOption This()
    {
        return (TApiOption)this;
    }
}
public class ApiOption<TApiResult> : ApiOption<object, TApiResult, ApiOption<TApiResult>> { }
public class ApiOption<TApiParam, TApiResult> : ApiOption<TApiParam, TApiResult, ApiOption<TApiParam, TApiResult>> { }
