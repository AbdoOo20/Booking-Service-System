import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PaymentsService } from '@services/payments.service';
import { PayPalService } from '@services/pay-pal.service';
import { BookingService } from '@services/booking.service';
import { DecodingTokenService } from '@services/decoding-token.service';
import { ServiceForConfirmation } from '@services/ServiceForConfirmation';

@Component({
  selector: 'app-confirmation',
  standalone: true,
  imports: [], // Make sure to add CommonModule here if needed
  templateUrl: './confirmation.component.html',
  styles: [],
})
export class ConfirmationComponent implements OnInit {
  confirmationMessage: string;
  confirmationDate: string;
  transactionId: string;
  serviceName: string;
  customerName: string;
  eventDate: any;
  bookingStatus: any;
  totalPrice: number;
  amount: string;
  paymentId: string;
  bookingData: any;
  BookingIdFromPayInstallment: any;
  CustomerID: string;
  formattedEventDate: string; // New property to hold formatted date
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private paymentsService: PaymentsService,
    private payPal: PayPalService,
    private bookingService: BookingService,
    private decodeService: DecodingTokenService,
    private ServiceForConfirmation: ServiceForConfirmation,
  ) {
    this.confirmationMessage = 'Your Payment has been confirmed!';
    this.confirmationDate = new Date().toLocaleString();
    this.transactionId = '';
    this.amount = '';
    this.totalPrice = 0; // Initialize totalPrice
  }

  ngOnInit(): void {
    this.CustomerID = this.decodeService.getUserIdFromToken();

    // Retrieve the payment ID from query parameters
    this.route.queryParams.subscribe(queryParams => {
      this.paymentId = queryParams['paymentId'];
      this.BookingIdFromPayInstallment = localStorage.getItem('bookingID');

      // Retrieve booking data from local storage if available
      const savedBookingData = localStorage.getItem('bookingData');
      if (savedBookingData) {
        this.bookingData = JSON.parse(savedBookingData);
        this.bookingData = JSON.parse(savedBookingData);
        console.log('Booking data retrieved:', this.bookingData); // Log the booking data
        this.bookingData = JSON.parse(savedBookingData);
        console.log('Booking data retrieved:', this.bookingData); // Log the booking data
      }

      if (this.paymentId) {
        this.transactionId = this.paymentId; // Set the transaction ID
        this.getPaymentDetails(this.paymentId); // Fetch payment details
      }
    });
  }

  getPaymentDetails(paymentId: string): void {
    this.payPal.getPaymentsById(paymentId).subscribe({
      next: (response) => {
        if (response.state === 'created') {
          this.amount = response.transactions[0].amount.total;

          if (this.bookingData) {
            this.totalPrice = this.bookingData.price; // Assign totalPrice from booking data

            if (this.amount == this.bookingData.price) {
              this.bookingData.status = "Paid";
              this.bookingData.cashOrCashByHandOrInstallment = "Cash";
            } else {
              this.bookingData.status = "Pending";
              this.bookingData.cashOrCashByHandOrInstallment = "Installment";
            }
            this.eventDate = this.bookingData.eventDate;
            this.formattedEventDate = this.formatEventDate(this.eventDate);
            this.bookingStatus = this.bookingData.status;

            // Fetch service name
            this.ServiceForConfirmation.getServiceName(this.bookingData.serviceId).subscribe({
              next: (data) => {
                this.serviceName = data.serviceName; // Adjust based on your response
              }
            });

            // Fetch customer name
            this.customerName = this.decodeService.getUserNameFromToken();

            // Add booking
            this.bookingService.addBooking(JSON.stringify(this.bookingData)).subscribe({
              next: (bookingResponse) => {
                this.paymentsService.addPayment({
                  customerId: this.CustomerID,
                  bookingId: bookingResponse.id,
                  paymentDate: this.bookingData.selectedDate,
                  paymentValue: this.amount,
                }).subscribe();
              }
            });

          } else if (this.BookingIdFromPayInstallment) {
            // Payment without booking
            this.paymentsService.addPayment({
              customerId: this.CustomerID,
              bookingId: this.BookingIdFromPayInstallment,
              paymentDate: new Date().toISOString(),
              paymentValue: this.amount,
            }).subscribe();
          }
        }
      },
      error: (err) => {
        console.error('Error fetching payment details:', err);
      }
    });
  }

  formatEventDate(dateString: string): string {
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'long', // 'short' for abbreviated month names
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true, // Set to false for 24-hour format
    };
    return new Date(dateString).toLocaleString('en-US', options);
  }

  // Redirect to home page
  goToHome(): void {
    this.router.navigate(['/home']);
    localStorage.removeItem('bookingData');
    localStorage.removeItem('bookingID');
  }

  // Print the confirmation
  printConfirmation(): void {
    const printContents = document.getElementById('printSection')?.innerHTML;
    const originalContents = document.body.innerHTML;

    if (printContents) {
      document.body.innerHTML = printContents;
      window.print();
      document.body.innerHTML = originalContents;
      window.location.reload();
    }
  }

}
