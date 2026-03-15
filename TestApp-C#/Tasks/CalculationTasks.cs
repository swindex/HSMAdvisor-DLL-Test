using System;
using System.Collections.Generic;
using HSMAdvisor;
using HSMAdvisorDatabase.ToolDataBase;

namespace TestApp_C_.Tasks
{
    /// <summary>
    /// Basic milling calculation
    /// </summary>
    public class BasicMillingCalculationTask : DemoTask
    {
        public BasicMillingCalculationTask()
        {
            Name = "Basic Milling Calculation";
            Description = "Performs a basic endmill calculation with configurable parameters.";
            Category = "Calculation Examples";
            
            Parameters.Add(new TaskParameter
            {
                Name = "Diameter",
                Label = "Tool Diameter (inches)",
                ParameterType = typeof(double),
                DefaultValue = 0.5,
                Description = "Diameter of the endmill"
            });
            
            Parameters.Add(new TaskParameter
            {
                Name = "FluteCount",
                Label = "Number of Flutes",
                ParameterType = typeof(int),
                DefaultValue = 4,
                Description = "Number of flutes on the tool"
            });
            
            Parameters.Add(new TaskParameter
            {
                Name = "DOC",
                Label = "Depth of Cut (inches, 0=auto)",
                ParameterType = typeof(double),
                DefaultValue = 0.0,
                Description = "Axial depth of cut (0 for recommended)"
            });
            
            Parameters.Add(new TaskParameter
            {
                Name = "WOC",
                Label = "Width of Cut (inches, 0=auto)",
                ParameterType = typeof(double),
                DefaultValue = 0.0,
                Description = "Radial width of cut (0 for recommended)"
            });
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("BASIC MILLING CALCULATION");
            
            try
            {
                double diameter = GetParameter<double>(parameterValues, "Diameter", 0.5);
                int fluteCount = GetParameter<int>(parameterValues, "FluteCount", 4);
                double doc = GetParameter<double>(parameterValues, "DOC", 0.0);
                double woc = GetParameter<double>(parameterValues, "WOC", 0.0);
                
                output += OutputFormatter.FormatStep(1, "Creating calculation object...");
                output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                var calc = new Calculation();
                
                output += OutputFormatter.FormatStep(2, "Setting tool parameters...");
                output += OutputFormatter.FormatApiCall("calc.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calc.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calc.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calc.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calc.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calc.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calc.Diameter", diameter);
                calc.Diameter = diameter;
                output += OutputFormatter.FormatApiProperty("calc.Flute_N", fluteCount);
                calc.Flute_N = fluteCount;
                
                output += OutputFormatter.FormatProperty("Tool Type", "Solid Endmill");
                output += OutputFormatter.FormatProperty("Tool Material", "Carbide");
                output += OutputFormatter.FormatProperty("Coating", "TiAlN");
                output += OutputFormatter.FormatProperty("Diameter", diameter + " inches");
                output += OutputFormatter.FormatProperty("Flutes", fluteCount);
                
                output += OutputFormatter.FormatStep(3, "Setting material (ID 227 - license-free demo material)...");
                output += OutputFormatter.FormatApiCall("calc.SetMaterial(227)");
                calc.SetMaterial(227);
                output += OutputFormatter.FormatApiCallWithReturn("calc.MaterialName", calc.MaterialName);
                output += OutputFormatter.FormatProperty("Material", calc.MaterialName);
                
                output += OutputFormatter.FormatStep(4, "Setting machining parameters...");
                output += OutputFormatter.FormatApiProperty("calc.Chip_Thinning", true);
                calc.Chip_Thinning = true;
                output += OutputFormatter.FormatApiProperty("calc.HSM", true);
                calc.HSM = true;
                
                // Set DOC/WOC if provided
                if (doc > 0)
                {
                    output += OutputFormatter.FormatApiProperty("calc.DOC", doc);
                    calc.DOC = doc;
                    output += OutputFormatter.FormatProperty("DOC (User Specified)", doc + " inches");
                }
                
                if (woc > 0)
                {
                    output += OutputFormatter.FormatApiProperty("calc.WOC", woc);
                    calc.WOC = woc;
                    output += OutputFormatter.FormatProperty("WOC (User Specified)", woc + " inches");
                }
                
                output += OutputFormatter.FormatStep(5, "Performing calculation...");
                output += OutputFormatter.FormatApiCall("calc.Calculate(false)");
                bool success = calc.Calculate(false);
                
                if (success)
                {
                    output += OutputFormatter.FormatApiCallWithReturn("calc.Calculate(false)", success);
                    output += OutputFormatter.FormatSuccess("Calculation completed successfully!");
                    output += OutputFormatter.FormatSection("Results");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.RPM", $"{calc.RPM:F0}");
                    output += OutputFormatter.FormatProperty("RPM", $"{calc.RPM:F0}");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.FEED", $"{calc.FEED:F1}");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{calc.FEED:F1} in/min");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.DOC", $"{calc.DOC:F4}");
                    output += OutputFormatter.FormatProperty("DOC (Recommended)", $"{calc.DOC:F4} inches");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.WOC", $"{calc.WOC:F4}");
                    output += OutputFormatter.FormatProperty("WOC (Recommended)", $"{calc.WOC:F4} inches");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.IPT", $"{calc.IPT:F5}");
                    output += OutputFormatter.FormatProperty("Chipload", $"{calc.IPT:F5} in/tooth");
                    
                    // Show chip thickness when chip thinning is active and value differs
                    if (calc.Chip_Thinning && Math.Abs(calc.Chip_Thickness - calc.IPT) > 0.00001)
                    {
                        output += OutputFormatter.FormatApiCallWithReturn("calc.Chip_Thickness", $"{calc.Chip_Thickness:F5}");
                        output += OutputFormatter.FormatProperty("Chip Thickness", $"{calc.Chip_Thickness:F5} in/tooth");
                    }
                    
                    output += OutputFormatter.FormatApiCallWithReturn("calc.SFM", $"{calc.SFM:F0}");
                    output += OutputFormatter.FormatProperty("Surface Speed", $"{calc.SFM:F0} SFM");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.Gages.MRR", $"{calc.Gages.MRR:F3}");
                    output += OutputFormatter.FormatProperty("MRR", $"{calc.Gages.MRR:F3} in³/min");
                    
                    if (calc.Chip_Thinning)
                    {
                        output += OutputFormatter.FormatInfo("Chip thinning compensation is enabled (HSM mode)");
                    }
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed. Check license and parameters.");
                }
                
                return output;
            }
            catch (Exception ex)
            {
                return output + OutputFormatter.FormatError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// Drilling calculation
    /// </summary>
    public class DrillingCalculationTask : DemoTask
    {
        public DrillingCalculationTask()
        {
            Name = "Drilling Calculation";
            Description = "Performs a drilling operation calculation.";
            Category = "Calculation Examples";
            
            Parameters.Add(new TaskParameter
            {
                Name = "Diameter",
                Label = "Drill Diameter (inches)",
                ParameterType = typeof(double),
                DefaultValue = 0.375,
                Description = "Diameter of the drill"
            });
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("DRILLING CALCULATION");
            
            try
            {
                double diameter = GetParameter<double>(parameterValues, "Diameter", 0.375);
                
                output += OutputFormatter.FormatStep(1, "Creating calculation object...");
                output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                var calc = new Calculation();
                
                output += OutputFormatter.FormatStep(2, "Setting drill parameters...");
                output += OutputFormatter.FormatApiCall("calc.SetToolType(Enums.ToolTypes.JobberTwistDrill)");
                calc.SetToolType(Enums.ToolTypes.JobberTwistDrill);
                output += OutputFormatter.FormatApiCall("calc.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calc.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calc.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calc.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calc.Diameter", diameter);
                calc.Diameter = diameter;
                
                output += OutputFormatter.FormatProperty("Tool Type", "Drill");
                output += OutputFormatter.FormatProperty("Diameter", diameter + " inches");
                
                output += OutputFormatter.FormatStep(3, "Setting material (ID 227 - license-free demo material)...");
                output += OutputFormatter.FormatApiCall("calc.SetMaterial(227)");
                calc.SetMaterial(227);
                output += OutputFormatter.FormatApiCallWithReturn("calc.MaterialName", calc.MaterialName);
                output += OutputFormatter.FormatProperty("Material", calc.MaterialName);
                
                output += OutputFormatter.FormatStep(4, "Performing calculation...");
                output += OutputFormatter.FormatApiCall("calc.Calculate(false)");
                bool success = calc.Calculate(false);
                
                if (success)
                {
                    output += OutputFormatter.FormatApiCallWithReturn("calc.Calculate(false)", success);
                    output += OutputFormatter.FormatSuccess("Calculation completed successfully!");
                    output += OutputFormatter.FormatSection("Results");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.RPM", $"{calc.RPM:F0}");
                    output += OutputFormatter.FormatProperty("RPM", $"{calc.RPM:F0}");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.FEED", $"{calc.FEED:F1}");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{calc.FEED:F1} in/min");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.IPT", $"{calc.IPT:F5}");
                    output += OutputFormatter.FormatProperty("Chipload", $"{calc.IPT:F5} in/rev");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.SFM", $"{calc.SFM:F0}");
                    output += OutputFormatter.FormatProperty("Surface Speed", $"{calc.SFM:F0} SFM");
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed.");
                }
                
                return output;
            }
            catch (Exception ex)
            {
                return output + OutputFormatter.FormatError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// Show full HSMAdvisor dialog
    /// </summary>
    public class ShowHSMAdvisorDialogTask : DemoTask
    {
        public ShowHSMAdvisorDialogTask()
        {
            Name = "Show Full HSMAdvisor Dialog";
            Description = "Opens the complete HSMAdvisor calculation dialog with all features.";
            Category = "Calculation Examples";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("FULL HSMADVISOR DIALOG");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Creating initial calculation...");
                output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                var calc = new Calculation();
                
                output += OutputFormatter.FormatApiCall("calc.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calc.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calc.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calc.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calc.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calc.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calc.Diameter", 0.5);
                calc.Diameter = 0.5;
                output += OutputFormatter.FormatApiProperty("calc.Flute_N", 4);
                calc.Flute_N = 4;
                
                output += OutputFormatter.FormatApiCall("calc.SetMaterial(227)");
                calc.SetMaterial(227);
                
                output += OutputFormatter.FormatStep(2, "Opening HSMAdvisor dialog...");
                output += OutputFormatter.FormatInfo("The full HSMAdvisor window will open. Click OK when done.");
                
                output += OutputFormatter.FormatApiCall("Core.ShowHSMAdvisorDialog(calc, true, \"Configure your calculation and click OK\")");
                var resultCalc = Core.ShowHSMAdvisorDialog(calc, true, "Configure your calculation and click OK");
                
                output += OutputFormatter.FormatStep(3, "Dialog closed.");
                
                if (resultCalc != null)
                {
                    output += OutputFormatter.FormatSuccess("Calculation returned from dialog!");
                    output += OutputFormatter.FormatSection("Final Results");
                    output += OutputFormatter.FormatProperty("Tool Type", resultCalc.Tool_Type);
                    output += OutputFormatter.FormatProperty("Diameter", $"{resultCalc.Diameter:F4} inches");
                    output += OutputFormatter.FormatProperty("Material", resultCalc.MaterialName);
                    output += OutputFormatter.FormatProperty("RPM", $"{resultCalc.RPM:F0}");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{resultCalc.FEED:F1} in/min");
                    output += OutputFormatter.FormatProperty("DOC", $"{resultCalc.DOC:F4} inches");
                    output += OutputFormatter.FormatProperty("WOC", $"{resultCalc.WOC:F4} inches");
                }
                else
                {
                    output += OutputFormatter.FormatInfo("Dialog was cancelled.");
                }
                
                return output;
            }
            catch (Exception ex)
            {
                return output + OutputFormatter.FormatError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// HSM machining calculation
    /// </summary>
    public class HSMMachiningCalculationTask : DemoTask
    {
        public HSMMachiningCalculationTask()
        {
            Name = "HSM Machining Calculation";
            Description = "Demonstrates high-speed machining calculation with chip thinning.";
            Category = "Calculation Examples";
            
            Parameters.Add(new TaskParameter
            {
                Name = "Diameter",
                Label = "Tool Diameter (inches)",
                ParameterType = typeof(double),
                DefaultValue = 0.25,
                Description = "Diameter of the endmill"
            });
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("HSM MACHINING CALCULATION");
            
            try
            {
                double diameter = GetParameter<double>(parameterValues, "Diameter", 0.25);
                
                output += OutputFormatter.FormatStep(1, "Creating HSM calculation...");
                output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                var calc = new Calculation();
                
                output += OutputFormatter.FormatStep(2, "Configuring for HSM...");
                output += OutputFormatter.FormatApiCall("calc.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calc.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calc.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calc.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calc.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calc.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calc.Diameter", diameter);
                calc.Diameter = diameter;
                output += OutputFormatter.FormatApiProperty("calc.Flute_N", 4);
                calc.Flute_N = 4;
                
                // HSM settings
                output += OutputFormatter.FormatApiProperty("calc.HSM", true);
                calc.HSM = true;
                output += OutputFormatter.FormatApiProperty("calc.Chip_Thinning", true);
                calc.Chip_Thinning = true;
                
                // Small WOC for HSM
                double wocValue = diameter * 0.1;
                output += OutputFormatter.FormatApiProperty("calc.WOC", $"{wocValue:F4} (10% radial engagement)");
                calc.WOC = wocValue; // 10% radial engagement
                
                output += OutputFormatter.FormatProperty("HSM Mode", "Enabled");
                output += OutputFormatter.FormatProperty("Chip Thinning", "Enabled");
                output += OutputFormatter.FormatProperty("WOC (10% Radial)", $"{calc.WOC:F4} inches");
                
                output += OutputFormatter.FormatStep(3, "Setting material (ID 227 - license-free demo material)...");
                output += OutputFormatter.FormatApiCall("calc.SetMaterial(227)");
                calc.SetMaterial(227);
                output += OutputFormatter.FormatApiCallWithReturn("calc.MaterialName", calc.MaterialName);
                output += OutputFormatter.FormatProperty("Material", calc.MaterialName);
                
                output += OutputFormatter.FormatStep(4, "Calculating...");
                output += OutputFormatter.FormatApiCall("calc.Calculate(false)");
                bool success = calc.Calculate(false);
                
                if (success)
                {
                    output += OutputFormatter.FormatApiCallWithReturn("calc.Calculate(false)", success);
                    output += OutputFormatter.FormatSuccess("HSM calculation completed!");
                    output += OutputFormatter.FormatSection("HSM Results");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.RPM", $"{calc.RPM:F0}");
                    output += OutputFormatter.FormatProperty("RPM", $"{calc.RPM:F0}");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.FEED", $"{calc.FEED:F1}");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{calc.FEED:F1} in/min");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.DOC", $"{calc.DOC:F4}");
                    output += OutputFormatter.FormatProperty("DOC", $"{calc.DOC:F4} inches");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.WOC", $"{calc.WOC:F4}");
                    output += OutputFormatter.FormatProperty("WOC", $"{calc.WOC:F4} inches");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.IPT", $"{calc.IPT:F5}");
                    output += OutputFormatter.FormatProperty("Chipload", $"{calc.IPT:F5} in/tooth");
                    
                    // Show chip thickness when chip thinning is active and value differs
                    if (calc.Chip_Thinning && Math.Abs(calc.Chip_Thickness - calc.IPT) > 0.00001)
                    {
                        output += OutputFormatter.FormatApiCallWithReturn("calc.Chip_Thickness", $"{calc.Chip_Thickness:F5}");
                        output += OutputFormatter.FormatProperty("Chip Thickness", $"{calc.Chip_Thickness:F5} in/tooth");
                    }
                    
                    output += OutputFormatter.FormatApiCallWithReturn("calc.Gages.MRR", $"{calc.Gages.MRR:F3}");
                    output += OutputFormatter.FormatProperty("MRR", $"{calc.Gages.MRR:F3} in³/min");
                    
                    output += OutputFormatter.FormatInfo("Note: Chip thinning compensation allows higher feed rates at low radial engagement.");
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed.");
                }
                
                return output;
            }
            catch (Exception ex)
            {
                return output + OutputFormatter.FormatError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// Slotting calculation
    /// </summary>
    public class SlottingCalculationTask : DemoTask
    {
        public SlottingCalculationTask()
        {
            Name = "Slotting Calculation";
            Description = "Calculates parameters for full-width slotting operations.";
            Category = "Calculation Examples";
            
            Parameters.Add(new TaskParameter
            {
                Name = "Diameter",
                Label = "Tool Diameter (inches)",
                ParameterType = typeof(double),
                DefaultValue = 0.375,
                Description = "Diameter of the endmill"
            });
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("SLOTTING CALCULATION");
            
            try
            {
                double diameter = GetParameter<double>(parameterValues, "Diameter", 0.375);
                
                output += OutputFormatter.FormatStep(1, "Creating slotting calculation...");
                output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                var calc = new Calculation();
                
                output += OutputFormatter.FormatStep(2, "Setting slotting parameters...");
                output += OutputFormatter.FormatApiCall("calc.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calc.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calc.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calc.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calc.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calc.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calc.Diameter", diameter);
                calc.Diameter = diameter;
                output += OutputFormatter.FormatApiProperty("calc.Flute_N", 3);
                calc.Flute_N = 3; // Fewer flutes better for chip evacuation in slotting
                
                // Force slotting - WOC = diameter
                output += OutputFormatter.FormatApiProperty("calc.WOC", diameter);
                calc.WOC = diameter;
                output += OutputFormatter.FormatApiProperty("calc.Slotting", true);
                calc.Slotting = true;
                
                output += OutputFormatter.FormatProperty("Operation", "Slotting (Full Width)");
                output += OutputFormatter.FormatProperty("WOC", $"{calc.WOC:F4} inches (100% engagement)");
                output += OutputFormatter.FormatProperty("Flutes", "3 (optimized for chip evacuation)");
                
                output += OutputFormatter.FormatStep(3, "Setting material (ID 227 - license-free demo material)...");
                output += OutputFormatter.FormatApiCall("calc.SetMaterial(227)");
                calc.SetMaterial(227);
                output += OutputFormatter.FormatApiCallWithReturn("calc.MaterialName", calc.MaterialName);
                output += OutputFormatter.FormatProperty("Material", calc.MaterialName);
                
                output += OutputFormatter.FormatStep(4, "Calculating...");
                output += OutputFormatter.FormatApiCall("calc.Calculate(false)");
                bool success = calc.Calculate(false);
                
                if (success)
                {
                    output += OutputFormatter.FormatApiCallWithReturn("calc.Calculate(false)", success);
                    output += OutputFormatter.FormatSuccess("Slotting calculation completed!");
                    output += OutputFormatter.FormatSection("Slotting Results");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.RPM", $"{calc.RPM:F0}");
                    output += OutputFormatter.FormatProperty("RPM", $"{calc.RPM:F0}");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.FEED", $"{calc.FEED:F1}");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{calc.FEED:F1} in/min");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.DOC", $"{calc.DOC:F4}");
                    output += OutputFormatter.FormatProperty("DOC", $"{calc.DOC:F4} inches");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.IPT", $"{calc.IPT:F5}");
                    output += OutputFormatter.FormatProperty("Chipload", $"{calc.IPT:F5} in/tooth");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.Gages.MRR", $"{calc.Gages.MRR:F3}");
                    output += OutputFormatter.FormatProperty("MRR", $"{calc.Gages.MRR:F3} in³/min");
                    
                    output += OutputFormatter.FormatWarning("Note: Slotting puts maximum load on the tool. Reduce DOC for difficult materials.");
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed.");
                }
                
                return output;
            }
            catch (Exception ex)
            {
                return output + OutputFormatter.FormatError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
