Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovProvvedimenti
''' <summary>
''' Classe rende disponibili le interfacce di configurazione
''' </summary>
Public Class ServiziGestioneConfigurazione
    Inherits MarshalByRefObject
    Implements IGestioneConfigurazione


    Public Function GetTipoVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetTipoVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetTipoVoci = objCOMPlusBusinessObject.getTipoVoci(StringConnectionProvv, IdEnte)

            Return GetTipoVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myConnectionString"></param>
    ''' <returns></returns>
    ''' <revisionHistory><revision date="14/804/2020">Sono cambiate le regole di applicazione sanzione</revision></revisionHistory>
    Public Function GetSanzioniRavvedimentoOperoso(myConnectionString As String) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetSanzioniRavvedimentoOperoso

        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            GetSanzioniRavvedimentoOperoso = objCOMPlusBusinessObject.GetSanzioniRavvedimentoOperoso(myConnectionString)

            Return GetSanzioniRavvedimentoOperoso

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function SetTipoVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable, ByRef strIDTIPOVOCE As String) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetTipoVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetTipoVoci = objCOMPlusBusinessObject.SetTipoVoci(StringConnectionProvv, IdEnte, strIDTIPOVOCE)

            Return SetTipoVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function DelTipoVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelTipoVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelTipoVoci = objCOMPlusBusinessObject.DelTipoVoci(StringConnectionProvv, IdEnte)

            Return DelTipoVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetValoriVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetValoriVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetValoriVoci = objCOMPlusBusinessObject.GetValoriVoci(StringConnectionProvv, IdEnte)

            Return GetValoriVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetTipoInteresse(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetTipoInteresse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetTipoInteresse = objCOMPlusBusinessObject.GetTipoInteresse(StringConnectionProvv, IdEnte)

            Return GetTipoInteresse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetAnniProvvedimenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetAnniProvvedimenti = objCOMPlusBusinessObject.GetAnniProvvedimenti(StringConnectionProvv, IdEnte)

            Return GetAnniProvvedimenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetValoriVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable, ByRef intRetVal As Integer) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetValoriVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetValoriVoci = objCOMPlusBusinessObject.SetValoriVoci(StringConnectionProvv, IdEnte, intRetVal)

            Return SetValoriVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function DelValoriVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelValoriVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelValoriVoci = objCOMPlusBusinessObject.DelValoriVoci(StringConnectionProvv, IdEnte)

            Return DelValoriVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function DelAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelAnniProvvedimenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelAnniProvvedimenti = objCOMPlusBusinessObject.DelAnniProvvedimenti(StringConnectionProvv, IdEnte)

            Return DelAnniProvvedimenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function SetTassiInteresse(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetTassiInteresse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetTassiInteresse = objCOMPlusBusinessObject.SetTassiInteresse(StringConnectionProvv, IdEnte)

            Return SetTassiInteresse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function SetAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetAnniProvvedimenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetAnniProvvedimenti = objCOMPlusBusinessObject.SetAnniProvvedimenti(StringConnectionProvv, IdEnte)

            Return SetAnniProvvedimenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function DelTassiInteresse(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelTassiInteresse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelTassiInteresse = objCOMPlusBusinessObject.DelTassiInteresse(StringConnectionProvv, IdEnte)

            Return DelTassiInteresse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function GetMotivazioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetMotivazioni
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetMotivazioni = objCOMPlusBusinessObject.GetMotivazioni(StringConnectionProvv, IdEnte)

            Return GetMotivazioni

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetMotivazioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetMotivazioni
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetMotivazioni = objCOMPlusBusinessObject.SetMotivazioni(StringConnectionProvv, IdEnte)

            Return SetMotivazioni

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function DelMotivazioni(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelMotivazioni
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelMotivazioni = objCOMPlusBusinessObject.DelMotivazioni(StringConnectionProvv, IdEnte)

            Return DelMotivazioni

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetTipologieVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetTipologieVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetTipologieVoci = objCOMPlusBusinessObject.GetTipologieVoci(StringConnectionProvv, IdEnte)

            Return GetTipologieVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetTipologieVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetTipologieVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetTipologieVoci = objCOMPlusBusinessObject.SetTipologieVoci(StringConnectionProvv, IdEnte)

            Return SetTipologieVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function DelTipologieVoci(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelTipologieVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelTipologieVoci = objCOMPlusBusinessObject.DelTipologieVoci(StringConnectionProvv, IdEnte)

            Return DelTipologieVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetGeneraleICI(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetGeneraleICI
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetGeneraleICI = objCOMPlusBusinessObject.GetGeneraleICI(StringConnectionProvv, IdEnte)

            Return GetGeneraleICI

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetGeneraleICI(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetGeneraleICI
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetGeneraleICI = objCOMPlusBusinessObject.SetGeneraleICI(StringConnectionProvv, IdEnte)

            Return SetGeneraleICI

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function GetListTipoPossesso(StringConnectionICI As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetListTipoPossesso
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetListTipoPossesso = objCOMPlusBusinessObject.GetListTipoPossesso(StringConnectionICI)

            Return GetListTipoPossesso

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetListCategorie(StringConnectionICI As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetListCategorie
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetListCategorie = objCOMPlusBusinessObject.GetListCategorie(StringConnectionICI)

            Return GetListCategorie

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetListTipoRendita(StringConnectionICI As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetListTipoRendita
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetListTipoRendita = objCOMPlusBusinessObject.GetListTipoRendita(StringConnectionICI)

            Return GetListTipoRendita

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetListClasse(StringConnectionICI As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetListClasse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetListClasse = objCOMPlusBusinessObject.GetListClasse(StringConnectionICI)

            Return GetListClasse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function



    Public Function GetScadenzaInteressi(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetScadenzaInteressi
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetScadenzaInteressi = objCOMPlusBusinessObject.GetScadenzaInteressi(StringConnectionProvv, IdEnte)

            Return GetScadenzaInteressi

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function SetScadenzaInteressi(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetScadenzaInteressi
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetScadenzaInteressi = objCOMPlusBusinessObject.SetScadenzaInteressi(StringConnectionProvv, IdEnte)

            Return SetScadenzaInteressi

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try


    End Function
End Class
