using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    internal static class MemberAccessExpressionSyntaxExpression
    {
        public static bool IsDisposeCall(this MemberAccessExpressionSyntax memberAccessExpression)
        {
            return memberAccessExpression.Name.Identifier.Text == Constants.Dispose;
        }

        [Obsolete]
        public static bool IsDisposeCallFor(this MemberAccessExpressionSyntax memberAccessExpression,
            string variable)
        {
            return memberAccessExpression != null
                   && memberAccessExpression.IsDisposeCall()
                   && (memberAccessExpression.Expression as IdentifierNameSyntax)?.Identifier.Text == variable;
        }

        internal static MemberPath GetMemberPath(this MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            if (memberAccessExpressionSyntax == null) return MemberPath.Empty;
            if(memberAccessExpressionSyntax.Expression is IdentifierNameSyntax ins) return new MemberPath(ins.Identifier.Text, memberAccessExpressionSyntax.Name.Identifier.Text);
            var maes = memberAccessExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            var path = GetMemberPath(maes);
            return path.Suffix(memberAccessExpressionSyntax.Name.Identifier.Text);
        }

        internal class MemberPath
        {
            public static MemberPath Empty { get; } = new MemberPath(new List<string>());

            private readonly List<string> _parts = new List<string>();

            public MemberPath(string identifier, string member)
            {
                _parts.Add(identifier);
                _parts.Add(member);
            }

            private MemberPath(List<string> parts)
            {
                _parts = parts;
            }

            protected bool Equals(MemberPath other)
            {
                return _parts.Count == other._parts.Count && _parts.SequenceEqual(other._parts);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((MemberPath) obj);
            }

            public override int GetHashCode()
            {
                return (_parts != null ? _parts.GetHashCode() : 0);
            }

            public static bool operator ==(MemberPath left, MemberPath right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(MemberPath left, MemberPath right)
            {
                return !Equals(left, right);
            }

            public MemberPath Prefix(string identifier)
            {
                var parts = new List<string> {identifier};
                parts.AddRange(_parts);
                return new MemberPath(parts);
            }
            public MemberPath Suffix(string identifier)
            {
                var parts = new List<string>(_parts);
                parts.Add(identifier);
                return new MemberPath(parts);
            }
            public bool EndsWith(string identifier)
            {
                if (_parts.Count == 0) return false;
                return _parts[_parts.Count - 1] == identifier;
            }

            public bool IsDisposeCall()
            {
                return EndsWith(Constants.Dispose);
            }

            public override string ToString()
            {
                return string.Join(".", _parts);
            }
        }
    }
}