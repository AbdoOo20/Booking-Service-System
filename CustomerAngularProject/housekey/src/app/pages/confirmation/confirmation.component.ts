import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PaymentsService } from '@services/payments.service';
import { PayPalService } from '@services/pay-pal.service';
import { BookingService } from '@services/booking.service';
import { DecodingTokenService } from '@services/decoding-token.service';

@Component({
  selector: 'app-confirmation',
  standalone: true,
  imports: [],
  templateUrl: './confirmation.component.html',
  styles: ``,
})
export class ConfirmationComponent implements OnInit {
  confirmationMessage: string;
  confirmationDate: string;
  transactionId: string;
  amount: string;
  paymentId: string;
  paymentDetails: any;
  bookingDetails: any;
  bookingData: any;
  NewBookingObject: any;
  serviceObject: Object;
  BookingIdFromPayInstallment: any;
  CustomerID: string;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private paymentsService: PaymentsService,
    private payPal: PayPalService,
    private bookingService: BookingService,
    private decodeService: DecodingTokenService,
  ) {
    this.confirmationMessage = 'Your Payment has been confirmed!';
    this.confirmationDate = new Date().toLocaleString();
    this.transactionId = '';
    this.amount = '';
  }

  ngOnInit(): void {
    console.log(this.decodeService.getUserIdFromToken());
    this.CustomerID = this.decodeService.getUserIdFromToken();
    // Retrieve the payment ID from query parameters
    this.route.queryParams.subscribe(queryParams => {
      this.paymentId = queryParams['paymentId'];
      console.log('Payment ID retrieved:', this.paymentId); // Log the payment ID


      this.BookingIdFromPayInstallment = localStorage.getItem('bookingID');
      if (this.BookingIdFromPayInstallment)
        console.log(this.BookingIdFromPayInstallment);

      // Retrieve booking data from local storage if available
      const savedBookingData = localStorage.getItem('bookingData');
      if (savedBookingData) {
        this.bookingData = JSON.parse(savedBookingData);
        console.log('Booking data retrieved:', this.bookingData); // Log the booking data
      }

      if (this.paymentId) {
        this.transactionId = this.paymentId;
        this.getPaymentDetails(this.paymentId);  // Fetch payment details
      }
    });
  }

  getPaymentDetails(paymentId: string): void {
    console.log('Fetching payment details for ID:', paymentId); // Log fetching details
    this.payPal.getPaymentsById(paymentId).subscribe({
      next: (response) => {
        console.log('Payment details response:', response); // Log the response
        if (response.state === 'created') {
          this.amount = response.transactions[0].amount.total;
          console.log('Payment amount:', this.amount); // Log the payment amount

          // Check if booking details exist before adding booking
          if (this.bookingData) {
            // Add booking
            console.log("this in Confirmation and BookingObject:");

            console.log(this.bookingData);
            this.bookingService.addBooking(JSON.stringify(this.bookingData, null, 2)).subscribe({
              
              next: (bookingResponse) => {
                console.log('Booking added successfully:', bookingResponse); // Log booking response
                // Add payment
                this.paymentsService.addPayment({
                  customerId: this.CustomerID,
                  bookingId: bookingResponse.id,
                  paymentDate: this.bookingData.selectedDate,
                  paymentValue: response.transactions[0].amount.total
                }).subscribe({
                  next: (paymentResponse) => {
                    console.log('Payment added successfully:', paymentResponse); // Log payment response
                  },
                  error: (err) => {
                    console.error('Error adding payment:', err); // Log payment error
                  }
                });
              },
              error: (err) => {
                console.error('Error adding booking:', err); // Log booking error
              }
            });
            localStorage.removeItem('bookingData');
          }
          else if (this.BookingIdFromPayInstallment) {
            console.log("Payment without Book" + this.BookingIdFromPayInstallment);
            this.paymentsService.addPayment({
              customerId: this.CustomerID,
              bookingId: this.BookingIdFromPayInstallment,
              paymentDate: new Date().toISOString(),
              paymentValue: response.transactions[0].amount.total
            }).subscribe({
              next: (paymentResponse) => {
                console.log('Payment added successfully:', paymentResponse); // Log payment response
              },
              error: (err) => {
                console.error('Error adding payment:', err); // Log payment error
              }
            });
            localStorage.removeItem('bookingID');
          }
        }
      },
      error: (err) => {
        console.error('Error fetching payment details:', err); // Log error fetching payment details
      }
    });
  }

  // Redirect to home page
  goToHome(): void {
    console.log('Navigating to home page'); // Log navigation action
    this.router.navigate(['/home']);
  }

  // Print the confirmation
  printConfirmation() {
    const printContents = document.getElementById('printSection')?.innerHTML;
    const originalContents = document.body.innerHTML;

    if (printContents) {
      console.log('Printing confirmation'); // Log print action
      document.body.innerHTML = printContents;
      window.print();
      document.body.innerHTML = originalContents;
      window.location.reload();
    }
  }
}
