Imports System
Imports System.Data.SqlClient
Imports System.EnterpriseServices
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports RIBESFrameWork

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per la gestione della configurazione per il calcolo degli atti di accertamento
    ''' </summary>
    Friend Class COMPlusGestioneConfigurazione
        Protected objUtility As New MotoreProvUtility
        Public m_objHashTable As Hashtable
        Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect
        Dim objDBOPENgovProvvedimentiUpdate As COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate

        Public Function getTipoVoci(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                getTipoVoci = objDBOPENgovProvvedimentiSelect.GetTipoVoci(StringConnectionProvv, IdEnte, m_objHashTable)

            Catch ex As Exception
                Throw New Exception("Function::getTipoVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function SetTipoVoci(StringConnectionProvv As String, IdEnte As String, ByRef strIDTIPOVOCE As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                SetTipoVoci = objDBOPENgovProvvedimentiUpdate.SetTipoVoci(StringConnectionProvv, IdEnte, m_objHashTable, strIDTIPOVOCE)

            Catch ex As Exception
                Throw New Exception("Function::SetTipoVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function DelTipoVoci(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                DelTipoVoci = objDBOPENgovProvvedimentiUpdate.DelTipoVoci(StringConnectionProvv, IdEnte, m_objHashTable)

            Catch ex As Exception
                Throw New Exception("Function::DelTipoVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function GetValoriVoci(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                GetValoriVoci = objDBOPENgovProvvedimentiSelect.GetValoriVoci(StringConnectionProvv, IdEnte, m_objHashTable)

            Catch ex As Exception
                Throw New Exception("Function::GetValoriVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function GetTipoInteresse(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                GetTipoInteresse = objDBOPENgovProvvedimentiSelect.GetTipoInteresse(StringConnectionProvv, IdEnte, m_objHashTable)

            Catch ex As Exception
                Throw New Exception("Function::GetTipoInteresse::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function GetScadenzaInteressi(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                GetScadenzaInteressi = objDBOPENgovProvvedimentiSelect.GetScadenzaInteressi(StringConnectionProvv, IdEnte, m_objHashTable)

            Catch ex As Exception
                Throw New Exception("Function::GetScadenzaInteressi::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function GetAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                GetAnniProvvedimenti = objDBOPENgovProvvedimentiSelect.GetAnniProvvedimenti(StringConnectionProvv, IdEnte, m_objHashTable)

            Catch ex As Exception
                Throw New Exception("Function::GetAnniProvvedimenti::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function



        Public Function SetValoriVoci(StringConnectionProvv As String, IdEnte As String, ByRef intRetVal As Integer) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                SetValoriVoci = objDBOPENgovProvvedimentiUpdate.SetValoriVoci(StringConnectionProvv, IdEnte, m_objHashTable, intRetVal)
                Return SetValoriVoci
            Catch ex As Exception
                Throw New Exception("Function::SetValoriVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function DelValoriVoci(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                DelValoriVoci = objDBOPENgovProvvedimentiUpdate.DelValoriVoci(StringConnectionProvv, IdEnte, m_objHashTable)
                Return DelValoriVoci
            Catch ex As Exception
                Throw New Exception("Function::DelValoriVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function DelAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                DelAnniProvvedimenti = objDBOPENgovProvvedimentiUpdate.DelAnniProvvedimenti(StringConnectionProvv, IdEnte, m_objHashTable)
                Return DelAnniProvvedimenti
            Catch ex As Exception
                Throw New Exception("Function::DelValoriVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function


        Public Function SetTassiInteresse(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                SetTassiInteresse = objDBOPENgovProvvedimentiUpdate.SetTassiInteresse(StringConnectionProvv, IdEnte, m_objHashTable)
                Return SetTassiInteresse
            Catch ex As Exception
                Throw New Exception("Function::SetValoriVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function


        Public Function SetScadenzaInteressi(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                SetScadenzaInteressi = objDBOPENgovProvvedimentiUpdate.SetScadenzaInteressi(StringConnectionProvv, IdEnte, m_objHashTable)
                Return SetScadenzaInteressi
            Catch ex As Exception
                Throw New Exception("Function::SetScadenzaInteressi::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function SetAnniProvvedimenti(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                SetAnniProvvedimenti = objDBOPENgovProvvedimentiUpdate.SetAnniProvvedimenti(StringConnectionProvv, IdEnte, m_objHashTable)
                Return SetAnniProvvedimenti
            Catch ex As Exception
                Throw New Exception("Function::SetAnniProvvedimenti::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function DelTassiInteresse(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                DelTassiInteresse = objDBOPENgovProvvedimentiUpdate.DelTassiInteresse(StringConnectionProvv, IdEnte, m_objHashTable)
                Return DelTassiInteresse
            Catch ex As Exception
                Throw New Exception("Function::DelValoriVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function DelScadenzaInteressi(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                DelScadenzaInteressi = objDBOPENgovProvvedimentiUpdate.DelScadenzaInteressi(StringConnectionProvv, IdEnte, m_objHashTable)
                Return DelScadenzaInteressi
            Catch ex As Exception
                Throw New Exception("Function::DelScadenzaInteressi::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function GetMotivazioni(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                GetMotivazioni = objDBOPENgovProvvedimentiSelect.GetMotivazioni(StringConnectionProvv, IdEnte)

            Catch ex As Exception
                Throw New Exception("Function::GetMotivazioni::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function SetMotivazioni(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                SetMotivazioni = objDBOPENgovProvvedimentiUpdate.SetMotivazioni(StringConnectionProvv, IdEnte, m_objHashTable)
                Return SetMotivazioni
            Catch ex As Exception
                Throw New Exception("Function::SetMotivazioni::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function DelMotivazioni(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                DelMotivazioni = objDBOPENgovProvvedimentiUpdate.DelMotivazioni(StringConnectionProvv, IdEnte, m_objHashTable)
                Return DelMotivazioni
            Catch ex As Exception
                Throw New Exception("Function::DelMotivazioni::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function GetTipologieVoci(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                GetTipologieVoci = objDBOPENgovProvvedimentiSelect.GetTipologieVoci(StringConnectionProvv, IdEnte)

            Catch ex As Exception
                Throw New Exception("Function::GetTipologieVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myConnectionString"></param>
        ''' <returns></returns>
        ''' <revisionHistory><revision date="14/804/2020">Sono cambiate le regole di applicazione sanzione</revision></revisionHistory>
        Public Function GetSanzioniRavvedimentoOperoso(myConnectionString As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                GetSanzioniRavvedimentoOperoso = objDBOPENgovProvvedimentiSelect.GetSanzioniRavvedimentoOperoso(myConnectionString)

            Catch ex As Exception
                Throw New Exception("Function::GetSanzioniRavvedimentoOperoso::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function SetTipologieVoci(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                SetTipologieVoci = objDBOPENgovProvvedimentiUpdate.SetTipologieVoci(StringConnectionProvv, IdEnte, m_objHashTable)
                Return SetTipologieVoci
            Catch ex As Exception
                Throw New Exception("Function::SetTipologieVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function DelTipologieVoci(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                DelTipologieVoci = objDBOPENgovProvvedimentiUpdate.DelTipologieVoci(StringConnectionProvv, IdEnte, m_objHashTable)
                Return DelTipologieVoci
            Catch ex As Exception
                Throw New Exception("Function::DelTipologieVoci::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function GetGeneraleICI(StringConnectionProvv As String, IdEnte As String) As DataSet
            Try

                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                GetGeneraleICI = objDBOPENgovProvvedimentiSelect.GetGeneraleICI(StringConnectionProvv, IdEnte, m_objHashTable)

            Catch ex As Exception
                Throw New Exception("Function::GetGeneraleICI::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function
        Public Function SetGeneraleICI(StringConnectionProvv As String, IdEnte As String) As Boolean
            Try

                objDBOPENgovProvvedimentiUpdate = New DBOPENgovProvvedimentiUpdate
                SetGeneraleICI = objDBOPENgovProvvedimentiUpdate.SetGeneraleICI(StringConnectionProvv, IdEnte, m_objHashTable)
                Return SetGeneraleICI
            Catch ex As Exception
                Throw New Exception("Function::SetGeneraleICI::COMPlusGestioneConfigurazione:: " & ex.Message)
            End Try
        End Function

        Public Function GetListTipoPossesso(StringConnectionICI As String, ByVal objHashTable As System.Collections.Hashtable) As DataSet
            Try
                Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
                objCOMPlusBusinessObject.InizializeObject(objHashTable)

                GetListTipoPossesso = objCOMPlusBusinessObject.GetListTipoPossesso(StringConnectionICI)

                Return GetListTipoPossesso

            Catch ex As Exception
                Throw New Exception(ex.Message & "::" & ex.StackTrace)
            End Try
        End Function

        Public Function GetListCategorie(StringConnectionICI As String, ByVal objHashTable As System.Collections.Hashtable) As DataSet
            Try
                Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
                objCOMPlusBusinessObject.InizializeObject(objHashTable)

                GetListCategorie = objCOMPlusBusinessObject.GetListCategorie(StringConnectionICI)

                Return GetListCategorie

            Catch ex As Exception
                Throw New Exception(ex.Message & "::" & ex.StackTrace)
            End Try
        End Function

        Public Function GetListTipoRendita(StringConnectionICI As String, ByVal objHashTable As System.Collections.Hashtable) As DataSet
            Try
                Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
                objCOMPlusBusinessObject.InizializeObject(objHashTable)

                GetListTipoRendita = objCOMPlusBusinessObject.GetListTipoRendita(StringConnectionICI)

                Return GetListTipoRendita

            Catch ex As Exception
                Throw New Exception(ex.Message & "::" & ex.StackTrace)
            End Try
        End Function

        Public Function GetListClasse(StringConnectionICI As String, ByVal objHashTable As System.Collections.Hashtable) As DataSet
            Try
                Dim objCOMPlusBusinessObject As New COMPlusBusinessObject
                objCOMPlusBusinessObject.InizializeObject(objHashTable)

                GetListClasse = objCOMPlusBusinessObject.GetListClasse(StringConnectionICI)

                Return GetListClasse

            Catch ex As Exception
                Throw New Exception(ex.Message & "::" & ex.StackTrace)
            End Try
        End Function

        Public Sub New(ByVal objHashTable As Hashtable)
            m_objHashTable = objHashTable
        End Sub
    End Class
End Namespace
