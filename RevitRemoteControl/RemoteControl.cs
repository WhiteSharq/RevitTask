using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace RevitRemoteControl
{
    ////[Transaction(TransactionMode.Manual)]
    ////[Regeneration(RegenerationOption.Manual)]
    public class RemoteControl : IExternalApplication
    {
        private bool _started;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Run();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.Idling += ApplicationOnIdling;
            
            return Result.Succeeded;
        }

        private void ApplicationOnIdling(object sender, IdlingEventArgs e)
        {
            if (!_started)
            {
                Run();
            }

            _started = true;
        }

        public void Run()
        {
            var handler = new RequestHandler();

            var listener = new RevitHttpListener(handler);

            Task.Run(() => listener.Start());
        }
    }
}
