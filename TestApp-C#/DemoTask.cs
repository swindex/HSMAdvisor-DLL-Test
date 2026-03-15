using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TestApp_C_
{
    /// <summary>
    /// Base class for all demo tasks
    /// </summary>
    public abstract class DemoTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        
        /// <summary>
        /// Parameters that the task needs
        /// </summary>
        public List<TaskParameter> Parameters { get; set; } = new List<TaskParameter>();

        /// <summary>
        /// Execute the task and return formatted output
        /// </summary>
        public abstract string Execute(Dictionary<string, object> parameterValues);

        /// <summary>
        /// Get parameter value with fallback to default
        /// </summary>
        protected T GetParameter<T>(Dictionary<string, object> parameterValues, string paramName, T defaultValue = default(T))
        {
            if (parameterValues.ContainsKey(paramName) && parameterValues[paramName] != null)
            {
                try
                {
                    return (T)Convert.ChangeType(parameterValues[paramName], typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
    }

    /// <summary>
    /// Represents a parameter for a task
    /// </summary>
    public class TaskParameter
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public Type ParameterType { get; set; }
        public object DefaultValue { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        
        // For combo box options
        public List<string> Options { get; set; }
    }

    /// <summary>
    /// Helper class for formatting output
    /// </summary>
    public static class OutputFormatter
    {
        public static string FormatHeader(string text)
        {
            return $"\n{'='.ToString().PadRight(60, '=')}\n{text}\n{'='.ToString().PadRight(60, '=')}\n";
        }

        public static string FormatSection(string title)
        {
            return $"\n--- {title} ---\n";
        }

        public static string FormatProperty(string name, object value)
        {
            return $"  {name}: {value}\n";
        }

        public static string FormatSuccess(string message)
        {
            return $"✓ SUCCESS: {message}\n";
        }

        public static string FormatError(string message)
        {
            return $"✗ ERROR: {message}\n";
        }

        public static string FormatWarning(string message)
        {
            return $"⚠ WARNING: {message}\n";
        }

        public static string FormatInfo(string message)
        {
            return $"ℹ INFO: {message}\n";
        }

        public static string FormatStep(int step, string description)
        {
            return $"\n[Step {step}] {description}\n";
        }

        public static string FormatApiCall(string methodCall)
        {
            return $"  → API: {methodCall}\n";
        }

        public static string FormatApiCallWithReturn(string methodCall, object returnValue)
        {
            return $"  → API: {methodCall}\n     Returns: {returnValue}\n";
        }

        public static string FormatApiProperty(string propertyName, object value)
        {
            return $"  → API: {propertyName} = {value}\n";
        }

        public static string FormatObject(object obj)
        {
            if (obj == null) return "null";
            
            var sb = new System.Text.StringBuilder();
            var props = obj.GetType().GetProperties();
            
            foreach (var prop in props)
            {
                try
                {
                    var value = prop.GetValue(obj);
                    sb.Append(FormatProperty(prop.Name, value ?? "null"));
                }
                catch (Exception ex)
                {
                    sb.Append(FormatProperty(prop.Name, $"<Error: {ex.Message}>"));
                }
            }
            
            return sb.ToString();
        }
    }
}
