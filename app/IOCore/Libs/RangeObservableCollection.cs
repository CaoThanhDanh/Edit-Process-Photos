using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace IOCore.Libs
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void ClearSilently()
        {
            _suppressNotification = true;
            Clear();
            _suppressNotification = false;
        }

        public void PrependRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            items = items.Concat(this.ToList());
            Clear();
            foreach (var i in items)
                Add(i);

            _suppressNotification = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            foreach (var i in items)
                Add(i);

            _suppressNotification = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ReplaceRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            Clear();
            foreach (var i in items)
                Add(i);

            _suppressNotification = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ReplaceOne(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _suppressNotification = true;

            Clear();
            Add(item);

            _suppressNotification = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
