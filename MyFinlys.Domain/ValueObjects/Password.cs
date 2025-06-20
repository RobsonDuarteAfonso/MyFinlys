using MyFinlys.Domain.Common;
using MyFinlys.Domain.Validations;

namespace MyFinlys.Domain.ValueObjects
{
    public sealed class Password : ValueObjectBase<Password>
    {
        public string Value { get; private set; } = default!;

        private Password() { }

        private Password(string hashedValue)
        {
            Value = hashedValue;
        }

        public static Password Create(string plainText)
        {
            PasswordValidator.Validate(plainText);
            var hash = BCrypt.Net.BCrypt.HashPassword(plainText);
            return new Password(hash);
        }

        public static Password FromHashed(string hash)
        {
            Guard.AgainstNullOrEmpty(hash, nameof(hash));
            return new Password(hash);
        }

        public bool Verify(string plainText) => BCrypt.Net.BCrypt.Verify(plainText, Value);

        public override string ToString() => "[PROTECTED]";
    }
}