using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Rogero.ReactiveProperty.ReactivePropertyStreamExtensions
{
    public static class ReactivePropertyStreamExtensionMethods
    {
        public static ReactivePropertyStream<T> ToReactivePropertyStream<T>(this IObservable<T> stream)=> new ReactivePropertyStream<T>(stream);

        public static ReactivePropertyStream<T> ToReactivePropertyStream<T>(this ReactiveProperty<T> stream, bool retrieveCurrentValue)
        {
            return new ReactivePropertyStream<T>(stream, retrieveCurrentValue);
        }
    }
}

namespace Rogero.ReactiveProperty
{
    /// <summary>
    /// A type that holds a value that can be subscribed against. Also this wrapper implements
    /// INotifyPropertyChanged against the wrapped value for data-binding.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReactiveProperty<T> : INotifyPropertyChanged, IObservable<T>, IDisposable, IComparable
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

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var subscription = _valueObservable.Subscribe(observer);
            return subscription;
        }

        public virtual void Dispose() => _valueObservable.Dispose();

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public int CompareTo(object obj)
        {
            var comparable = _value as IComparable;
            if (comparable != null)
            {
                return comparable.CompareTo(obj);
            }
            throw new InvalidOperationException($"The underlying type {(typeof(T)).FullName} does not implement IComparable so a comparison is not possible.");
        }

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
            _streamSubscription = stream.Subscribe(SetValue);
        }

        public ReactivePropertyStream(ReactiveProperty<T> stream, bool retrieveCurrentValue)
        {
            _streamSubscription = stream.Subscribe(SetValue);

            if (retrieveCurrentValue)
            {
                SetValue(stream.Value);
            }
        }

        private void SetValue(T z)
        {
            Value = z;
            base.Value = z;
        }

        public override void Dispose()
        {
            _streamSubscription.Dispose();
            base.Dispose();
        }
    }
}
