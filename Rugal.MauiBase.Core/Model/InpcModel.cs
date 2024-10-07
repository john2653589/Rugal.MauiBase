using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rugal.MauiBase.Core.Model
{
    public class InpcModel : INotifyPropertyChanged
    {
        protected Dictionary<string, object> Store { get; set; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (!string.IsNullOrWhiteSpace(PropertyName))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        protected void SetValue<TValue>(TValue Value, [CallerMemberName] string PropertyName = "")
        {
            UpdateStore(PropertyName, Value);
        }
        protected TValue GetValue<TValue>([CallerMemberName] string PropertyName = "")
        {
            if (string.IsNullOrWhiteSpace(PropertyName))
                throw new Exception($"PropertyName is null or empty");

            if (!Store.TryGetValue(PropertyName, out var Value))
                return default;

            if (Value is TValue Result)
                return Result;

            throw new Exception($"Property {PropertyName} can not convert to {typeof(TValue)}");
        }
        public void UpdateStore(string StorePath, object Data)
        {
            Store[StorePath] = Data;
            OnPropertyChanged(StorePath);
        }
    }
}