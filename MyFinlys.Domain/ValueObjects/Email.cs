using MyFinlys.Shared.Validations;

namespace MyFinlys.Domain.ValueObjects
{
    public sealed class Email : ValueObjectBase<Email>
    {
        public string Value { get; private set; } = default!;

        private Email() { }

        public Email(string value)
        {
            EmailValidator.Validate(value);
            Value = value;
        }

        public static implicit operator string(Email email) => email.Value;

        public override string ToString() => Value;
    }
}