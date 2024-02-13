using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("Name={Name}, PCount={PCount}")]
    public class AbiMethod
    {
        /// <summary>
        /// Method name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parameters count
        /// </summary>
        public int PCount { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="pCount">Parameters count</param>
        public AbiMethod(string name, int pCount)
        {
            Name = name;
            PCount = pCount;
        }

        /// <summary>
        /// Create from expression
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>AbiMethod</returns>
        public static AbiMethod? FromExpression(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                if (memberExpression.Member is PropertyInfo pInfo)
                {
                    if (pInfo.CanRead)
                    {
                        if (pInfo.CanWrite)
                        {
                            // TODO if it's a property we should return both methods?
                        }

                        // propertyOnlyRead

                        var display = pInfo.GetGetMethod()?.GetCustomAttribute<DisplayNameAttribute>();
                        var name = display is not null ? display.DisplayName : memberExpression.Member.Name;

                        return new AbiMethod(name, 0);
                    }
                }
            }
            else if (expression is MethodCallExpression methodExpression)
            {
                if (methodExpression.Method is MethodInfo mInfo)
                {
                    var display = mInfo.GetCustomAttribute<DisplayNameAttribute>();
                    var name = display is not null ? display.DisplayName : mInfo.Name;

                    return new AbiMethod(name, mInfo.GetParameters().Length);
                }
            }

            return null;
        }
    }
}
