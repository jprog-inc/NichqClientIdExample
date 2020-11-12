Imports System.Security.Cryptography

Public Class NichqClientId

    'Makes the NICHQ Client ID
    'If the args have issuer an Argument exception is thrown
    Public Shared Function MakeClientId(pOrgId As String, pClientType As String, pUrn As String) As String

        assertOrgIdOk(pOrgId)
        assertOrgClientTypeIsOk(pClientType)

        Dim urnHash As String = AnonymizeURN(pUrn)

        Return pOrgId + pClientType + urnHash

    End Function


    'Makes a URN from the supplied PII
    'Throws an ArgumentException if the supplied values do not meet the minimum standards
    Public Shared Function MakeHrsaUrn(pLastName As String, pFirstName As String, pDateOfBirth As Date, pGender As String, Optional pUrnSuffix As String = "U")

        Dim lastNamePart As String = makeURNNamePart(pLastName)

        Dim firstNamePart As String = makeURNNamePart(pFirstName)

        Dim birthDayPart As String = makeBirthDayPart(pDateOfBirth)

        assertGenderIsValidCode(pGender)

        assertUrnSuffixIsOk(pUrnSuffix)

        Return (firstNamePart + lastNamePart + birthDayPart + pGender + pUrnSuffix).ToUpper

    End Function

    'Gets a SHA1 hash for a URN and encodes it as a Base32 string
    Public Shared Function AnonymizeURN(pUrn As String) As String

        assertUrnIsOk(pUrn)

        Using thisSha1Obj As SHA1 = SHA1.Create()

            'Convert the URN string to bytes
            Dim urnBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(pUrn)

            'Compute the SHA1 hash from the urn
            Dim sha1Bytes As Byte() = thisSha1Obj.ComputeHash(urnBytes)

            'Convetr the SHA1 bytes to a Base32 String
            Return toBase32String(sha1Bytes)

        End Using


    End Function

    'Takes a frist name or last name and makes a 2 character code for it
    'this code comforms to the URN specs of 1st and 3rd character
    'with 9s for non characters
    Private Shared Function makeURNNamePart(ByVal NamePart As String) As String

        If NamePart.Length >= 3 Then

            If Char.IsLetter(NamePart.Chars(0)) AndAlso Char.IsLetter(NamePart.Chars(2)) Then
                Return NamePart.Substring(0, 1) + NamePart.Substring(2, 1)
            ElseIf Char.IsLetter(NamePart.Chars(0)) Then
                Return NamePart.Substring(0, 1) + "9"
            ElseIf Char.IsLetter(NamePart.Chars(2)) Then
                Return "9" + NamePart.Substring(2, 1)
            Else
                Return "99"
            End If

        ElseIf NamePart.Length > 0 Then

            If Char.IsLetter(NamePart.Chars(0)) Then
                Return NamePart.Substring(0, 1) + "9"
            Else
                Return "99"
            End If

        Else

            'We should never get here because we check the names
            'before genterating the base URN
            Return ""

        End If

    End Function

    'Formats a birth day for the urn
    Private Shared Function makeBirthDayPart(pBirthDay As Date) As String


        If pBirthDay > Date.Today Then

            Throw New ArgumentException("Birth day (" + pBirthDay.ToShortDateString + ") cannot be in the future.")

        End If

        If Date.Today.Subtract(pBirthDay).TotalDays / 360 > 150 Then

            Throw New ArgumentException("Birth day (" + pBirthDay.ToShortDateString + ") makes client too old.")
        End If

        Return Format(pBirthDay.Month, "00") & Format(pBirthDay.Day, "00") & Right(pBirthDay.Year.ToString, 2)

    End Function
    ' Trows an exception if the gender is not a valid code
    Private Shared Sub assertGenderIsValidCode(pGender As String)

        '1	Male
        '2	Female
        '3	Transgender Other
        '4	Transgender MtF
        '5	Transgender FtM	       
        '6	Refused to Report
        '9	Unknown


        assertStringHasData(pGender, "Gender is required for URN")

        'We need to mae a way to dynamically load coded or check RFT validation checks
        If Not (pGender = "1" OrElse pGender = "2" OrElse pGender = "3" OrElse pGender = "4" OrElse pGender = "5" OrElse pGender = "6" OrElse pGender = "9") Then

            Throw New ArgumentException(pGender + " is not a valid code for a URN.")

        End If



    End Sub

    'Throws exception if the URN suffix dies not meet the rules
    Private Shared Sub assertUrnSuffixIsOk(pUrnSuffix)

        assertStringHasData(pUrnSuffix, "The URN suffix cannot be blank.")

        If pUrnSuffix.Length <> 1 OrElse (Not Char.IsLetter(pUrnSuffix.Chars(0))) Then

            Throw New ArgumentException("URN Suffix can only be 1 character A-Z")

        End If

    End Sub

    Private Shared Sub assertOrgClientTypeIsOk(pClientId As String)

        assertStringHasData(pClientId, "Client Type is required when making NICHQ Client ID")

        If pClientId <> "PP" AndAlso pClientId <> "EC" Then

            Throw New ArgumentException("Client Type must be PP or EC")

        End If

    End Sub
    'Throws exception if the string has no data
    Private Shared Sub assertStringHasData(pStr As String, pErrMsg As String)

        If pStr Is Nothing OrElse pStr = "" Then

            Throw New ArgumentException(pErrMsg)

        End If

    End Sub

    Private Shared Sub assertOrgIdOk(pOrgId As String)

        assertStringHasData(pOrgId, "Org ID is required for making NICHQ Client ID")

        If pOrgId.Length <> 3 Then

            Throw New ArgumentException("Org ID length must be 3 when making NICHQ Client ID")

        End If

        For Each c As Char In pOrgId.ToCharArray

            If Not Char.IsDigit(c) Then

                Throw New ArgumentException("Org Id can only contain digits when making NICHQ Client ID")

            End If

        Next

    End Sub

    Private Shared Sub assertUrnIsOk(pUrn As String)

        assertStringHasData(pUrn, "URN is required for making NICHQ Client ID")

        If pUrn.Length <> 12 Then

            Throw New ArgumentException("The length of a URN must be 12")

        End If

        'Check the name portion
        For Each c As Char In pUrn.Substring(0, 4).ToCharArray

            If Char.IsLetter(c) Then

                If Not Char.IsUpper(c) Then

                    Throw New ArgumentException("Any letter in the first 4 characters must be upper case of a URN")

                End If

            ElseIf Char.IsDigit(c) Then

                If c <> "9"c Then

                    Throw New ArgumentException("The digit 9 is the only digit allowed in the first 4 characters of a URN")

                End If

            Else

                Throw New ArgumentException("Only letters and digits are allowed in the 4 characters of a URN")

            End If

        Next

        'check the month portion of the birth day
        Dim firstMonthChar As Char = pUrn.Substring(4, 1).Chars(0)
        Dim secondMonthChar As Char = pUrn.Substring(5, 1).Chars(0)
        Dim firstDayChar As Char = pUrn.Substring(6, 1).Chars(0)
        Dim secondDayChar As Char = pUrn.Substring(7, 1).Chars(0)
        Dim firstYearChar As Char = pUrn.Substring(8, 1).Chars(0)
        Dim SecondYearChar As Char = pUrn.Substring(9, 1).Chars(0)
        If Not (Char.IsDigit(firstMonthChar) AndAlso Char.IsDigit(secondMonthChar) AndAlso Char.IsDigit(firstDayChar) AndAlso Char.IsDigit(secondDayChar) AndAlso Char.IsDigit(firstYearChar) AndAlso Char.IsDigit(SecondYearChar)) Then

            Throw New ArgumentException("The date porion (characters 5 through 10) can only be digits in a URN")

        End If

        Dim month As Integer = Integer.Parse(firstMonthChar.ToString + secondMonthChar.ToString)

        If month < 1 OrElse month > 12 Then
            Throw New ArgumentException("Birth month is invalid for URN")
        End If

        Dim day As Integer = Integer.Parse(firstMonthChar.ToString + secondMonthChar.ToString)

        If day < 1 OrElse day > 31 Then

            Throw New ArgumentException("Birth day is invalid for URN")

        End If

        'since we only have 2 digits for the year we will skip year validation and skip checking for valid date

        assertGenderIsValidCode(pUrn.Substring(10, 1))

        Dim modifier As String = pUrn.Substring(11, 1).Chars(0)

        If Not (Char.IsLetter(modifier) AndAlso Char.IsUpper(modifier)) Then

            Throw New ArgumentException("The URN modifier must be an  upper case letter")

        End If

    End Sub

    '.Net does not have a native Base 32 encoding function
    'so the rest of this code implements the standard
    Private Shared base32Chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"

    Private Shared Function toBase32String(ByVal bytes() As Byte) As String

        Dim bs8Bit As String = ""
        For i As Integer = 0 To bytes.Length - 1
            'convert each byte to 8-bit
            bs8Bit &= Convert.ToString(bytes(i), 2).PadLeft(8, "0"c)
        Next

        Dim base32EncodedSB As New System.Text.StringBuilder
        'evaluate every 5 bits and convert each to an integer and match it to the base32 character set
        For i As Integer = 0 To bs8Bit.Length - 1 Step 5

            Dim bs5Bit As String = bs8Bit.Substring(i)
            'make sure every segment is 5-bits
            If bs5Bit.Length > 5 Then
                bs5Bit = bs8Bit.Substring(i, 5)
            Else
                bs5Bit = bs5Bit.PadRight(5, "0"c)
            End If

            'Convert the 5 bit number to its integer value
            Dim base32CharPos As Integer = binary5BitStringToInteger(bs5Bit)

            'Encode the integer to it's Base32 character
            Dim b32Char As String = base32Chars(base32CharPos)

            'Append the Base32 character to the encoded string
            base32EncodedSB.Append(b32Char)

        Next

        Return base32EncodedSB.ToString

    End Function

    Private Shared Function binary5BitStringToInteger(bin5BitStr As String) As Integer

        Dim rVal As Integer = 0
        Dim bPos As Integer = 0
        For i As Integer = bin5BitStr.Length - 1 To 0 Step -1
            If bin5BitStr(i) = "1"c Then
                'calculate the value of each 1 from it's postition in the string
                rVal += (2 ^ bPos)
            End If
            bPos += 1
        Next

        Return rVal

    End Function


End Class
