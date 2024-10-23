import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { PassTokenWithHeaderService } from "./pass-token-with-header.service";
import { Service } from "../common/interfaces/service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { BehaviorSubject } from "rxjs";
import { Property } from "@models/app.models";

@Injectable({
  providedIn: "root",
})
export class WishlistService {
  private apiUrl = "https://lilynightapi.runasp.net/api/wishlist"; // الأساس للـ API
  private wishlist: Set<number> = new Set(); // To store property IDs in the wishlist
  private wishlistCount = new BehaviorSubject<number>(0);
  wishlistCount$ = this.wishlistCount.asObservable();

  constructor(
    private http: HttpClient,
    public header: PassTokenWithHeaderService,
    private snackBar: MatSnackBar
  ) {}

  public addToFavorites(service: Service, direction: any, CustomerID: string) {
    var headers = this.header.getHeaders();
    var data = {
      serviceId: service.id,
      customerId: CustomerID,
    };
    if (!this.wishlist.has(service.id)) {
      this.http
        .post(this.apiUrl, JSON.stringify(data), {
          headers,
          responseType: "text",
        })
        .subscribe({
          next: (response) => {
            this.wishlist.add(service.id);
            this.updateWishlistCount();
            this.snackBar.open(
              'The service "' + service.name + '" has been added to favorites.',
              "×",
              {
                verticalPosition: "top",
                duration: 3000,
                direction: direction,
              }
            );
          },
          error: (err) => {
            this.snackBar.open("This Service Already In Wish List.", "×", {
              verticalPosition: "top",
              duration: 3000,
              direction: direction,
            });
          },
        });
    }
  }

  getWishlistServices(customerId: string): Observable<any> {
    var headers = this.header.getHeaders();
    return this.http.get(`${this.apiUrl}/${customerId}`, {
      headers,
      responseType: "text",
    });
  }

  addToWishList(data: Property[]) {
    data.forEach((element) => {
      this.wishlist.add(element.id);
      this.updateWishlistCount();
    });
  }

  removeFromWishList(id: number) {
    this.wishlist.delete(id);
    this.updateWishlistCount();
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

  isInWishlist(propertyId: number): boolean {
    return this.wishlist.has(propertyId);
  }

  public getWishlistCount(): number {
    return this.wishlist.size;
  }

  private updateWishlistCount() {
    this.wishlistCount.next(this.wishlist.size);
  }
}
