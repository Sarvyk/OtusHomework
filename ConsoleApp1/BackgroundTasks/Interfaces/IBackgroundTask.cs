using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.BackgroundTasks.Interfaces
{
    internal interface IBackgroundTask
    {
        Task Start(CancellationToken ct);
    }
}