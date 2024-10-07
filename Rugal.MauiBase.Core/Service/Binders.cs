using Rugal.MauiBase.Core.Model;
using System.Linq.Expressions;

namespace Rugal.MauiBase.Core.Service;

public class Binder<TModel> where TModel : InpcModel
{
    public Binder<TModel> SetBind<TView>(TView View, BindableProperty Property, Expression<Func<TModel, object>> Exp)
        where TView : View
    {
        var ExpBody = Exp.Body;
        var Properties = new Stack<string>();
        while (ExpBody is MemberExpression MemberExp)
        {
            Properties.Push(MemberExp.Member.Name);
            ExpBody = MemberExp.Expression;
        }

        var PropertyName = string.Join('.', Properties);
        var CreateBinding = new Binding(PropertyName, BindingMode.TwoWay);
        View.SetBinding(Property, CreateBinding);
        return this;
    }
    public ViewBinder<TView, TModel> WithView<TView>(TView View) where TView : View
    {
        return new ViewBinder<TView, TModel>(View);
    }
    public ViewBinder<TView, TModel> WithView<TView>(TView View, BindableProperty Property, Expression<Func<TModel, object>> Exp)
        where TView : View
    {
        var Binder = WithView(View)
            .Bind(Property, Exp);
        return Binder;
    }
    public PropertyBinder<TModel> WithProperty(BindableProperty Property)
    {
        return new PropertyBinder<TModel>(Property);
    }
    public PropertyBinder<TModel> WithProperty<TView>(TView View, BindableProperty Property, Expression<Func<TModel, object>> Exp)
        where TView : View
    {
        var Binder = WithProperty(Property)
            .Bind(View, Exp);
        return Binder;
    }
}
public class ViewBinder<TView, TModel> : Binder<TModel>
    where TView : View
    where TModel : InpcModel
{
    public TView View { get; set; }
    public ViewBinder(TView _View)
    {
        View = _View;
    }
    public ViewBinder<TView, TModel> Bind(BindableProperty Property, Expression<Func<TModel, object>> Exp)
    {
        SetBind(View, Property, Exp);
        return this;
    }
}
public class PropertyBinder<TModel> : Binder<TModel>
    where TModel : InpcModel
{
    public BindableProperty Property { get; private set; }
    public PropertyBinder(BindableProperty _Property)
    {
        Property = _Property;
    }
    public PropertyBinder<TModel> Bind<TView>(TView View, Expression<Func<TModel, object>> Exp) where TView : View
    {
        SetBind(View, Property, Exp);
        return this;
    }
}