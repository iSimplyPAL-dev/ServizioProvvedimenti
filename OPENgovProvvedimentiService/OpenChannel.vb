Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters
Imports System.Runtime.Remoting.ObjRef
Imports System.Threading
Imports System.Collections
Imports ComPlusInterface
Imports System.ServiceProcess
Imports System.Configuration

Imports log4net
Imports log4net.Config
Imports System.IO
''' <summary>
''' Classe di iniziazione del servizio.
''' 
''' Il servizio si occupa di calcolare e gestire gli atti di accertamento.
''' </summary>
Public Class OPENChannel
  Inherits System.ServiceProcess.ServiceBase
    Private chan As HttpChannel
	Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(OPENChannel))
    'true --> quando si deve buildare il servizio
    'false --> quando si vuole lanciare in console per il debug
    Private Shared _runService As Boolean = True

#Region " Component Designer generated code "

    Public Sub New()
    MyBase.New()

    ' This call is required by the Component Designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call

  End Sub

  'UserService overrides dispose to clean up the component list.
  Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing Then
      If Not (components Is Nothing) Then
        components.Dispose()
      End If
    End If
    MyBase.Dispose(disposing)
  End Sub

  ' The main entry point for the process
  <MTAThread()> _
  Shared Sub Main()
    Dim ServicesToRun() As System.ServiceProcess.ServiceBase

    ' More than one NT Service may run within the same process. To add
    ' another service to this process, change the following line to
    ' create a second service object. For example,
    '
    '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
		'
		If (_runService) Then
			ServicesToRun = New System.ServiceProcess.ServiceBase() {New OPENChannel}
			System.ServiceProcess.ServiceBase.Run(ServicesToRun)
		Else
			Dim oServizio As New OPENChannel
			oServizio.OnStart(Nothing)
            Console.WriteLine("Benvenuto noi siamo il Motore Provvedimenti...")
            Console.WriteLine("3...2...1...partito...")
            Console.ReadLine()
		End If
	End Sub

	'Required by the Component Designer
	Private components As System.ComponentModel.IContainer

	' NOTE: The following procedure is required by the Component Designer
	' It can be modified using the Component Designer.  
	' Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		components = New System.ComponentModel.Container
		Me.ServiceName = "Service1"
	End Sub

#End Region

  Protected Overrides Sub OnStart(ByVal args() As String)
		Dim pathfileinfo As String = ConfigurationSettings.AppSettings("pathfileconflog4net").ToString()
		Dim fileconfiglog4net As New FileInfo(pathfileinfo)
		XmlConfigurator.ConfigureAndWatch(fileconfiglog4net)

		RegistraServizi()
	End Sub

	Protected Overrides Sub OnStop()
		ChannelServices.UnregisterChannel(chan)
	End Sub

    'Private Sub RegistraServizi()
    '	Try
    '		'Dim serverProv As BinaryServerFormatterSinkProvider = New BinaryServerFormatterSinkProvider
    '		Dim serverProv As SoapServerFormatterSinkProvider = New SoapServerFormatterSinkProvider
    '		serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
    '		'Dim clientProv As BinaryClientFormatterSinkProvider = New BinaryClientFormatterSinkProvider
    '		Dim clientProv As SoapClientFormatterSinkProvider = New SoapClientFormatterSinkProvider

    '		Dim props As IDictionary = New Hashtable

    '		props("port") = ConfigurationSettings.AppSettings("HTTP_PORT")
    '		props("typeFilterLevel") = TypeFilterLevel.Full

    '		Dim chan As HttpChannel = New HttpChannel(props, clientProv, serverProv)

    '		ChannelServices.RegisterChannel(chan)

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneLettere), "COMPLUSElaborazioneLettere.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPLUSElaborazioneLettere")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneQuestionari), "COMPLUSElaborazioneQuestionari.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPLUSElaborazioneQuestionari")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneLiquidazioni), "COMPLUSElaborazioneLiquidazioni.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPLUSElaborazioneLiquidazioni")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneAtti), "COMPLUSElaborazioneAtti.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPLUSElaborazioneAtti")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneAccertamenti), "COMPLUSElaborazioneAccertamenti.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPLUSElaborazioneAccertamenti")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziGestioneConfigurazione), "COMPLUSGestioneConfigurazione.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPLUSGestioneConfigurazione")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneStampe), "ElaborazioneStampe.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato ElaborazioneStampe")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziFreezer), "COMPlusFreezer.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPlusFreezer")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneANCI_CNC), "ElaborazioneANCI_CNC.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato ElaborazioneANCI_CNC")

    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneRicercheCatasto), "COMPLUSRicercheCatasto.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPLUSRicercheCatasto")

    '           ' Ruolo Coattivo ICI
    '           RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziRuoloCoattivo), "COMPLUSRuoloCoattivo.soap", WellKnownObjectMode.SingleCall)
    '           Log.Debug("Registrato COMPLUSRuoloCoattivo")

    '       Catch Err As Exception
    '		Throw New Exception(Err.Message)
    '	End Try
    'End Sub
    Private Sub RegistraServizi()
        Try
            Dim serverProv As SoapServerFormatterSinkProvider = New SoapServerFormatterSinkProvider
            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
            Dim clientProv As SoapClientFormatterSinkProvider = New SoapClientFormatterSinkProvider

            Dim props As IDictionary = New Hashtable

            props("port") = ConfigurationSettings.AppSettings("HTTP_PORT")
            props("typeFilterLevel") = TypeFilterLevel.Full

            Dim chan As HttpChannel = New HttpChannel(props, clientProv, serverProv)

            ChannelServices.RegisterChannel(chan)

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneLiquidazioni), "COMPLUSElaborazioneLiquidazioni.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSElaborazioneLiquidazioni")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneAtti), "COMPLUSElaborazioneAtti.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSElaborazioneAtti")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneAccertamenti), "COMPLUSElaborazioneAccertamenti.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSElaborazioneAccertamenti")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziGestioneConfigurazione), "COMPLUSGestioneConfigurazione.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSGestioneConfigurazione")
        Catch Err As Exception
            Throw New Exception(Err.Message)
        End Try
    End Sub
End Class
