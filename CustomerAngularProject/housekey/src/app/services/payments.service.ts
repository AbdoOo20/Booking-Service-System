import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PaymentsService {
  private endPoint = "http://localhost:18105/api/Payments";
  private token = localStorage.getItem('token');
  private Id;
  constructor(private http: HttpClient) { }

  // Method to create headers with token
  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.token}` // Add token in the Authorization header
    });
  }

  // GET request
  getPayments(): Observable<any> {
    return this.http.get<any>(this.endPoint, { headers: this.getHeaders() });
  }
  // GET By ID request
  getPaymentsById(): Observable<any> {
    return this.http.get<any>(this.endPoint + "/" + this.Id, { headers: this.getHeaders() });
  }

  // POST request , { headers: this.getHeaders() }
  addPayment(paymentData: { customerId: string, bookingId: number, paymentDate: string, paymentValue: string }): Observable<any> {
    return this.http.post<any>(this.endPoint, paymentData);
  }




}
