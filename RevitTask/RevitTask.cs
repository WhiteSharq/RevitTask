using Autodesk.Revit.UI;
using System;
using System.Threading.Tasks;

/// <summary>
/// <see cref="System.Threading.Tasks.Task"/> wrapper
/// for <see cref="Autodesk.Revit.UI.IExternalEventHandler"/>
/// </summary>
/// <typeparam name="TResult"></typeparam>
public class RevitTask<TResult>
{
    private EventHandler _handler;
    private TaskCompletionSource<TResult> _tcs;

    /// <summary>
    /// Sets required <paramref name="func"/> as a body
    /// of <see cref="IExternalEventHandler.Execute(UIApplication)"/>
    /// method and raises related <see cref="Autodesk.Revit.UI.ExternalEvent"/>
    /// </summary>
    /// <param name="func">Any function that depends on
    /// <see cref="Autodesk.Revit.UI.UIApplication"/>
    /// and results in object of <see cref="TResult"/> type.</param>
    /// <param name="continueOnCapturedContext">This parameter
    /// can be used to control the thread, on which
    /// continuation will take place.</param>
    public Task<TResult> Run(
        Func<UIApplication, TResult> func,
        bool continueOnCapturedContext = true)
    {
        _tcs = new TaskCompletionSource<TResult>();

        _tcs.Task.ConfigureAwait(continueOnCapturedContext);

        _handler = new EventHandler(func);

        _handler.EventCompleted += OnEventCompleted;

        var _externalEvent = ExternalEvent.Create(_handler);

        _externalEvent.Raise();

        return _tcs.Task;
    }

    /// <summary>
    /// Sets Task result to object of TResult type or Exception
    /// after <see cref="IExternalEventHandler.Execute(UIApplication)"/>
    /// completes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnEventCompleted(object sender, TResult e)
    {
        if (_handler.Exception == null)
        {
            _tcs.TrySetResult(e);
        }
        else
        {
            _tcs.TrySetException(_handler.Exception);
        }
    }

    private class EventHandler :
        IExternalEventHandler
    {
        private readonly Func<UIApplication, TResult> _func;

        public EventHandler(Func<UIApplication, TResult> func)
        {
            _func = func;
        }

        public event EventHandler<TResult> EventCompleted;

        public Exception Exception { get; private set; }

        public void Execute(UIApplication app)
        {
            TResult result = default;
            try
            {
                result = _func(app);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            EventCompleted?.Invoke(this, result);
        }

        public string GetName()
        {
            return "some func";
        }
    }
}