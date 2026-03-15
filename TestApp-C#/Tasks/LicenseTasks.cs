using System;
using System.Collections.Generic;
using HSMAdvisor;

namespace TestApp_C_.Tasks
{
    /// <summary>
    /// Check current license status
    /// </summary>
    public class CheckLicenseStatusTask : DemoTask
    {
        public CheckLicenseStatusTask()
        {
            Name = "Check License Status";
            Description = "Checks if a valid license is present and displays license information.";
            Category = "License Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("LICENSE STATUS CHECK");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Checking license...");
                output += OutputFormatter.FormatApiCall("Core.isLicense()");
                bool hasLicense = Core.isLicense();
                output += OutputFormatter.FormatApiCallWithReturn("Core.isLicense()", hasLicense);
                output += OutputFormatter.FormatProperty("Has Valid License", hasLicense);
                
                if (hasLicense)
                {
                    output += OutputFormatter.FormatSuccess("Valid license found!");
                }
                else
                {
                    output += OutputFormatter.FormatWarning("No valid license found. Trial may be available.");
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
    /// Show detailed license information
    /// </summary>
    public class ShowLicenseLevelTask : DemoTask
    {
        public ShowLicenseLevelTask()
        {
            Name = "Show License Level Details";
            Description = "Displays detailed information about the current license level.";
            Category = "License Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("LICENSE LEVEL DETAILS");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Getting license level...");
                output += OutputFormatter.FormatApiCall("Core.GetLicenseLevel()");
                var licenseLevel = Core.GetLicenseLevel();
                output += OutputFormatter.FormatApiCallWithReturn("Core.GetLicenseLevel()", licenseLevel.ToString());
                output += OutputFormatter.FormatProperty("License Level", licenseLevel.ToString());
                
                bool isTrial = licenseLevel == License_Level_.Trial;
                output += OutputFormatter.FormatProperty("Is Trial", isTrial);
                
                output += OutputFormatter.FormatSection("Available License Levels");
                output += "  - Trial: Limited trial version\n";
                output += "  - Hobby: Hobby/personal use license\n";
                output += "  - Full: Full commercial license\n";
                
                if (isTrial)
                {
                    output += OutputFormatter.FormatInfo("Running in trial mode. Some features may be limited.");
                }
                else
                {
                    output += OutputFormatter.FormatSuccess("Full license active!");
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
    /// Show license activation dialog
    /// </summary>
    public class ShowLicenseDialogTask : DemoTask
    {
        public ShowLicenseDialogTask()
        {
            Name = "Show License Activation Dialog";
            Description = "Opens the HSMAdvisor license activation dialog for entering/managing license keys.";
            Category = "License Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("LICENSE ACTIVATION DIALOG");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Opening license dialog...");
                output += OutputFormatter.FormatInfo("The license dialog will open. Close it to continue.");
                
                output += OutputFormatter.FormatApiCall("Core.ShowLicenseDialog()");
                Core.ShowLicenseDialog();
                
                output += OutputFormatter.FormatStep(2, "Dialog closed. Checking license status...");
                
                output += OutputFormatter.FormatApiCall("Core.isLicense()");
                bool hasLicense = Core.isLicense();
                output += OutputFormatter.FormatApiCall("Core.GetLicenseLevel()");
                var licenseLevel = Core.GetLicenseLevel();
                
                output += OutputFormatter.FormatProperty("Has Valid License", hasLicense);
                output += OutputFormatter.FormatProperty("License Level", licenseLevel.ToString());
                
                if (hasLicense)
                {
                    output += OutputFormatter.FormatSuccess("License is valid!");
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
    /// Validate license for calculation
    /// </summary>
    public class ValidateLicenseForCalculationTask : DemoTask
    {
        public ValidateLicenseForCalculationTask()
        {
            Name = "Validate License for Calculation";
            Description = "Creates a calculation object and validates that the license allows calculations.";
            Category = "License Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("LICENSE VALIDATION FOR CALCULATION");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Checking license status...");
                output += OutputFormatter.FormatApiCall("Core.isLicense()");
                bool hasLicense = Core.isLicense();
                output += OutputFormatter.FormatApiCallWithReturn("Core.isLicense()", hasLicense);
                output += OutputFormatter.FormatProperty("Has Valid License", hasLicense);
                
                if (!hasLicense)
                {
                    output += OutputFormatter.FormatWarning("No valid license. Attempting to show activation dialog...");
                    output += OutputFormatter.FormatApiCall("Core.ShowLicenseDialog()");
                    Core.ShowLicenseDialog();
                    output += OutputFormatter.FormatApiCall("Core.isLicense()");
                    hasLicense = Core.isLicense();
                }
                
                output += OutputFormatter.FormatStep(2, "Creating calculation object...");
                output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                var calc = new Calculation();
                
                output += OutputFormatter.FormatStep(3, "Setting up basic parameters...");
                output += OutputFormatter.FormatApiCall("calc.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calc.SetToolType(HSMAdvisorDatabase.ToolDataBase.Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calc.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calc.SetToolMaterial(HSMAdvisorDatabase.ToolDataBase.Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calc.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calc.SetToolCoating(HSMAdvisorDatabase.ToolDataBase.Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calc.Diameter", 0.5);
                calc.Diameter = 0.5;
                output += OutputFormatter.FormatApiCall("calc.SetMaterial(227)");
                calc.SetMaterial(227);
                
                output += OutputFormatter.FormatStep(4, "Attempting calculation...");
                output += OutputFormatter.FormatApiCall("calc.Calculate(false)");
                bool calculationSuccess = calc.Calculate(false);
                
                if (calculationSuccess)
                {
                    output += OutputFormatter.FormatApiCallWithReturn("calc.Calculate(false)", calculationSuccess);
                    output += OutputFormatter.FormatSuccess("Calculation succeeded! License is valid for calculations.");
                    output += OutputFormatter.FormatApiCallWithReturn("calc.RPM", calc.RPM);
                    output += OutputFormatter.FormatProperty("RPM", calc.RPM);
                    output += OutputFormatter.FormatApiCallWithReturn("calc.FEED", calc.FEED);
                    output += OutputFormatter.FormatProperty("Feed Rate", calc.FEED);
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed. License may not permit calculations.");
                    output += OutputFormatter.FormatApiCall("Core.GetLicenseLevel()");
                    output += OutputFormatter.FormatProperty("Current License Level", Core.GetLicenseLevel());
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
