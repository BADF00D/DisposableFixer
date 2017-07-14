namespace DisposableFixer.Configuration
{
    public class MethodCall
    {
        public MethodCall(string name, string[] parameter, bool isStatic)
        {
            Name = name;
            Parameter = parameter;
            IsStatic = isStatic;
        }
        internal string Name { get; }
        internal string[] Parameter { get; }
        internal bool IsStatic { get; }

        protected bool Equals(MethodCall other)
        {
            return Equals(Parameter, other.Parameter) && IsStatic == other.IsStatic;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodCall) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Parameter?.GetHashCode() ?? 0)*397) ^ IsStatic.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"Parameter: {string.Join(", ",Parameter)}, IsStatic: {IsStatic}";
        }
    }
}