import { DatePipe, CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AppService } from '@services/app.service';
import { CustomerPayments } from '../../../common/interfaces/customer-payments';
import { CustomerPaymentsService } from '@services/customer-payments.service';

@Component({
  selector: 'app-my-payments',
  standalone: true,
  imports: [
    RouterModule,
    MatTableModule,
    MatSortModule,
    TranslateModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatPaginatorModule,
    DatePipe,
    CommonModule,
  ],
  templateUrl: './my-payments.component.html'
})
export class MyPymentsComponent implements OnInit {
  displayedColumns: string[] = ['paymentNum', 'paymentDate', 'paymentValue', 'bookingPrice'];
  dataSource: MatTableDataSource<CustomerPayments>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  customerPayments: CustomerPayments[] = [];
  bookingStatus: string;
  bookingPrice: number;

  constructor(
    public appService: AppService,
    private _customerPaymentsService: CustomerPaymentsService,
    private route: ActivatedRoute,
    private router: Router
  ) {

  }

  ngOnInit() {
    const bookingID = this.route.snapshot.paramMap.get('id');
    if (bookingID) {
      this._customerPaymentsService.getCustomerPayments(+bookingID).subscribe({
        next: (res) => {
          this.customerPayments = res;
          if (this.customerPayments.length > 0 && this.customerPayments[0].bookingStatus) {
            this.bookingStatus = this.customerPayments[0].bookingStatus;
            this.bookingPrice = this.customerPayments[0].bookingPrice;
          }
          //console.log(this.customerPayments);
          this.initDataSource(this.customerPayments);
        },
        error: (error) => {
          console.error('Error fetching customer payments', error);
        },
      })
    }
  }

  calculateResidual(payment: CustomerPayments): number {
    const paymentsForBooking = this.customerPayments.filter(p => p.bookingID === payment.bookingID);
    let remainingAmount = payment.bookingPrice;
    let totalPayments = 0;

    for (let index = 0; index < paymentsForBooking.length; index++) {
      const currentPayment = paymentsForBooking[index];
      totalPayments += currentPayment.paymentValue;
      remainingAmount = payment.bookingPrice - totalPayments;

      if (currentPayment === payment) {
        return remainingAmount;
      }
    }

    return remainingAmount;
  }

   navigateToPaymentPage() {
    const bookingID = this.route.snapshot.paramMap.get('id');
    const lastPayment = this.customerPayments[this.customerPayments.length - 1];
    if (lastPayment) {
      const remainingValue = this.calculateResidual(lastPayment); 
      const targetPage = '/NewPayment';
      this.router.navigate([targetPage], { queryParams: { bookingID, remainingValue } });
    } else {
      console.error('No payments available to calculate the remaining value.');
    }
  }


  public initDataSource(data: CustomerPayments[]) {
    this.dataSource = new MatTableDataSource<CustomerPayments>(data);
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  public applyFilter(ev: EventTarget) {
    let filterValue = (ev as HTMLInputElement).value;
    this.dataSource.filter = filterValue?.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
}
