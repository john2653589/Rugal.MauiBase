using Rugal.MauiBase.Extention;
using Rugal.MauiBase.Model;
using Rugal.MauiBase.Service;

namespace Rugal.MauiBase.Page;
public class ModelPage<TModel> : ContentPage where TModel : InpcModel
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
    public ModelPage() : base()
    {
        Client = Application.Current.GetRequiredService<ApiClient>();
        Binder = new Binder<TModel>();

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
}