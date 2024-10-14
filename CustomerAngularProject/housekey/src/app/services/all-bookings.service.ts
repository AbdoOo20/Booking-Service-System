import { HttpClient } from '@angular/common/http';
import { inject ,Injectable } from '@angular/core';
import { map, mergeMap, Observable, forkJoin } from 'rxjs';
import { PassTokenWithHeaderService } from './pass-token-with-header.service';

@Injectable({
  providedIn: 'root'
})
export class AllBookingsService {
_httpClient = inject(HttpClient);
_apiURL = "http://localhost:18105/api";

  constructor(private _passToken: PassTokenWithHeaderService) { }

  cancelBooking(id: number): Observable<void> {
    return this._httpClient.delete<void>(`${this._apiURL}/Book/${id}`, {
      headers: this._passToken.getHeaders()
    });
  }

  getBooking(id : string): Observable<any> {
    return this._httpClient.get(`${this._apiURL}/Book/GetBookingsForCustomer/${id}`); 
  }

  getService(serviceId : number):  Observable<any> {
    return this._httpClient.get(`${this._apiURL}/Services/${serviceId}`);
  }

  getBookingWithService(id: string): Observable<any[]> {
    return this.getBooking(id).pipe(
      mergeMap((bookings: any[]) => {
        const serviceRequests = bookings.map((booking) =>
          this.getService(booking.serviceId).pipe(
            map(service => ({
              ...booking,
              service
            }))
          )
        );

        return forkJoin(serviceRequests);
      })
    );
  }
}
