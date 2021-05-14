Imports System
Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports System.Reflection

Namespace COMPlusOPENgovProvvedimenti
    ''' <summary>
    ''' Classe per le funzioni generali di utilità
    ''' </summary>
    Public Class MotoreProvUtility
        Public Const DBType_SQL As String = "SQLClient"

        ''' <summary>
        ''' Viene utilizzato quando il valore di una stringa deve essere contatenata ad una stringa SQL
        ''' </summary>
        ''' <param name="vInput"></param>
        ''' <param name="blnClearSpace"></param>
        ''' <param name="blnUseNull"></param>
        ''' <returns>stringa</returns>
        ''' <remarks></remarks>
        Public Function CStrToDB(ByVal vInput As Object, Optional ByRef blnClearSpace As Boolean = False, Optional ByVal blnUseNull As Boolean = False) As String
            Dim sTesto As String
            If blnUseNull Then
                CStrToDB = "Null"
            Else
                CStrToDB = "''"
            End If



            If Not IsDBNull(vInput) And Not IsNothing(vInput) Then

                sTesto = CStr(vInput)
                If blnClearSpace Then
                    sTesto = Trim(sTesto)
                End If
                If Trim(sTesto) <> "" Then
                    CStrToDB = "'" & Replace(sTesto, "'", "''") & "'"
                Else
                    CStrToDB = "''"
                End If
            End If

        End Function

        Public Function CStrToDBForIn(ByVal vInput As String) As String
            Dim sTesto As String
            Dim i As Integer

            CStrToDBForIn = "''"
            Dim arrayIn() As String
            arrayIn = Split(vInput, ",")

            For i = -1 To UBound(arrayIn) - 1
                sTesto = sTesto & "'" & arrayIn(i + 1) & "',"
            Next

            sTesto = Left(sTesto, Len(sTesto) - 1)

            Return sTesto

        End Function

        ''' <summary>
        ''' Viene utilizzato per convertire un valore in stringa considerando se il valore passato e un NULL
        ''' </summary>
        ''' <param name="strInput"></param>
        ''' <returns>stringa</returns>
        ''' <remarks>Se il valore passato è un Null ritorna una stringa vuota</remarks>
        Public Function CToStr(ByVal strInput As Object) As String

            CToStr = ""

            If Not IsDBNull(strInput) And Not IsNothing(strInput) Then
                CToStr = CStr(strInput)
            End If

            Return CToStr

        End Function
        Public Function cToInt(ByVal objInput As Object) As Integer
            cToInt = 0
            If Not IsDBNull(objInput) And Not IsNothing(objInput) Then
                If IsNumeric(objInput) Then
                    cToInt = Convert.ToInt32(objInput)
                End If
            End If
        End Function

        Public Function cToLong(ByVal objInput As Object) As Long
            cToLong = 0
            If Not IsDBNull(objInput) And Not IsNothing(objInput) Then
                If IsNumeric(objInput) Then
                    cToLong = Convert.ToInt64(objInput)
                End If
            End If
        End Function

        Public Function cToDbl(ByVal objInput As Object) As Double

            cToDbl = 0

            If Not IsDBNull(objInput) And Not IsNothing(objInput) Then
                If IsNumeric(objInput) Then
                    cToDbl = CDbl(objInput)
                End If
            End If
        End Function
        Public Function CIdToDB(ByVal vInput As Object) As String

            CIdToDB = "Null"

            If Not IsDBNull(vInput) And Not IsNothing(vInput) Then
                If IsNumeric(vInput) Then
                    If CDbl(vInput) > 0 Then
                        CIdToDB = CStr(CDbl(vInput))
                    End If
                End If
            End If

        End Function
        Public Function CIdFromDB(ByVal vInput As Object) As String

            CIdFromDB = "-1"

            If Not IsDBNull(vInput) And Not IsNothing(vInput) And Not IsNothing(vInput) Then
                If IsNumeric(vInput) Then
                    If CDbl(vInput) > 0 Then
                        CIdFromDB = CStr(CDbl(vInput))
                    End If
                End If
            End If

        End Function
        Public Function FormattaData(ByVal data As String, ByVal sTypeFormat As String) As String
            'STYPEFORMAT
            'G=leggo la data nel formato aaaammgg  e la metto nel formato gg/mm/aaaa
            'A=leggo la data nel formato gg/mm/aaaa  e la metto nel formato aaaammgg
            Dim Giorno As String
            Dim Mese As String
            Dim Anno As String
            Try
                If data <> "" Then
                    Select Case sTypeFormat
                        Case "G"
                            Giorno = Mid(data, 7, 2)
                            Mese = Mid(data, 5, 2)
                            Anno = Mid(data, 1, 4)
                            Return Giorno & "/" & Mese & "/" & Anno
                        Case "A"
                            Try
                                Giorno = CStr(CDate(data).Day).PadLeft(2, "0")
                                Mese = CStr(CDate(data).Month).PadLeft(2, "0")
                                Anno = CStr(CDate(data).Year).PadLeft(4, "0")
                            Catch ex As Exception
                                Giorno = Mid(data, 1, 2)
                                Mese = Mid(data, 4, 2)
                                Anno = Mid(data, 7, 4)
                            End Try
                            Return Anno & Mese & Giorno
                        Case Else
                            Return ""
                    End Select
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Function
        ''' <summary>
        ''' leggo la data nel formato gg/mm/aaaa e la metto nel formato aaaammgg
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GiraData(ByVal data As Object) As String
            'leggo la data nel formato gg/mm/aaaa e la metto nel formato aaaammgg

            Dim Giorno As String
            Dim Mese As String
            Dim Anno As String

            GiraData = ""
            data = CToStr(data)
            If Not IsDBNull(data) And Not IsNothing(data) Then

                If data <> "" Then

                    Giorno = Right("0" & Mid(data, 1, 2), 2)
                    Mese = Right("0" & Mid(data, 4, 2), 2)
                    Anno = Mid(data, 7, 4)

                    GiraData = Anno & Mese & Giorno

                End If

            End If

            Return GiraData

        End Function
        ''' <summary>
        ''' leggo la data nel formato ggmmaaaa e la metto nel formato gg/mm/aaaa
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FormatDataFromDB(ByVal data As Object) As String
            'leggo la data nel formato ggmmaaaa e la metto nel formato gg/mm/aaaa
            Dim Giorno As String
            Dim Mese As String
            Dim Anno As String
            FormatDataFromDB = ""
            data = CToStr(data)
            If Not IsDBNull(data) And Not IsNothing(data) Then

                If data <> "" Then
                    Giorno = Mid(data, 1, 2)
                    Mese = Mid(data, 3, 2)
                    Anno = Mid(data, 5, 4)
                    FormatDataFromDB = Giorno & "/" & Mese & "/" & Anno
                End If

                If IsDate(FormatDataFromDB) = False Then
                    Giorno = Mid(data, 1, 2)
                    Mese = Mid(data, 3, 2)
                    Anno = Mid(data, 5, 4)
                    FormatDataFromDB = Mese & "/" & Giorno & "/" & Anno
                End If
            End If
            Return FormatDataFromDB
        End Function
        ''' <summary>
        ''' leggo la data nel formato aaaammgg  e la metto nel formato gg/mm/aaaa
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GiraDataFromDB(ByVal data As Object) As String
            'leggo la data nel formato aaaammgg  e la metto nel formato gg/mm/aaaa
            Dim Giorno As String
            Dim Mese As String
            Dim Anno As String
            GiraDataFromDB = ""
            data = CToStr(data)
            If Not IsDBNull(data) And Not IsNothing(data) Then

                If data <> "" Then
                    Giorno = Mid(data, 7, 2)
                    Mese = Mid(data, 5, 2)
                    Anno = Mid(data, 1, 4)
                    GiraDataFromDB = Giorno & "/" & Mese & "/" & Anno
                End If

                If IsDate(GiraDataFromDB) = False Then
                    Giorno = Mid(data, 7, 2)
                    Mese = Mid(data, 5, 2)
                    Anno = Mid(data, 1, 4)
                    GiraDataFromDB = Mese & "/" & Giorno & "/" & Anno
                End If
            End If
            Return GiraDataFromDB
        End Function

        Public Function CToBit(ByVal vInput As Object) As Short

            CToBit = 0

            If Not IsDBNull(vInput) And Not IsNothing(vInput) Then
                If CBool(vInput) Then
                    CToBit = 1
                Else
                    CToBit = 0
                End If
            End If

        End Function
        Public Function CToBool(ByVal vInput As Object) As Boolean

            CToBool = False

            If Not IsDBNull(vInput) And Not IsNothing(vInput) Then
                CToBool = Convert.ToBoolean(vInput)
            End If

        End Function

        Public Function CDoubleToDB(ByVal vInput As Object) As String

            Dim strToDbl As String = "Null"

            If Not IsDBNull(vInput) And Not IsNothing(vInput) Then
                strToDbl = CStr(vInput)
                If strToDbl <> "" Then
                    strToDbl = Replace(strToDbl, ".", "")
                    strToDbl = Replace(strToDbl, ",", ".")
                Else
                    strToDbl = "NULL"
                End If
            End If
            Return strToDbl
        End Function

        Public Function CBoolToDB(ByVal vInput As Object) As Integer

            Dim blnToDB As Boolean = False


            If Not IsDBNull(vInput) And Not IsNothing(vInput) Then
                blnToDB = vInput
                CBoolToDB = Convert.ToInt32(blnToDB)
            Else
                CBoolToDB = 0
            End If

            Return CBoolToDB

        End Function

        Public Function CalcolaValoreCatasto(ByVal RENDITA As Decimal, ByVal CATEGORIA As String) As String


            Dim RIVALUTAZIONE As Decimal = CDec(1.05)

            If RENDITA = 0 Then
                Return "0"
            Else
                Dim Percentuale As Decimal = 0
                Dim ValoreCalcolato As Decimal = 0

                If CATEGORIA = "C01" Then

                    Percentuale = 34
                ElseIf CATEGORIA = "A10" Then
                    Percentuale = 50
                ElseIf CATEGORIA = "D01" Or CATEGORIA = "D02" Or CATEGORIA = "D03" Or CATEGORIA = "D04" Or CATEGORIA = "D05" Or CATEGORIA = "D06" Or CATEGORIA = "D07" Or CATEGORIA = "D08" Or CATEGORIA = "D09" Or CATEGORIA = "D010" Or CATEGORIA = "D012" Then
                    Percentuale = 50
                Else
                    Percentuale = 100
                End If

                ValoreCalcolato = RENDITA * Percentuale * RIVALUTAZIONE


                Return ValoreCalcolato.ToString()
            End If

        End Function

        'Public Function CalcoloValoredaRendita_NONUSATA(ByVal dblRendita As Double, ByVal strTipoImm As String, ByVal strCateg As String, ByVal strAnno As String) As Double

        '    Dim AppoggioValore As Double
        '    If dblRendita <> 0 Then
        '        Select Case strTipoImm

        '            Case "TA"

        '                AppoggioValore = dblRendita * 75
        '                If strAnno > "1997" Then
        '                    AppoggioValore = AppoggioValore + ((AppoggioValore * 25) / 100)
        '                End If

        '            Case "AF", "LC"

        '                AppoggioValore = dblRendita

        '            Case Else

        '                If strCateg.ToUpper = "A/10" Or InStr(strCateg, "D") <> 0 Then
        '                    '              AppoggioValore = dblRendita * 1.05
        '                    AppoggioValore = AppoggioValore * 50
        '                ElseIf strCateg.ToUpper = "C/1" Then
        '                    '             AppoggioValore = dblRendita * 1.05
        '                    AppoggioValore = AppoggioValore * 34
        '                Else
        '                    '            AppoggioValore = dblRendita * 1.05
        '                    AppoggioValore = AppoggioValore * 100
        '                End If
        '                If strAnno > "1997" Then
        '                    'GIULIA 12082005
        '                    AppoggioValore = AppoggioValore * 1.05
        '                    'AppoggioValore = dblRendita * 1.05
        '                End If

        '        End Select
        '    End If

        '    'giulia 18082005
        '    AppoggioValore = AppoggioValore * 1000
        '    If InStr(AppoggioValore, ",") <> 0 Then
        '        AppoggioValore = AppoggioValore + 0.5
        '        If InStr(AppoggioValore, ",") <> 0 Then
        '            AppoggioValore = Mid(AppoggioValore, 1, InStr(AppoggioValore, ","))
        '        Else
        '            AppoggioValore = AppoggioValore
        '        End If
        '    Else
        '        AppoggioValore = AppoggioValore
        '    End If
        '    AppoggioValore = AppoggioValore / 1000

        '    'AppoggioValore = AppoggioValore * 100
        '    'If InStr(AppoggioValore, ",") <> 0 Then
        '    '  AppoggioValore = AppoggioValore + 0.5
        '    '  If InStr(AppoggioValore, ",") <> 0 Then
        '    '    AppoggioValore = Mid(AppoggioValore, 1, InStr(AppoggioValore, ","))
        '    '  Else
        '    '    AppoggioValore = AppoggioValore
        '    '  End If
        '    'Else
        '    '  AppoggioValore = AppoggioValore
        '    'End If
        '    'AppoggioValore = AppoggioValore / 100

        'End Function

        Friend Function getNumeroAvviso(ByVal anno As Integer, ByVal idProvvedimento As String) As String
            'Dim idProv As New String("0", 10)
            Dim idProv As New String("0", 7)
            idProv = idProv & idProvvedimento
            'getNumeroAvviso = anno & Right(idProv, Len(idProv) - (Len(idProv) - 10))
            getNumeroAvviso = Right(anno, 2) & Right(idProv, Len(idProv) - (Len(idProv) - 7))
            Return getNumeroAvviso
        End Function
        Public Function GiraDataFromDBANCI_CNC(ByVal data As Object) As String
            'leggo la data nel formato aaaammgg  e la metto nel formato gg/mm/aaaa
            Dim Giorno As String
            Dim Mese As String
            Dim Anno As String


            GiraDataFromDBANCI_CNC = ""
            data = CToStr(data)
            If Not IsDBNull(data) And Not IsNothing(data) Then
                If data <> "" Then
                    Giorno = Mid(data, 7, 2)
                    Mese = Mid(data, 5, 2)
                    Anno = Mid(data, 1, 4)
                    Anno = Right(Anno, 2)

                    GiraDataFromDBANCI_CNC = Giorno & Mese & Anno

                End If
            End If
            Return GiraDataFromDBANCI_CNC
        End Function
        Public Function getANNOFromDBANCI_CNC(ByVal data As Object) As String
            'leggo la data nel formato aaaammgg  e la metto nel formato gg/mm/aaaa

            Dim Anno As String
            getANNOFromDBANCI_CNC = ""
            data = CToStr(data)
            If Not IsDBNull(data) And Not IsNothing(data) Then
                If data <> "" Then

                    Anno = Mid(data, 1, 4)
                    Anno = Right(Anno, 2)

                    getANNOFromDBANCI_CNC = Anno
                End If
            End If
            Return getANNOFromDBANCI_CNC
        End Function

        Public Function ReplaceDataForDB(ByVal myString As String) As String
            Dim sReturn As String
            Try
                sReturn = CDate(myString).ToString(System.Configuration.ConfigurationSettings.AppSettings("lingua_date")).Replace(".", ":")
                Return sReturn
            Catch ex As Exception
                Throw ex
                Exit Function
            End Try
        End Function
        '''' <summary>
        '''' legge la lista di parametri dell'SqlCommand in ingresso e li concatena in una stringa che restituisce
        '''' </summary>
        '''' <param name="MyCMD">SqlCommand</param>
        '''' <returns>stringa</returns>
        '''' <remarks></remarks>
        'Public Function GetValParamCmd(ByVal MyCMD As SqlClient.SqlCommand) As String
        '    Dim sReturn As String
        '    Dim x As Integer

        '    For x = 0 To MyCMD.Parameters.Count - 1
        '        sReturn += MyCMD.Parameters(x).ParameterName & "="
        '        If MyCMD.Parameters(x).DbType = DbType.String Or MyCMD.Parameters(x).DbType = DbType.DateTime Then
        '            sReturn += "'" + MyCMD.Parameters(x).Value & "',"
        '        Else
        '            sReturn += MyCMD.Parameters(x).Value & ","
        '        End If
        '    Next
        '    Return sReturn
        'End Function
        Public Function ConvertToDouble(myItem As String, sDecimalSeparator As String) As Double
            Dim myDouble As Double = 0
            Try
                If IsNumeric(myItem) Then
                    If sDecimalSeparator = "." Then
                        myDouble = myItem.Replace(",", ".")
                    ElseIf sDecimalSeparator = "," Then
                        myDouble = myItem.Replace(".", ",")
                    End If
                End If
            Catch ex As Exception
                Throw New Exception("ConvertToDouble.errore::", ex)
            End Try
            Return myDouble
        End Function
    End Class
End Namespace
