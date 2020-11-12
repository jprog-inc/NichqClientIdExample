Public Class NichqClientIdTests

    Public Shared Sub CheckTypicalUse()

        Try


            'Typical use case

            'Make the URN
            Dim lastName As String = "Heartland"
            Dim firstName As String = "Harry"
            Dim dateOfBirth As Date = Date.Parse("1/23/1989")

            '1	Male
            '2	Female
            '3	Transgender Other
            '4	Transgender MtF
            '5	Transgender FtM	       
            '6	Refused to Report
            '9	Unknown
            Dim gender As String = "1"

            Dim harrysUrn As String = NichqClientId.MakeHrsaUrn(lastName, firstName, dateOfBirth, gender)
            Console.WriteLine("Harry's URN: " + harrysUrn)

            Dim orgId As String = "001"
            Dim clientType As String = "PP"

            Dim NichqClnId As String = NichqClientId.MakeClientId(orgId, clientType, harrysUrn)

            Console.WriteLine("Harry's Client ID: " + NichqClnId)
            AssertExpectedGot("001PPG3J7AZKZ7HNGWBZHQG3D7VV5SQOQHZNN", NichqClnId)

        Catch ex As Exception

            Throw New Exception("FAIL " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + ex.Message)

        End Try

    End Sub
    Private Shared Sub AssertExpectedGot(pExpected, pGot)
        If pExpected <> pGot Then
            Throw New Exception("Expected: " + pExpected + " Got: " + pGot)
        End If
    End Sub

    Public Shared Sub CheckSha1AndBase32Encoding()
        Try
            'Check the SHA1 hash and Base32 Encoding against codes generated on the same URNs in a Google Go test app

            AssertExpectedGot("ZS6OBDYFQQSJWMP2CL444C4OALRD7BFG", NichqClientId.AnonymizeURN("JMJM0101991U"))
            AssertExpectedGot("FMEOWFT7QY6W35ANOI23H22KXVUUKCNC", NichqClientId.AnonymizeURN("LAKD0101992U"))

        Catch ex As Exception

            Throw New Exception("FAIL " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + ex.Message)

        End Try
    End Sub

    Public Shared Sub CheckOddCharsInName()
        Try
            AssertExpectedGot("M9E90101891U", NichqClientId.MakeHrsaUrn("El'druin", "Ma`rk", Date.Parse("01/01/1989"), "1"))
            AssertExpectedGot("M9E90101891U", NichqClientId.MakeHrsaUrn("El'druin", "Ma`rk", Date.Parse("01/01/1989"), "1"))
            AssertExpectedGot("R9D90101891U", NichqClientId.MakeHrsaUrn("D2", "R2", Date.Parse("01/01/1989"), "1"))
        Catch ex As Exception

            Throw New Exception("FAIL " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + ex.Message)

        End Try
    End Sub
    Public Shared Sub CheckBirthDates()

        Try

            Dim exThrown As Boolean = False

            Try


                NichqClientId.MakeHrsaUrn("D2", "R2", Date.Today.AddDays(1), "1")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("future") = -1 Then
                    Throw New Exception("Wrong future date")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed future date")
                End If
            End Try


            Try

                exThrown = False


                NichqClientId.MakeHrsaUrn("D2", "R2", Date.Today.AddDays(-1 * (365 * 150)), "1")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("too old") = -1 Then
                    Throw New Exception("Wrong too old date")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed date past 150 years old ")
                End If
            End Try

            Try

                exThrown = False

                'nothing translates to min date
                NichqClientId.MakeHrsaUrn("D2", "R2", Nothing, "1")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("too old") = -1 Then
                    Throw New Exception("Wrong too old date")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed date past 150 years old ")
                End If
            End Try
        Catch ex As Exception

            Throw New Exception("FAIL " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + ex.Message)

        End Try
    End Sub

    Public Shared Sub CheckGender()

        Try

            Dim exThrown As Boolean = False

            Try


                NichqClientId.MakeHrsaUrn("D2", "R2", Date.Parse("01/01/1989"), "")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("Gender is required") = -1 Then
                    Throw New Exception("Wrong gender message")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed blank gender")
                End If
            End Try

            Try

                exThrown = False

                NichqClientId.MakeHrsaUrn("D2", "R2", Date.Parse("01/01/1989"), "M")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("is not a valid code") = -1 Then
                    Throw New Exception("Wrong gender message")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed invalid gender code")
                End If
            End Try


        Catch ex As Exception

            Throw New Exception("FAIL " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + ex.Message)

        End Try
    End Sub
    Public Shared Sub CheckUrnSuffix()

        Try

            Dim exThrown As Boolean = False

            Try


                NichqClientId.MakeHrsaUrn("D2", "R2", Date.Parse("01/01/1989"), "1", "")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("The URN suffix") = -1 Then
                    Throw New Exception("Wrong suffix message")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed blank suffix")
                End If
            End Try

            Try

                exThrown = False

                NichqClientId.MakeHrsaUrn("D2", "R2", Date.Parse("01/01/1989"), "1", "4")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("character A-Z") = -1 Then
                    Throw New Exception("Wrong suffix message 2")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed invalid suffix 2")
                End If
            End Try


        Catch ex As Exception

            Throw New Exception("FAIL " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + ex.Message)

        End Try
    End Sub
    Public Shared Sub CheckOrgID()

        Try

            Dim exThrown As Boolean = False

            Try

                NichqClientId.MakeClientId("", "PP", "JMJM0101991U")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("Org ID is required") = -1 Then
                    Throw New Exception("Wrong Org ID message")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed blank Org ID")
                End If
            End Try

            Try

                exThrown = False

                NichqClientId.MakeClientId("2", "PP", "JMJM0101991U")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("Org ID length must be 3") = -1 Then
                    Throw New Exception("Wrong Org ID message 2")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed short Org ID")
                End If
            End Try

            Try

                exThrown = False

                NichqClientId.MakeClientId("12S", "PP", "JMJM0101991U")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf(" only contain digits") = -1 Then
                    Throw New Exception("Wrong Org ID about digits")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed Org ID with a letter")
                End If
            End Try

        Catch ex As Exception

            Throw New Exception("FAIL " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + ex.Message)

        End Try
    End Sub

    Public Shared Sub CheckClientType()

        Try

            Dim exThrown As Boolean = False

            Try

                NichqClientId.MakeClientId("001", "", "JMJM0101991U")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("Client Type is required") = -1 Then
                    Throw New Exception("Wrong client type message")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed blank client type")
                End If
            End Try

            Try

                exThrown = False

                NichqClientId.MakeClientId("200", "Priority", "JMJM0101991U")

            Catch ex As Exception

                exThrown = True

                If ex.Message.IndexOf("Client Type must be PP or EC") = -1 Then
                    Throw New Exception("Wrong client type message 2")
                End If
            Finally
                If Not exThrown Then
                    Throw New Exception("Allowed wrong client type")
                End If
            End Try

        Catch ex As Exception

            Throw New Exception("FAIL " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " " + ex.Message)

        End Try
    End Sub

End Class
