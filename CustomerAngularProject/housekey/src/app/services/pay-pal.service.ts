import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PayPalService {
  private endPoint = 'http://localhost:18105/api/PayPal/';

  constructor(private http: HttpClient) {}

  // Helper to create headers with token
  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token'); // Ensure token exists
    if (!token) throw new Error('Auth token is missing!');
    
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  // GET request
  getPaymentsById(paymentId: string): Observable<any> {
    return this.http.get<any>(`${this.endPoint}payment/${paymentId}`, {
      headers: this.getHeaders()
    });
  }

  // POST request
  addPayment(paymentData: any): Observable<any> {
    return this.http.post<any>(`${this.endPoint}create-payment`, paymentData, {
      headers: this.getHeaders()
    });
  }
}
