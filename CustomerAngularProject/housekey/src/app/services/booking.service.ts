import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { PassTokenWithHeaderService } from "./pass-token-with-header.service";

@Injectable({
  providedIn: "root",
})
export class BookingService {
  private readonly DB_URL = "http://localhost:18105/api";
  constructor(
    private readonly http: HttpClient,
    public header: PassTokenWithHeaderService
  ) {}

  getService(id: number): Observable<any> {
    var headers = this.header.getHeaders();
    const url = `${this.DB_URL}/Services/${encodeURIComponent(id)}`;
    return this.http.get(url, {headers});
  }

  getBooingTimeForService(id: number, date: string): Observable<any> {
    var headers = this.header.getHeaders();
    const url = `${this.DB_URL}/Book/GetBookingsForService/${encodeURIComponent(
      id
    )}?date=${date}`;
    return this.http.get(url, {headers});
  }

  addBooking(BookingData: any): Observable<any> {
    var headers = this.header.getHeaders();
    return this.http.post(`${this.DB_URL}/Book`, BookingData, { headers }); //responseType: 'text'
  }
}
