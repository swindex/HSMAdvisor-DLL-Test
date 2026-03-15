using System;
using System.Collections.Generic;
using HSMAdvisor;
using HSMAdvisorDatabase.ToolDataBase;
using HSMAdvisorDatabase.MachineDataBase;

namespace TestApp_C_.Tasks
{
    /// <summary>
    /// List all available machines
    /// </summary>
    public class ListMachinesTask : DemoTask
    {
        public ListMachinesTask()
        {
            Name = "List All Machines";
            Description = "Retrieves and displays all machines from the machine database.";
            Category = "Machine Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("MACHINE LIST");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Retrieving machine list...");
                output += OutputFormatter.FormatApiCall("Core.GetMachineList()");
                var machines = Core.GetMachineList();
                
                if (machines == null || machines.Count == 0)
                {
                    output += OutputFormatter.FormatWarning("No machines found in database.");
                    return output;
                }
                
                output += OutputFormatter.FormatSuccess($"Found {machines.Count} machine(s)");
                output += OutputFormatter.FormatSection("Machine Details");
                
                int count = 1;
                foreach (var machine in machines)
                {
                    output += $"  [{count}] ID: {machine.GUID} | Name: {machine.Name}\n";
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
    /// Show machine profile dialog
    /// </summary>
    public class ShowMachineProfileDialogTask : DemoTask
    {
        public ShowMachineProfileDialogTask()
        {
            Name = "Show Machine Profile Dialog";
            Description = "Opens the HSMAdvisor machine profile editor dialog.";
            Category = "Machine Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("MACHINE PROFILE DIALOG");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Opening machine profile dialog...");
                output += OutputFormatter.FormatInfo("The dialog will open. You can edit or create machines.");
                
                output += OutputFormatter.FormatApiCall("Core.ShowEditMachineDatabaseDialog()");
                string selectedMachineId = Core.ShowEditMachineDatabaseDialog();
                
                output += OutputFormatter.FormatStep(2, "Dialog closed.");
                
                if (!string.IsNullOrEmpty(selectedMachineId))
                {
                    output += OutputFormatter.FormatSuccess($"Machine selected/created: {selectedMachineId}");
                }
                else
                {
                    output += OutputFormatter.FormatInfo("No machine was selected.");
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
    /// Create a new machine programmatically
    /// </summary>
    public class CreateMachineProgrammaticallyTask : DemoTask
    {
        public CreateMachineProgrammaticallyTask()
        {
            Name = "Create New Machine Programmatically";
            Description = "Creates a new machine in the database using code (no dialog).";
            Category = "Machine Operations";
            
            Parameters.Add(new TaskParameter
            {
                Name = "MachineName",
                Label = "Machine Name",
                ParameterType = typeof(string),
                DefaultValue = "Demo Machine " + DateTime.Now.ToString("HHmmss"),
                Description = "Name for the new machine"
            });
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("CREATE MACHINE PROGRAMMATICALLY");
            
            try
            {
                string machineName = GetParameter<string>(parameterValues, "MachineName", "Demo Machine");
                
                output += OutputFormatter.FormatStep(1, "Creating new machine object...");
                output += OutputFormatter.FormatApiCall("var machine = new Machine()");
                var machine = new Machine();
                output += OutputFormatter.FormatApiProperty("machine.Name", machineName);
                machine.Name = machineName;
                
                output += OutputFormatter.FormatProperty("Machine Name", machine.Name);
                output += OutputFormatter.FormatApiCallWithReturn("machine.GUID", machine.GUID);
                output += OutputFormatter.FormatProperty("Machine GUID", machine.GUID);
                
                output += OutputFormatter.FormatStep(2, "Inserting machine into database...");
                output += OutputFormatter.FormatApiCall("Core.MachineLibrary.InsertMachine(machine)");
                Core.MachineLibrary.InsertMachine(machine);
                
                output += OutputFormatter.FormatSuccess("Machine created successfully!");
                output += OutputFormatter.FormatInfo("Note: Changes are automatically saved to the database.");
                
                output += OutputFormatter.FormatStep(3, "Verifying machine exists...");
                output += OutputFormatter.FormatApiCall("Core.GetMachineList()");
                var machines = Core.GetMachineList();
                bool found = false;
                
                foreach (var m in machines)
                {
                    if (m.GUID == machine.GUID)
                    {
                        found = true;
                        output += OutputFormatter.FormatSuccess($"Machine verified in database: {m.Name}");
                        break;
                    }
                }
                
                if (!found)
                {
                    output += OutputFormatter.FormatWarning("Machine not found in verification check.");
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
    /// Assign machine to calculation
    /// </summary>
    public class AssignMachineToCalculationTask : DemoTask
    {
        public AssignMachineToCalculationTask()
        {
            Name = "Assign Machine to Calculation";
            Description = "Creates a calculation and assigns a machine to it.";
            Category = "Machine Operations";
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("ASSIGN MACHINE TO CALCULATION");
            
            try
            {
                output += OutputFormatter.FormatStep(1, "Getting available machines...");
                output += OutputFormatter.FormatApiCall("Core.GetMachineList()");
                var machines = Core.GetMachineList();
                
                if (machines == null || machines.Count == 0)
                {
                    output += OutputFormatter.FormatWarning("No machines available. Creating a default machine...");
                    
                    output += OutputFormatter.FormatApiCall("var newMachine = new Machine()");
                    var newMachine = new Machine();
                    output += OutputFormatter.FormatApiProperty("newMachine.Name", "Auto-Created Machine");
                    newMachine.Name = "Auto-Created Machine";
                    output += OutputFormatter.FormatApiCall("Core.MachineLibrary.InsertMachine(newMachine)");
                    Core.MachineLibrary.InsertMachine(newMachine);
                    
                    output += OutputFormatter.FormatSuccess($"Created machine: {newMachine.Name} ({newMachine.GUID})");
                    
                    output += OutputFormatter.FormatStep(2, "Creating calculation...");
                    output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                    var calc = new Calculation();
                    
                    output += OutputFormatter.FormatStep(3, "Assigning machine to calculation...");
                    output += OutputFormatter.FormatApiCall($"calc.SetMachine(\"{newMachine.GUID}\")");
                    calc.SetMachine(newMachine.GUID);
                    
                    output += OutputFormatter.FormatSuccess("Machine assigned to calculation!");
                    output += OutputFormatter.FormatProperty("Machine ID", newMachine.GUID);
                    output += OutputFormatter.FormatProperty("Machine Name", newMachine.Name);
                }
                else
                {
                    output += OutputFormatter.FormatSuccess($"Found {machines.Count} machine(s)");
                    
                    // Use the first machine
                    var machine = machines[0];
                    
                    output += OutputFormatter.FormatStep(2, "Creating calculation...");
                    output += OutputFormatter.FormatApiCall("var calc = new Calculation()");
                    var calc = new Calculation();
                    
                    output += OutputFormatter.FormatStep(3, "Assigning machine to calculation...");
                    output += OutputFormatter.FormatApiCall($"calc.SetMachine(\"{machine.GUID}\")");
                    calc.SetMachine(machine.GUID);
                    
                    output += OutputFormatter.FormatSuccess("Machine assigned to calculation!");
                    output += OutputFormatter.FormatProperty("Machine ID", machine.GUID);
                    output += OutputFormatter.FormatProperty("Machine Name", machine.Name);
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
    /// Machine RPM limit with HSM calculation
    /// </summary>
    public class MachineRPMLimitWithHSMTask : DemoTask
    {
        public MachineRPMLimitWithHSMTask()
        {
            Name = "Machine RPM Limit with HSM + Chip Thinning";
            Description = "Demonstrates how machine RPM limits affect HSM calculations with chip thinning enabled.";
            Category = "Machine Operations";
            
            Parameters.Add(new TaskParameter
            {
                Name = "MaxRPM",
                Label = "Machine Max RPM",
                ParameterType = typeof(int),
                DefaultValue = 15000,
                Description = "Maximum RPM limit for the machine"
            });
            
            Parameters.Add(new TaskParameter
            {
                Name = "ToolDiameter",
                Label = "Tool Diameter (inches)",
                ParameterType = typeof(double),
                DefaultValue = 0.25,
                Description = "Small diameter to generate high RPM"
            });
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("MACHINE RPM LIMIT WITH HSM + CHIP THINNING");
            
            try
            {
                int maxRPM = GetParameter<int>(parameterValues, "MaxRPM", 15000);
                double toolDiameter = GetParameter<double>(parameterValues, "ToolDiameter", 0.25);
                
                output += OutputFormatter.FormatInfo("This test demonstrates the scenario where HSM + Chip Thinning");
                output += OutputFormatter.FormatInfo("produces high SFM that would exceed the machine's RPM limit.");
                output += OutputFormatter.FormatInfo("We'll compare results WITH and WITHOUT machine assignment.\n");
                
                // ===== PART 1: WITHOUT MACHINE (UNCONSTRAINED) =====
                output += OutputFormatter.FormatSection("PART 1: CALCULATION WITHOUT MACHINE (UNCONSTRAINED)");
                
                output += OutputFormatter.FormatStep(1, "Creating calculation without machine assignment...");
                output += OutputFormatter.FormatApiCall("var calcNoMachine = new Calculation()");
                var calcNoMachine = new Calculation();
                
                output += OutputFormatter.FormatStep(2, "Configuring for HSM with chip thinning...");
                output += OutputFormatter.FormatApiCall("calcNoMachine.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calcNoMachine.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calcNoMachine.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calcNoMachine.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calcNoMachine.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calcNoMachine.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calcNoMachine.Diameter", toolDiameter);
                calcNoMachine.Diameter = toolDiameter;
                output += OutputFormatter.FormatApiProperty("calcNoMachine.Flute_N", 4);
                calcNoMachine.Flute_N = 4;
                
                // Enable HSM and Chip Thinning
                output += OutputFormatter.FormatApiProperty("calcNoMachine.HSM", true);
                calcNoMachine.HSM = true;
                output += OutputFormatter.FormatApiProperty("calcNoMachine.Chip_Thinning", true);
                calcNoMachine.Chip_Thinning = true;
                
                // Set small WOC for HSM (10% radial engagement)
                double wocValue = toolDiameter * 0.1;
                output += OutputFormatter.FormatApiProperty("calcNoMachine.WOC", $"{wocValue:F4} (10% radial)");
                calcNoMachine.WOC = wocValue;
                
                output += OutputFormatter.FormatApiCall("calcNoMachine.SetMaterial(227)");
                calcNoMachine.SetMaterial(227);
                
                output += OutputFormatter.FormatStep(3, "Calculating (unconstrained)...");
                output += OutputFormatter.FormatApiCall("calcNoMachine.Calculate(false)");
                bool successNoMachine = calcNoMachine.Calculate(false);
                
                if (successNoMachine)
                {
                    output += OutputFormatter.FormatSuccess("Calculation completed!");
                    output += OutputFormatter.FormatProperty("SFM", $"{calcNoMachine.Real_SFM:F0}");
                    output += OutputFormatter.FormatProperty("RPM", $"{calcNoMachine.RPM:F0}");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{calcNoMachine.FEED:F1} in/min");
                    output += OutputFormatter.FormatProperty("IPT (Chipload)", $"{calcNoMachine.Real_IPT:F5} in/tooth");
                    output += OutputFormatter.FormatProperty("Chip Thickness", $"{calcNoMachine.Chip_Thickness:F5} in/tooth");
                    output += OutputFormatter.FormatProperty("DOC", $"{calcNoMachine.DOC:F4} inches");
                    output += OutputFormatter.FormatProperty("WOC", $"{calcNoMachine.WOC:F4} inches");
                    output += OutputFormatter.FormatProperty("MRR", $"{calcNoMachine.Gages.MRR:F3} in³/min");
                    
                    if (calcNoMachine.RPM > maxRPM)
                    {
                        output += OutputFormatter.FormatWarning($"RPM ({calcNoMachine.RPM:F0}) EXCEEDS target limit of {maxRPM:F0}!");
                    }
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed.");
                    return output;
                }
                
                // ===== PART 2: WITH MACHINE (CONSTRAINED) =====
                output += OutputFormatter.FormatSection("PART 2: CALCULATION WITH MACHINE (CONSTRAINED)");
                
                output += OutputFormatter.FormatStep(4, "Creating machine with RPM limit...");
                output += OutputFormatter.FormatApiCall("var machine = new Machine()");
                var machine = new Machine();
                output += OutputFormatter.FormatApiProperty("machine.Name", $"Test Machine (Max {maxRPM:F0} RPM)");
                machine.Name = $"Test Machine (Max {maxRPM:F0} RPM)";
                output += OutputFormatter.FormatApiProperty("machine.Max_RPM", maxRPM);
                machine.Max_RPM = maxRPM;
                
                output += OutputFormatter.FormatApiCall("Core.MachineLibrary.InsertMachine(machine)");
                Core.MachineLibrary.InsertMachine(machine);
                output += OutputFormatter.FormatSuccess($"Machine created with GUID: {machine.GUID}");
                
                output += OutputFormatter.FormatStep(5, "Creating calculation with machine assignment...");
                output += OutputFormatter.FormatApiCall("var calcWithMachine = new Calculation()");
                var calcWithMachine = new Calculation();
                
                // Same setup as before
                output += OutputFormatter.FormatApiCall("calcWithMachine.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calcWithMachine.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calcWithMachine.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calcWithMachine.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calcWithMachine.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calcWithMachine.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calcWithMachine.Diameter", toolDiameter);
                calcWithMachine.Diameter = toolDiameter;
                output += OutputFormatter.FormatApiProperty("calcWithMachine.Flute_N", 4);
                calcWithMachine.Flute_N = 4;
                output += OutputFormatter.FormatApiProperty("calcWithMachine.HSM", true);
                calcWithMachine.HSM = true;
                output += OutputFormatter.FormatApiProperty("calcWithMachine.Chip_Thinning", true);
                calcWithMachine.Chip_Thinning = true;
                output += OutputFormatter.FormatApiProperty("calcWithMachine.WOC", wocValue);
                calcWithMachine.WOC = wocValue;
                output += OutputFormatter.FormatApiCall("calcWithMachine.SetMaterial(227)");
                calcWithMachine.SetMaterial(227);
                
                // Assign machine BEFORE calculation
                output += OutputFormatter.FormatStep(6, "Assigning machine to calculation...");
                output += OutputFormatter.FormatApiCall($"calcWithMachine.SetMachine(\"{machine.GUID}\")");
                calcWithMachine.SetMachine(machine.GUID);
                
                output += OutputFormatter.FormatStep(7, "Calculating (with machine constraints)...");
                output += OutputFormatter.FormatApiCall("calcWithMachine.Calculate(false)");
                bool successWithMachine = calcWithMachine.Calculate(false);
                
                if (successWithMachine)
                {
                    output += OutputFormatter.FormatSuccess("Calculation completed!");
                    output += OutputFormatter.FormatProperty("SFM", $"{calcWithMachine.Real_SFM:F0}");
                    output += OutputFormatter.FormatProperty("RPM", $"{calcWithMachine.RPM:F0}");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{calcWithMachine.FEED:F1} in/min");
                    output += OutputFormatter.FormatProperty("IPT (Chipload)", $"{calcWithMachine.Real_IPT:F5} in/tooth");
                    output += OutputFormatter.FormatProperty("Chip Thickness", $"{calcWithMachine.Chip_Thickness:F5} in/tooth");
                    output += OutputFormatter.FormatProperty("DOC", $"{calcWithMachine.DOC:F4} inches");
                    output += OutputFormatter.FormatProperty("WOC", $"{calcWithMachine.WOC:F4} inches");
                    output += OutputFormatter.FormatProperty("MRR", $"{calcWithMachine.Gages.MRR:F3} in³/min");
                    
                    if (calcWithMachine.RPM <= maxRPM)
                    {
                        output += OutputFormatter.FormatSuccess($"RPM ({calcWithMachine.RPM:F0}) is within machine limit!");
                    }
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed.");
                    return output;
                }
                
                // ===== COMPARISON =====
                output += OutputFormatter.FormatSection("COMPARISON: WITH vs WITHOUT MACHINE");
                
                output += OutputFormatter.FormatProperty("RPM Change", $"{calcNoMachine.RPM:F0} → {calcWithMachine.RPM:F0} ({((calcWithMachine.RPM - calcNoMachine.RPM) / calcNoMachine.RPM * 100):F1}%)");
                output += OutputFormatter.FormatProperty("SFM Change", $"{calcNoMachine.Real_SFM:F0} → {calcWithMachine.Real_SFM:F0} ({((calcWithMachine.Real_SFM - calcNoMachine.Real_SFM) / calcNoMachine.Real_SFM * 100):F1}%)");
                output += OutputFormatter.FormatProperty("Feed Change", $"{calcNoMachine.FEED:F1} → {calcWithMachine.FEED:F1} in/min ({((calcWithMachine.FEED - calcNoMachine.FEED) / calcNoMachine.FEED * 100):F1}%)");
                output += OutputFormatter.FormatProperty("IPT Change", $"{calcNoMachine.Real_IPT:F5} → {calcWithMachine.Real_IPT:F5} ({((calcWithMachine.Real_IPT - calcNoMachine.Real_IPT) / calcNoMachine.Real_IPT * 100):F1}%)");
                output += OutputFormatter.FormatProperty("WOC Change", $"{calcNoMachine.WOC:F4} → {calcWithMachine.WOC:F4} ({((calcWithMachine.WOC - calcNoMachine.WOC) / calcNoMachine.WOC * 100):F1}%)");
                output += OutputFormatter.FormatProperty("DOC Change", $"{calcNoMachine.DOC:F4} → {calcWithMachine.DOC:F4} ({((calcWithMachine.DOC - calcNoMachine.DOC) / calcNoMachine.DOC * 100):F1}%)");
                
                output += "\n";
                if (calcWithMachine.RPM == calcNoMachine.RPM && calcNoMachine.RPM > maxRPM)
                {
                    output += OutputFormatter.FormatWarning("⚠ ISSUE DETECTED: RPM was NOT capped by machine limit!");
                    output += OutputFormatter.FormatWarning("The calculation did not automatically apply machine constraints.");
                    output += OutputFormatter.FormatInfo("This may require additional API calls or a different sequence.");
                }
                else if (calcWithMachine.RPM < calcNoMachine.RPM)
                {
                    output += OutputFormatter.FormatSuccess("✓ RPM was successfully constrained by machine limit!");
                    
                    if (Math.Abs(calcWithMachine.Real_IPT - calcNoMachine.Real_IPT) < 0.00001 && 
                        Math.Abs(calcWithMachine.WOC - calcNoMachine.WOC) < 0.00001)
                    {
                        output += OutputFormatter.FormatWarning("⚠ However, IPT/WOC did NOT adjust to the constrained RPM.");
                        output += OutputFormatter.FormatInfo("Only RPM/SFM/Feed were capped, but cutting parameters remain unchanged.");
                    }
                    else
                    {
                        output += OutputFormatter.FormatSuccess("✓ Cutting parameters (IPT/WOC/DOC) adjusted to constrained RPM!");
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
    /// Machine Feed limit with HSM calculation
    /// </summary>
    public class MachineFeedLimitWithHSMTask : DemoTask
    {
        public MachineFeedLimitWithHSMTask()
        {
            Name = "Machine Feed Limit with HSM + Chip Thinning";
            Description = "Demonstrates how machine feed rate limits affect HSM calculations with chip thinning enabled.";
            Category = "Machine Operations";
            
            Parameters.Add(new TaskParameter
            {
                Name = "MaxFeed",
                Label = "Machine Max Feed (in/min)",
                ParameterType = typeof(double),
                DefaultValue = 100.0,
                Description = "Maximum feed rate limit for the machine"
            });
            
            Parameters.Add(new TaskParameter
            {
                Name = "ToolDiameter",
                Label = "Tool Diameter (inches)",
                ParameterType = typeof(double),
                DefaultValue = 0.5,
                Description = "Tool diameter for the calculation"
            });
        }

        public override string Execute(Dictionary<string, object> parameterValues)
        {
            var output = OutputFormatter.FormatHeader("MACHINE FEED LIMIT WITH HSM + CHIP THINNING");
            
            try
            {
                double maxFeed = GetParameter<double>(parameterValues, "MaxFeed", 200.0);
                double toolDiameter = GetParameter<double>(parameterValues, "ToolDiameter", 0.5);
                
                output += OutputFormatter.FormatInfo("This test demonstrates the scenario where HSM + Chip Thinning");
                output += OutputFormatter.FormatInfo("produces high feed rates that would exceed the machine's feed limit.");
                output += OutputFormatter.FormatInfo("We'll compare results WITH and WITHOUT machine assignment.\n");
                
                // ===== PART 1: WITHOUT MACHINE (UNCONSTRAINED) =====
                output += OutputFormatter.FormatSection("PART 1: CALCULATION WITHOUT MACHINE (UNCONSTRAINED)");
                
                output += OutputFormatter.FormatStep(1, "Creating calculation without machine assignment...");
                output += OutputFormatter.FormatApiCall("var calcNoMachine = new Calculation()");
                var calcNoMachine = new Calculation();
                
                output += OutputFormatter.FormatStep(2, "Configuring for HSM with chip thinning...");
                output += OutputFormatter.FormatApiCall("calcNoMachine.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calcNoMachine.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calcNoMachine.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calcNoMachine.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calcNoMachine.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calcNoMachine.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calcNoMachine.Diameter", toolDiameter);
                calcNoMachine.Diameter = toolDiameter;
                output += OutputFormatter.FormatApiProperty("calcNoMachine.Flute_N", 4);
                calcNoMachine.Flute_N = 4;
                
                // Enable HSM and Chip Thinning
                output += OutputFormatter.FormatApiProperty("calcNoMachine.HSM", true);
                calcNoMachine.HSM = true;
                output += OutputFormatter.FormatApiProperty("calcNoMachine.Chip_Thinning", true);
                calcNoMachine.Chip_Thinning = true;
                
                // Set small WOC for HSM (10% radial engagement) - generates high feed
                double wocValue = toolDiameter * 0.1;
                output += OutputFormatter.FormatApiProperty("calcNoMachine.WOC", $"{wocValue:F4} (10% radial)");
                calcNoMachine.WOC = wocValue;
                
                output += OutputFormatter.FormatApiCall("calcNoMachine.SetMaterial(227)");
                calcNoMachine.SetMaterial(227);
                
                output += OutputFormatter.FormatStep(3, "Calculating (unconstrained)...");
                output += OutputFormatter.FormatApiCall("calcNoMachine.Calculate(false)");
                bool successNoMachine = calcNoMachine.Calculate(false);
                
                if (successNoMachine)
                {
                    output += OutputFormatter.FormatSuccess("Calculation completed!");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{calcNoMachine.FEED:F1} in/min");
                    output += OutputFormatter.FormatProperty("RPM", $"{calcNoMachine.RPM:F0}");
                    output += OutputFormatter.FormatProperty("SFM", $"{calcNoMachine.Real_SFM:F0}");
                    output += OutputFormatter.FormatProperty("IPT (Chipload)", $"{calcNoMachine.Real_IPT:F5} in/tooth");
                    output += OutputFormatter.FormatProperty("Chip Thickness", $"{calcNoMachine.Chip_Thickness:F5} in/tooth");
                    output += OutputFormatter.FormatProperty("DOC", $"{calcNoMachine.DOC:F4} inches");
                    output += OutputFormatter.FormatProperty("WOC", $"{calcNoMachine.WOC:F4} inches");
                    output += OutputFormatter.FormatProperty("MRR", $"{calcNoMachine.Gages.MRR:F3} in³/min");
                    
                    if (calcNoMachine.FEED > maxFeed)
                    {
                        output += OutputFormatter.FormatWarning($"Feed Rate ({calcNoMachine.FEED:F1}) EXCEEDS target limit of {maxFeed:F1} in/min!");
                    }
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed.");
                    return output;
                }
                
                // ===== PART 2: WITH MACHINE (CONSTRAINED) =====
                output += OutputFormatter.FormatSection("PART 2: CALCULATION WITH MACHINE (CONSTRAINED)");
                
                output += OutputFormatter.FormatStep(4, "Creating machine with feed limit...");
                output += OutputFormatter.FormatApiCall("var machine = new Machine()");
                var machine = new Machine();
                output += OutputFormatter.FormatApiProperty("machine.Name", $"Test Machine (Max {maxFeed:F1} in/min Feed)");
                machine.Name = $"Test Machine (Max {maxFeed:F1} in/min Feed)";
                output += OutputFormatter.FormatApiProperty("machine.Max_Feed", maxFeed);
                machine.Max_Feed = maxFeed;
                
                output += OutputFormatter.FormatApiCall("Core.MachineLibrary.InsertMachine(machine)");
                Core.MachineLibrary.InsertMachine(machine);
                output += OutputFormatter.FormatSuccess($"Machine created with GUID: {machine.GUID}");
                
                output += OutputFormatter.FormatStep(5, "Creating calculation with machine assignment...");
                output += OutputFormatter.FormatApiCall("var calcWithMachine = new Calculation()");
                var calcWithMachine = new Calculation();
                
                // Same setup as before
                output += OutputFormatter.FormatApiCall("calcWithMachine.SetToolType(Enums.ToolTypes.SolidEndMill)");
                calcWithMachine.SetToolType(Enums.ToolTypes.SolidEndMill);
                output += OutputFormatter.FormatApiCall("calcWithMachine.SetToolMaterial(Enums.ToolMaterials.Carbide)");
                calcWithMachine.SetToolMaterial(Enums.ToolMaterials.Carbide);
                output += OutputFormatter.FormatApiCall("calcWithMachine.SetToolCoating(Enums.ToolCoatings.TiAlN)");
                calcWithMachine.SetToolCoating(Enums.ToolCoatings.TiAlN);
                output += OutputFormatter.FormatApiProperty("calcWithMachine.Diameter", toolDiameter);
                calcWithMachine.Diameter = toolDiameter;
                output += OutputFormatter.FormatApiProperty("calcWithMachine.Flute_N", 4);
                calcWithMachine.Flute_N = 4;
                output += OutputFormatter.FormatApiProperty("calcWithMachine.HSM", true);
                calcWithMachine.HSM = true;
                output += OutputFormatter.FormatApiProperty("calcWithMachine.Chip_Thinning", true);
                calcWithMachine.Chip_Thinning = true;
                output += OutputFormatter.FormatApiProperty("calcWithMachine.WOC", wocValue);
                calcWithMachine.WOC = wocValue;
                output += OutputFormatter.FormatApiCall("calcWithMachine.SetMaterial(227)");
                calcWithMachine.SetMaterial(227);
                
                // Assign machine BEFORE calculation
                output += OutputFormatter.FormatStep(6, "Assigning machine to calculation...");
                output += OutputFormatter.FormatApiCall($"calcWithMachine.SetMachine(\"{machine.GUID}\")");
                calcWithMachine.SetMachine(machine.GUID);
                
                output += OutputFormatter.FormatStep(7, "Calculating (with machine constraints)...");
                output += OutputFormatter.FormatApiCall("calcWithMachine.Calculate(false)");
                bool successWithMachine = calcWithMachine.Calculate(false);
                
                if (successWithMachine)
                {
                    output += OutputFormatter.FormatSuccess("Calculation completed!");
                    output += OutputFormatter.FormatProperty("Feed Rate", $"{calcWithMachine.FEED:F1} in/min");
                    output += OutputFormatter.FormatProperty("RPM", $"{calcWithMachine.RPM:F0}");
                    output += OutputFormatter.FormatProperty("SFM", $"{calcWithMachine.Real_SFM:F0}");
                    output += OutputFormatter.FormatProperty("IPT (Chipload)", $"{calcWithMachine.Real_IPT:F5} in/tooth");
                    output += OutputFormatter.FormatProperty("Chip Thickness", $"{calcWithMachine.Chip_Thickness:F5} in/tooth");
                    output += OutputFormatter.FormatProperty("DOC", $"{calcWithMachine.DOC:F4} inches");
                    output += OutputFormatter.FormatProperty("WOC", $"{calcWithMachine.WOC:F4} inches");
                    output += OutputFormatter.FormatProperty("MRR", $"{calcWithMachine.Gages.MRR:F3} in³/min");
                    
                    if (calcWithMachine.FEED <= maxFeed)
                    {
                        output += OutputFormatter.FormatSuccess($"Feed Rate ({calcWithMachine.FEED:F1}) is within machine limit!");
                    }
                }
                else
                {
                    output += OutputFormatter.FormatError("Calculation failed.");
                    return output;
                }
                
                // ===== COMPARISON =====
                output += OutputFormatter.FormatSection("COMPARISON: WITH vs WITHOUT MACHINE");
                
                output += OutputFormatter.FormatProperty("Feed Change", $"{calcNoMachine.FEED:F1} → {calcWithMachine.FEED:F1} in/min ({((calcWithMachine.FEED - calcNoMachine.FEED) / calcNoMachine.FEED * 100):F1}%)");
                output += OutputFormatter.FormatProperty("RPM Change", $"{calcNoMachine.RPM:F0} → {calcWithMachine.RPM:F0} ({((calcWithMachine.RPM - calcNoMachine.RPM) / calcNoMachine.RPM * 100):F1}%)");
                output += OutputFormatter.FormatProperty("SFM Change", $"{calcNoMachine.Real_SFM:F0} → {calcWithMachine.Real_SFM:F0} ({((calcWithMachine.Real_SFM - calcNoMachine.Real_SFM) / calcNoMachine.Real_SFM * 100):F1}%)");
                output += OutputFormatter.FormatProperty("IPT Change", $"{calcNoMachine.Real_IPT:F5} → {calcWithMachine.Real_IPT:F5} ({((calcWithMachine.Real_IPT - calcNoMachine.Real_IPT) / calcNoMachine.Real_IPT * 100):F1}%)");
                output += OutputFormatter.FormatProperty("WOC Change", $"{calcNoMachine.WOC:F4} → {calcWithMachine.WOC:F4} ({((calcWithMachine.WOC - calcNoMachine.WOC) / calcNoMachine.WOC * 100):F1}%)");
                output += OutputFormatter.FormatProperty("DOC Change", $"{calcNoMachine.DOC:F4} → {calcWithMachine.DOC:F4} ({((calcWithMachine.DOC - calcNoMachine.DOC) / calcNoMachine.DOC * 100):F1}%)");
                
                output += "\n";
                if (calcWithMachine.FEED == calcNoMachine.FEED && calcNoMachine.FEED > maxFeed)
                {
                    output += OutputFormatter.FormatWarning("⚠ ISSUE DETECTED: Feed was NOT capped by machine limit!");
                    output += OutputFormatter.FormatWarning("The calculation did not automatically apply machine constraints.");
                    output += OutputFormatter.FormatInfo("This may require additional API calls or a different sequence.");
                }
                else if (calcWithMachine.FEED < calcNoMachine.FEED)
                {
                    output += OutputFormatter.FormatSuccess("✓ Feed Rate was successfully constrained by machine limit!");
                    
                    if (Math.Abs(calcWithMachine.Real_IPT - calcNoMachine.Real_IPT) < 0.00001 && 
                        Math.Abs(calcWithMachine.WOC - calcNoMachine.WOC) < 0.00001)
                    {
                        output += OutputFormatter.FormatWarning("⚠ However, IPT/WOC did NOT adjust to the constrained feed.");
                        output += OutputFormatter.FormatInfo("Only Feed/RPM/SFM were capped, but cutting parameters remain unchanged.");
                    }
                    else
                    {
                        output += OutputFormatter.FormatSuccess("✓ Cutting parameters (IPT/WOC/DOC) adjusted to constrained feed!");
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
}
