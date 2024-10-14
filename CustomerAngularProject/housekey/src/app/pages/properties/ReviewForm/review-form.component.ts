import { Component, EventEmitter, input, Input, OnInit, Output } from '@angular/core';
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
export class ReviewFormComponent implements OnInit {
  customerReply: string = '';
  customerRating: number | null = null;
  public  bookingId:number;
  public customerid:string;
 public AllBooking: any[] = [];
  serviceId : number;
 
  @Output() reviewSubmitted = new EventEmitter<{ rating: number; reply: string }>();
  //@Input() bookId : number;
 @Input() bookids : any[] ;

 // @Input() service : ServiceDetails;
constructor( public serv:ReviewServiceService,public bookServ:AllBookingsService,public decodingCustomerID:DecodingTokenService,private route: ActivatedRoute){
 // this.customerid="88d000e7-4154-4752-9ea6-a01072490748";
 // this.customerid=this.decodingCustomerID.getUserIdFromToken();
  
  this.route.paramMap.subscribe((params)=>{
    this.serviceId=Number(params.get("id"));
    console.log(this.serviceId);

 // this.AllBooking.push(this.bookServ.getBooking(this.customerid));
 
  //this.serviceId = this.service.id;
//  this.serviceId=247;

  
})

}
  ngOnInit(): void {
    this.customerid=this.decodingCustomerID.getUserIdFromToken();
    this.AllBooking.push(this.bookServ.getBooking(this.customerid));
    console.log(this.AllBooking);
    console.log(this.bookids);
  }
AddCustomerReview(customerReply: string, customerRating: number) {
 
  // Check if the user has provided both a reply and a rating
  if (!customerReply || customerRating == null) { // Use `== null` to check for null or undefined
    alert('Please provide both a rating and a review.');
    return; // Exit the function if validation fails
  }

  console.log(customerReply, customerRating); // Log the provided reply and rating
/*this.AllBooking.forEach(booking=>{
  if(booking.serviceId==this.serviceId){
    this.bookids.push(booking.bookId);
  }
  
})*/
console.log(this.bookids);
console.log(this.bookids[length-1]);
//this.customerid="88d000e7-4154-4752-9ea6-a01072490748";
 /* this.review.customerid="88d000e7-4154-4752-9ea6-a01072490748";
   this.review.rating=customerRating;
   this.review.reply=customerReply;
  this.review.bookingid=this.bookIds[length-1];*/
      // Post the review to the server
      const review={customerId:this.decodingCustomerID.getUserIdFromToken(),
        bookingId :this.bookids[length-1],
       rating:customerRating, 
       customerComment : customerReply}
       console.log(review);
      
      this.serv.postReview(review).subscribe({
        next: () => {
          // Emit the review only after the post operation succeeds
          this.reviewSubmitted.emit({ rating: customerRating, reply: customerReply });
        },
        error: (err) => {
          console.error('Error posting review:', err); // Log any errors
        }
      });
  
  }




}
 

