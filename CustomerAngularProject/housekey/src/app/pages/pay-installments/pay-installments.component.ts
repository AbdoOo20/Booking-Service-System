import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PayPalService } from '@services/pay-pal.service';
import { log } from 'console';

@Component({
  selector: 'app-pay-installments',
  standalone: true,
  templateUrl: './pay-installments.component.html',
  styles: [],
  imports: [FormsModule], // Add FormsModule here for standalone component
})
export class PayInstallmentsComponent implements OnInit {
  bookingID: string | null = null;
  remainingValue: number | null = null;
  amount: number | null = null;

  constructor(private route: ActivatedRoute, private payPal: PayPalService) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.bookingID = params['bookingID'];
      this.remainingValue = params['remainingValue'] ? +params['remainingValue'] : null; // Convert to number
    });
    localStorage.setItem('bookingID', this.bookingID.toString());
    console.log(this.bookingID.toString());
    
  }

  submitPayment() {
    if (this.amount && this.amount <= (this.remainingValue || 0)) {
      // Handle payment submission logic here
      console.log(`Paying ${this.amount} for booking ID ${this.bookingID}`);
      const paymentData = {
        total: this.amount,
        currency: "USD",
        description: "New Transaction",
        returnUrl: "http://localhost:4200/confirmation",
        cancelUrl: "http://localhost:4200/submit-property",
      };
      // Call the addPayment method
      this.payPal.addPayment(paymentData).subscribe({
        next: (response) => {
          window.location.href = response.approvalUrl;
        },
        error: (error) => {
          console.error("Payment Error:", error);
        },
      });
    } else {
      alert('Please enter a valid amount to pay.');
    }
  }
}
