CREATE OR ALTER PROCEDURE UpdateReservedBalance
AS
BEGIN
    BEGIN TRY
        -- Start the transaction
        BEGIN TRANSACTION;

        DECLARE @BookingId INT,
                @Price DECIMAL(18, 2),
                @PaymentIncomeId INT,
                @Percentage DECIMAL(5, 2);
        
        -- Declare a cursor for the bookings
        DECLARE booking_cursor CURSOR FOR
        SELECT b.BookingId, b.Price, b.PaymentIncomeId
        FROM Bookings b
        WHERE b.Status = 'Paid'
          AND b.EventDate <= DATEADD(day, 5, GETDATE());

        -- Open the cursor
        OPEN booking_cursor;

        -- Fetch the first row
        FETCH NEXT FROM booking_cursor INTO @BookingId, @Price, @PaymentIncomeId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Get the payment percentage for the current booking
            SELECT @Percentage = p.Percentage
            FROM PaymentIncomes p
            WHERE p.PaymentIncomeId = @PaymentIncomeId;

            -- Update the ReservedBalance for the corresponding service providers
            UPDATE sp
            SET sp.ReservedBalance += (@Price - (@Price * @Percentage / 100))
            FROM ServiceProviders sp
            JOIN Services s ON sp.ProviderId = s.ProviderId
            JOIN BookingServices bs ON s.ServiceId = bs.ServiceId
            WHERE bs.BookingId = @BookingId;

            -- Fetch the next row
            FETCH NEXT FROM booking_cursor INTO @BookingId, @Price, @PaymentIncomeId;
        END

        -- Close and deallocate the cursor
        CLOSE booking_cursor;
        DEALLOCATE booking_cursor;

        -- Update the booking status to 'Confirmed'
        UPDATE Bookings 
        SET Status = 'Confirmed'
        WHERE Status = 'Paid'
          AND EventDate <= DATEADD(day, 5, GETDATE());

        -- Check if any rows were updated
        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Failed to update Booking status or Reserved Balance.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Commit the transaction if no errors
        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        -- Handle errors
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END
        
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(), 
               @ErrorSeverity = ERROR_SEVERITY(), 
               @ErrorState = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

exec UpdateReservedBalance
