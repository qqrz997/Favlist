using System;
using System.Threading.Tasks;
using IPA.Utilities.Async;

namespace Favlist.Common;

internal static class CallbackAfterDelay
{
    public static Task Start(int milliseconds, Action callback) =>
        Task.Run(async () =>
        {
            await Task.Delay(milliseconds);
            callback();
        });

    public static Task StartUnitySafe(int milliseconds, Action callback) =>
        UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
        {
            await Task.Delay(milliseconds);
            callback();
        });
}