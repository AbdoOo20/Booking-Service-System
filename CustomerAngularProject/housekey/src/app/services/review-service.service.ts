import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, switchMap, of, forkJoin, catchError, throwError } from 'rxjs';
import { AllBookingsService } from './all-bookings.service';
import { PassTokenWithHeaderService } from './pass-token-with-header.service';

@Injectable({
  providedIn: "root",
})
export class ReviewServiceService {
  private readonly _APIUrl = "https://lilynightapi.runasp.net/api";

  constructor(
    public http: HttpClient,
    public bookingServ: AllBookingsService,
    private PassTokenWithHeaderService: PassTokenWithHeaderService
  ) {}

  getCustomerBookings(customerId: string): Observable<any[]> {
    return this.http.get<any[]>(
      `${this._APIUrl}/Book/GetBookingsForCustomer/${customerId}`,
      { headers: this.PassTokenWithHeaderService.getHeaders() }
    );
  }

  getReview(bookingId: number): Observable<any> {
    return this.http
      .get(`${this._APIUrl}/Reviews/${bookingId}`, {
        headers: this.PassTokenWithHeaderService.getHeaders(),
      })
      .pipe(
        catchError((error) => {
          if (error.status === 404) {
            // If the review is not found, return null
            return of(null);
          } else {
            // Re-throw any other errors
            return throwError(error);
          }
        })
      );
  }

  getFilteredBookingsAndReviews(
    customerId: string,
    serviceId: number
  ): Observable<any[]> {
    return this.getCustomerBookings(customerId).pipe(
      map((bookings) =>
        bookings.filter(
          (booking) =>
            booking.serviceId === serviceId && booking.status == "Confirmed"
        )
      ),
      switchMap((filteredBookings) => {
        if (filteredBookings.length > 0) {
          console.log("Filtered Bookings:", filteredBookings);
          const bookingWithReviewRequests = filteredBookings.map((booking) =>
            this.getReview(booking.bookId).pipe(
              map((review) => ({
                booking,
                review: review || null,
              }))
            )
          );

          return forkJoin(bookingWithReviewRequests);
        } else {
          return of([]);
        }
      })
    );
  }

  postReview(review: any): Observable<any> {
    return this.http.post<any>(`${this._APIUrl}/Reviews`, review);
  }
}
