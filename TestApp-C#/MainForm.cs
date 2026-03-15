using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSMAdvisor;
using TestApp_C_.Tasks;

namespace TestApp_C_
{
    public partial class MainForm : Form
    {
        private List<DemoTask> allTasks;
        private DemoTask currentTask;
        private Dictionary<string, Control> parameterControls;

        public MainForm()
        {
            InitializeComponent();
            parameterControls = new Dictionary<string, Control>();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Initialize HSMAdvisor Core
                txtOutput.Text = "=== HSMAdvisor DLL Demo Application ===\n\n";
                txtOutput.AppendText("Initializing HSMAdvisor Core...\n");
                txtOutput.AppendText("  → API: Core.init()\n");
                Core.init();
                txtOutput.AppendText("✓ SUCCESS: Core initialized\n\n");
                
                // Load all tasks
                LoadAllTasks();
                
                // Build tree view
                BuildTaskTree();
                
                // Show welcome message
                txtOutput.AppendText("Welcome! Select a task from the left to begin.\n\n");
                txtOutput.AppendText("This demo showcases the capabilities of the HSMAdvisorCore DLL.\n");
                txtOutput.AppendText("Each task demonstrates different API features and calculations.\n\n");
                
                txtOutput.AppendText("Checking license status...\n");
                txtOutput.AppendText("  → API: Core.isLicense()\n");
                bool hasLicense = Core.isLicense();
                txtOutput.AppendText($"     Returns: {hasLicense}\n");
                txtOutput.AppendText("  → API: Core.GetLicenseLevel()\n");
                var licenseLevel = Core.GetLicenseLevel();
                txtOutput.AppendText($"     Returns: {licenseLevel}\n\n");
                
                txtOutput.AppendText($"License Status: {(hasLicense ? "Valid" : "Trial/Invalid")}\n");
                txtOutput.AppendText($"License Level: {licenseLevel}\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application:\n{ex.Message}", 
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllTasks()
        {
            allTasks = new List<DemoTask>();
            
            // License tasks
            allTasks.Add(new CheckLicenseStatusTask());
            allTasks.Add(new ShowLicenseLevelTask());
            allTasks.Add(new ShowLicenseDialogTask());
            allTasks.Add(new ValidateLicenseForCalculationTask());
            
            // Machine tasks
            allTasks.Add(new ListMachinesTask());
            allTasks.Add(new ShowMachineProfileDialogTask());
            allTasks.Add(new CreateMachineProgrammaticallyTask());
            allTasks.Add(new AssignMachineToCalculationTask());
            allTasks.Add(new MachineRPMLimitWithHSMTask());
            allTasks.Add(new MachineFeedLimitWithHSMTask());

            // Material tasks
            allTasks.Add(new ListMaterialsTask());
            allTasks.Add(new ShowMaterialSelectionDialogTask());
            allTasks.Add(new GetRecentMaterialsTask());
            allTasks.Add(new AssignMaterialToCalculationTask());
            
            // Calculation tasks
            allTasks.Add(new BasicMillingCalculationTask());
            allTasks.Add(new ShowHSMAdvisorDialogTask());
            allTasks.Add(new HSMMachiningCalculationTask());
            allTasks.Add(new SlottingCalculationTask());
            //allTasks.Add(new CompleteWorkflowTask());
            
            // Advanced tasks
            allTasks.Add(new MetricImperialComparisonTask());
            allTasks.Add(new ResetCalculationTask());
        }

        private void BuildTaskTree()
        {
            treeViewTasks.Nodes.Clear();
            
            // Group tasks by category
            var categories = allTasks.GroupBy(t => t.Category).OrderBy(g => g.Key);
            
            foreach (var category in categories)
            {
                TreeNode categoryNode = new TreeNode(category.Key);
                categoryNode.NodeFont = new Font(treeViewTasks.Font, FontStyle.Bold);
                
                foreach (var task in category.OrderBy(t => t.Name))
                {
                    TreeNode taskNode = new TreeNode(task.Name);
                    taskNode.Tag = task;
                    categoryNode.Nodes.Add(taskNode);
                }
                
                treeViewTasks.Nodes.Add(categoryNode);
            }
            
            // Expand all categories
            treeViewTasks.ExpandAll();
        }

        private void treeViewTasks_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is DemoTask task)
            {
                currentTask = task;
                DisplayTaskInfo(task);
                CreateParameterControls(task);
                btnRunTask.Enabled = true;
            }
            else
            {
                currentTask = null;
                lblTaskTitle.Text = "HSMAdvisor DLL Demo";
                lblTaskDescription.Text = "Select a task from the list to see its description.";
                panelParameters.Controls.Clear();
                btnRunTask.Enabled = false;
            }
        }

