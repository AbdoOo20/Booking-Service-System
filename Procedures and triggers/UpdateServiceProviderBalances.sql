create procedure UpdateServiceProviderBalances
as
begin
    begin transaction;
    declare @PreviousDate date = cast(dateadd(day, -1, getdate()) as date);

    ;with ConfirmedBookings as (
        select 
            sp.ProviderId, sum(b.Price) as TotalPrice
        from 
            BookingServices.dbo.Bookings b
            join BookingServices.dbo.BookingServices bs on b.BookingId = bs.BookingId
            join BookingServices.dbo.[Services] s on bs.ServiceId = s.ServiceId
            join BookingServices.dbo.ServiceProviders sp on s.ProviderId = sp.ProviderId
        where 
            b.EventDate = @PreviousDate and b.Status = 'Confirmed'
        group by 
            sp.ProviderId
    )

    update sp
    set 
        sp.Balance = sp.Balance + cb.TotalPrice,
        sp.ReservedBalance = sp.ReservedBalance - cb.TotalPrice
    from 
        BookingServices.dbo.ServiceProviders sp
        join ConfirmedBookings cb on sp.ProviderId = cb.ProviderId;

    commit transaction;
end;
