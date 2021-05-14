Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports log4net
''' <summary>
''' Classe rende disponibili le interfacce di gestione atti
''' </summary>
Class ServiziElaborazioneAtti
  Inherits MarshalByRefObject
  Implements IElaborazioneAtti
    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(ServiziElaborazioneAtti))

    ''' <summary>
    ''' GetDatiAttiRicercaSemplice() Function <a name="GetDatiAttiRicercaSemplice"></a>
    ''' Il metodo GetDatiAttiRicercaSemplice ritorna un DataSet
    ''' Viene utilizzato quando la ricerca Atti 
    ''' I parametri passati sono un oggetto di tipo Hashtable 
    ''' Ritorna un oggetto dataset con i tutti gli atti del contribuente selezionato in fase di ricerca
    ''' o i il singlo atto (Ricerca per numero avviso) 
    ''' </summary>
    ''' <param name="StringConnectionProvv"></param>
    ''' <param name="objHashTable"></param>
    ''' <returns></returns>
    Public Function GetDatiAttiRicercaSemplice(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.GetDatiAttiRicercaSemplice

        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetDatiAttiRicercaSemplice = objCOMPlusBusinessObject.getAttiRicercaSemplice(StringConnectionProvv, IdEnte)

            Return GetDatiAttiRicercaSemplice

        Catch ex As Exception
            Log.Debug("ServiziElaborazioneAtti::GetDatiAttiRicercaSemplice::errore::" & ex.Message)
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objHashTable"></param>
    ''' <returns></returns>
    ''' <revisionHistory>
    ''' <revision date="12/04/2019">
    ''' <strong>Qualificazione AgID-analisi_rel01</strong>
    ''' <em>Analisi eventi</em>
    ''' </revision>
    ''' </revisionHistory>
    Public Function GetDatiProvvedimenti(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByRef myAtto As OggettoAtto) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.GetDatiProvvedimenti
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)
            Return objCOMPlusBusinessObject.LoadProvvedimenti(StringConnectionProvv, myAtto)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function




    Public Function setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByRef NUMERO_ATTO As String) As Boolean Implements ComPlusInterface.IElaborazioneAtti.setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA
        Try


            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA = objCOMPlusBusinessObject.setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA(StringConnectionProvv, NUMERO_ATTO)

            Return setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myDBType"></param>
    ''' <param name="myStringConnection"></param>
    ''' <param name="myAtto"></param>
    ''' <param name="Operatore"></param>
    ''' <returns></returns>
    ''' <revisionHistory>
    ''' <revision date="12/04/2019">
    ''' <strong>Qualificazione AgID-analisi_rel01</strong>
    ''' <em>Analisi eventi</em>
    ''' </revision>
    ''' </revisionHistory>
    Public Function setProvvedimentoAttoLiquidazione(myDBType As String, ByVal myStringConnection As String, myAtto As OggettoAtto, Operatore As String) As Integer Implements ComPlusInterface.IElaborazioneAtti.SetProvvedimentoAttoLiquidazione
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(New Hashtable)

            Return objCOMPlusBusinessObject.setProvvedimentoAttoLiquidazione(myDBType, myStringConnection, myAtto, Operatore)
        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    '***************************************************************
    'Popola un data set in base ai parametri di ricrca passati
    '***************************************************************
    Public Function GetDatiAttiRicercaAvanzata(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable, ParamSearch As ObjSearchAtti) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.GetDatiAttiRicercaAvanzata
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetDatiAttiRicercaAvanzata = objCOMPlusBusinessObject.GetDatiAttiRicercaAvanzata(StringConnectionProvv, IdEnte, ParamSearch)

            Return GetDatiAttiRicercaAvanzata

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function
    '*********************************************************************************************
    'Aggiorna ossia o eleimina o cambia date nei provvedimenti selezionati
    '*********************************************************************************************
    Public Function setDATE_PROVVEDIMENTI_MASSIVA(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable, ByVal objSELEZIONE_DATASET As DataSet) As Long Implements ComPlusInterface.IElaborazioneAtti.setDATE_PROVVEDIMENTI_MASSIVA
        Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
        objCOMPlusBusinessObject.InizializeObject(objHashTable)

        setDATE_PROVVEDIMENTI_MASSIVA = objCOMPlusBusinessObject.setDATE_PROVVEDIMENTI_MASSIVA(StringConnectionProvv, objSELEZIONE_DATASET)

        Return setDATE_PROVVEDIMENTI_MASSIVA

    End Function

    Public Function SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(StringConnectionProvv As String, ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IElaborazioneAtti.SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO = objCOMPlusBusinessObject.SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(StringConnectionProvv)

            Return SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function getDatiProvvedimento_PerTipo(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.getDatiProvvedimento_PerTipo
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getDatiProvvedimento_PerTipo = objCOMPlusBusinessObject.getDatiProvvedimento_PerTipo(StringConnectionProvv, IdEnte)

            Return getDatiProvvedimento_PerTipo

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function


    Public Function getProvvedimentiContribuente(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.getProvvedimentiContribuente
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getProvvedimentiContribuente = objCOMPlusBusinessObject.getProvvedimentiContribuente(StringConnectionProvv, IdEnte)

            Return getProvvedimentiContribuente

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function


End Class
