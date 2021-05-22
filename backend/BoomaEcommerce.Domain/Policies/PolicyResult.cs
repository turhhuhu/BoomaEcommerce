using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Domain.Policies
{
    public struct PolicyResult
    {
        public bool IsOk { get; set; }
        public string PolicyError { get; set; }

        public PolicyResult(bool state)
        {
            IsOk = state;
            PolicyError = "";
        }
        public PolicyResult(bool state, string error)
        {
            IsOk = state;
            PolicyError = error;
        }

        public static PolicyResult Fail() => new(false, "");
        public static PolicyResult Fail(string error) => new(false, error);
        public static PolicyResult Ok() => new(true);

        public static PolicyResult CombineFails(IEnumerable<PolicyResult> failResults, string prefix = "", string infix = "", string suffix = "")
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(prefix);
            stringBuilder.AppendJoin("\n" + infix, failResults.Select(res => res.PolicyError));
            return new PolicyResult(false, stringBuilder.ToString());
        }

    }
}
