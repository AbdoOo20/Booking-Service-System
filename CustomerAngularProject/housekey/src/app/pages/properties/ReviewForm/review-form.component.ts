import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AllBookingsService } from '@services/all-bookings.service';
import { ReviewServiceService } from '@services/review-service.service';
import { ServicesService } from '@services/services.service';
import { PropertyComponent } from '../property/property.component';
import { ServiceDetails } from '../../../common/interfaces/ServiceDetails';

@Component({
  selector: 'app-review-form',
  standalone: true,
  imports: [FormsModule,PropertyComponent],
  providers:[ReviewServiceService,AllBookingsService,ServicesService],
  templateUrl: './review-form.component.html',
  styleUrl: './review-form.component.scss'
})
export class ReviewFormComponent {
  customerReply: string = '';
  customerRating: number | null = null;
  public  bookingId:number;
  public customerid="529d93df-bcdd-4b22-8f71-dd355f994798";
  public AllBooking:any[];
  serviceId : number;
  review :{reply:any,rating:any,bookingid:number,customerid:string};
  @Input() service : ServiceDetails;
constructor( public serv:ReviewServiceService,public bookServ:AllBookingsService){

this.AllBooking.push(this.bookServ.getBooking(this.customerid));
this.serviceId = this.service.id;

}
AddCustomerReview(customerReply:any,customerRating:any){
  
  for (const booking of this.AllBooking){
    if(booking.serviceId = this.serviceId){
      this.bookingId = booking.id;
      
      this.review.reply=customerReply;
       this.review.rating=customerRating;
       this.review.bookingid=this.bookingId;
       this.review.customerid=this.customerid;
      this.serv.postReview(this.review);
       if (this.review.rating === null) {
        alert('Please select a rating!');
        return;}
      this.serv.postReview(this.review).subscribe();
      }
    }
}
}
 

