﻿using System;
using System.Threading;

namespace MoonSharp.Interpreter
{
    /// <summary>
    /// This class provides an interface to control execution of Lua scripts ran asynchronously.
    /// 
    /// This class is supported only on .NET 4.x and .NET 4.x PCL targets. 
    /// On other targets, it acts as a dummy.
    /// </summary>
    public class ExecutionControlToken
    {
        public static readonly ExecutionControlToken Dummy = new ExecutionControlToken() { m_IsDummy = true };

        CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

        bool m_IsDummy;

        /// <summary>
        ///  Creates an usable execution control token.
        /// </summary>
        /// <returns>
        public ExecutionControlToken()
        {
            m_IsDummy = false;
        }

        /// <summary>
        ///  Aborts the execution of the script that is associated with this token.
        /// </summary>
        public void Terminate()
        {
            if (!m_IsDummy)
            {
                m_CancellationTokenSource.Cancel(true);
            }
        }

        internal bool IsAbortRequested
        {
            get
            {
                return m_CancellationTokenSource.IsCancellationRequested;
            }
        }

        internal void Wait(TimeSpan timeSpan)
        {
            m_CancellationTokenSource.Token.WaitHandle.WaitOne(timeSpan);
            
        }
    }
}
