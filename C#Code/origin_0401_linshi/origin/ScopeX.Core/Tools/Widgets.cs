using System;
using System.Threading;

namespace ScopeX.Core.Tools
{
    internal static class Widgets
    {
        public delegate Int32 Morpher<TResult, TArgument>(Int32 startValue, TArgument argument, out TResult morphResult);

        public static TResult Morph<TResult, TArgument>(ref Int32 target, TArgument argument, Morpher<TResult, TArgument> morpher)
        {
            TResult morphresult;
            Int32 currentval = target, startval, desiredval;

            do
            {
                startval = currentval;
                desiredval = morpher(startval, argument, out morphresult);
                currentval = Interlocked.CompareExchange(ref target, desiredval, startval);
            } while (startval != currentval);

            return morphresult;
        }

        public static UInt64 InterlockedOr(ref UInt64 target, UInt64 value)
        {
            UInt64 currentval = target, startval, desiredval;

            // Don't access target in the loop except in an attempt
            // to change it because another thread may be touching it
            do
            {
                // Record this iteration's starting value
                startval = currentval;
                // Calculate the desired value in terms of startVal and value
                desiredval = startval | value;
                // NOTE: the thread could be preempted here!
                // if (target == startVal) target = desiredVal
                // Value prior to potential change is returned
                currentval = Interlocked.CompareExchange(ref target, desiredval, startval);
                // If the starting value changed during this iteration, repeat
            } while (startval != currentval);

            // Return the maximum value when this thread tried to set it
            return desiredval;
        }
    }
}
