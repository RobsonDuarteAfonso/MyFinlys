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

        //IMplementar o padrão Factories
        //Alterar as validações para Guards

        public static Year Create(int value)
        {
            if (value < 1900 || value > 2100)
                throw new ArgumentOutOfRangeException(nameof(value), "Invalid year. Year must be between 1900 and 2100.");

            return new Year(value);
        }

        public override string ToString() => Value.ToString();

        public static implicit operator int(Year year) => year.Value;
    }
}