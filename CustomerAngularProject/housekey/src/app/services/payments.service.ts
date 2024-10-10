import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PaymentsService {
  private endPoint = "http://localhost:18105/api/Payments";
  private token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIzZmZlMTA1Yi0zYzgxLTQwNzItOWQ1MS1jZWFjYzM3NTJmYTQiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImNmOWNmN2Y0LTE3NjEtNDA3Zi1iOTMzLWVkODljOGU5MjdiYyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJtMi5zaGEyMjVAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQ3VzdG9tZXIiLCJleHAiOjE3Mjg0NTg1NzEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI4NS8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjQyMDAvIn0.uQdqB96Tru61ksvH4Q5qYSyNB2iFvIfsDt44R7Mdbac'; // Replace with the actual token
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
