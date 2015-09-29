using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrafficReports
{
    public class StructuredString
    {
        public string Template { get; private set; }
        public object[] Arguments { get; private set; }

        public StructuredString(string template, params object[] arguments)
        {
            this.Template = template;
            this.Arguments = arguments;
        }

        public string Format()
        {
            return Format(this.Template, this.Arguments);
        }

        public Dictionary<string, string> GetNamedArguments()
        {
            return GetNamedArguments(this.Template, this.Arguments);
        }

        public override string ToString()
        {
            return Format();
        }

        public static implicit operator StructuredString(string s)
        {
            return new StructuredString(s);
        }

        public static implicit operator string(StructuredString s)
        {
            return s.Format();
        }

        /// <summary>
        /// Formats a string with structured placeholders ("{parameter1} and {parameter2}") 
        /// to a regular index-based template ("{0} and {1}"), then does regular string format on the result.
        /// If a placeholder is repeated multiple times ("{parameter} and {parameter}", it is replaced with the same index ("{0} and {0}").
        /// Throws same exceptions as string.Format when to few arguments are passed.
        /// It is not recursive, so if the value for "{parameter}" is "{parameter2}", the resulting string will contain "{parameter2}"
        /// </summary>
        /// <param name="template"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string Format(string template, params object[] arguments)
        {
            if (string.IsNullOrEmpty(template) || arguments == null || !arguments.Any())
                return template;

            var uniqueParameters = new List<string>();
            var matches = Regex.Matches(template, @"{@?\w+}");
            // Since we are replacing one string with another that will most likely be of different length, 
            // we want to keep the difference so that subsequent replaces are done at the right place in the string
            var delta = 0;
            foreach (Match match in matches)
            {
                if (!uniqueParameters.Contains(match.Value))
                {
                    uniqueParameters.Add(match.Value);
                }
                var index = uniqueParameters.IndexOf(match.Value);
                var newParameter = "{" + index + "}";
                template = template.Substring(0, match.Index - delta) + newParameter + template.Substring(match.Index + match.Length - delta);
                delta += (match.Length - newParameter.Length);
            }
            return string.Format(template, arguments);
        }

        public static Dictionary<string, string> GetNamedArguments(string template, params object[] arguments)
        {
            if (string.IsNullOrEmpty(template) && (arguments == null || !arguments.Any()))
                return arguments == null ? null : new Dictionary<string, string>();

            var namedParameters = new Dictionary<string, string>();
            var matches = Regex.Matches(template, @"{@?\w+}");
            foreach (Match match in matches)
            {
                var key = match.Value;
                key = key.Substring(1, key.Length - 2);
                if (key.StartsWith("@"))
                    key = key.Substring(1, key.Length - 1);
                if (!namedParameters.ContainsKey(key))
                {
                    namedParameters.Add(key, arguments[namedParameters.Count].ToString());
                }
            }
            return namedParameters;
        }
    }
}
