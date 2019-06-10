using System.Web.Http.Routing.Constraints;

namespace HandlerTemplates.Constraints
{
    /// <summary>
    /// Simple constraint based on a regex; using a class like this instead
    /// of putting the regex expression into the route attribute makes it
    /// easier to change the expression later in one place and have it active
    /// everywhere.
    /// </summary>
    public class RegexBasedConstraintTemplate : RegexRouteConstraint
    {
        public const string DefaultConstraintName = "myRegex";

        public const string _RegExExpression = "^[a-zA-Z0-9]*$";
        public RegexBasedConstraintTemplate() : base(_RegExExpression)
        { }
    }
}