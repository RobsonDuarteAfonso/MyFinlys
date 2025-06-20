using MyFinlys.Domain.Common;

namespace MyFinlys.Domain.ValueObjects
{
    public sealed class Year : ValueObjectBase<Year>
    {
        public int Value { get; }

        private Year() { }

        private Year(int value)
        {
            Value = value;
        }

        public static Year Create(int value)
        {
            Guard.AgainstYearOutsideAllowedRange(value, nameof(value));
            return new Year(value);
        }

        public override string ToString() => Value.ToString();

        public static implicit operator int(Year year) => year.Value;
    }
}