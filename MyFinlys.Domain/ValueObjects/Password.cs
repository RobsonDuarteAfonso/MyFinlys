using MyFinlys.Domain.Validations;

namespace MyFinlys.Domain.ValueObjects
{
    public sealed class Password : ValueObjectBase<Password>
    {
        public string Value { get; private set; } = default!;

        private Password() { }

        public Password(string value)
        {
            PasswordValidator.Validate(value);
            Value = BCrypt.Net.BCrypt.HashPassword(value);
        }

        public static Password FromHashed(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Invalid hash.", nameof(hash));

            return new Password { Value = hash };
        }

        public bool Verify(string value) => BCrypt.Net.BCrypt.Verify(value, Value);

        public override string ToString() => "[PROTECTED]";
    }
}