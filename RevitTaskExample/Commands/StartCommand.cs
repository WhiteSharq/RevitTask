using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Threading;

namespace RevitTaskExample
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class StartCommand :
        IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var id = Thread.CurrentThread.ManagedThreadId;

            var viewModel = new MainViewModel();

            var mainWindow = new MainWindow(viewModel);

            mainWindow.Show();

            return Result.Succeeded;
        }
    }
}