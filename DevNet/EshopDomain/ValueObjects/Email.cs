namespace EshopDomain.ValueObjects
{
    public sealed class Email
    {
        public string Value { get; }
        private Email(string value)
        {
            Value = value;
        }
        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email is required");
            if (!value.Contains('@'))
                throw new ArgumentException("Invalid email format");
            return new Email(value.Trim().ToLower());
        }
        public static implicit operator string(Email email) => email.Value;
        public override bool Equals(object obj)
        {
            return obj is Email other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
