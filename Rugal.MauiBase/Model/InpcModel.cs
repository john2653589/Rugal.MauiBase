using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rugal.MauiBase.Model
{
    public class InpcModel : INotifyPropertyChanged
    {
        protected Dictionary<string, object> Data { get; set; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (!string.IsNullOrWhiteSpace(PropertyName))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected void SetValue<TValue>(TValue Value, [CallerMemberName] string PropertyName = "")
        {
            Data.Remove(PropertyName);
            Data.Add(PropertyName, Value);
            OnPropertyChanged(PropertyName);
        }
        protected TValue GetValue<TValue>([CallerMemberName] string PropertyName = "")
        {
            if (string.IsNullOrWhiteSpace(PropertyName))
                throw new Exception($"PropertyName is null or empty");

            if (!Data.TryGetValue(PropertyName, out var Value))
                return default;

            if (Value is TValue Result)
                return Result;

            throw new Exception($"Property {PropertyName} can not convert to {typeof(TValue)}");
        }
    }
}