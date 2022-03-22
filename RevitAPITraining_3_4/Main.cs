using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_3_4
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Pipe> pipes = new FilteredElementCollector(doc)
             .OfClass(typeof(Pipe))
             .Cast<Pipe>()
             .ToList();

            foreach (var pipe in pipes)
            {
                var pipeOutsideDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();
                var pipeInsideDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble();
                pipeOutsideDiameter = UnitUtils.ConvertFromInternalUnits(pipeOutsideDiameter, UnitTypeId.Millimeters);
                pipeInsideDiameter = UnitUtils.ConvertFromInternalUnits(pipeInsideDiameter, UnitTypeId.Millimeters);

                using (Transaction ts = new Transaction(doc, "Set parameters"))
                {
                    ts.Start();
                    Parameter commentParameter = pipe.LookupParameter("Наименование");
                    commentParameter.Set($"Труба {Math.Round(pipeOutsideDiameter, 1)} мм / {Math.Round(pipeInsideDiameter, 1)} мм");
                    ts.Commit();
                }
            }
            TaskDialog.Show("Запись параметра", "Запись параметра выполнена");
            return Result.Succeeded;
        }
    }
}
