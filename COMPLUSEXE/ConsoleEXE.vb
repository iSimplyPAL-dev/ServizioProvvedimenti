Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels.Tcp
Imports System.Runtime.Remoting
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters
Imports System.Runtime.Remoting.ObjRef
Imports System.Threading
Imports System.Collections
Imports ComPlusInterface

Imports log4net
Imports log4net.Config
Imports System.IO
Imports System.Configuration

Public Class ConsoleEXE

    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(ConsoleEXE))

    Public Shared Sub Main()

        Dim pathfileinfo As String = ConfigurationSettings.AppSettings("pathfileconflog4net").ToString()
        Dim fileconfiglog4net As New FileInfo(pathfileinfo)
        XmlConfigurator.ConfigureAndWatch(fileconfiglog4net)

        RegistraServizi()
    End Sub

    Private Shared Sub RegistraServizi()



        Try

            Dim clientProvider As BinaryClientFormatterSinkProvider = New BinaryClientFormatterSinkProvider
            Dim serverProvider As BinaryServerFormatterSinkProvider = New BinaryServerFormatterSinkProvider
            serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
            Dim props As IDictionary = New Hashtable
            props("port") = ConfigurationSettings.AppSettings("TCP_PORT")
            props("typeFilterLevel") = TypeFilterLevel.Full
            Dim TCPchan As TcpChannel = New TcpChannel(props, clientProvider, serverProvider)

            'Dim serverProv As BinaryServerFormatterSinkProvider = New BinaryServerFormatterSinkProvider
            Dim serverProv As SoapServerFormatterSinkProvider = New SoapServerFormatterSinkProvider
            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
            'Dim clientProv As BinaryClientFormatterSinkProvider = New BinaryClientFormatterSinkProvider
            Dim clientProv As SoapClientFormatterSinkProvider = New SoapClientFormatterSinkProvider
            props("port") = ConfigurationSettings.AppSettings("HTTP_PORT")
            props("typeFilterLevel") = TypeFilterLevel.Full
            Dim HTTPchan As HttpChannel = New HttpChannel(props, clientProv, serverProv)


            ChannelServices.RegisterChannel(TCPchan)
            ChannelServices.RegisterChannel(HTTPchan)


            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneLettere), _
            "COMPLUSElaborazioneLettere.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSElaborazioneLettere")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneQuestionari), _
            "COMPLUSElaborazioneQuestionari.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSElaborazioneQuestionari")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneLiquidazioni), _
            "COMPLUSElaborazioneLiquidazioni.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSElaborazioneLiquidazioni")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneAtti), _
            "COMPLUSElaborazioneAtti.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSElaborazioneAtti")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneAccertamenti), _
            "COMPLUSElaborazioneAccertamenti.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSElaborazioneAccertamenti")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziGestioneConfigurazione), _
            "COMPLUSGestioneConfigurazione.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSGestioneConfigurazione")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneStampe), _
            "ElaborazioneStampe.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato ElaborazioneStampe")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziFreezer), _
            "COMPlusFreezer.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPlusFreezer")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneANCI_CNC), _
            "ElaborazioneANCI_CNC.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato ElaborazioneANCI_CNC")

            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziElaborazioneRicercheCatasto), _
            "COMPLUSRicercheCatasto.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSRicercheCatasto")

            ' Ruolo Coattivo ICI
            RemotingConfiguration.RegisterWellKnownServiceType(GetType(ServiziRuoloCoattivo), _
            "COMPLUSRuoloCoattivo.soap", WellKnownObjectMode.SingleCall)
            Log.Debug("Registrato COMPLUSRuoloCoattivo")

            Dim wkste As WellKnownServiceTypeEntry() = RemotingConfiguration.GetRegisteredWellKnownServiceTypes()
            Dim s As String = wkste(0).ObjectUri


            Dim WellKnownServiceTypeEntries As WellKnownServiceTypeEntry() = RemotingConfiguration.GetRegisteredWellKnownServiceTypes()
            Dim entry As WellKnownServiceTypeEntry

            For Each entry In WellKnownServiceTypeEntries
                Console.WriteLine(String.Format("{0}", entry))
            Next entry

            Console.WriteLine("Display registerd channels")
            Dim Channels As IChannel() = ChannelServices.RegisteredChannels
            Dim CHN As IChannel
            For Each CHN In Channels
                Console.WriteLine(String.Format("Channel-Name='{0}'; Priority='{1}'", CHN.ChannelName, CHN.ChannelPriority))
            Next CHN
            Console.WriteLine()
            Console.WriteLine("Any key to exit")
            Console.ReadLine()


            ChannelServices.UnregisterChannel(TCPchan)
            ChannelServices.UnregisterChannel(HTTPchan)

        Catch Err As Exception
            Throw New Exception(Err.Message)
        End Try

    End Sub

End Class
