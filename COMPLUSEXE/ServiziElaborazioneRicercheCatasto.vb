Imports System
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Channels
Imports ComPlusInterface
Imports COMPlusService.COMPlusOPENgovCATASTO
Public Class ServiziElaborazioneRicercheCatasto
  Inherits MarshalByRefObject
  Implements IRicercheCatasto
  Public Function GetDatiPerSoggetto(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IRicercheCatasto.GetDatiPerSoggetto
    Try
      Dim objCOMPlusBusinessObjectCatasto As New COMPlusBussinesObjectCatasto
      objCOMPlusBusinessObjectCatasto.InizializeObject(objHashTable)

      GetDatiPerSoggetto = objCOMPlusBusinessObjectCatasto.GetDatiPerSoggetto

      Return GetDatiPerSoggetto

    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try

  End Function
  Public Function GetDettaglioDatiImmobilePerSoggetto(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IRicercheCatasto.GetDettaglioDatiImmobilePerSoggetto
    Try
      Dim objCOMPlusBusinessObjectCatasto As New COMPlusBussinesObjectCatasto
      objCOMPlusBusinessObjectCatasto.InizializeObject(objHashTable)

      GetDettaglioDatiImmobilePerSoggetto = objCOMPlusBusinessObjectCatasto.GetDettaglioDatiImmobilePerSoggetto

      Return GetDettaglioDatiImmobilePerSoggetto

    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try
  End Function

  Public Function GetDatiPerIndirizzo(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IRicercheCatasto.GetDatiPerIndirizzo
    Try
      Dim objCOMPlusBusinessObjectCatasto As New COMPlusBussinesObjectCatasto
      objCOMPlusBusinessObjectCatasto.InizializeObject(objHashTable)

      GetDatiPerIndirizzo = objCOMPlusBusinessObjectCatasto.GetDatiPerIndirizzo

      Return GetDatiPerIndirizzo

    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try
  End Function

  Public Function GetDatiPerIDENTIFICATIVO(ByVal objHashTable As System.Collections.Hashtable) As System.Data.DataSet Implements ComPlusInterface.IRicercheCatasto.GetDatiPerIDENTIFICATIVO
    Try
      Dim objCOMPlusBusinessObjectCatasto As New COMPlusBussinesObjectCatasto
      objCOMPlusBusinessObjectCatasto.InizializeObject(objHashTable)

      GetDatiPerIDENTIFICATIVO = objCOMPlusBusinessObjectCatasto.GetDatiPerIDENTIFICATIVO

      Return GetDatiPerIDENTIFICATIVO

    Catch ex As Exception
      Throw New Exception(ex.Message & "::" & ex.StackTrace)
    End Try
  End Function
End Class
