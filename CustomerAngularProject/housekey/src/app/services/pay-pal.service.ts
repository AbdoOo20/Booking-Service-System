import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PassTokenWithHeaderService } from './pass-token-with-header.service';

@Injectable({
  providedIn: 'root'
})
export class PayPalService {
  private endPoint = 'http://localhost:18105/api/PayPal/';

  constructor(private http: HttpClient, private PassTokenWithHeaderService: PassTokenWithHeaderService) { }

  // GET request
  getPaymentsById(paymentId: string): Observable<any> {
    return this.http.get<any>(`${this.endPoint}payment/${paymentId}`, {
      headers: this.PassTokenWithHeaderService.getHeaders()
    });
  }

  // POST request
  addPayment(paymentData: any): Observable<any> {
    return this.http.post<any>(`${this.endPoint}create-payment`, paymentData, {
      headers: this.PassTokenWithHeaderService.getHeaders()
    });
  }
}
