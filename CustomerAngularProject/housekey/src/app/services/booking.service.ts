import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: "root",
})
export class BookingService {
  private readonly DB_URL = "http://localhost:18105/api";
  constructor(private readonly http: HttpClient) {}

  getService(id: number) {
    const url = `${this.DB_URL}/Services/${encodeURIComponent(id)}`;
    return this.http.get(url);
  }

  getBooingTimeForService(id: number, date:string) {
    const url = `${this.DB_URL}/Book/GetBookingsForService/${encodeURIComponent(id)}?date=${date}`;
    return this.http.get(url);
  }

  // addStudent(user: User) {
  //   return this.http.post<User>(this.DB_URL, user);
  // }
}
