using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Rogero.ReactiveProperty.ReactivePropertyStreamExtensions;

namespace Rogero.ReactiveProperty.ReactivePropertyStreamExtensions
{
    public static class ReactivePropertyStreamExtensionMethods
    {
        public static ReactivePropertyStream<T> ToReactivePropertyStream<T>(this IObservable<T> stream)=> new ReactivePropertyStream<T>(stream);
    }
}

namespace Rogero.ReactiveProperty
{
    public class ReactiveProperty<T> : INotifyPropertyChanged, IObservable<T>, IDisposable
    {
        protected T _value;
        protected readonly Subject<T> _valueObservable = new Subject<T>();

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

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public void Dispose() => _valueObservable.Dispose();

        public IDisposable Subscribe(IObserver<T> observer) => _valueObservable.Subscribe(observer);
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class ReactivePropertyStream<T> : ReactiveProperty<T>
    {
        private readonly IObservable<T> _stream;
        private T lastValue = default(T);

        public ReactivePropertyStream(IObservable<T> stream)
        {
            _stream = stream;
            _stream.Subscribe(z =>
            {
                lastValue = z;
                base.Value = z;
            });
        }

        public new T Value => lastValue;
    }
}
