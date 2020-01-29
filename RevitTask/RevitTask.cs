using Autodesk.Revit.UI;
using System;
using System.Threading.Tasks;

/// <summary>
/// <see cref="System.Threading.Tasks.Task"/> wrapper
/// for <see cref="Autodesk.Revit.UI.IExternalEventHandler"/>
/// </summary>
/// <typeparam name="TResult"></typeparam>
public class RevitTask
{
    private EventHandler _handler;
    private TaskCompletionSource<object> _tcs;
    private ExternalEvent _externalEvent;

    /// <summary>
    /// .ctor
    /// </summary>
    public RevitTask()
    {
        _handler = new EventHandler();

        _handler.EventCompleted += OnEventCompleted;

        _externalEvent = ExternalEvent.Create(_handler);
    }

    /// <summary>
    /// Sets required <paramref name="func"/> as a body
    /// of <see cref="IExternalEventHandler.Execute(UIApplication)"/>
    /// method and raises related <see cref="Autodesk.Revit.UI.ExternalEvent"/>
    /// </summary>
    /// <param name="func">Any function that depends on
    /// <see cref="Autodesk.Revit.UI.UIApplication"/>
    /// and results in object of <see cref="TResult"/> type.</param>
    public Task<object> Run(Func<UIApplication, object> func)
    {
        _tcs = new TaskCompletionSource<object>();

        _handler.Func = func;

        _externalEvent.Raise();

        return _tcs.Task;
    }

    /// <summary>
    /// Sets required <paramref name="func"/> as a body
    /// of <see cref="IExternalEventHandler.Execute(UIApplication)"/>
    /// method and raises related <see cref="Autodesk.Revit.UI.ExternalEvent"/>
    /// </summary>
    /// <param name="func">Any function that depends on
    /// <see cref="Autodesk.Revit.UI.UIDocument"/>
    /// and results in object of <see cref="TResult"/> type.</param>
    public Task<object> Run(Func<UIDocument, object> func)
    {
        _tcs = new TaskCompletionSource<object>();

        _handler.Func = (app) => func(app.ActiveUIDocument);

        _externalEvent.Raise();

        return _tcs.Task;
    }

    /// <summary>
    /// Sets Task result to object of TResult type or Exception
    /// after <see cref="IExternalEventHandler.Execute(UIApplication)"/>
    /// completes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="result"></param>
    private void OnEventCompleted(object sender, object result)
    {
        if (_handler.Exception == null)
        {
            _tcs.TrySetResult(result);
        }
        else
        {
            _tcs.TrySetException(_handler.Exception);
        }
    }

    private class EventHandler :
        IExternalEventHandler
    {
        private Func<UIApplication, object> _func;

        public event EventHandler<object> EventCompleted;

        public Exception Exception { get; private set; }

        public Func<UIApplication, object> Func
        {
            get => _func;
            set => _func = value ??
                throw new ArgumentNullException();
        }

        public void Execute(UIApplication app)
        {
      //object result = default; // Error CS8107  Feature 'default literal' is not available in C# 7.0. Please use language version 7.1 or greater.	
      object result = null; // jeremy used null to fix Error CS8107

            try
            {
                result = Func(app);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            EventCompleted?.Invoke(this, result);
        }

        public string GetName()
        {
            return "RevitTask";
        }
    }
}