using Rugal.MauiBase.Core.Extention;
using Rugal.MauiBase.Core.Model;
using Rugal.MauiBase.Core.Service;

namespace Rugal.MauiBase.Core.Page;
public abstract class ModelPage<TModel> : ContentPage where TModel : InpcModel, new()
{
    public static readonly BindableProperty ModelProperty = BindableProperty.Create(
        propertyName: nameof(Model),
        returnType: typeof(TModel),
        declaringType: typeof(ModelPage<TModel>),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay);
    public TModel Model
    {
        get => (TModel)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }
    public ApiClient Client { get; private set; }
    protected Binder<TModel> Binder { get; private set; }
    protected IServiceProvider Provider => Application.Current.GetProvider();
    public ModelPage() : base()
    {
        Model = new();
        Binder = new Binder<TModel>();
        Client = Provider.GetRequiredService<ApiClient>();
        Client.OnSuccess += SuccessUpdate;

        SetBinding(BindingContextProperty, new Binding(nameof(Model), BindingMode.TwoWay, source: this));
    }
    public ModelPage(TModel Model) : this()
    {
        UpdateModel(Model);
    }
    public ModelPage<TModel> UpdateModel(TModel SetModel)
    {
        Model = SetModel;
        return this;
    }
    protected void SuccessUpdate(object Result, ApiSet Set, HttpResponseMessage ApiResponse)
    {
        var StorePath = Set.Info.Path;
        if (string.IsNullOrWhiteSpace(StorePath))
        {
            if (Result is TModel ResultModel)
                UpdateModel(ResultModel);
            return;
        }

        Model.UpdateStore(StorePath, Result);
    }
}