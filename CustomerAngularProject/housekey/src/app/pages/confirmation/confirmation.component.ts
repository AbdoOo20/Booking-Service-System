import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { PaymentsService } from "@services/payments.service";
import { PayPalService } from "@services/pay-pal.service";
import { BookingService } from "@services/booking.service";
import { DecodingTokenService } from "@services/decoding-token.service";
import { ServiceForConfirmation } from "@services/ServiceForConfirmation";

@Component({
  selector: "app-confirmation",
  standalone: true,
  imports: [], // Make sure to add CommonModule here if needed
  templateUrl: "./confirmation.component.html",
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
  paymentDetails: any; // To store the payment details after the first call

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private paymentsService: PaymentsService,
    private payPal: PayPalService,
    private bookingService: BookingService,
    private decodeService: DecodingTokenService,
    private ServiceForConfirmation: ServiceForConfirmation
  ) {
    this.confirmationMessage = "Your Payment has been confirmed!";
    this.confirmationDate = new Date().toLocaleString();
    this.transactionId = "";
    this.amount = "";
    this.totalPrice = 0; // Initialize totalPrice
  }

  ngOnInit(): void {
    this.CustomerID = this.decodeService.getUserIdFromToken();

    // Retrieve the payment ID from query parameters
    this.route.queryParams.subscribe((queryParams) => {
      this.paymentId = queryParams["paymentId"];
      console.log(localStorage.getItem("bookingID"));

      this.BookingIdFromPayInstallment = localStorage.getItem("bookingID");

      // Retrieve booking data from local storage if available
      const savedBookingData = localStorage.getItem("bookingData");

      if (savedBookingData) {
        this.bookingData = JSON.parse(savedBookingData);
        let eventDate = new Date(this.bookingData.eventDate);
        eventDate.setHours(eventDate.getHours() + 3);
        this.bookingData.eventDate = eventDate;

        let bookDate = new Date(this.bookingData.bookDate);
        bookDate.setHours(bookDate.getHours() + 3);
        this.bookingData.bookDate = bookDate;
      }

      if (this.paymentId) {
        this.transactionId = this.paymentId; // Set the transaction ID
        this.getPaymentDetails(this.paymentId); // Fetch payment details
      }
    });
  }

  getPaymentDetails(paymentId: string): void {

    if (localStorage.getItem('firstCall') == 'true') {

      // Call the API for the first time    
      this.payPal.getPaymentsById(paymentId).subscribe({
        next: (response) => {
          localStorage.setItem('firstCall', 'false');
          this.paymentDetails = response; // Store the response for later use

          if (response.state === "created") {
            this.amount = response.transactions[0].amount.total;

            this.processPaymentDetails(response); // Process payment details
          }
        },
        error: (err) => {
          console.error("Error fetching payment details:", err);
        },
      });
    } else if (localStorage.getItem('firstCall') == 'false') return;

  }

  // A method to process the payment details, reducing redundancy
  processPaymentDetails(response: any): void {

    let bankAccount;
    if (localStorage.getItem('SetBankAccount') == 'true') {
      bankAccount = response.payer.payer_info.email;
    }

    if (this.bookingData) {
      this.totalPrice = this.bookingData.price;

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

      this.ServiceForConfirmation.getServiceName(this.bookingData.serviceId).subscribe({
        next: (data) => {
          this.serviceName = data.serviceName; // Adjust based on your response
        },
      });

      this.customerName = this.decodeService.getUserNameFromToken();

      this.bookingService.addBooking(this.bookingData).subscribe({
        next: (bookingResponse) => {
          this.paymentsService.addPayment({
            customerId: this.CustomerID,
            bookingId: bookingResponse.id,
            paymentDate: this.bookingData.selectedDate,
            paymentValue: this.amount,
          }).subscribe();

          if (localStorage.getItem('SetBankAccount') == 'true')
            this.ServiceForConfirmation.setBankAccount(
              this.decodeService.getUserIdFromToken(),
              { bankAccount: bankAccount }
            ).subscribe({
              next: () => {
                //console.log("Added Successfully");
              }
            });
        }
      });
    } else if (this.BookingIdFromPayInstallment) {

      this.paymentsService.addPayment({
        customerId: this.CustomerID,
        bookingId: this.BookingIdFromPayInstallment,
        paymentDate: new Date().toISOString(),
        paymentValue: this.amount,
      }).subscribe();
    }
    else {
      localStorage.removeItem("bookingID");
      localStorage.removeItem("bookingData");
      localStorage.removeItem("firstCall");
    }
  }

  formatEventDate(dateString: string): string {
    const options: Intl.DateTimeFormatOptions = {
      year: "numeric",
      month: "long", // 'short' for abbreviated month names
      day: "numeric",
    };
    return new Date(dateString).toLocaleDateString("en-US", options);
  }

  // Redirect to home page
  goToHome(): void {
    this.router.navigate(["/home"]);
    localStorage.removeItem("bookingData");
    localStorage.removeItem("bookingID");
    localStorage.removeItem("firstCall");
  }

  // Print the confirmation
  printConfirmation(): void {
    const printContents = document.getElementById("printSection")?.innerHTML;
    const originalContents = document.body.innerHTML;

    if (printContents) {
      document.body.innerHTML = printContents;
      window.print();
      document.body.innerHTML = originalContents;
      window.location.reload();
    }
  }
}
