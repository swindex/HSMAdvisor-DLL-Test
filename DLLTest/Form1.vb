Imports HSMAdvisor
Imports ObjectToolDatabase.ToolDataBase

Public Class Form1
    Private cut As Calculation

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'initialize HSMAdvisor 
        init()

        'Check if license is found or trial is available and if not, display the License dialog
        If Not isLicense() Then
            ShowLicenseDialog()
        Else
            MsgBox(String.Format(
                "License Level:{0}, Is License {1}? {2}", GetLicenseLevel, License_Level_.Trial.ToString, GetLicenseLevel() = License_Level_.Trial
                ))
        End If

        'Create an instance of Calculation class
        cut = New Calculation


        If MsgBox("Want to Set up machine?", vbYesNo
            ) = MsgBoxResult.Yes Then
            'Show Edit machine dialog
            Dim machine_id = ShowEditMachineDatabaseDialog()

            'Set machine that the dialog returns
            cut.SetMachine(machine_id)
        End If

        'Set workpiece material
        'Use GetMaterialsTable() to get IDs and Names of materials
        Dim materialsTable = GetMaterialsList()

        'set material by ID
        cut.SetMaterial(25)

        'Set tool type
        'Calculation.ToolTypes has references to all tool types
        cut.SetToolType(Enums.ToolTypes.SolidEndMill)

        'Set tool material
        cut.SetToolMaterial(Enums.ToolMaterials.Carbide)

        'Set tool coating
        cut.SetToolCoating(Enums.ToolCoatings.TiAlN)

        'optional
        'Set metric flag. If true, everything is metric, othervise imperial
        'default is imperial
        'cut.SetMetric(true)

        'Use this to reset calculation
        'Calculation.ResetDataAreas defines which part of Calculation we need to default
        cut.Reset(Calculation.ResetDataAreas.All)

        'Set dimater to 0.375
        cut.Diameter = 0.5

        'Set chip thinning to true
        cut.Chip_Thinning = True

        'Set HSM machining to true
        cut.HSM = True

        'Try to Do calculation 
        If cut.Calculate(False) Then
            'Calculation done. Display some of the results:
            MsgBox(
                $"Material: {cut.MaterialName}
                RPM:{cut.RPM} 
                FEED:{cut.FEED} 
                DOC:{cut.DOC} 
                WOC:{ cut.WOC}", MsgBoxStyle.OkOnly, "Result!")
        Else
            'License validation failed
            MsgBox(String.Format(
                "Invalid license Level:{0}", GetLicenseLevel()
                ))
        End If

        'Ask if we want to launch HSMAdvisor Window
        If MsgBox("Want to launch HSMAdvisor window?", vbYesNo
            ) = MsgBoxResult.Yes Then

            'Launch HSMADvisor window and return a new calculation
            cut = ShowHSMAdvisorDialog(cut, True, "Click OK to return the calculation")
        End If

    End Sub
End Class
