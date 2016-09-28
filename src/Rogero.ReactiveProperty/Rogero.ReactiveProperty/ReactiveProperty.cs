using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Rogero.ReactiveProperty.ReactivePropertyStreamExtensions
{
    public static class ReactivePropertyStreamExtensionMethods
    {
        public static ReactivePropertyStream<T> ToReactivePropertyStream<T>(this IObservable<T> stream)=> new ReactivePropertyStream<T>(stream);
    }
}

namespace Rogero.ReactiveProperty
{
    /// <summary>
    /// A type that holds a value that can be subscribed against. Also this wrapper implements
    /// INotifyPropertyChanged against the wrapped value for data-binding.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

        public IDisposable Subscribe(IObserver<T> observer) => _valueObservable.Subscribe(observer);

        public virtual void Dispose() => _valueObservable.Dispose();

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Easy way to wrap an existing IObservable<T> so that you can gain access to the last element
    /// in the stream at any time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReactivePropertyStream<T> : ReactiveProperty<T>
    {
        public new T Value { get; private set; } = default(T);

        private readonly IDisposable _streamSubscription;

        public ReactivePropertyStream(IObservable<T> stream)
        {
            _streamSubscription = stream.Subscribe(z =>
            {
                Value = z;
                base.Value = z;
            });
        }

        public override void Dispose()
        {
            _streamSubscription.Dispose();
            base.Dispose();
        }
    }
}
