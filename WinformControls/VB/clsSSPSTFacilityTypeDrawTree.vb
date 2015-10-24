Imports System.Collections.Generic
Imports System.Linq


''Usage: 
'Private Sub PaintTree()
'       Dim drawTree As New SSPSTFacilityTypeDrawTree(facilityNodes, pnlDrawBoard.Font)
'        Dim pic As Bitmap = drawTree.Render()
'        picTree.Size = pic.Size
'        picTree.Image = Nothing
'
'        picTree.Image = pic
'        picTree.Invalidate()
'End Sub


Public Class SSPSTFacilityTypeDrawTree
    Private data As List(Of SSPSTFacilityNode)
    Private _nodes As List(Of TreeNode)
    Private _g As System.Drawing.Graphics
    Private _font As Font
    Private _root As TreeNode
    Dim space As Single = 10
    Dim currLevel As Integer = 0
    Dim recMargin As Single = 10
    Dim textPadding As Single = 5
    Dim vertSpace As Integer = 30
    Private maxLevel As Integer = 0
    Private _maxVertical As Integer
    Public ReadOnly Property MaxVertical As Integer
        Get
            Return _maxVertical
        End Get
    End Property
    Private _maxHorizontal As Integer
    Public ReadOnly Property MaxHorizontal As Integer
        Get
            Return _maxHorizontal
        End Get
    End Property

    Public Sub New(ByVal facilityNodes As List(Of SSPSTFacilityNode), f As Font)
        data = facilityNodes
        _font = f
        _maxVertical = 0
        _maxHorizontal = 0
        _nodes = New List(Of TreeNode)

        InitRoot()
    End Sub

    Private Sub InitRoot()
        Dim root As SSPSTFacilityNode = data.FirstOrDefault(Function(x) x.ParentGroupID = 0)

        root.GroupID = -1

        _root = New TreeNode(root, Nothing, _g)
        _root.Level = 0

        _nodes.Add(_root)
        _root.Children = GetChildNodes(_root)
    End Sub

    Private Function GetChildNodes(parent As TreeNode) As List(Of TreeNode)
        Dim nodes As New List(Of TreeNode)

        For Each item As SSPSTFacilityNode In data.Where(Function(x) x.ParentGroupID = parent.data.GroupID)
            Dim child As New TreeNode(item, parent, _g)
            child.Level = parent.Level + 1
            If child.Level > maxLevel Then maxLevel = child.Level
            _nodes.Add(child)
            child.Children = GetChildNodes(child)
            nodes.Add(child)
        Next

        Return nodes
    End Function

    Public Function Render() As Bitmap
        For i As Integer = maxLevel To 0 Step -1
            SetRectangles(_root, i)
        Next

        Dim pic As New Bitmap(MaxHorizontal, MaxVertical)

        _g = Graphics.FromImage(pic)

        _g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        _g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        _g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
        _g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality

        DrawNodes(_root)
        ConnectTheDots(_root)

        Return pic
    End Function

    Private Sub DrawNodes(node As TreeNode)
        _g.DrawRectangle(Pens.Black, node.Rectangle)
        _g.DrawString(node.ToString(), _font, Brushes.Red, New Point(CInt(node.Rectangle.X + textPadding), CInt(node.Rectangle.Y + textPadding)))


        For Each child As TreeNode In node.Children
            DrawNodes(child)
        Next
    End Sub

    Private Function SetRectangles(node As TreeNode, level As Integer) As Boolean
        Dim levelMatched As Boolean = False
        If node.Level = level Then
            Dim x As Integer = GetXFromChildren(node)
            Dim y As Integer = CInt(2 * vertSpace * level + node.NodeTextSize(_font).Height + recMargin)

            If x = 0 Then x = GetXFromCousin(node)

            If _root.Level = level Then
                _root.Rectangle = New Rectangle(New Point(x, y), New Size(CInt(_root.NodeTextSize(_font).Width + recMargin), CInt(_root.NodeTextSize(_font).Height + recMargin)))

                If _root.Rectangle.Y + _root.Rectangle.Height > MaxVertical Then
                    _maxVertical = _root.Rectangle.Y + _root.Rectangle.Height + 10
                End If
                If _root.Rectangle.X + _root.Rectangle.Width > MaxHorizontal Then
                    _maxHorizontal = _root.Rectangle.X + _root.Rectangle.Width + 10
                End If
            Else
                For Each child As TreeNode In node.Parent.Children
                    Dim childX As Integer = GetXFromChildren(child)

                    If childX <> 0 Then
                        If x > childX Then
                            ShiftChildrenAndTheirCousins(child, CInt(x - childX + space))
                            x = GetXFromChildren(child)
                        Else
                            x = childX
                        End If

                    End If

                    child.Rectangle = New Rectangle(New Point(x, y), New Size(CInt(child.NodeTextSize(_font).Width + recMargin), CInt(child.NodeTextSize(_font).Height + recMargin)))

                    If child.Rectangle.Y + child.Rectangle.Height > MaxVertical Then
                        _maxVertical = child.Rectangle.Y + child.Rectangle.Height + 10
                    End If
                    If child.Rectangle.X + child.Rectangle.Width > MaxHorizontal Then
                        _maxHorizontal = child.Rectangle.X + child.Rectangle.Width + 10
                    End If
                    

                    x += CInt(child.Rectangle.Width + space)
                Next
            End If

            levelMatched = True
        Else
            For Each child As TreeNode In node.Children
                If SetRectangles(child, level) Then Exit For
            Next
        End If

        Return levelMatched
    End Function

    Private Sub ShiftChildrenAndTheirCousins(node As TreeNode, shiftValue As Integer)
        'shift children
        For Each child As TreeNode In node.Children
            If child.ToString = "Dirt - 2" Then
                Stop
            End If
            child.Rectangle = New Rectangle(New Point(child.Rectangle.X + shiftValue, child.Rectangle.Y), New Size(child.Rectangle.Width, child.Rectangle.Height))
            ShiftChildrenAndTheirCousins(child, shiftValue)
        Next

        'shift right cousins
        Dim foundMyParent As Boolean = False

        For Each uncle As TreeNode In _nodes.Where(Function(y) y.Level = node.Parent.Level)
            If uncle Is node.Parent Then
                foundMyParent = True
            ElseIf foundMyParent Then
                For Each child As TreeNode In uncle.Children
                    ShiftChildrenAndTheirCousins(child, shiftValue)
                Next
            End If
        Next

        'Dim toMyRight As Boolean = False

        'For Each cousin As TreeNode In node.Parent.Children
        '    If toMyRight Then
        '        ShiftChildrenAndTheirCousins(cousin, shiftValue)
        '        Exit For
        '    End If
        '    If cousin Is node Then toMyRight = True
        'Next
    End Sub

    Private Function GetXFromCousin(node As TreeNode) As Integer
        Dim x As Integer = 0

        'If Not node.Parent Is Nothing AndAlso node.Parent.Parent IsNot Nothing Then
        '    For Each child As TreeNode In node.Parent.Parent.Children

        '        If child Is node.Parent Then
        '            Exit For

        '        Else
        '            For Each cousin As TreeNode In child.Children
        '                x = CInt(cousin.Rectangle.X + cousin.Rectangle.Width + space)
        '            Next

        '        End If
        '    Next
        'End If

        If Not node.Parent Is Nothing Then
            For Each item As TreeNode In _nodes.Where(Function(y) y.Level = node.Parent.Level)
                If item Is node.Parent Then
                    Exit For
                Else
                    For Each child As TreeNode In item.Children
                        x = CInt(child.Rectangle.X + child.Rectangle.Width + space)
                    Next
                End If
            Next
        End If

        'Dim rightMostCousin As TreeNode = _nodes.Where(Function(y) y.Level = node.Level AndAlso y.Parent IsNot node.Parent).OrderByDescending(Function(y) y.Rectangle.X).Last()
        'Dim rightMostCousin As TreeNode = _nodes.Where(Function(y) y.Level = node.Level AndAlso y.Parent IsNot node.Parent).OrderByDescending(Function(y) y.Rectangle.X).Last()

        'If rightMostCousin IsNot Nothing Then
        '    x = CInt(rightMostCousin.Rectangle.X + rightMostCousin.Rectangle.Width + space)
        'End If

        'Dim toMyRight As Boolean = False

        'For Each item As TreeNode In _nodes.Where(Function(y) y.Level = node.Level AndAlso y.Parent IsNot node.Parent).OrderByDescending(Function(y) y.Rectangle.X).Last()

        '    If toMyRight Then
        '        x = CInt(item.Rectangle.X + item.Rectangle.Width + space)
        '        Exit For
        '    End If
        '    If item Is node Then toMyRight = True
        'Next

        Return x
    End Function

    Private Function GetXFromChildren(node As TreeNode) As Integer
        Dim leftX As Integer = 0
        Dim rightX As Integer = 0

        Dim firstLoop As Boolean = True

        For Each child As TreeNode In node.Children
            If firstLoop Then
                leftX = child.Rectangle.X
                rightX = child.Rectangle.X + child.Rectangle.Width
                firstLoop = False
            Else
                If leftX > child.Rectangle.X Then leftX = child.Rectangle.X
                If rightX < child.Rectangle.X + child.Rectangle.Width Then rightX = child.Rectangle.X + child.Rectangle.Width
            End If
        Next

        leftX += CInt(((rightX - leftX) / 2) - ((node.NodeTextSize(_font).Width + recMargin) / 2))

        If leftX < 0 Then leftX = 0

        Return leftX

    End Function

    Private Sub ConnectTheDots(node As TreeNode)
        If node.Children.Count > 0 Then
            Dim center As Integer = CInt(node.Rectangle.X + (node.Rectangle.Width / 2))
            Dim bottom As Integer = node.Rectangle.Y + node.Rectangle.Height
            Dim top As Integer = node.Children(0).Rectangle.Y
            Dim len As Integer = CInt((top - bottom) / 2)

            _g.DrawLine(Pens.Black, New Point(center, bottom), New Point(center, bottom + len))

            For Each child As TreeNode In node.Children
                Dim childCenter As Integer = CInt(child.Rectangle.X + (child.Rectangle.Width / 2))

                _g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

                _g.DrawLine(Pens.Black, New Point(childCenter, top), New Point(childCenter, top - len))
                _g.DrawLine(Pens.Black, New Point(childCenter, top - len), New Point(center, bottom + len))
                ConnectTheDots(child)
            Next
        End If
    End Sub


    Protected Class TreeNode
        Public Property data As SSPSTFacilityNode
        Public Property Rectangle As Rectangle
        Public Property Graphics As Graphics
        Public Property Children As List(Of TreeNode)
        Public Property Level As Integer
        Public Property Parent As TreeNode

        Public Sub New(facNode As SSPSTFacilityNode, parent As TreeNode, g As Graphics)
            data = facNode
            Graphics = g
            Children = New List(Of TreeNode)
            Me.Parent = parent
        End Sub

        Public Function NodeTextSize(font As Font) As SizeF

            Dim bmp As New Bitmap(1, 1)
            Dim g As Graphics = System.Drawing.Graphics.FromImage(bmp)
            Return g.MeasureString(Me.ToString, font)

        End Function

        Public Overrides Function ToString() As String
            Return data.GroupName & " - " & data.Result.ToString
        End Function
    End Class


End Class


