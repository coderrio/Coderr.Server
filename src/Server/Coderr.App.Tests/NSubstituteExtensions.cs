using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NSubstitute.Core;

namespace codeRR.Server.App.Tests
{
    internal static class NSubstituteExtensions
    {
        public static MethodListWrapper Method(this object instance, string methodName)
        {
            var calls = instance.ReceivedCalls().Where(x => x.GetMethodInfo().Name == methodName).ToList();
            return new MethodListWrapper(instance, calls);
        }

        public static MethodWrapper Method(this object instance, string methodName, int indexer)
        {
            var calls = instance.ReceivedCalls().Where(x => x.GetMethodInfo().Name == methodName).ToList();
            if (calls.Count <= indexer)
                throw new InvalidOperationException("There are only " + calls.Count + " calls available, you specified index " + indexer);

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
            if ((index < 0) || (index >= _call.GetArguments().Length))
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

    internal class MethodListWrapper
    {
        private readonly IList<ICall> _calls;
        private readonly object _instance;

        public MethodListWrapper(object instance, IList<ICall> calls)
        {
            _instance = instance;
            _calls = calls;
        }

        public TArgument Arg<TArgument>(int index)
        {
            List<object> values = new List<object>();
            foreach (var call in _calls)
            {
                if ((index < 0) || (index >= call.GetArguments().Length))
                    continue;

                if (call.GetArguments()[index] is TArgument)
                    values.Add(call.GetArguments()[index]);
            }
            if (values.Count > 1)
                throw new InvalidOperationException("There was multiple calls to "+ _calls.First().GetMethodInfo().Name + " with an argument of type "+ typeof(TArgument) + ". Use method call indexer.");
            if (values.Count == 1)
                return (TArgument) values[0];

            throw new InvalidOperationException("None of the calls to '" + _calls.First().GetMethodInfo().Name + "' had an argument of type " + typeof(TArgument));
        }

        public TArgument Arg<TArgument>()
        {
            List<object> values = new List<object>();
            foreach (var call in _calls)
            {
                var arg = call.GetArguments().FirstOrDefault(x => x.GetType() == typeof(TArgument));
                if (arg != null)
                    values.Add(arg);
            }
            if (values.Count > 1)
                throw new InvalidOperationException("There was multiple calls to " + _calls.First().GetMethodInfo().Name + " with an argument of type " + typeof(TArgument) + ". Use method call indexer.");
            if (values.Count == 1)
                return (TArgument) values[0];

            throw new InvalidOperationException("None of the calls to '" + _calls.First().GetMethodInfo().Name + "' had an argument of type " + typeof(TArgument));
        }
    }
}