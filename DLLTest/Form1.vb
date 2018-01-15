Imports HSMAdvisor

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

		'create an instance of Calculation class
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
		cut.SetMaterial(25)

		'Set tool type.
		'Calculation.ToolTypes has references to all tool types
		cut.SetToolType(Calculation.ToolTypes.SolidEndMill)

		'Set tool material
		cut.SetToolMaterial(Calculation.ToolMaterials.Carbide)

		'Set tool coating
		cut.SetToolCoating(Calculation.ToolCoatings.TiAlN)

		'Set metric flag. If true, everything is metric, othervise imperial
		cut.SetMetric(False)

		'use this to reset calculation
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
			'Calculation done. Display some results:
			MsgBox(String.Format(
				"Hurray RPM:{0} FEED:{1} DOC:{2} WOC:{3}", cut.RPM, cut.FEED, cut.DOC, cut.WOC
					))
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

		' Other calls that can be used by plugin:
		'
		'' <summary>
		'' Shows the Add Tool dialog 
		'' </summary>
		'' <param name="cut"></param>
		'' <returns>id if tool was added othervise returns 0</returns>
		'' Public Function ShowAddToolToDatabaseDialog(cut As Calculation) As Integer

		'' <summary>
		'' Shows the Edit Tool dialog 
		'' </summary>
		'' <param name="cut"></param>
		'' <returns>True if tool was saved</returns>
		'' Public Function ShowEditToolInDatabaseDialog(cut As Calculation) As Boolean

		'' <summary>
		'' Show Add Cut dialog
		'' </summary>
		'' <param name="cut"></param>
		'' <returns>returns ID of the new cut or </returns>
		'' Public Function ShowAddCutToDatabaseDialog(cut As Calculation) As Integer

		'' <summary>
		'' Show Edit Cut dialog
		'' </summary>
		'' <param name="cut"></param>
		'' <returns>True if tool was saved</returns>
		'' Public Function ShowEditCutInDatabaseDialog(cut As Calculation) As Boolean

		'' <summary>
		'' Launch Select Tool/Cut dialog
		'' </summary>
		'' <param name="title"></param>
		'' <param name="DBChanged"></param>
		'' <param name="library"></param>
		'' <param name="tool_id"></param>
		'' <param name="cut_id"></param>
		'' <returns>Calculation object</returns>
		'' Public Function ShowSelectToolCutFromDatabaseDialog(title As String, ByRef DBChanged As Boolean, Optional library As String = "", Optional tool_id As Integer = 0, Optional cut_id As Integer = 0) As Calculation


	End Sub
End Class
