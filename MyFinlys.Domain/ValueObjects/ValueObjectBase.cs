using System.Reflection;

namespace MyFinlys.Domain.ValueObjects
{
    public abstract class ValueObjectBase<T> : IEquatable<T> where T : ValueObjectBase<T>
    {
        private static readonly PropertyInfo[] Properties = typeof(T)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public override bool Equals(object? obj)
            => Equals(obj as T);

        public bool Equals(T? other)
        {
            if (other is null)
                return false;

            foreach (var property in Properties)
            {
                var thisValue = property.GetValue(this);
                var otherValue = property.GetValue(other);

                if (thisValue is null ^ otherValue is null)
                    return false;

                if (thisValue is not null && !thisValue.Equals(otherValue))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var property in Properties)
            {
                var value = property.GetValue(this);
                if (value is not null)
                    hash.Add(value);
            }
            return hash.ToHashCode();
        }

        public static bool operator ==(ValueObjectBase<T>? a, ValueObjectBase<T>? b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObjectBase<T>? a, ValueObjectBase<T>? b)
            => !(a == b);
    }
}
