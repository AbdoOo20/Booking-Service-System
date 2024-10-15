import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin, map, Observable, switchMap } from 'rxjs';
import { AllBookingsService } from './all-bookings.service';


@Injectable({
  providedIn: 'root'
})
export class ReviewServiceService {
private readonly reviews_API="http://localhost:18105/api/Reviews";
//private readonly bookingForservice="http://localhost:18105/api/Book/GetBookingsForService";

  constructor(public http:HttpClient, public bookingServ:AllBookingsService) { 

  }

  getReview(customerId:string){
    return this.http.get(this.reviews_API+"/"+customerId);
  }
  postReview( postReview:any){
    return this.http.post(this.reviews_API,postReview);
  }

// Method to get filtered bookings based on serviceId, then retrieve reviews
getAllReviewsForBookings(customerId: string, serviceId: number): Observable<any[]> {
  return this.bookingServ.getBooking(customerId).pipe(
    switchMap((bookings: any) => {
      // Ensure the result from bookings is an array
      if (!Array.isArray(bookings)) {
        throw new Error('Expected bookings to be an array');
      }

      // Filter bookings based on the serviceId
      const filteredBookings = bookings.filter(booking => booking.serviceId === serviceId);
      console.log(filteredBookings);
      // For each filtered booking, get the reviews
      const reviewObservables = filteredBookings.map((booking) => {
        return this.getReview(customerId).pipe(
          map((reviews) => ({
            ...booking,  // Booking data for the service
            reviews      // Review data (array of reviews for this customer)
          }))
        );
      });

      // Wait for all review requests to complete and return the final result
      return forkJoin(reviewObservables);
    }),
   
  );
}

}
