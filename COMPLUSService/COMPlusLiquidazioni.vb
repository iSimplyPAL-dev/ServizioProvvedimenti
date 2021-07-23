Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.EnterpriseServices
Imports System.Runtime.InteropServices
Imports log4net

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per la stampa dei dati del confronto fra dichiarato e versato
    ''' </summary>
    Friend Class COMPlusLiquidazioni
        Private Shared Log As ILog = LogManager.GetLogger(GetType(COMPlusLiquidazioni))

        Protected objUtility As New MotoreProvUtility
        Protected objdbIci As New DBIci
        Protected objCostanti As New COSTANTValue.CostantiProv
        '*******************************************************************
        'variabili di istanza oggetti provenienti dal client
        'Hashtable
        'RIBESFrameWork.Session (sessione del FrameWork
        '*******************************************************************
        Public m_objHashTable As Hashtable
        Dim objDBOPENgovProvvedimentiSelect As DBOPENgovProvvedimentiSelect
        Dim objDBOPENgovProvvedimentiUpdate As COMPlusOPENgovProvvedimenti.DBOPENgovProvvedimentiUpdate

        Public Function getProvvedimentoPerStampaLiquidazione(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet

            Try

                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getProvvedimentoPerStampaLiquidazione(StringConnectionProvv, ID_PROCEDIMENTO)

                Return ds

            Catch ex As Exception
                Throw New Exception("Function::getProvvedimentoPerStampaLiquidazione::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try

        End Function

        Public Function getVersamentiPerStampaLiquidazione(StringConnectionProvv As String, ByVal ID_PROCEDIMENTO As Long) As DataSet

            Try

                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.getVersamentiPerStampaLiquidazione(StringConnectionProvv, ID_PROCEDIMENTO)

                Return ds

            Catch ex As Exception
                Throw New Exception("Function::getVersamentiPerStampaLiquidazione::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try

        End Function


        Public Function GetElencoInteressiPerStampaLiquidazione(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.GetElencoInteressiPerStampaLiquidazione(StringConnectionProvv, ID_PROVVEDIMENTO)

                Return ds

            Catch ex As Exception
                Throw New Exception("Function::GetElencoInteressiPerStampaLiquidazione::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try

        End Function

        Public Function GetInteressiTotaliPerStampaLiquidazione(StringConnectionProvv As String, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.GetInteressiTotaliPerStampaLiquidazione(StringConnectionProvv, ID_PROVVEDIMENTO)

                Return ds

            Catch ex As Exception
                Throw New Exception("Function::GetInteressiTotaliPerStampaLiquidazione::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try

        End Function

        Public Function GetElencoSanzioniPerStampaLiquidazione(StringConnectionProvv As String, IdEnte As String, ByVal objHashTable As Hashtable, ByVal ID_PROVVEDIMENTO As String) As DataSet

            Try

                Dim ds As New DataSet
                objDBOPENgovProvvedimentiSelect = New DBOPENgovProvvedimentiSelect
                ds = objDBOPENgovProvvedimentiSelect.GetElencoSanzioniPerStampaLiquidazione(StringConnectionProvv, IdEnte, m_objHashTable, ID_PROVVEDIMENTO)

                Return ds

            Catch ex As Exception
                Throw New Exception("Function::GetElencoSanzioniPerStampaLiquidazione::COMPlusLiquidazioni::" & " " & ex.Message)
            End Try


        End Function
        Public Sub New(ByVal objHashTable As Hashtable)
            m_objHashTable = objHashTable
        End Sub

    End Class

End Namespace