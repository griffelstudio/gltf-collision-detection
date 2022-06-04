using Microsoft.Extensions.Logging;
using System;

namespace GS.Gltf.Collisions.Demo
{
    internal class DemoLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.Write(formatter(state, exception));
        }
    }
}
