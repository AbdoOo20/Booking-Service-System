import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
    providedIn: "root",
})
export class WishlistService {
    private apiUrl = "http://localhost:18105/api/wishlist"; // الأساس للـ API

    constructor(private http: HttpClient) {}

    // Get wishlist services
    getWishlistServices(customerId: string): Observable<any> {
        // إدخال customerId في الـ URL
        return this.http.get(`${this.apiUrl}/${2}`);
    }

    // Delete a service from wishlist
    deleteServiceFromWishlist(
        customerId: string,
        serviceId: number
    ): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${2}/${9}`);
    }
}
