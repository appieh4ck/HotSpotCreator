Imports System.Net.NetworkInformation
Imports MaterialSkin
Imports MaterialSkin.Controls

Public Class Form1
    Dim virtAdapterName As String
    Private Sub Button1_Click(sender As Object, e As EventArgs)




    End Sub

    Private Function EnableDisableICS(ByVal sPublicConnectionName As String, ByVal sPrivateConnectionName As String, ByVal bEnable As Boolean)
        Dim bFound As Boolean
        Dim oNetSharingManager, oConnectionCollection, oItem, EveryConnection, objNCProps

        oNetSharingManager = CreateObject("HNetCfg.HNetShare.1")

        oConnectionCollection = oNetSharingManager.EnumEveryConnection
        For Each oItem In oConnectionCollection
            EveryConnection = oNetSharingManager.INetSharingConfigurationForINetConnection(oItem)
            objNCProps = oNetSharingManager.NetConnectionProps(oItem)
            If objNCProps.name = sPrivateConnectionName Then
                bFound = True
                'MsgBox("Setting ICS Private to " & bEnable & " on connection: " & objNCProps.name)
                If bEnable Then
                    EveryConnection.EnableSharing(1)
                Else
                    EveryConnection.DisableSharing()
                End If
            End If
        Next

        oConnectionCollection = oNetSharingManager.EnumEveryConnection
        For Each oItem In oConnectionCollection
            EveryConnection = oNetSharingManager.INetSharingConfigurationForINetConnection(oItem)
            objNCProps = oNetSharingManager.NetConnectionProps(oItem)

            If objNCProps.name = sPublicConnectionName Then
                bFound = True
                'MsgBox("Setting ICS Public to " & bEnable & " on connection: " & objNCProps.name)
                If bEnable Then
                    EveryConnection.EnableSharing(0)
                Else
                    EveryConnection.DisableSharing()
                End If
            End If
        Next

    End Function

    Public Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RefreshComboBox()
        DisplayTypeAndAddress()
        Dim SkinManager As MaterialSkinManager = MaterialSkinManager.Instance
        SkinManager.AddFormToManage(Me)
        SkinManager.Theme = MaterialSkinManager.Themes.DARK
        SkinManager.ColorScheme = New ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE)
    End Sub
    Public Sub RefreshComboBox()
        ComboBox1.Items.Clear()
        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As NetworkInterface
        For Each adapter In adapters
            ComboBox1.Items.Add(adapter.Name)
        Next
    End Sub

    Public Sub DisplayTypeAndAddress()
        Dim computerProperties As IPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties()
        Dim nics As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()

        Dim adapter As NetworkInterface
        For Each adapter In nics
            Dim properties As IPInterfaceProperties = adapter.GetIPProperties()

            If adapter.Description = "Microsoft Hosted Network Virtual Adapter" Then

                virtAdapterName = adapter.Name
            End If
        Next adapter

    End Sub


    Private Sub chkShowPass_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs)

    End Sub


    Public Function CMDStart(ByVal strCMD As String)
        Dim startInfo As New ProcessStartInfo("CMD.EXE")
        startInfo.WindowStyle = ProcessWindowStyle.Minimized
        startInfo.WindowStyle = ProcessWindowStyle.Hidden
        startInfo.CreateNoWindow = True
        startInfo.UseShellExecute = False
        startInfo.Arguments = strCMD
        Process.Start(startInfo)
    End Function

    Private Sub MaterialRaisedButton1_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        Try
            If TextBox1.Text = "" Or TextBox2.Text = "" Then
                MsgBox("Gelieve de invoervelden controleren aub")
            Else
                'Process.Start("CMD", "/C netsh wlan set hostednetwork mode=allow ssid=" & TextBox1.Text & " key=" & TextBox2.Text)
                'Process.Start("CMD", "/C netsh wlan start hostednetwork")
                CMDStart(CStr("/C netsh wlan set hostednetwork mode=allow ssid=" & TextBox1.Text & " key=" & TextBox2.Text))
                CMDStart(CStr("/C netsh wlan start hostednetwork"))
                Threading.Thread.Sleep(500)

                EnableDisableICS(CStr(ComboBox1.SelectedItem), virtAdapterName, 1)
            End If
        Catch
        End Try
    End Sub

    Private Sub MaterialFlatButton1_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        EnableDisableICS(CStr(ComboBox1.SelectedItem), virtAdapterName, 0)
        ' Process.Start("CMD", "/C netsh wlan stop hostednetwork")
        CMDStart("/C netsh wlan stop hostednetwork")
        RefreshComboBox()
    End Sub

    Private Sub MaterialCheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPass.CheckedChanged
        If chkShowPass.Checked Then
            TextBox2.UseSystemPasswordChar = False
        Else
            TextBox2.UseSystemPasswordChar = True
        End If

    End Sub
End Class
