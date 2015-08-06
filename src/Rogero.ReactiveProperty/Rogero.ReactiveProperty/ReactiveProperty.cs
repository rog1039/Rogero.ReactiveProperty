using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Rogero.ReactiveProperty
{
    public class ReactiveProperty<T> : INotifyPropertyChanged, IObservable<T>, IDisposable
    {
        private T _value;
        private readonly Subject<T> _valueObservable = new Subject<T>();

        public ReactiveProperty(T initialValue = default(T))
        {
            _value = initialValue;
        }

        public T Value
        {
            get { return _value; }
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnPropertyChanged();
                    _valueObservable.OnNext(value);
                }
            }
        }

        public static implicit operator T(ReactiveProperty<T> reactiveProperty) => reactiveProperty.Value;

        public override string ToString() => Value == null ? string.Empty : Value.ToString();

        public void Dispose() => _valueObservable.Dispose();

        public IDisposable Subscribe(IObserver<T> observer) => _valueObservable.Subscribe(observer);
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
