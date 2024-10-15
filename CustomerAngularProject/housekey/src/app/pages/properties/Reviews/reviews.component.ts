import { Component, Input, OnInit } from '@angular/core';
import { AllBookingsService } from '@services/all-bookings.service';
import { ReviewServiceService } from '@services/review-service.service';
import { ServicesService } from '@services/services.service';
import { ServiceDetails } from '../../../common/interfaces/ServiceDetails';
import { PropertyComponent } from '../property/property.component';
import { DecodingTokenService } from '@services/decoding-token.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [PropertyComponent],
  providers:[ReviewServiceService,AllBookingsService,ServicesService,DecodingTokenService],
  templateUrl: './reviews.component.html',
  styleUrl: './reviews.component.scss'
})
export class ReviewsComponent implements OnInit {
  bookingId:number;
  customerid:string;
  @Input() reviewData: any;
  allBooking : any[];
  serviceid:number;
 // @Input() service : ServiceDetails;
constructor(public serv:ReviewServiceService,public bookServ:AllBookingsService ,public decodeCustomerID:DecodingTokenService,private route: ActivatedRoute){}
  ngOnInit(): void {
  // this.customerid=this.decodeCustomerID.getUserIdFromToken();
   this.route.paramMap.subscribe((params)=>{
    this.serviceid=Number(params.get("id"));
    console.log(this.serviceid)})

   this.customerid=this.decodeCustomerID.getUserIdFromToken();;

  this.serv.getAllReviewsForBookings(this.customerid,this.serviceid).subscribe({
    next:(data)=>{console.log(data);},
    error:(err)=>{console.log(err)}
  })
  }
  }
    
   
