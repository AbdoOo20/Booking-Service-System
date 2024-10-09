import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { RouterModule, ActivatedRoute } from '@angular/router';
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
    DatePipe
  ],
  templateUrl: './my-payments.component.html' 
})
export class MyPymentsComponent implements OnInit {
  displayedColumns: string[] = ['paymentNum', 'paymentDate', 'paymentValue'];
  dataSource: MatTableDataSource<CustomerPayments>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  customerPayments: CustomerPayments[] = [];

  constructor(
    public appService: AppService, 
    private _customerPaymentsService: CustomerPaymentsService,
    private route: ActivatedRoute
  ){ 
    
  }

  ngOnInit() {
    const bookingID = this.route.snapshot.paramMap.get('id');
    if(bookingID) {
      this._customerPaymentsService.getCustomerPayments(+bookingID).subscribe({
        next:(res) => {
          this.customerPayments = res;
          this.initDataSource(this.customerPayments);   
        },
        error:(error) => {
          console.error('Error fetching customer payments', error);
        },
      })
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
