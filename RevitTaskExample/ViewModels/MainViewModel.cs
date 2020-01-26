using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Input;

namespace RevitTaskExample
{
    public class MainViewModel
    {
        private RevitTask _revitTask = new RevitTask();

        public MainViewModel()
        {
            RunLongRevit = new UiCommand(PlaceInstances);
        }

        public ICommand RunLongRevit { get; set; }

        private FamilyInstance CreateFamilyInstance(
            Document document,
            FamilySymbol fs)
        {
            using (var t = new Transaction(document))
            {
                t.Start(nameof(CreateFamilyInstance));

                var instance = document.Create.NewFamilyInstance(
                    new XYZ(),
                    fs,
                    document.ActiveView);

                t.Commit();

                return instance;
            }
        }

        private async void PlaceInstances()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var instance = (FamilyInstance)await _revitTask.Run((doc) =>
                       CreateFamilyInstance(
                           doc.Application.ActiveUIDocument.Document,
                           null));
                }
            }
            catch(Exception e)
            {
                TaskDialog.Show("Debug", e.Message);
            }
        }
    }
}