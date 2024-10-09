import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { Property } from '@models/app.models';
import { TranslateModule } from '@ngx-translate/core';
import { AppService } from '@services/app.service';
import { CustomerBookings } from '../../../common/interfaces/customer-bookings';
import { AllBookingsService } from '@services/all-bookings.service';

@Component({
  selector: 'app-my-properties',
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
  templateUrl: './my-properties.component.html' 
})
export class MyPropertiesComponent implements OnInit {
  displayedColumns: string[] = ['bookNum', 'serviceImage', 'serviceName', 'bookDate', 'status', 'price', 'moreDetails'];
  dataSource: MatTableDataSource<CustomerBookings>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  customerBookings: CustomerBookings[] = [];

  constructor(
    public appService: AppService, 
    private _allBookingService: AllBookingsService
  ){ 
    
  }

  ngOnInit() {
    this._allBookingService.getBookingWithService("850e858a-c6e5-4fbd-b22e-723cc5be91e0").subscribe({
      next:(res) => {
        this.customerBookings = res;
        console.log(this.customerBookings);
        this.initDataSource(this.customerBookings);   
      },
      error:(error) => {
        console.error('Error fetching customer bookings', error);
      },
    })
  }

  public initDataSource(data: CustomerBookings[]) {
    this.dataSource = new MatTableDataSource<CustomerBookings>(data);
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
