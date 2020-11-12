
Module Module1

    Sub Main()

        Try
            NichqClientIdTests.CheckTypicalUse()
            NichqClientIdTests.CheckSha1AndBase32Encoding()
            NichqClientIdTests.CheckOddCharsInName()
            NichqClientIdTests.CheckBirthDates()
            NichqClientIdTests.CheckGender()
            NichqClientIdTests.CheckUrnSuffix()
            NichqClientIdTests.CheckOrgID()
            NichqClientIdTests.CheckClientType()

            Console.WriteLine("All Tests Pass!")
        Catch ex As Exception

            Console.WriteLine(ex.Message)

        End Try

        Console.ReadLine()

    End Sub


End Module
