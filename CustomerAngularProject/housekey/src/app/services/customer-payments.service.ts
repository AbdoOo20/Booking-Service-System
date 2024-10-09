import { HttpClient } from '@angular/common/http';
import { inject ,Injectable } from '@angular/core';
import { map ,mergeMap, Observable, forkJoin, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerPaymentsService {
  _httpClient = inject(HttpClient);
  _apiURL = "http://localhost:18105/api";

  constructor() { }

  getPayments(): Observable<any> {
    return this._httpClient.get(`${this._apiURL}/Payments`)
  }

  getPaymentCustomer(customerId : string): Observable<any> {
    return this._httpClient.get(`${this._apiURL}/Customer/${customerId}`)
  }

  getCustomerPayments(bookingID: number): Observable<any[]> {
    return this.getPayments().pipe(
      map((payments: any[]) => {
        const filteredPayments = payments.filter(payment => payment.bookingID === bookingID);
        return filteredPayments;
      }),
      mergeMap((filteredPayments: any[]) => {
        if (filteredPayments.length === 0) {
          return of([]);
        }
  
        return forkJoin(
          filteredPayments.map(payment =>
            this.getPaymentCustomer(payment.customerID).pipe(
              map(customer => ({
                ...payment,
                customer
              }))
            )
          )
        );
      })
    );
  }  
}
