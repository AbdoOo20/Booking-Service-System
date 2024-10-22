import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PassTokenWithHeaderService } from './pass-token-with-header.service';
@Injectable({
  providedIn: "root",
})
export class PaymentsService {
  private endPoint = "http://lilynightapi.runasp.net/api/Payments";

  constructor(
    private http: HttpClient,
    private PassTokenWithHeaderService: PassTokenWithHeaderService
  ) {}

  // GET request
  getPayments(): Observable<any> {
    return this.http.get<any>(this.endPoint, {
      headers: this.PassTokenWithHeaderService.getHeaders(),
    });
  }
  // GET By ID request
  getPaymentsById(id: number): Observable<any> {
    return this.http.get<any>(this.endPoint + "/" + id, {
      headers: this.PassTokenWithHeaderService.getHeaders(),
    });
  }

  // POST request
  addPayment(paymentData: any): Observable<any> {
    return this.http.post<any>(this.endPoint, paymentData, {
      headers: this.PassTokenWithHeaderService.getHeaders(),
    });
  }

  //Get PaymentIncoms For Display
  getAllPaymentIncoms(): Observable<any> {
    return this.http.get<any>(this.endPoint + "/PaymentGetways", {
      headers: this.PassTokenWithHeaderService.getHeaders(),
    });
  }
}
