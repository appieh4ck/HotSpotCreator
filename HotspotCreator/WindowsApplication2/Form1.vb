Imports System.Net.NetworkInformation

Public Class Form1
    Dim virtAdapterName As String
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        Process.Start("CMD", "/C netsh wlan set hostednetwork mode=allow ssid=" & TextBox1.Text & " key=" & TextBox2.Text)
        Process.Start("CMD", "/C netsh wlan start hostednetwork")

        Threading.Thread.Sleep(500)

        EnableDisableICS(CStr(ComboBox1.SelectedItem), virtAdapterName, 1)

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

            If adapter.Description = "Microsoft Hosted Network Virtual Adapter" Or adapter.Description = "Microsoft Virtual WiFi Miniport Adapter" Then

                virtAdapterName = adapter.Name
            End If
        Next adapter

    End Sub


    Private Sub chkShowPass_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPass.CheckedChanged
        If chkShowPass.Checked Then
            TextBox2.UseSystemPasswordChar = False
        Else
            TextBox2.UseSystemPasswordChar = True
        End If

    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        EnableDisableICS(CStr(ComboBox1.SelectedItem), virtAdapterName, 0)
        Process.Start("CMD", "/C netsh wlan stop hostednetwork")
        RefreshComboBox()
    End Sub


    Public Class MyUtilities
        Shared Sub RunCommandCom(command As String, arguments As String, permanent As Boolean)
            Dim p As Process = New Process()
            Dim pi As ProcessStartInfo = New ProcessStartInfo()
            pi.Arguments = " " + If(permanent = True, "/K", "/C") + " " + command + " " + arguments
            pi.FileName = "cmd.exe"
            p.StartInfo = pi
            p.Start()
        End Sub
    End Class
End Class
