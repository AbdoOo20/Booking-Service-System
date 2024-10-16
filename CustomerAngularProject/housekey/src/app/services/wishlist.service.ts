import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { PassTokenWithHeaderService } from "./pass-token-with-header.service";

@Injectable({
  providedIn: "root",
})
export class WishlistService {
  private apiUrl = "http://localhost:18105/api/wishlist"; // الأساس للـ API

  constructor(
    private http: HttpClient,
    public header: PassTokenWithHeaderService
  ) {}

  getWishlistServices(customerId: string): Observable<any> {
    var headers = this.header.getHeaders();
    return this.http.get(`${this.apiUrl}/${customerId}`, {
      headers,
      responseType: "text",
    });
  }

  deleteServiceFromWishlist(
    customerId: string,
    serviceId: number
  ): Observable<any> {
    var headers = this.header.getHeaders();
    return this.http.delete(`${this.apiUrl}/${customerId}/${serviceId}`, {
      headers,
      responseType: "text",
    });
  }
}
