using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class NameSyntaxExtension
    {
        public static bool IsIDisposable(this NameSyntax nameSyntax)
        {
            switch (nameSyntax)
            {
                case SimpleNameSyntax sns:
                    return sns.Identifier.Text == Constants.IDisposable;
                //case AliasQualifiedNameSyntax aqns:
                //    return aqns.Name.Identifier.Text == Constants.IDisposable;
                case QualifiedNameSyntax qns:
                    return qns.Right.Identifier.Text == Constants.IDisposable
                           && qns.Left is SimpleNameSyntax sns2 && sns2.Identifier.Text == Constants.System;
            }

            return false;
        }
    }
}