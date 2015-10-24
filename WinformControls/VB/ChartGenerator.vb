
'Usage:
'Dim chartGenerator as ChartGenerator = new ChartGenerator()
'Dim chart as Chart = chartGenerator.GenerateChart(dataTable, width, height, color, chartType,...bla bla)
Public Class ChartGenerator

 

    Private chart As Chart

 

    Public ReadOnly Property GetChart() As Chart

        Get

            Return chart

        End Get

    End Property

 

    Public Sub New()

        chart = New Chart

        AddHandler chart.MouseDown, AddressOf chart_MouseDown

        AddHandler chart.MouseMove, AddressOf chart_MouseMove

        AddHandler chart.MouseUp, AddressOf chart_MouseUp

    End Sub

 

    Private MouseDowned As Boolean

    Private Sub chart_MouseDown(sender As Object, e As MouseEventArgs)

        If e.Button <> Windows.Forms.MouseButtons.Left Then

            Return

        End If

 

        Dim ptrChart = CType(sender, Chart)

        Dim ptrChartArea As ChartArea = ptrChart.ChartAreas(0)

        MouseDowned = True

        ptrChartArea.CursorX.SelectionStart = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X)

        ptrChartArea.CursorY.SelectionStart = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y)

        ptrChartArea.CursorX.SelectionEnd = ptrChartArea.CursorX.SelectionStart

        ptrChartArea.CursorY.SelectionEnd = ptrChartArea.CursorY.SelectionStart

    End Sub

 

    Private Sub chart_MouseMove(sender As Object, e As MouseEventArgs)

        Dim ptrChart = CType(sender, Chart)

        Dim selX, selY As Double

        Try

            selX = ptrChart.ChartAreas(0).AxisX.PixelPositionToValue(e.Location.X)

            selY = ptrChart.ChartAreas(0).AxisY.PixelPositionToValue(e.Location.Y)

        Catch ex As Exception

 

        End Try

        If MouseDowned Then

            ptrChart.ChartAreas(0).CursorX.SelectionEnd = selX

            ptrChart.ChartAreas(0).CursorY.SelectionEnd = selY

        End If

    End Sub

 

    Private Sub chart_MouseUp(sender As Object, e As MouseEventArgs)

        If e.Button <> Windows.Forms.MouseButtons.Left Then

            Return

        End If

        MouseDowned = False

        Dim ptrChart = CType(sender, Chart)

        Dim ptrChartArea = ptrChart.ChartAreas(0)

        Dim XStart As Double = ptrChartArea.CursorX.SelectionStart

        Dim XEnd As Double = ptrChartArea.CursorX.SelectionEnd

        Dim YStart As Double = ptrChartArea.CursorY.SelectionStart

        Dim YEnd As Double = ptrChartArea.CursorY.SelectionEnd

        'Zoom area for Y2 Axis

        Dim YMin As Double = ptrChartArea.AxisY.ValueToPosition(Math.Min(YStart, YEnd))

        Dim YMax As Double = ptrChartArea.AxisY.ValueToPosition(Math.Max(YStart, YEnd))

 

        If XStart = XEnd And YStart = YEnd Then

            Return

        End If

 

        'Zoom operation

        ptrChartArea.AxisX.ScaleView.Zoom(

            Math.Min(XStart, XEnd), Math.Max(XStart, XEnd))

        ptrChartArea.AxisY.ScaleView.Zoom(

            Math.Min(YStart, YEnd), Math.Max(YStart, YEnd))

        ptrChartArea.AxisY2.ScaleView.Zoom( _

            ptrChartArea.AxisY2.PositionToValue(YMin), _

            ptrChartArea.AxisY2.PositionToValue(YMax))

 

        'Clear selection

        ptrChartArea.CursorX.SelectionStart = ptrChartArea.CursorX.SelectionEnd

        ptrChartArea.CursorY.SelectionStart = ptrChartArea.CursorY.SelectionEnd

    End Sub



 
'A chart with 2 series
    Public Function GenerateChart(dtChartDataSource As DataTable, width As Integer, height As Integer, bgColor As String, intType As Integer, legend1 As String, legend2 As String) As Chart

        'Dim chart As Chart = New Chart()

        chart.Width = width

        chart.Height = height

 

        Dim legend As New Legend()

        legend.Name = "Legend"

        chart.Legends.Add(legend)

        chart.Legends(0).Docking = Docking.Top

 

        Dim chartArea As ChartArea = New ChartArea()

        chartArea.Name = "ChartArea"

        chartArea.CursorX.IsUserEnabled = True

        chartArea.CursorX.IsUserSelectionEnabled = True

        chartArea.AxisX.ScaleView.Zoomable = True

        chartArea.AxisX.ScrollBar.IsPositionedInside = True

 

        chartArea.CursorY.IsUserEnabled = True

        chartArea.CursorY.IsUserSelectionEnabled = True

        chartArea.AxisY.ScaleView.Zoomable = True

        chartArea.AxisY.ScrollBar.IsPositionedInside = True

 

        'remove x-axis grid lines

        chartArea.AxisX.MajorGrid.LineWidth = 0

        'remove y-axis grid lines

        chartArea.AxisY.MajorGrid.LineWidth = 0

        chartArea.BackColor = Color.FromName(bgColor)

        chart.ChartAreas.Add(chartArea)

        chart.Palette = ChartColorPalette.BrightPastel

        Dim series As String = String.Empty

 

        'create series and add data points to the series

        If dtChartDataSource IsNot Nothing Then

            chartArea.AxisX.Title = dtChartDataSource.Columns(0).ToString

            chartArea.AxisY.Title = dtChartDataSource.Columns(1).ToString

 

            chart.DataSource = dtChartDataSource

            chart.Series.Add("Series1")

            chart.Series(0).ChartType = CType(intType, SeriesChartType)

            chart.Series(0).Name = legend1

            chart.Series(0).XValueMember = dtChartDataSource.Columns(0).ToString()

            chart.Series(0).YValueMembers = dtChartDataSource.Columns(1).ToString()

 

            chart.Series.Add("Series2")

            chart.Series(1).ChartType = CType(intType, SeriesChartType)

            chart.Series(1).Name = legend2

            chart.Series(1).XValueMember = dtChartDataSource.Columns(0).ToString()

            chart.Series(1).YValueMembers = dtChartDataSource.Columns(3).ToString()

        End If

        Return chart

    End Function

End Class
