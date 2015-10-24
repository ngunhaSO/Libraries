Dim xInitial As Integer = 30
    Dim yInitial As Integer = 160
    Dim x As Integer = 20
    Dim y As Integer = 20

    Private Sub locateNodes(drawingPanel As Panel, originalTree As TreeView, tempTree As TreeView, ByRef labeledNodes As ArrayList)
        tempTree.Nodes.Clear()
        Dim labelAux As Label
        labeledNodes = New ArrayList
        If originalTree.Nodes.Count = 0 Then
            Return
        End If
        Dim board As Graphics = drawingPanel.CreateGraphics()

        Dim maxDepth As Integer = 0
        Dim list As ArrayList
        list = New ArrayList()
        Dim totalNodes As Integer = getAllChildNodes(originalTree.Nodes, list)
        For Each n As TreeNode In list
            If n.Level > maxDepth Then
                maxDepth = n.Level 'maximum depth
            End If
        Next
        Dim listByLevel(maxDepth) As ArrayList
        For Each n As TreeNode In list
            If n.Level = 0 Then
                tempTree.Nodes.Add(CType(n.Clone(), TreeNode))
            End If
        Next
        list = New ArrayList
        totalNodes = getAllChildNodes(tempTree.Nodes, list)
        For Each n As TreeNode In list
            If n.Nodes.Count = 0 And n.Level < maxDepth Then
                addBlankNode(n, maxDepth) 'recursive func
            End If
        Next
        list = New ArrayList
        totalNodes = getAllChildNodes(tempTree.Nodes, list)
        For Each n As TreeNode In list
            If listByLevel(n.Level) Is Nothing Then
                listByLevel(n.Level) = New ArrayList()
            End If
            listByLevel(n.Level).Add(n)
        Next
        x = xInitial
        y = yInitial
        For z As Integer = maxDepth To 0 Step -1
            For index As Integer = 0 To listByLevel(z).Count - 1 Step 1
                Dim nodeToPaint As TreeNode = CType((listByLevel(z)(index)), TreeNode)
                labelAux = New Label()
                labelAux.Name = nodeToPaint.Name
                labelAux.Text = nodeToPaint.Text.Substring(0, nodeToPaint.Text.IndexOf("-"))
                labelAux.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                labelAux.AutoSize = True
                labelAux.BorderStyle = BorderStyle.FixedSingle
                labelAux.Tag = nodeToPaint
                unionNodeLinesPen = New Pen(Color.Red)
                labelAux.Location = New Point(x, y)
                nodeToPaint.Tag = New Rectangle(labelAux.Left, labelAux.Top, labelAux.PreferredWidth, labelAux.PreferredHeight)
                If z < maxDepth Then
                    Dim posFirstChild As Point = getRectangleCenter(CType(nodeToPaint.FirstNode.Tag, Rectangle))
                    Dim posLastChild As Point = getRectangleCenter(CType(nodeToPaint.LastNode.Tag, Rectangle))
                    Dim relocatedPoint As Point = labelAux.Location
                    relocatedPoint.X = (posFirstChild.X + posLastChild.X) / 2 - labelAux.PreferredWidth / 2
                    labelAux.Location = relocatedPoint
                    nodeToPaint.Tag = New Rectangle(labelAux.Left, labelAux.Top, labelAux.PreferredWidth, labelAux.PreferredHeight)
                End If
                For Each t As TreeNode In nodeToPaint.Nodes
                    If t.Text.Trim = "X-" Then
                        Continue For
                    End If
                    Dim r As Rectangle = New Rectangle(labelAux.Left, labelAux.Top, labelAux.PreferredWidth, labelAux.PreferredHeight)
                    Dim parentCenterPos As Point = getRectangleCenter(r)
                    Dim childCenterPos As Point = getRectangleCenter(CType(t.Tag, Rectangle))
                    childCenterPos.Y = (CType(t.Tag, Rectangle)).Top
                    board.DrawLine(unionNodeLinesPen, parentCenterPos, childCenterPos)
                Next
                labeledNodes.Add(labelAux)
                x = labelAux.Left + labelAux.PreferredWidth + 100
            Next
            y = y - 30
        Next
    End Sub

    Private Sub pnlDrawBoard_Paint()
        Dim labels As ArrayList = New ArrayList
        tvFacilityCopy.Nodes.Clear()
        copyTree(tvFacility, tvFacilityCopy)
        locateNodes(pnlDrawBoard, tvFacilityCopy, trvTempTree, labels)
        pnlDrawBoard.AutoScroll = True
        For i As Integer = 0 To labels.Count - 1 Step 1
            If CType(labels(i), Label).Text.Trim = "X" Then
                Continue For
            End If
            pnlDrawBoard.Controls.Add(CType(labels(i), Label))
        Next
    End Sub

    Private Sub copyTree(treeview1 As TreeView, treeview2 As TreeView)
        Dim newTn As TreeNode
        For Each tn As TreeNode In treeview1.Nodes
            newTn = New TreeNode(tn.Text)
            CopyChilds(newTn, tn)
            treeview2.Nodes.Add(newTn)
        Next
    End Sub

    Private Sub CopyChilds(parent As TreeNode, willCopied As TreeNode)
        Dim newTn As TreeNode
        For Each tn As TreeNode In willCopied.Nodes
            newTn = New TreeNode(tn.Text)
            CopyChilds(newTn, tn)
            parent.Nodes.Add(newTn)
        Next
    End Sub

    Private Sub tbCtrl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tbCtrl.SelectedIndexChanged
        If tbCtrl.SelectedTab.Equals(tbCtrl.TabPages("tpFacilityTree")) Then
            pnlDrawBoard.Controls.Clear()
            pnlDrawBoard.Refresh()
            pnlDrawBoard_Paint()
        End If
    End Sub

    Private Sub splitContainerFacility_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles splitContainerFacility.SplitterMoved
        pnlDrawBoard.Refresh()
    End Sub

    Private Sub pnlDrawBoard_Paint(sender As Object, e As PaintEventArgs) Handles pnlDrawBoard.Paint
        e.Graphics.TranslateTransform(pnlDrawBoard.AutoScrollPosition.X, pnlDrawBoard.AutoScrollPosition.Y)
        trvTempTree.Nodes.Clear()
        Dim labeledNodes As ArrayList
        Dim labelAux As Label
        labeledNodes = New ArrayList
        If tvFacilityCopy.Nodes.Count = 0 Then
            Return
        End If
        Dim board As Graphics = e.Graphics
        
        Dim maxDepth As Integer = 0
        Dim list As ArrayList
        list = New ArrayList()
        Dim totalNodes As Integer = getAllChildNodes(tvFacilityCopy.Nodes, list)
        For Each n As TreeNode In list
            If n.Level > maxDepth Then
                maxDepth = n.Level
            End If
        Next
        Dim listByLevel(maxDepth) As ArrayList
        For Each n As TreeNode In list
            If n.Level = 0 Then
                trvTempTree.Nodes.Add(CType(n.Clone(), TreeNode))
            End If
        Next
        list = New ArrayList
        totalNodes = getAllChildNodes(trvTempTree.Nodes, list)
        For Each n As TreeNode In list
            If n.Nodes.Count = 0 And n.Level < maxDepth Then
                addBlankNode(n, maxDepth) 'recursive func
            End If
        Next
        list = New ArrayList
        totalNodes = getAllChildNodes(trvTempTree.Nodes, list)
        For Each n As TreeNode In list
            If listByLevel(n.Level) Is Nothing Then
                listByLevel(n.Level) = New ArrayList()
            End If
            listByLevel(n.Level).Add(n)
        Next
        x = xInitial
        y = yInitial
        For z As Integer = maxDepth To 0 Step -1
            For index As Integer = 0 To listByLevel(z).Count - 1 Step 1
                Dim nodeToPaint As TreeNode = CType((listByLevel(z)(index)), TreeNode)
                labelAux = New Label()
                labelAux.Name = nodeToPaint.Name
                labelAux.Text = nodeToPaint.Text.Substring(0, nodeToPaint.Text.IndexOf("-"))
                labelAux.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                labelAux.AutoSize = True
                labelAux.BorderStyle = BorderStyle.FixedSingle
                labelAux.Tag = nodeToPaint
                unionNodeLinesPen = New Pen(Color.Red)
                labelAux.Location = New Point(x, y)
                nodeToPaint.Tag = New Rectangle(labelAux.Left, labelAux.Top, labelAux.PreferredWidth, labelAux.PreferredHeight)
                If z < maxDepth Then
                    Dim posFirstChild As Point = getRectangleCenter(CType(nodeToPaint.FirstNode.Tag, Rectangle))
                    Dim posLastChild As Point = getRectangleCenter(CType(nodeToPaint.LastNode.Tag, Rectangle))
                    Dim relocatedPoint As Point = labelAux.Location
                    relocatedPoint.X = (posFirstChild.X + posLastChild.X) / 2 - labelAux.PreferredWidth / 2
                    labelAux.Location = relocatedPoint
                    nodeToPaint.Tag = New Rectangle(labelAux.Left, labelAux.Top, labelAux.PreferredWidth, labelAux.PreferredHeight)
                End If
                For Each t As TreeNode In nodeToPaint.Nodes
                    If t.Text.Trim = "X-" Then
                        Continue For
                    End If
                    Dim r As Rectangle = New Rectangle(labelAux.Left, labelAux.Top, labelAux.PreferredWidth, labelAux.PreferredHeight)
                    Dim parentCenterPos As Point = getRectangleCenter(r)
                    Dim childCenterPos As Point = getRectangleCenter(CType(t.Tag, Rectangle))
                    childCenterPos.Y = (CType(t.Tag, Rectangle)).Top
                    board.DrawLine(unionNodeLinesPen, parentCenterPos, childCenterPos)
                Next
                labeledNodes.Add(labelAux)
                x = labelAux.Left + labelAux.PreferredWidth + 100
            Next
            y = y - 30
        Next
    End Sub

    Private Sub pnlDrawBoard_Scroll(sender As Object, e As ScrollEventArgs) Handles pnlDrawBoard.Scroll
        pnlDrawBoard.Invalidate()
    End Sub


    Private dragging As Boolean
    Private Sub pnlDrawBoard_MouseDown(sender As Object, e As MouseEventArgs) Handles pnlDrawBoard.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            dragging = True
            xInitial = e.Location.X + pnlDrawBoard.Location.X
            yInitial = e.Location.Y + pnlDrawBoard.Location.Y
        End If
    End Sub

    Private Sub pnlDrawBoard_MouseMove(sender As Object, e As MouseEventArgs) Handles pnlDrawBoard.MouseMove
        If dragging Then
            pnlDrawBoard.Location = New Point(pnlDrawBoard.Left + e.Location.X - xInitial, pnlDrawBoard.Top + e.Location.Y - yInitial)
            pnlDrawBoard.Controls.Clear()
            pnlDrawBoard.Refresh()
            pnlDrawBoard.Invalidate()
            pnlDrawBoard_Paint()
        End If
    End Sub

    Private Sub pnlDrawBoard_MouseUp(sender As Object, e As MouseEventArgs) Handles pnlDrawBoard.MouseUp
        dragging = False
    End Sub
