To be able to use this library in your Revit addin project, it should have a reference to `RevitAPIUI.dll`.

The main purpose of RevitTask class is to simplify work with `IExternalEventHandler` implementations
taking advantage of `async`/`await` style.

General use case is obtaning result of an arbitrary function that depends (or even not) on `UIApplication`.

CAUTION: RevitTask class is not intended to be used from `IExternalCommand` context, though it may work well.

The `RevitTask` class instance should not be created on non-Revit context threads, UI for example, because 
by default it continues on captured context. Meaning that you have to follow MVVM-pattern and allocate tasks 
inside your ViewModel or Model layer.

Keep in mind that this library doesn't add any degree of parallelism to Revit, only asynchrony.

Example:

    var task = new RevitTask<int>();

    string mess = string.Empty;
    try
    {
      var i = await task.Run(app =>
      {
        return app.ActiveUIDocument
          .Selection
          .PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element)
          .Count;
      });

      mess = i.ToString();
    }
    catch (Exception ex)
    {
      mess = ex.Message;
    }

    Autodesk.Revit.UI.TaskDialog.Show("Deb", mess);

For any feedback, please e-mail me at [sis000@mail.ru](mailto:sis000@mail.ru).
