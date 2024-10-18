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
    selector: "app-review-form",
    standalone: true,
    imports: [FormsModule, PropertyComponent],
    providers: [
        ReviewServiceService,
        AllBookingsService,
        ServicesService,
        DecodingTokenService,
    ],
    templateUrl: "./review-form.component.html",
    styleUrl: "./review-form.component.scss",
})
export class ReviewFormComponent implements OnInit {
  customerReply: string = '';
  customerRating: number | null = null;
  public  bookingId:number;
  public customerid:string;
  public AllBooking: any[] = [];
  serviceId : number;
 
  @Output() reviewSubmitted = new EventEmitter<{ rating: number; reply: string }>();
  @Input() bookids: number[] = [] ;

constructor( public serv:ReviewServiceService, 
  public bookServ:AllBookingsService, 
  public decodingCustomerID:DecodingTokenService, 
  private route: ActivatedRoute)
  {
    this.route.paramMap.subscribe((params)=>{
      this.serviceId=Number(params.get("id"));
      console.log(this.serviceId);
    })
  }

  ngOnInit(): void {
    this.customerid=this.decodingCustomerID.getUserIdFromToken();
    this.bookServ.getBooking(this.customerid).subscribe({
      next:(data)=> {
        this.AllBooking.push(data);
      },
      error:(err) => {
        console.log(err)
      }
    });
  }

  AddCustomerReview(customerReply: string, customerRating: number) {
    if (!customerReply || customerRating == null) {
      alert('Please provide both a rating and a review.');
      return;
    }

    if (!this.bookids || this.bookids.length === 0) {
      alert('No booking IDs available. Please try again later.');
      return;
  }

    const lastBookingId = this.bookids[this.bookids.length - 1];
    const review={
      customerId:this.decodingCustomerID.getUserIdFromToken(),
      bookingId :lastBookingId,
      rating:customerRating, 
      customerComment : customerReply
    }
      console.log(review);
    
    this.serv.postReview(review).subscribe({
      next: () => {
        // Emit the review only after the post operation succeeds
        this.reviewSubmitted.emit({ rating: customerRating, reply: customerReply });
        this.customerReply = '';
        this.customerRating = null;
      },
      error: (err) => {
        console.error('Error posting review:', err); // Log any errors
      }
    });
  }
}
 

