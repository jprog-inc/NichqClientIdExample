
Module Module1

    'These comments are not intended to provide any useful information about this code.
    'I am just testing making some code change and pushing it to the repository using the GitKraken GUI tool.
    'Feel free to delete this.

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
