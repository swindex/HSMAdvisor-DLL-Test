using System;
using System.Collections.Generic;
using HSMAdvisor;
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
}
