import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AllBookingsService } from '@services/all-bookings.service';
import { ReviewServiceService } from '@services/review-service.service';
import { ServicesService } from '@services/services.service';
import { PropertyComponent } from '../property/property.component';
import { ServiceDetails } from '../../../common/interfaces/ServiceDetails';
import { DecodingTokenService } from '@services/decoding-token.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-review-form',
  standalone: true,
  imports: [FormsModule,PropertyComponent],
  providers:[ReviewServiceService,AllBookingsService,ServicesService,DecodingTokenService],
  templateUrl: './review-form.component.html',
  styleUrl: './review-form.component.scss'
})
export class ReviewFormComponent {
  customerReply: string = '';
  customerRating: number | null = null;
  public  bookingId:number;
  public customerid:string;
 public AllBooking: any[] = [];
  serviceId : number;
  review :{reply:any,rating:any,bookingid:number,customerid:string};
  @Input() service : ServiceDetails;
constructor( public serv:ReviewServiceService,public bookServ:AllBookingsService,public decodingCustomerID:DecodingTokenService,private route: ActivatedRoute){
  //this.customerid="529d93df-bcdd-4b22-8f71-dd355f994798";
  this.customerid=this.decodingCustomerID.getUserIdFromToken();
  
  this.route.paramMap.subscribe((params)=>{
    this.serviceId=Number(params.get("id"));
    console.log(this.serviceId);

  this.AllBooking.push(this.bookServ.getBooking(this.customerid));
  //this.serviceId = this.service.id;
//  this.serviceId=247;
//this.customerid=this.decodingCustomerID.getUserIdFromToken();
  
})

}
AddCustomerReview(customerReply:string,customerRating:number){
  if (!customerReply || !customerRating) {
    alert('Please provide both a rating and a review.');}

   console.log(customerReply,customerRating);
  for (const booking of this.AllBooking){
    if(booking.serviceId = this.serviceId){
      this.bookingId = booking.id;
      //
   //   this.review.reply=customerReply;
       //this.review.rating=customerRating;
     // // this.review.bookingid=this.bookingId;
     //  this.review.customerid=this.customerid;
     // this.serv.postReview({customerReply,customerRating});
       if (customerRating === null) {
        alert('Please select a rating!');
        return;}
      this.serv.postReview({customerReply,customerRating}).subscribe();
      }
    }
}
}
 

