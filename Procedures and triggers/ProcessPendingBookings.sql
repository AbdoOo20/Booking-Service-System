CREATE OR ALTER PROCEDURE ProcessPendingBookings
    --@AffectedRows INT OUTPUT
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @ProviderPercentage DECIMAL(5, 2) = 10;

        CREATE TABLE #TotalPayments (
            BookingId INT,
            BankAccount NVARCHAR(MAX),
            TotalPaid DECIMAL(18, 2)
        );

        INSERT INTO #TotalPayments (BookingId, BankAccount, TotalPaid)
        SELECT p.BookingId, c.BankAccount, SUM(p.PaymentValue) AS TotalPaid
        FROM [BookingServices].[dbo].[Payments] p
        INNER JOIN [BookingServices].[dbo].[Bookings] b ON p.BookingId = b.BookingId
        INNER JOIN [BookingServices].[dbo].[Customers] c ON b.CustomerId = c.CustomerId  -- Link with customer to get BankAccount
        WHERE b.EventDate <= DATEADD(day, 4, GETDATE())  -- Only for bookings with event date within 4 days
          AND b.Status = 'Pending'
        GROUP BY p.BookingId, c.BankAccount;

        UPDATE sp
        SET sp.Balance = sp.Balance + (tp.TotalPaid * (@ProviderPercentage / 100))
        FROM [BookingServices].[dbo].[ServiceProviders] sp
        INNER JOIN [BookingServices].[dbo].[Services] s ON sp.ProviderId = s.ProviderId
        INNER JOIN [BookingServices].[dbo].[BookingServices] bs ON bs.ServiceId = s.ServiceId
        INNER JOIN [BookingServices].[dbo].[Bookings] b ON bs.BookingId = b.BookingId
        INNER JOIN #TotalPayments tp ON tp.BookingId = b.BookingId
        INNER JOIN [BookingServices].[dbo].[PaymentIncomes] pi ON pi.PaymentIncomeId = b.PaymentIncomeId
        WHERE b.EventDate <= DATEADD(day, 4, GETDATE())
          AND b.Status = 'Pending';


        ;WITH RemainingMoneyCalc AS (
        SELECT tp.BankAccount, tp.TotalPaid - (tp.TotalPaid * (pi.Percentage / 100)) - (tp.TotalPaid * (@ProviderPercentage / 100)) AS RemainingMoney
        FROM #TotalPayments tp
        INNER JOIN [BookingServices].[dbo].[Bookings] b ON tp.BookingId = b.BookingId
        INNER JOIN [BookingServices].[dbo].[PaymentIncomes] pi ON pi.PaymentIncomeId = b.PaymentIncomeId
        )

        INSERT INTO RemainingCustomerBalances (BankAccount, RemainingAmount)
        SELECT BankAccount, RemainingMoney FROM RemainingMoneyCalc;

        UPDATE [BookingServices].[dbo].[Bookings]
        SET Status = 'Canceled'
        WHERE EventDate <= DATEADD(day, 4, GETDATE())
          AND Status = 'Pending';

        --SET @AffectedRows = @@ROWCOUNT;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;




--DECLARE @AffectedRows INT;
--EXEC ProcessPendingBookings @AffectedRows OUTPUT;
--SELECT @AffectedRows AS AffectedRows;

--go

--select * from RemainingCustomerBalances;