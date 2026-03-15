using System;
using System.Collections.Generic;
using HSMAdvisor;

namespace TestApp_C_.Tasks
{
    /// <summary>
    /// List all available materials
    /// </summary>
    public class ListMaterialsTask : DemoTask
    {
        public ListMaterialsTask()
        {
            Name = "List All Materials";
            Description = "Retrieves and displays all materials from the material database.";
            Category = "Material Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("MATERIALS LIST");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Retrieving materials list...");
                output += OutputFormatter.FormatApiCall("Core.GetMaterialsList()");
                var materials = Core.GetMaterialsList();
                
                if (materials == null || materials.Count == 0)
                {
                    output += OutputFormatter.FormatWarning("No materials found in database.");
                    return output;
                }
                
                output += OutputFormatter.FormatSuccess($"Found {materials.Count} material(s)");
                output += OutputFormatter.FormatSection("Material Details");
                
                int count = 1;
                foreach (var material in materials)
                {
                    output += $"  [{count}] ID: {material.id} | Name: {material.name}\n";
                    count++;
                    
                    if (count > 20)
                    {
                        output += $"  ... and {materials.Count - 20} more materials\n";
                        break;
                    }
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
    /// Show material selection dialog
    /// </summary>
    public class ShowMaterialSelectionDialogTask : DemoTask
    {
        public ShowMaterialSelectionDialogTask()
        {
            Name = "Show Material Selection Dialog";
            Description = "Opens the HSMAdvisor material selection dialog.";
            Category = "Material Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("MATERIAL SELECTION DIALOG");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Opening material selection dialog...");
                output += OutputFormatter.FormatInfo("The dialog will open. Select a material and close the dialog.");
                
                output += OutputFormatter.FormatApiCall("Core.ShowSelectMaterialDialog()");
                var material = Core.ShowSelectMaterialDialog();
                
                output += OutputFormatter.FormatStep(2, "Dialog closed.");
                
                if (material != null)
                {
                    output += OutputFormatter.FormatSuccess("Material selected!");
                    output += OutputFormatter.FormatSection("Selected Material Details");
                    output += OutputFormatter.FormatObject(material);
                }
                else
                {
                    output += OutputFormatter.FormatInfo("No material was selected.");
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
    /// Get recent materials - simplified version without search
    /// </summary>
    public class GetRecentMaterialsTask : DemoTask
    {
        public GetRecentMaterialsTask()
        {
            Name = "Get Recent Materials";
            Description = "Retrieves the list of recently used materials.";
            Category = "Material Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("RECENT MATERIALS");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Retrieving recent materials...");
                output += OutputFormatter.FormatApiCall("Core.GetRecentMaterialsTable()");
                var recentMaterials = Core.GetRecentMaterialsTable();
                
                if (recentMaterials == null || recentMaterials.Count == 0)
                {
                    output += OutputFormatter.FormatWarning("No recent materials found.");
                    output += OutputFormatter.FormatInfo("Use material selection dialog or calculations to populate recent materials.");
                    return output;
                }
                
                output += OutputFormatter.FormatSuccess($"Found {recentMaterials.Count} recent material(s)");
                output += OutputFormatter.FormatSection("Recent Materials");
                
                // Get full material list to lookup names
                output += OutputFormatter.FormatApiCall("Core.GetMaterialsList()");
                var allMaterials = Core.GetMaterialsList();
                
                int count = 1;
                foreach (var recentMat in recentMaterials)
                {
                    // Find material name
                    string materialName = "Unknown";
                    foreach (var mat in allMaterials)
                    {
                        if (mat.id == recentMat.Id)
                        {
                            materialName = mat.name;
                            break;
                        }
                    }
                    
                    output += $"  [{count}] ID: {recentMat.Id} | Name: {materialName}\n";
                    count++;
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
    /// Assign material to calculation using dialog
    /// </summary>
    public class AssignMaterialToCalculationTask : DemoTask
    {
        public AssignMaterialToCalculationTask()
        {
            Name = "Assign Material to Calculation";
            Description = "Creates a calculation and assigns a material to it using the selection dialog.";
            Category = "Material Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("ASSIGN MATERIAL TO CALCULATION");
            
            try
            {
                
                output += OutputFormatter.FormatStep(1, "Creating calculation...");
                output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                var calc = new Calculation();
                
                output += OutputFormatter.FormatStep(2, "Assigning material to calculation (ID 227 - license-free demo material)...");
                output += OutputFormatter.FormatApiCall("calc.SetMaterial(227)");
                calc.SetMaterial(227);
                
                output += OutputFormatter.FormatSuccess("Material assigned to calculation!");
                output += OutputFormatter.FormatProperty("Material ID", 227);
                output += OutputFormatter.FormatApiCallWithReturn("calc.MaterialName", calc.MaterialName);
                output += OutputFormatter.FormatProperty("Material Name", calc.MaterialName);
                
                return output;
            }
            catch (Exception ex)
            {
                return output + OutputFormatter.FormatError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
