// Unlicensed code. All credits
// to edongashi on GitHub.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace WinPath.Extensions
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ChildVerbsAttribute : Attribute
    {
        public Type[] Types { get; }

        public ChildVerbsAttribute(params Type[] types)
        {
            Types = types;
        }
    }

    /// <summary>
    /// Extension methods to allow multi level verb parsing.
    /// </summary>
    public static class ParserVerbExtensions
    {
        public static ParserResult<object> ParseVerbs(this Parser parser, IEnumerable<string> args, params Type[] types)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            var argsArray = args as string[] ?? args.ToArray();
            if (argsArray.Length == 0 || argsArray[0].StartsWith("-"))
            {
                return parser.ParseArguments(argsArray, types);
            }

            var verb = argsArray[0];
            foreach (var type in types)
            {
                var verbAttribute = type.GetCustomAttribute<VerbAttribute>();
                if (verbAttribute == null || verbAttribute.Name != verb)
                {
                    continue;
                }

                var subVerbsAttribute = type.GetCustomAttribute<ChildVerbsAttribute>();
                if (subVerbsAttribute != null)
                {
                    return ParseVerbs(parser, argsArray.Skip(1).ToArray(), subVerbsAttribute.Types);
                }

                break;
            }

            return parser.ParseArguments(argsArray, types);
        }

        public static ParserResult<object> ParseVerbs<T1, T2>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4>(this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5>(this Parser parser,
            IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6>(this Parser parser,
            IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6, T7>(this Parser parser,
            IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6, T7, T8>(this Parser parser,
            IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Parser parser,
            IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Parser parser,
            IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13));
        }

        public static ParserResult<object> ParseVerbs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            this Parser parser, IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14));
        }

        public static ParserResult<object> ParseVerbs
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Parser parser,
                IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15));
        }

        public static ParserResult<object> ParseVerbs
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Parser parser,
                IEnumerable<string> args)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.ParseVerbs(args, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16));
        }
    }
}