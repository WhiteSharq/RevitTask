using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitRemoteControl
{
    public class RequestHandler
    {
        private readonly RevitTask _revitTask = new RevitTask();

        public async Task<string[]> HandleAsync(string request)
        {
            string[] result;

            Task<string[]> task;

            if (request == "FILEDIALOG")
            {
                task = _revitTask
                    .Run((app) =>
                    {
                        //// var document = app.Document;

                        var dialog = new FileOpenDialog("Revit Files (*.rvt)|*.rvt");

                        var dialogResult = dialog.Show();

                        var modelPath = dialog.GetSelectedModelPath();

                        var path = ModelPathUtils
                            .ConvertModelPathToUserVisiblePath(modelPath);

                        return new[] { path };
                    });
            }
            else if (request == "VIEWLIST")
            {
                task = _revitTask
                    .Run((uiapp) =>
                    {
                        if (uiapp.ActiveUIDocument?.Document == null)
                        {
                            return new[] {"No opened documents"};
                        }

                        var document = uiapp.ActiveUIDocument.Document;

                        var plans = new FilteredElementCollector(document)
                            .WhereElementIsNotElementType()
                            .OfClass(typeof(View))
                            .Select(x => x.Name)
                            .ToArray();

                        return plans;
                    });
            }
            else
            {
                task = _revitTask
                .Run(uiapp =>
                {
                    //// TaskDialog.Show("Deb", $"Requested: {request}");

                    var command = (PostableCommand)Enum.Parse(
                        typeof(PostableCommand),
                        request,
                        true);

                    var id = RevitCommandId
                        .LookupPostableCommandId(command);

                    uiapp.PostCommand(id);

                    return new[] { $"Successfully posted command {command}" };
                });
            }


            try
            {
                result = await task;
            }
            catch (Exception e)
            {
                result = new[] { $"{e.Message} in {e.StackTrace}" };
            }

            return result;
        }
    }
}
