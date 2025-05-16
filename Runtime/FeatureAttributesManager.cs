using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace FeatureAttributes
{
    public class FeatureAttributesManager<T> : IDisposable where T : class, IDisposable
    {
        private readonly ReactiveDictionary<Type, T> _attributes = new();
        public IEnumerable<T> Values => _attributes.Values;

        public virtual void Dispose()
        {
            var attributesArray = _attributes.Values.ToArray();
            for (var i = attributesArray.Length - 1; i >= 0; i--)
                attributesArray[i].Dispose();

            _attributes.Clear();
        }

        public TChild Get<TChild>() where TChild : class, T =>
            AttributeExists<TChild>() ? _attributes[typeof(TChild)] as TChild : null;

        public TChild Add<TChild>(TChild attribute) where TChild : class, T
        {
            _attributes[attribute.GetType()] = attribute;
            return attribute;
        }

        public void Remove<TChild>() where TChild : class, T
        {
            if (!AttributeExists<TChild>()) return;
            var attribute = _attributes[typeof(TChild)];
            attribute.Dispose();
            _attributes.Remove(typeof(TChild));
        }

        private bool AttributeExists<TChild>() where TChild : class, T => _attributes.ContainsKey(typeof(TChild));
    }
}