        private void DisplayTaskInfo(DemoTask task)
        {
            lblTaskTitle.Text = task.Name;
            lblTaskDescription.Text = task.Description;
            
            if (task.Parameters.Count > 0)
            {
                lblTaskDescription.Text += $"\n\nThis task has {task.Parameters.Count} configurable parameter(s).";
            }
        }

        private void CreateParameterControls(DemoTask task)
        {
            panelParameters.Controls.Clear();
            parameterControls.Clear();
            
            if (task.Parameters.Count == 0)
            {
                Label noParamsLabel = new Label
                {
                    Text = "This task has no configurable parameters.",
                    AutoSize = true,
                    Location = new Point(10, 10),
                    ForeColor = Color.Gray
                };
                panelParameters.Controls.Add(noParamsLabel);
                return;
            }
            
            int yPos = 10;
            
            foreach (var param in task.Parameters)
            {
                // Label
                Label label = new Label
                {
                    Text = param.Label + ":",
                    Location = new Point(10, yPos),
                    AutoSize = true,
                    Width = 150
                };
                panelParameters.Controls.Add(label);
                
                // Input control
                Control inputControl;
                
                if (param.ParameterType == typeof(bool))
                {
                    CheckBox checkBox = new CheckBox
                    {
                        Location = new Point(170, yPos - 2),
                        Checked = param.DefaultValue != null && (bool)param.DefaultValue,
                        Width = 300
                    };
                    inputControl = checkBox;
                }
                else if (param.Options != null && param.Options.Count > 0)
                {
                    ComboBox comboBox = new ComboBox
                    {
                        Location = new Point(170, yPos),
                        Width = 300,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    comboBox.Items.AddRange(param.Options.ToArray());
                    if (param.DefaultValue != null)
                    {
                        comboBox.SelectedItem = param.DefaultValue.ToString();
                    }
                    inputControl = comboBox;
                }
                else
                {
                    TextBox textBox = new TextBox
                    {
                        Location = new Point(170, yPos),
                        Width = 300,
                        Text = param.DefaultValue?.ToString() ?? ""
                    };
                    inputControl = textBox;
                }
                
                panelParameters.Controls.Add(inputControl);
                parameterControls[param.Name] = inputControl;
                
                // Description (smaller text)
                if (!string.IsNullOrEmpty(param.Description))
                {
                    Label descLabel = new Label
                    {
                        Text = param.Description,
                        Location = new Point(170, yPos + 25),
                        AutoSize = false,
                        Width = 500,
                        Height = 30,
                        ForeColor = Color.Gray,
                        Font = new Font(this.Font.FontFamily, 8f)
                    };
                    panelParameters.Controls.Add(descLabel);
                    yPos += 30;
                }
                
                yPos += 35;
            }
        }

        private void btnRunTask_Click(object sender, EventArgs e)
        {
            if (currentTask == null)
            {
                MessageBox.Show("Please select a task first.", "No Task Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                // Gather parameter values
                Dictionary<string, object> paramValues = new Dictionary<string, object>();
                
                foreach (var param in currentTask.Parameters)
                {
                    if (parameterControls.ContainsKey(param.Name))
                    {
                        Control control = parameterControls[param.Name];
                        
                        if (control is TextBox textBox)
                        {
                            paramValues[param.Name] = textBox.Text;
                        }
                        else if (control is CheckBox checkBox)
                        {
                            paramValues[param.Name] = checkBox.Checked;
                        }
                        else if (control is ComboBox comboBox)
                        {
                            paramValues[param.Name] = comboBox.SelectedItem?.ToString();
                        }
                    }
                }
                
                // Clear output
                txtOutput.Clear();
                txtOutput.AppendText($"Executing: {currentTask.Name}\n");
                txtOutput.AppendText($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
                txtOutput.AppendText(new string('-', 60) + "\n\n");
                
                // Execute task
                btnRunTask.Enabled = false;
                btnRunTask.Text = "Running...";
                Application.DoEvents();
                
                string result = currentTask.Execute(paramValues);
                
                // Display result
                txtOutput.AppendText(result);
                txtOutput.AppendText("\n\n" + new string('=', 60) + "\n");
                txtOutput.AppendText("Task completed.\n");
                
                // Scroll to top
                txtOutput.SelectionStart = 0;
                txtOutput.ScrollToCaret();
            }
            catch (Exception ex)
            {
                txtOutput.AppendText("\n\n" + OutputFormatter.FormatError($"FATAL ERROR: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}"));
                MessageBox.Show($"Error executing task:\n{ex.Message}", "Execution Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRunTask.Enabled = true;
                btnRunTask.Text = "▶ Run Task";
            }
        }
    }
}
