using MyFinlys.Domain.Validations;

namespace MyFinlys.Domain.ValueObjects
{
    public sealed class Email : ValueObjectBase<Email>
    {
        public string Value { get; private set; } = default!;

        private Email() { }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            EmailValidator.Validate(value);
            return new Email(value);
        }

        public static implicit operator string(Email email) => email.Value;

        public override string ToString() => Value;
    }
}