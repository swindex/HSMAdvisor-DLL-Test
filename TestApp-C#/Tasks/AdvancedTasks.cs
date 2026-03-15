using System;
using System.Collections.Generic;
using HSMAdvisor;
using HSMAdvisorDatabase.ToolDataBase;

namespace TestApp_C_.Tasks
{
    /// <summary>
    /// Metric vs Imperial calculation comparison
    /// </summary>
    public class MetricImperialComparisonTask : DemoTask
    {
        public MetricImperialComparisonTask()
        {
            Name = "Metric vs Imperial Comparison";
            Description = "Performs the same calculation in both metric and imperial units.";
            Category = "Advanced Features";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("METRIC VS IMPERIAL COMPARISON");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Creating imperial calculation...");
                output += OutputFormatter.FormatApiCall("var calcImperial = new Calculation()");
                var calcImperial = new Calculation();
                output += OutputFormatter.FormatApiCall("calcImperial.SetMetric(false)");
                calcImperial.SetMetric(false);
                output += OutputFormatter.FormatApiCall("calcImperial.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calcImperial.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calcImperial.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calcImperial.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calcImperial.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calcImperial.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calcImperial.Diameter", 0.5);
                calcImperial.Diameter = 0.5; // inches
                output += OutputFormatter.FormatApiProperty("calcImperial.Flute_N", 4);
                calcImperial.Flute_N = 4;
                
                output += OutputFormatter.FormatApiCall("calcImperial.SetMaterial(227)");
                calcImperial.SetMaterial(227);
                
                output += OutputFormatter.FormatApiCall("calcImperial.Calculate(false)");
                calcImperial.Calculate(false);
                
                output += OutputFormatter.FormatStep(2, "Creating metric calculation...");
                output += OutputFormatter.FormatApiCall("var calcMetric = new Calculation()");
                var calcMetric = new Calculation();
                output += OutputFormatter.FormatApiCall("calcMetric.SetMetric(true)");
                calcMetric.SetMetric(true);
                output += OutputFormatter.FormatApiCall("calcMetric.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calcMetric.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calcMetric.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calcMetric.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calcMetric.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calcMetric.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calcMetric.Diameter", 12.7);
                calcMetric.Diameter = 12.7; // mm (0.5 inches)
                output += OutputFormatter.FormatApiProperty("calcMetric.Flute_N", 4);
                calcMetric.Flute_N = 4;
                
                output += OutputFormatter.FormatApiCall("calcMetric.SetMaterial(227)");
                calcMetric.SetMaterial(227);
                
                output += OutputFormatter.FormatApiCall("calcMetric.Calculate(false)");
                calcMetric.Calculate(false);
                
                output += OutputFormatter.FormatSection("IMPERIAL RESULTS");
                output += OutputFormatter.FormatProperty("Diameter", $"{calcImperial.Diameter:F4} inches");
                output += OutputFormatter.FormatProperty("RPM", $"{calcImperial.RPM:F0}");
                output += OutputFormatter.FormatProperty("Feed Rate", $"{calcImperial.FEED:F1} in/min");
                output += OutputFormatter.FormatProperty("DOC", $"{calcImperial.DOC:F4} inches");
                output += OutputFormatter.FormatProperty("WOC", $"{calcImperial.WOC:F4} inches");
                output += OutputFormatter.FormatProperty("Chipload", $"{calcImperial.Real_IPT:F5} in/tooth");
                
                output += OutputFormatter.FormatSection("METRIC RESULTS");
                output += OutputFormatter.FormatProperty("Diameter", $"{calcMetric.Diameter:F2} mm");
                output += OutputFormatter.FormatProperty("RPM", $"{calcMetric.RPM:F0}");
                output += OutputFormatter.FormatProperty("Feed Rate", $"{calcMetric.FEED:F0} mm/min");
                output += OutputFormatter.FormatProperty("DOC", $"{calcMetric.DOC:F3} mm");
                output += OutputFormatter.FormatProperty("WOC", $"{calcMetric.WOC:F3} mm");
                output += OutputFormatter.FormatProperty("Chipload", $"{calcMetric.Real_IPT:F4} mm/tooth");
                
                output += OutputFormatter.FormatSection("VERIFICATION");
                output += OutputFormatter.FormatProperty("RPM Match", calcImperial.RPM == calcMetric.RPM ? "✓ YES" : "✗ NO");
                output += OutputFormatter.FormatProperty("Feed Conversion", $"{calcImperial.FEED * 25.4:F0} mm/min vs {calcMetric.FEED:F0} mm/min");
                
                output += OutputFormatter.FormatSuccess("Both calculations completed!");
                
                return output;
            }
            catch (Exception ex)
            {
                return output + OutputFormatter.FormatError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
    
    /// <summary>
    /// Reset calculation demonstration
    /// </summary>
    public class ResetCalculationTask : DemoTask
    {
        public ResetCalculationTask()
        {
            Name = "Reset Calculation";
            Description = "Demonstrates how to reset different areas of a calculation object.";
            Category = "Advanced Features";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("RESET CALCULATION DEMONSTRATION");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Creating and configuring calculation...");
                output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                var calc = new Calculation();
                output += OutputFormatter.FormatApiCall("calc.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calc.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiProperty("calc.Diameter", 0.5);
                calc.Diameter = 0.5;
                output += OutputFormatter.FormatApiProperty("calc.Flute_N", 4);
                calc.Flute_N = 4;
                output += OutputFormatter.FormatApiProperty("calc.HSM", true);
                calc.HSM = true;
                
                output += OutputFormatter.FormatProperty("Initial Diameter", calc.Diameter);
                output += OutputFormatter.FormatProperty("Initial HSM", calc.HSM);
                
                output += OutputFormatter.FormatStep(2, "Resetting all calculation areas...");
                output += OutputFormatter.FormatApiCall("calc.Reset(Calculation.ResetDataAreas.All)");
                calc.Reset(Calculation.ResetDataAreas.All);
                
                output += OutputFormatter.FormatProperty("After Reset Diameter", calc.Diameter);
                output += OutputFormatter.FormatProperty("After Reset HSM", calc.HSM);
                
                output += OutputFormatter.FormatSuccess("Reset completed!");
                output += OutputFormatter.FormatInfo("Available reset areas:\n" +
                    "  - All: Resets everything\n" +
                    "  - Tool: Resets tool parameters\n" +
                    "  - Cut: Resets cutting parameters\n" +
                    "  - Material: Resets material settings");
                
                return output;
            }
            catch (Exception ex)
            {
                return output + OutputFormatter.FormatError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
