using System.Linq;

namespace DisposableFixer.Configuration
{
    internal sealed class CtorCall
    {
        public CtorCall(string[] arguments, int positionOfFlagParameter, bool flagIndicationNonDisposedResource)
        {
            Parameter = arguments;
            PositionOfFlagParameter = positionOfFlagParameter;
            FlagIndicationNonDisposedResource = flagIndicationNonDisposedResource;
        }

        public string[] Parameter { get; }
        public int PositionOfFlagParameter { get; }
        public bool FlagIndicationNonDisposedResource { get; }

        public override string ToString()
        {
            return
                $"({string.Join(", ", Parameter)}) where {PositionOfFlagParameter}. parameter should be {FlagIndicationNonDisposedResource}";
        }

        private bool Equals(CtorCall other)
        {
            return 
                Parameter.SequenceEqual(other.Parameter)
                && PositionOfFlagParameter == other.PositionOfFlagParameter
                && FlagIndicationNonDisposedResource == other.FlagIndicationNonDisposedResource;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CtorCall && Equals((CtorCall) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Parameter?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ PositionOfFlagParameter;
                hashCode = (hashCode*397) ^ FlagIndicationNonDisposedResource.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(CtorCall left, CtorCall right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CtorCall left, CtorCall right)
        {
            return !Equals(left, right);
        }
    }
}