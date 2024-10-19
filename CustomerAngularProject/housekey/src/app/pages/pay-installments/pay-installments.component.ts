import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PayPalService } from '@services/pay-pal.service';

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
  loading: boolean = false; // Add this property to handle the loader

  constructor(private route: ActivatedRoute, private payPal: PayPalService) { }

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
      this.loading = true; // Show loader when starting the payment process

      const paymentData = {
        total: this.amount,
        currency: "USD",
        description: "New Transaction",
        returnUrl: "http://localhost:4200/confirmation",
        cancelUrl: "http://localhost:4200/submit-property",
      };

      this.payPal.addPayment(paymentData).subscribe({
        next: (response) => {
          this.loading = false; // Hide loader when payment is complete
          window.location.href = response.approvalUrl;
        },
        error: (error) => {
          this.loading = false; // Hide loader on error
          console.error("Payment Error:", error);
        }
      });
    } else {
      alert('Please enter a valid amount to pay.');
    }
  }
}
