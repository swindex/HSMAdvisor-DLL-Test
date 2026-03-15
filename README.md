# HSMAdvisor-DLL-Test

A comprehensive demonstration project showcasing the capabilities of the **HSMAdvisorCore.dll** library. This project provides a fully-featured Windows Forms test application with an interactive task-based demo system that demonstrates how to integrate HSMAdvisor's machining calculation engine into your own applications.

The HSMAdvisorCore library used in this project is the same as the one used in the HSMAdvisor standalone app, which can be downloaded from https://hsmadvisor.com/download.

To learn more about HSMAdvisor and its features, visit the project's homepage at https://hsmadvisor.com/.

## Overview

This demonstration application provides over 15 interactive tasks organized into 5 categories, each showcasing different aspects of the HSMAdvisor API:

- **License Management** - Check license status, display license dialogs, and validate licensing requirements
- **Machine Management** - List available machines, create custom machine profiles, and apply machine constraints to calculations
- **Material Management** - Browse materials, show material selection dialogs, and assign materials to calculations
- **Calculation Examples** - Perform various types of machining calculations (milling, drilling, HSM, slotting)
- **Advanced Features** - Unit conversions, metric/imperial comparisons, and calculation resets

## Key Features

### Task-Based Architecture
The application uses an extensible task-based architecture where each demo is a self-contained task with:
- Configurable parameters with dynamic UI generation
- Step-by-step execution logging
- Detailed API call tracking and result formatting
- Category-based organization in a tree view interface

### Comprehensive API Demonstrations
Each task demonstrates real-world usage of the HSMAdvisor API, including:
- **Basic Calculations**: Simple milling and drilling operations
- **Advanced HSM**: High-speed machining with chip thinning compensation
- **Slotting Operations**: Full-width cutting with optimized parameters
- **Material Selection**: Programmatic and dialog-based material assignment
- **Machine Profiles**: Creating custom machines and applying constraints
- **License Validation**: Checking license status and feature availability

### Output Formatting System
The application includes a sophisticated output formatting system that provides:
- Clearly formatted headers and sections
- API call tracking with method names and return values
- Step-by-step execution flow
- Success/error/warning/info messages with visual indicators
- Property and object inspection utilities

## Project Structure

```
HSMAdvisor-DLL-Test/
├── HSMAdvisorCore.dll          # Core calculation engine
├── HSMAdvisorCore.xml          # API documentation
├── HSMAdvisorDatabase.dll      # Material and tool database
├── HSMAdvisorPlugin.dll        # Plugin support
├── Newtonsoft.Json.dll         # JSON dependency
├── README.md                   # This file
├── DLLTest.sln                 # Visual Studio solution
└── TestApp-C#/                 # Demo application
    ├── Program.cs              # Application entry point
    ├── MainForm.cs             # Main UI form with task tree
    ├── MainForm.Designer.cs    # UI designer file
    ├── DemoTask.cs             # Base task class and utilities
    └── Tasks/                  # Demo task implementations
        ├── LicenseTasks.cs     # License checking and validation
        ├── MachineTasks.cs     # Machine management demos
        ├── MaterialTasks.cs    # Material selection demos
        ├── CalculationTasks.cs # Machining calculation demos
        └── AdvancedTasks.cs    # Advanced feature demos
```

## Getting Started

### Prerequisites
* Microsoft Windows 10 or 11
* Microsoft .NET Framework 4.8 or later
* Visual Studio 2015 or later (2019/2022 recommended)

### Building the Application

1. Clone this repository:
   ```
   git clone https://github.com/swindex/HSMAdvisor-DLL-Test.git
   cd HSMAdvisor-DLL-Test
   ```

2. Open `DLLTest.sln` in Visual Studio

3. Build the solution (Ctrl+Shift+B or Build > Build Solution)

4. Run the application (F5 or Debug > Start Debugging)

### Using the Application

1. **Select a Task**: Browse the task tree on the left side organized by category
2. **Configure Parameters**: If the task has parameters, they will appear in the parameters panel
3. **Run the Task**: Click the "▶ Run Task" button to execute
4. **View Results**: Detailed output appears in the right panel, showing:
   - API calls made
   - Step-by-step execution flow
   - Calculation results
   - Errors or warnings if any

## Example Tasks

### Basic Milling Calculation
Demonstrates a straightforward endmill calculation with configurable tool diameter, flute count, depth of cut (DOC), and width of cut (WOC). Shows the complete workflow from creating a calculation object to retrieving results.

