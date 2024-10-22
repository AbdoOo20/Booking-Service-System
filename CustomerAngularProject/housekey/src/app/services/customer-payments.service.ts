import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, mergeMap, Observable, forkJoin, of } from 'rxjs';
import { PassTokenWithHeaderService } from './pass-token-with-header.service';
@Injectable({
  providedIn: "root",
})
export class CustomerPaymentsService {
  _httpClient = inject(HttpClient);
  _apiURL = "http://lilynightapi.runasp.net/api";

  constructor(private PassTokenWithHeaderService: PassTokenWithHeaderService) {}

  getPayments(): Observable<any> {
    return this._httpClient.get(`${this._apiURL}/Payments`, {
      headers: this.PassTokenWithHeaderService.getHeaders(),
    });
  }

  getPaymentCustomer(customerId: string): Observable<any> {
    return this._httpClient.get(`${this._apiURL}/Customer/${customerId}`, {
      headers: this.PassTokenWithHeaderService.getHeaders(),
    });
  }

  getBooking(bookingID: number): Observable<any> {
    return this._httpClient.get(`${this._apiURL}/Book/${bookingID}`, {
      headers: this.PassTokenWithHeaderService.getHeaders(),
    });
  }

  getCustomerPayments(bookingID: number): Observable<any[]> {
    return this.getPayments().pipe(
      map((payments: any[]) => {
        const filteredPayments = payments.filter(
          (payment) => payment.bookingID === bookingID
        );
        return filteredPayments;
      }),
      mergeMap((filteredPayments: any[]) => {
        if (filteredPayments.length === 0) {
          return of([]);
        }

        return this.getBooking(bookingID).pipe(
          mergeMap((booking: any) => {
            return forkJoin(
              filteredPayments.map((payment) =>
                this.getPaymentCustomer(payment.customerID).pipe(
                  map((customer) => ({
                    ...payment,
                    customer,
                    bookingStatus: booking.status,
                    bookingPrice: booking.price,
                  }))
                )
              )
            );
          })
        );
      })
    );
  }
}
