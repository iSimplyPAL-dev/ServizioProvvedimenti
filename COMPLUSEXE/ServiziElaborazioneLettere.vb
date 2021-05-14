Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovProvvedimenti
'*******************************************************
'
' GetDatiLettere() Function <a name="GetDatiLettere"></a>
'
' La funzione GetDatiLettere
' sarà da utilizzare per la ricerca dei contribuenti con Dichiarazioni da Bonificare 
' Restituisce un dataset con i dati dei contribuenti  che saranno esposti in
'una griglia
'*******************************************************
Class ServiziElaborazioneLettere
  Inherits MarshalByRefObject
  Implements IElaborazioneLettere


  '**********************************************************************************************************
  ' GetDatiLettere() Function <a name="GetDatiLettere"></a>
  ' Il metodo GetDatiLettere ritorna un DataSet
  ' Viene utilizzato quando la ricerca lettere è di tipo Massiva con l'opzione Anagrafica  + Testo Fisso 
  ' I parametri passati sono un oggetto di tipo Hashtable e la session aperta lato client
  ' Ritorna un oggetto dataset con i dati dei contribuenti trovati
  '**********************************************************************************************************
    'Public Function GetDatiLettere(ByVal objHashTable As Hashtable, ByVal objSession As RIBESFrameWork.Session) As DataSet Implements IElaborazioneLettere.GetDatiLettere
    Public Function GetDatiLettere(ByVal objHashTable As Hashtable) As DataSet Implements IElaborazioneLettere.GetDatiLettere
        Try

            Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
            'objCOMPlusBusinessObject.InizializeObject(objHashTable, objSession)
            objCOMPlusBusinessObject.InizializeObject(objHashTable)

            '*******************************************************
            ' Modalita di ricerca Manuale
            '*******************************************************
            If CType(objHashTable("Manuale"), Boolean) Then

                If CType(objHashTable("AnagraficaTestoFisso"), Boolean) Then
                    GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaManualeTestoFisso
                End If

                If CType(objHashTable("AnagraficaTestoFissoDati"), Boolean) Then
                    GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaManualeTestoFissoDati
                End If

            End If
            '*******************************************************
            ' Fine Modalita di ricerca Manuale
            '*******************************************************

            '*******************************************************      
            ' Modalita di ricerca Massiva 
            '*******************************************************
            If CType(objHashTable("Massiva"), Boolean) Then

                If CType(objHashTable("CHECKDAA"), Boolean) Then

                    If CType(objHashTable("AnagraficaTestoFisso"), Boolean) Then
                        GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaMassiviaTestoFissoDAA
                    End If

                    If CType(objHashTable("AnagraficaTestoFissoDati"), Boolean) Then
                        GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaMassiviaTestoFissoDatiDAA
                    End If

                End If
                If CType(objHashTable("CHEKVIARESIDENZA"), Boolean) Then

                    If CType(objHashTable("AnagraficaTestoFisso"), Boolean) Then
                        GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaMassiviaTestoFissoVia()
                    End If

                    If CType(objHashTable("AnagraficaTestoFissoDati"), Boolean) Then
                        GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaMassiviaTestoFissoDatiVia()
                    End If

                End If
                If CType(objHashTable("CHEKAREE"), Boolean) Then

                    If CType(objHashTable("AnagraficaTestoFisso"), Boolean) Then
                        GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaMassiviaTestoFissoAREE
                    End If

                    If CType(objHashTable("AnagraficaTestoFissoDati"), Boolean) Then
                        GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaMassiviaTestoFissoDatiAREE()
                    End If

                End If
                If CType(objHashTable("CHEKUBICAZIONE"), Boolean) Then
                    If CType(objHashTable("AnagraficaTestoFisso"), Boolean) Then
                        GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaMassiviaTestoFissoUBICAZIONE
                    End If

                    If CType(objHashTable("AnagraficaTestoFissoDati"), Boolean) Then
                        GetDatiLettere = objCOMPlusBusinessObject.getLettereRicercaMassiviaTestoFissoDatiUBICAZIONE()
                    End If
                End If

            End If
            '*******************************************************      
            ' Fine Modalita di ricerca Massiva
            '*******************************************************

            Return GetDatiLettere

        Catch ex As Exception
            Throw New Exception(ex.Message & "::" & ex.StackTrace)
        End Try

    End Function


End Class
