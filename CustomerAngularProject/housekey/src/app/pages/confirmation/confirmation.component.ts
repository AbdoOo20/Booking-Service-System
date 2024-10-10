import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PaymentsService } from '@services/payments.service';
import { PayPalService } from '@services/pay-pal.service';
import { BookingService } from '@services/booking.service';
import { DataService } from '@services/data.service';
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
  paymentDetails: any; // Store payment details here
  BookingDeyails: any;


  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private paymentsService: PaymentsService, // Inject PaymentsService
    private payPal: PayPalService, // Inject PayPalService
    private Booking: BookingService, // Inject BookingService
    private dataService: DataService, // Inject DataService
  ) {
    this.confirmationMessage = 'Your Payment has been confirmed!';
    this.confirmationDate = new Date().toLocaleString(); // Current date and time
    this.transactionId = this.paymentId; // Example transaction ID
    this.amount = ''; // Example amount
  }

  ngOnInit(): void {
    // Access paymentId from query parameters
    this.dataService.data$.subscribe((data) => {
      this.BookingDeyails = data;
    });
    this.route.queryParams.subscribe(queryParams => {
      this.paymentId = queryParams['paymentId']; // Fetch the paymentId from URL
      console.log('Payment ID from URL:', this.paymentId);

      // Call the service to get payment details
      if (this.paymentId) {
        this.transactionId = this.paymentId;
        this.getPaymentDetails(this.paymentId);
      }
    });

  }

  // Method to get payment details by ID
  getPaymentDetails(paymentId: string): void {
    this.payPal.getPaymentsById(paymentId).subscribe(
      {
        next: (response) => {
          console.log('PayPal payment response:', response);
          if (response.state === "created") {
            this.amount = response.transactions[0].amount.total;
            console.log("Payment state is 'created'. Proceeding with addPayment.");
            this.paymentsService.addPayment({
              customerId: "244d9e75-8919-457c-ae3b-9c473167f1dc", // Example customerId
              bookingId: 113, // Example bookingId
              paymentDate: new Date().toISOString(), // Current date/time in ISO format
              paymentValue: response.transactions[0].amount.total // Extract payment value from PayPal response
            }).subscribe(
              {
                next: (paymentResponse) => {
                  console.log("Payment added successfully:", paymentResponse);
                },
                error: (err) => {
                  console.log("Error while adding payment:", err);
                }
              }
            );
            this.Booking.addBooking(this.BookingDeyails).subscribe({
              next: (BookingResponse) => {
                console.log("Booking AddSuccessfully", BookingResponse);
              },
              error: (err) => {
                console.log("Error while adding Booking:", err);
              }
            })
          } else {
            console.log("Payment state is not 'created'. State:", response.state);
          }
        },
        error: (err) => {
          console.log("Error fetching payment details:", err);
        }
      }
    );
  }


  // Redirect to home page
  goToHome(): void {
    this.router.navigate(['/home']);
  }

  // Print the confirmation
  printConfirmation() {
    const printContents = document.getElementById('printSection')?.innerHTML;
    const originalContents = document.body.innerHTML;

    if (printContents) {
      document.body.innerHTML = printContents;
      window.print();
      document.body.innerHTML = originalContents;
      window.location.reload(); // Reload the page to restore the content
    }
  }

}
