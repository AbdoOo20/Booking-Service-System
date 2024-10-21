create trigger ChangeBookingStatus_Trg 
on Payments
after insert
as
/* Declare Variables */
declare @bookingID int
declare @paymentValue decimal
declare @price decimal
declare @totalPaymentValue decimal
/* Select PaymentValue And BookingId */
select @paymentValue = PaymentValue  , @bookingID = BookingId 
FROM INSERTED
/* Select Booking Price */
select @price = Price 
from Bookings 
where BookingId = @bookingID
/* Select Total Payment Values*/
select @totalPaymentValue = SUM(PaymentValue)
from Payments
where BookingId = @bookingID
/* Updated Table*/
if(@totalPaymentValue >= @price)
begin 
	update Bookings set Status = 'Paid' where BookingId = @bookingID
end