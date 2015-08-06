using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rogero.ReactiveProperty
{
    public class ReactiveProperty<T> : INotifyPropertyChanged, IObservable<T>
    {
        private T _value;
        private Subject<T> ValueObservable = new Subject<T>();

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
                    ValueObservable.OnNext(value);
                }
            }
        }

        public static implicit operator T(ReactiveProperty<T> reactiveProperty)
        {
            return reactiveProperty.Value;
        }

        //public static implicit operator ReactiveProperty<T>(T value)
        //{
        //    var reactiveProperty = new ReactiveProperty<T>();
        //    reactiveProperty.Value = value;
        //    return reactiveProperty;
        //}
        public override string ToString()
        {
            return Value == null ? string.Empty : Value.ToString();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return ValueObservable.Subscribe(observer);
        }

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
