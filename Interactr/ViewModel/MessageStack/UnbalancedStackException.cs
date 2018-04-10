using System;

namespace Interactr.ViewModel.MessageStack
{
    /// <summary>
    /// Exception that is thrown when a stack cannot be evaluated because it is unbalanced.
    /// A message stack is balanced when each invocation message has a return message, 
    /// and the messages are correctly ordered so that the stack frames have proper nesting.
    /// </summary>
    public class UnbalancedStackException : Exception
    {
        public UnbalancedStackException(string msg) : base(msg)
        { }
    }
}
