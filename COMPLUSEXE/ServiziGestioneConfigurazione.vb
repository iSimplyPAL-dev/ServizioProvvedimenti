Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Public Class ServiziGestioneConfigurazione
    Inherits MarshalByRefObject
    Implements IGestioneConfigurazione

    Public Function GetCapitoli(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetCapitoli
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetCapitoli = objCOMPlusBusinessObject.getCapitoli

            Return GetCapitoli

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetTipoVoci(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetTipoVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetTipoVoci = objCOMPlusBusinessObject.getTipoVoci()

            Return GetTipoVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function SetTipoVoci(ByVal objHashTable As System.Collections.Hashtable, ByRef strIDTIPOVOCE As String) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetTipoVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetTipoVoci = objCOMPlusBusinessObject.SetTipoVoci(strIDTIPOVOCE)

            Return SetTipoVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function DelTipoVoci(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelTipoVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelTipoVoci = objCOMPlusBusinessObject.DelTipoVoci()

            Return DelTipoVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetValoriVoci(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetValoriVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetValoriVoci = objCOMPlusBusinessObject.GetValoriVoci()

            Return GetValoriVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetTipoInteresse(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetTipoInteresse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetTipoInteresse = objCOMPlusBusinessObject.GetTipoInteresse()

            Return GetTipoInteresse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetMaxDateTassiInteresse(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetMaxDateTassiInteresse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetMaxDateTassiInteresse = objCOMPlusBusinessObject.GetMaxDateTassiInteresse()

            Return GetMaxDateTassiInteresse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetAnniProvvedimenti(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetAnniProvvedimenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetAnniProvvedimenti = objCOMPlusBusinessObject.GetAnniProvvedimenti()

            Return GetAnniProvvedimenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetValoriVoci(ByVal objHashTable As System.Collections.Hashtable, ByRef intRetVal As Integer) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetValoriVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetValoriVoci = objCOMPlusBusinessObject.SetValoriVoci(intRetVal)

            Return SetValoriVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function DelValoriVoci(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelValoriVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelValoriVoci = objCOMPlusBusinessObject.DelValoriVoci()

            Return DelValoriVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function DelAnniProvvedimenti(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelAnniProvvedimenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelAnniProvvedimenti = objCOMPlusBusinessObject.DelAnniProvvedimenti()

            Return DelAnniProvvedimenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function SetTassiInteresse(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetTassiInteresse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetTassiInteresse = objCOMPlusBusinessObject.SetTassiInteresse()

            Return SetTassiInteresse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function SetAnniProvvedimenti(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetAnniProvvedimenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetAnniProvvedimenti = objCOMPlusBusinessObject.SetAnniProvvedimenti()

            Return SetAnniProvvedimenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function DelTassiInteresse(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelTassiInteresse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelTassiInteresse = objCOMPlusBusinessObject.DelTassiInteresse()

            Return DelTassiInteresse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetTipoInteresseEmptyAL(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.GetTipoInteresseEmptyAL
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetTipoInteresseEmptyAL = objCOMPlusBusinessObject.GetTipoInteresseEmptyAL()

            Return GetTipoInteresseEmptyAL

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetNoteQuestionari(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetNoteQuestionari
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetNoteQuestionari = objCOMPlusBusinessObject.GetNoteQuestionari()

            Return GetNoteQuestionari

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetNoteQuestionari(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetNoteQuestionari
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetNoteQuestionari = objCOMPlusBusinessObject.SetNoteQuestionari()

            Return SetNoteQuestionari

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function DelNoteQuestionari(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelNoteQuestionari
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelNoteQuestionari = objCOMPlusBusinessObject.DelNoteQuestionari()

            Return DelNoteQuestionari

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function GetDatiLettere(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetDatiLettere
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetDatiLettere = objCOMPlusBusinessObject.GetDatiLettere()

            Return GetDatiLettere

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetDatiLettere(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetDatiLettere
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetDatiLettere = objCOMPlusBusinessObject.SetDatiLettere()

            Return SetDatiLettere

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function DelDatiLettere(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelDatiLettere
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelDatiLettere = objCOMPlusBusinessObject.DelDatiLettere()

            Return DelDatiLettere

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetMotivazioni(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetMotivazioni
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetMotivazioni = objCOMPlusBusinessObject.GetMotivazioni()

            Return GetMotivazioni

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetMotivazioni(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetMotivazioni
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetMotivazioni = objCOMPlusBusinessObject.SetMotivazioni()

            Return SetMotivazioni

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function DelMotivazioni(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelMotivazioni
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelMotivazioni = objCOMPlusBusinessObject.DelMotivazioni()

            Return DelMotivazioni

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    Public Function GetTipologieVoci(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetTipologieVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetTipologieVoci = objCOMPlusBusinessObject.GetTipologieVoci()

            Return GetTipologieVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function GetSanzioniRavvedimentoOperoso(ByVal objHashTable As Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetSanzioniRavvedimentoOperoso

        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetSanzioniRavvedimentoOperoso = objCOMPlusBusinessObject.GetSanzioniRavvedimentoOperoso()

            Return GetSanzioniRavvedimentoOperoso

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetTipologieVoci(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetTipologieVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetTipologieVoci = objCOMPlusBusinessObject.SetTipologieVoci()

            Return SetTipologieVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function DelTipologieVoci(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelTipologieVoci
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelTipologieVoci = objCOMPlusBusinessObject.DelTipologieVoci()

            Return DelTipologieVoci

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetGeneraleICI(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetGeneraleICI
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetGeneraleICI = objCOMPlusBusinessObject.GetGeneraleICI()

            Return GetGeneraleICI

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function SetGeneraleICI(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetGeneraleICI
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetGeneraleICI = objCOMPlusBusinessObject.SetGeneraleICI()

            Return SetGeneraleICI

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function


    Public Function GetListTipoPossesso(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetListTipoPossesso
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetListTipoPossesso = objCOMPlusBusinessObject.GetListTipoPossesso()

            Return GetListTipoPossesso

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetListCategorie(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetListCategorie
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetListCategorie = objCOMPlusBusinessObject.GetListCategorie()

            Return GetListCategorie

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetListTipoRendita(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetListTipoRendita
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetListTipoRendita = objCOMPlusBusinessObject.GetListTipoRendita()

            Return GetListTipoRendita

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function GetListClasse(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetListClasse
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetListClasse = objCOMPlusBusinessObject.GetListClasse()

            Return GetListClasse

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function DelScadenzaInteressi(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.DelScadenzaInteressi
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            DelScadenzaInteressi = objCOMPlusBusinessObject.DelScadenzaInteressi

            Return DelScadenzaInteressi

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function GetScadenzaInteressi(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IGestioneConfigurazione.GetScadenzaInteressi
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetScadenzaInteressi = objCOMPlusBusinessObject.GetScadenzaInteressi()

            Return GetScadenzaInteressi

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function SetScadenzaInteressi(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IGestioneConfigurazione.SetScadenzaInteressi
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetScadenzaInteressi = objCOMPlusBusinessObject.SetScadenzaInteressi()

            Return SetScadenzaInteressi

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try


    End Function
End Class
