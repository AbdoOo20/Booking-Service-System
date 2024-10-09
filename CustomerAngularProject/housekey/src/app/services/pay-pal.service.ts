import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PayPalService {
  private endPoint = "http://localhost:18105/api/PayPal/";
  constructor(private http: HttpClient) { }

  // GET By ID request
  getPaymentsById(paymentId:string): Observable<any> {
    return this.http.get<any>(this.endPoint+'payment/' + paymentId);
  }

  // POST request
  addPayment(paymentData: any) {
    return this.http.post<any>(this.endPoint+'create-payment', paymentData);
  }

}
