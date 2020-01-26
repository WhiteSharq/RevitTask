# RevitTask

RevitTask is available as a [Nuget package](https://www.nuget.org/packages/RevitTask).

To be able to use this library in your Revit add-in, your project should also reference RevitAPIUI.dll.

The main purpose of the RevitTask class is to simplify work with `IExternalEventHandler` implementations,
taking advantage of `async`/`await` style.

General use case is obtaining result of an arbitrary function that depends (or even not) on `UIApplication`.

## CAUTION

RevitTask class is not intended to be used from an `IExternalCommand` context, though it may work well.

RevitTask class instances should not be created on non-Revit context threads, UI for example, because by default 
it continues on captured context. Meaning that you have to follow MVVM-pattern or the like and allocate tasks 
inside your ViewModel or Model layer.

Keep in mind that this library doesn't add any degree of parallelism to Revit, only asynchrony and 
a convenient way of returning results of ExternalEvent handlers and dealing with exceptions.

## Example

```
    var task = new RevitTask();

    string mess = string.Empty;
    try
    {
        var i = (int) await task.Run(app => app.ActiveUIDocument
                            .Selection
                            .PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element)
                            .Count);

        mess = i.ToString();
    }
    catch (Exception ex)
    {
        mess = ex.Message;
    }

    Autodesk.Revit.UI.TaskDialog.Show("Deb", mess);
```

For any feedback, please [e-mail sis000@mail.ru](mailto:sis000@mail.ru).

