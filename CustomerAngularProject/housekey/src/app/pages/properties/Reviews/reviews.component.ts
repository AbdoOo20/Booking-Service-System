import { Component, Input, OnInit } from '@angular/core';
import { AllBookingsService } from '@services/all-bookings.service';
import { ReviewServiceService } from '@services/review-service.service';
import { ServicesService } from '@services/services.service';
import { ServiceDetails } from '../../../common/interfaces/ServiceDetails';
import { PropertyComponent } from '../property/property.component';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [PropertyComponent],
  providers:[ReviewServiceService,AllBookingsService,ServicesService],
  templateUrl: './reviews.component.html',
  styleUrl: './reviews.component.scss'
})
export class ReviewsComponent implements OnInit {
  bookingId:number;
  customerid="4aba6942-2fe4-41a9-8a69-3e2196645645";
  review : any ;
  allBooking : any[];
  serviceid:number;
  @Input() service : ServiceDetails;
constructor(public serv:ReviewServiceService,public bookServ:AllBookingsService){}
  ngOnInit(): void {
   this.allBooking.push(this.bookServ.getBooking(this.customerid));
   this.serviceid = this.service.id;
   for (const booking of this.allBooking){
   if(booking.serviceId==this.serviceid) {
    this.bookingId=booking.Id;
    this.serv.getReview(this.bookingId,this.customerid).subscribe({         //customerID from Token
      next:(data)=>{
        this.review = data;
      },
      error:(err)=>{console.log(err)}
    });
   }
   }
    
  }
}
