import { DatePipe, CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AppService } from '@services/app.service';
import { CustomerBookings } from '../../../common/interfaces/customer-bookings';
import { AllBookingsService } from '@services/all-bookings.service';
import { DecodingTokenService } from '@services/decoding-token.service';
import { MatDialog } from '@angular/material/dialog';
import { AlertDialogComponent } from '@shared-components/alert-dialog/alert-dialog.component';
import { ConfirmDialogComponent, ConfirmDialogModel } from '@shared-components/confirm-dialog/confirm-dialog.component';

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
    DatePipe,
    CommonModule
  ],
  templateUrl: './my-properties.component.html' 
})
export class MyPropertiesComponent implements OnInit {
  displayedColumns: string[] = [ 'bookNum', 'serviceImage', 'serviceName', 
    'bookDate', 'eventDate', 'startTime', 'endTime','status', 'price', 'actions'];
  dataSource: MatTableDataSource<CustomerBookings>;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  customerBookings: CustomerBookings[] = [];

  constructor(
    public appService: AppService, 
    private _allBookingService: AllBookingsService,
    private decodeService: DecodingTokenService,
    private dialog: MatDialog
  ){ 
    
  }

  ngOnInit() {
    this.getCustomerBookings();
  }

  getCustomerBookings(): void {
    const decodedToken = this.decodeService.getUserIdFromToken();   
    this._allBookingService.getBookingWithService(decodedToken).subscribe({
      next:(res) => {
        console.log(res);
        this.customerBookings = res;
        this.initDataSource(this.customerBookings); 
      },
      error:(error) => {
        console.error('Error fetching customer bookings', error);
      },
    })
  }

  cancelBook(id: number): void {
    const dialogData = new ConfirmDialogModel('Confirm Cancelation', 'Are you sure you want to cancel this book?');
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "800px",
      data: dialogData
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result === true){
        this._allBookingService.cancelBooking(id).subscribe({
          next: () => {
            // Remove the deleted item from the local array
            // this.customerBookings = this.customerBookings.filter(item => item.bookId !== id);
            this.dialog.open(AlertDialogComponent, {
              maxWidth: "500px",
              data: 'Item canceled successfully.'
            });
            this.getCustomerBookings();
          },
          error: (err) => {
            console.error('Error deleting item', err);
            this.dialog.open(AlertDialogComponent, {
              maxWidth: "500px",
              data: 'Failed to cancel item.'
            });
          }
        });
      }
    });
  }

  convertToFullDate(time: string): Date {
    const today = new Date();
    const [hours, minutes, seconds] = time.split(':').map(Number);
    today.setHours(hours, minutes, seconds);
    return today;
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

  onImageError(event: Event) {
    const target = event.target as HTMLImageElement;
    target.src = 'images/bookingService/notFound.png';
  }

}
