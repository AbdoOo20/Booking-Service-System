import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class BookingService {
  private readonly DB_URL = "http://localhost:18105/api";
  constructor(private readonly http: HttpClient) {}

  getService(id: number): Observable<any> {
    const url = `${this.DB_URL}/Services/${encodeURIComponent(id)}`;
    return this.http.get(url);
  }

  getBooingTimeForService(id: number, date: string): Observable<any> {
    const url = `${this.DB_URL}/Book/GetBookingsForService/${encodeURIComponent(
      id
    )}?date=${date}`;
    return this.http.get(url);
  }

  addBooking(BookingData: any): Observable<any> {
    const headers = { "Content-Type": "application/json" };
    return this.http.post(`${this.DB_URL}/Book`, BookingData, { headers }); //responseType: 'text'
  }
}
