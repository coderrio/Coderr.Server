using System;
using System.Linq;
using NSubstitute;
using NSubstitute.Core;

namespace OneTrueError.App.Tests
{
    internal static class NSubstitueExtensions
    {
        public static MethodWrapper Method(this object instance, string methodName)
        {
            var calls = instance.ReceivedCalls().Where(x => x.GetMethodInfo().Name == methodName).ToList();
            if (calls.Count != 1)
                throw new InvalidOperationException("More than one call to '" + methodName +
                                                    "', use overload with invocation index.");

            return new MethodWrapper(instance, calls[0]);
        }
    }

    internal class MethodWrapper
    {
        private readonly ICall _call;
        private readonly object _instance;

        public MethodWrapper(object instance, ICall call)
        {
            _instance = instance;
            _call = call;
        }

        public TArgument Arg<TArgument>(int index)
        {
            if (index < 0 || index >= _call.GetArguments().Length)
                throw new InvalidOperationException("Method '" + _call.GetMethodInfo().Name +
                                                    "' do not have that many arguments.");

            if (_call.GetArguments()[index] is TArgument)
                return (TArgument) _call.GetArguments()[index];

            throw new InvalidOperationException("Argument " + index + " of '" + _call.GetMethodInfo().Name +
                                                "' cannot be converted to '" + typeof(TArgument) + "'.");
        }

        public TArgument Arg<TArgument>()
        {
            var args = _call.GetArguments().Where(x => x is TArgument).ToList();
            if (args.Count != 1)
                throw new InvalidOperationException("More than one argument of type '" + _call.GetMethodInfo().Name +
                                                    "' is of type " + typeof(TArgument));

            return (TArgument) args[0];
        }
    }
}