### HSM Machining Calculation
Showcases high-speed machining techniques with chip thinning compensation. Demonstrates how to optimize for low radial engagement scenarios typical in modern CAM strategies.

### Show Full HSMAdvisor Dialog
Opens the complete HSMAdvisor calculation interface, allowing users to explore all features available in the full application. Returns the configured calculation for programmatic use.

### Machine Profile Management
Demonstrates how to list available machines, create custom machine profiles programmatically, and apply machine-specific constraints (RPM limits, feed rate limits) to calculations.

### Material Selection
Shows multiple approaches to material selection: programmatic assignment by ID, using the material selection dialog, and retrieving recently used materials.

## Code Examples

### Creating a Basic Calculation
```csharp
using HSMAdvisor;

// Initialize HSMAdvisor Core
Core.init();

// Create a calculation object
var calc = new Calculation();

// Configure tool parameters
calc.SetToolType(Enums.ToolTypes.SolidEndMill);
calc.SetToolMaterial(Enums.ToolMaterials.Carbide);
calc.SetToolCoating(Enums.ToolCoatings.TiAlN);
calc.Diameter = 0.5;
calc.Flute_N = 4;

// Assign material (ID 227 is a license-free demo material)
calc.SetMaterial(227);

// Enable HSM features
calc.HSM = true;
calc.Chip_Thinning = true;

// Perform calculation
bool success = calc.Calculate(false);

if (success)
{
    Console.WriteLine($"RPM: {calc.RPM:F0}");
    Console.WriteLine($"Feed Rate: {calc.FEED:F1} in/min");
    Console.WriteLine($"DOC: {calc.DOC:F4} inches");
    Console.WriteLine($"WOC: {calc.WOC:F4} inches");
}
```

### Using the Material Selection Dialog
```csharp
// Show material selection dialog
var selectedMaterialId = Core.ShowMaterialSelectionDialog();

if (selectedMaterialId.HasValue)
{
    var calc = new Calculation();
    calc.SetMaterial(selectedMaterialId.Value);
    Console.WriteLine($"Selected: {calc.MaterialName}");
}
```

## Extending the Demo Application

The task-based architecture makes it easy to add new demonstrations:

1. **Create a new task class** inheriting from `DemoTask`
2. **Define parameters** in the constructor using `TaskParameter` objects
3. **Implement the Execute method** with your demo logic
4. **Add to the task list** in `MainForm.LoadAllTasks()`

Example:
```csharp
public class MyCustomTask : DemoTask
{
    public MyCustomTask()
    {
        Name = "My Custom Task";
        Description = "Demonstrates a custom feature";
        Category = "Custom Examples";
        
        Parameters.Add(new TaskParameter
        {
            Name = "ToolSize",
            Label = "Tool Diameter",
            ParameterType = typeof(double),
            DefaultValue = 0.5,
            Description = "Tool diameter in inches"
        });
    }

    public override string Execute(Dictionary<string, object> parameterValues)
    {
        double diameter = GetParameter<double>(parameterValues, "ToolSize", 0.5);
        
        var output = OutputFormatter.FormatHeader("MY CUSTOM TASK");
        output += OutputFormatter.FormatStep(1, "Performing custom operation...");
        
        // Your custom logic here
        
        return output;
    }
}
```

## API Documentation

API documentation is available in the included `HSMAdvisorCore.xml` file. Visual Studio will automatically display IntelliSense documentation when referencing the DLL.

Key classes and namespaces:
- `HSMAdvisor.Core` - Core initialization and dialog functions
- `HSMAdvisor.Calculation` - Main calculation class
- `HSMAdvisor.Enums` - Enumerations for tool types, materials, coatings, etc.
- `HSMAdvisorDatabase.ToolDataBase` - Material and machine database access

## License and Distribution

Distribution of the HSMAdvisorCore library is permitted without prior permission, as long as:
- The library is not used on computers that have circumvented the license mechanism
- The computers on which it is used are fully licensed for production use

**Note**: A demo material (ID 227) is available for license-free testing and demonstration purposes. Full material database access requires a valid HSMAdvisor license.

## Copyright

© Eldar Gerfanov, HSMAdvisor Inc. All rights reserved.

## Support

For questions, support, or licensing information, visit:
- Website: https://hsmadvisor.com/
- Download: https://hsmadvisor.com/download
- Contact: Available through the HSMAdvisor website

## Contributing

This is a public demonstration project. Contributions, improvements, and additional demo tasks are welcome via pull requests.
