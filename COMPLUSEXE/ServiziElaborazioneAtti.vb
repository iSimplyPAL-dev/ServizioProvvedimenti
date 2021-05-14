Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovProvvedimenti
Imports log4net

Class ServiziElaborazioneAtti
  Inherits MarshalByRefObject
  Implements IElaborazioneAtti
    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(GetType(ServiziElaborazioneAtti))

  '**********************************************************************************************************
  ' GetDatiAttiRicercaSemplice() Function <a name="GetDatiAttiRicercaSemplice"></a>
  ' Il metodo GetDatiAttiRicercaSemplice ritorna un DataSet
  ' Viene utilizzato quando la ricerca Atti 
  ' I parametri passati sono un oggetto di tipo Hashtable 
  ' Ritorna un oggetto dataset con i tutti gli atti del contribuente selezionato in fase di ricerca
  ' o i il singlo atto (Ricerca per numero avviso)
  '**********************************************************************************************************
  Public Function GetDatiAttiRicercaSemplice(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.GetDatiAttiRicercaSemplice

    Try
      Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
      objCOMPlusBusinessObject.InizializeObject(objHashTable)

      GetDatiAttiRicercaSemplice = objCOMPlusBusinessObject.getAttiRicercaSemplice

      Return GetDatiAttiRicercaSemplice

    Catch ex As Exception
            log.debug("ServiziElaborazioneAtti::GetDatiAttiRicercaSemplice::errore::" & ex.Message)
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try

  End Function
  Public Function getSUM_IMPORTI_TOTALI(ByVal objHashTable As System.Collections.Hashtable) As String Implements ComPlusInterface.IElaborazioneAtti.getSUM_IMPORTI_TOTALI

    Try

      Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
      objCOMPlusBusinessObject.InizializeObject(objHashTable)

      getSUM_IMPORTI_TOTALI = objCOMPlusBusinessObject.getSUM_IMPORTI_TOTALI

      Return getSUM_IMPORTI_TOTALI

    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try

  End Function
  Public Function getSUM_IMPORTI_RETTIFICATI(ByVal objHashTable As System.Collections.Hashtable) As String Implements ComPlusInterface.IElaborazioneAtti.getSUM_IMPORTI_RETTIFICATI

    Try


      Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
      objCOMPlusBusinessObject.InizializeObject(objHashTable)

      getSUM_IMPORTI_RETTIFICATI = objCOMPlusBusinessObject.getSUM_IMPORTI_RETTIFICATI

      Return getSUM_IMPORTI_RETTIFICATI

    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try

  End Function

  Public Function getDATI_PROVVEDIMENTI(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.getDATI_PROVVEDIMENTI
    Try


      Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
      objCOMPlusBusinessObject.InizializeObject(objHashTable)

      getDATI_PROVVEDIMENTI = objCOMPlusBusinessObject.getLOAD_PROVVEDIMENTI

      Return getDATI_PROVVEDIMENTI

    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try

  End Function
  '****************************************************************************************************************************************************************************
  'LA funzione è utilizzata per la gestione atti
  'per l'aggiornamento delle date in OPENgovProvvedimenti
  '****************************************************************************************************************************************************************************
  Public Function setPROVVEDIMENTO_ATTO(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IElaborazioneAtti.setPROVVEDIMENTO_ATTO
    Try


      Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
      objCOMPlusBusinessObject.InizializeObject(objHashTable)

      setPROVVEDIMENTO_ATTO = objCOMPlusBusinessObject.setPROVVEDIMENTO

      Return setPROVVEDIMENTO_ATTO

    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try

  End Function


    Public Function setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA(ByVal objHashTable As System.Collections.Hashtable, ByRef NUMERO_ATTO As String) As Boolean Implements ComPlusInterface.IElaborazioneAtti.setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA
        Try


            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA = objCOMPlusBusinessObject.setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA(NUMERO_ATTO)

            Return setPROVVEDIMENTO_ATTO_LIQUIDAZIONE_STAMPA

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function
    Public Function setPROVVEDIMENTO_ATTO_LIQUIDAZIONE(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IElaborazioneAtti.setPROVVEDIMENTO_ATTO_LIQUIDAZIONE
        Try


            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            setPROVVEDIMENTO_ATTO_LIQUIDAZIONE = objCOMPlusBusinessObject.setPROVVEDIMENTO_ATTO_LIQUIDAZIONE

            Return setPROVVEDIMENTO_ATTO_LIQUIDAZIONE

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function
    '***************************************************************
    'Popola un data set in base ai parametri di ricrca passati
    '***************************************************************
    Public Function GetDatiAttiRicercaAvanzata(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.GetDatiAttiRicercaAvanzata
        Try


            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            GetDatiAttiRicercaAvanzata = objCOMPlusBusinessObject.GetDatiAttiRicercaAvanzata

            Return GetDatiAttiRicercaAvanzata

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function


    Public Function setRateProvvedimenti(ByVal objHashTable As Hashtable, ByVal dsRate As DataSet) As Boolean Implements IElaborazioneAtti.setRateProvvedimenti

        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            setRateProvvedimenti = objCOMPlusBusinessObject.setRateProvvedimenti(dsRate)

            Return setRateProvvedimenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try


    End Function

    Public Function getRateProvvedimenti(ByVal objHashTable As Hashtable, ByVal idProvvedimento As String) As DataSet Implements IElaborazioneAtti.getRateProvvedimenti
        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getRateProvvedimenti = objCOMPlusBusinessObject.getRateProvvedimenti(idProvvedimento)

            Return getRateProvvedimenti

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function



    '*********************************************************************************************
    'Aggiorna ossia o eleimina o cambia date nei provvedimenti selezionati
    '*********************************************************************************************
    Public Function setDATE_PROVVEDIMENTI_MASSIVA(ByVal objHashTable As System.Collections.Hashtable, _
    ByVal objSELEZIONE_DATASET As DataSet) As Long Implements ComPlusInterface.IElaborazioneAtti.setDATE_PROVVEDIMENTI_MASSIVA
        Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
        objCOMPlusBusinessObject.InizializeObject(objHashTable)

        setDATE_PROVVEDIMENTI_MASSIVA = objCOMPlusBusinessObject.setDATE_PROVVEDIMENTI_MASSIVA(objSELEZIONE_DATASET)

        Return setDATE_PROVVEDIMENTI_MASSIVA

    End Function

    Public Function SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO(ByVal objHashTable As System.Collections.Hashtable) As Boolean Implements ComPlusInterface.IElaborazioneAtti.SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO = objCOMPlusBusinessObject.SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO

            Return SetPROVVEDIMENTOATTO_ANNULAMENTO_AVVISO

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function getDatiProvvedimento_PerTipo(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.getDatiProvvedimento_PerTipo
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getDatiProvvedimento_PerTipo = objCOMPlusBusinessObject.getDatiProvvedimento_PerTipo

            Return getDatiProvvedimento_PerTipo

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function getAnagrafica(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.getAnagrafica
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getAnagrafica = objCOMPlusBusinessObject.getAnagrafica

            Return getAnagrafica

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function
    Public Function getProvvedimentiContribuente(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.getProvvedimentiContribuente
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getProvvedimentiContribuente = objCOMPlusBusinessObject.getProvvedimentiContribuente

            Return getProvvedimentiContribuente

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function

    Public Function getDATI_TASK_REPOSITORY(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.getDATI_TASK_REPOSITORY
        Try
            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            getDATI_TASK_REPOSITORY = objCOMPlusBusinessObject.getDATI_TASK_REPOSITORY

            Return getDATI_TASK_REPOSITORY

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try
    End Function

    Public Function getVersamentiICI(ByVal objHashTable As Hashtable) As System.Data.DataSet Implements ComPlusInterface.IElaborazioneAtti.getVersamentiICI

        Try
            Dim objDBIci As New DBIci
            getVersamentiICI = objDBIci.GetVersamentiIci(objHashTable)

            Return getVersamentiICI


        Catch ex As Exception

        End Try

    End Function

End Class
