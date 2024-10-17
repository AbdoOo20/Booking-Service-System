import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, switchMap, of } from 'rxjs';
import { AllBookingsService } from './all-bookings.service';


@Injectable({
  providedIn: 'root'
})
export class ReviewServiceService {
private readonly _APIUrl="http://localhost:18105/api";

  constructor(public http:HttpClient, public bookingServ:AllBookingsService) { }

  getCustomerBookings(customerId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this._APIUrl}/Book/GetBookingsForCustomer/${customerId}`)
  }

  getReview(customerId:string): Observable<any>{
    return this.http.get(`${this._APIUrl}/Reviews/${customerId} `);
  }

  getFilteredBookingsAndReviews(customerId: string, serviceId: number): Observable<any[]> {
    return this.getCustomerBookings(customerId).pipe(
      map(bookings => bookings.filter(booking => booking.serviceId === serviceId)),
      switchMap(filteredBookings => {
        if (filteredBookings.length > 0) {
          return this.getReview(customerId).pipe(
            map(reviews => {
              return filteredBookings
                .map(booking => {
                  const review = reviews.find((rev: any) => rev.bookingId === booking.bookId);
                  if (review) {
                    return { booking, review };
                  } else {
                    return null;
                  }
                })
                .filter(result => result !== null);
            })
          );
        } else {
          return of([]);
        }
      })
    );
  }
  
  postReview( postReview:any){
    return this.http.post(this._APIUrl,postReview);
  }
}
