import { Component, OnInit, Output, EventEmitter } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";
import { MatToolbarModule } from "@angular/material/toolbar";
import { AppService } from "@services/app.service";
import { ContactsComponent } from "../contacts/contacts.component";
import { SocialIconsComponent } from "../social-icons/social-icons.component";
import { CurrencyComponent } from "../currency/currency.component";
import { LangComponent } from "../lang/lang.component";
import { UserMenuComponent } from "../user-menu/user-menu.component";
import { LogoComponent } from "@shared-components/logo/logo.component";
import { HorizontalMenuComponent } from "../menu/horizontal-menu/horizontal-menu.component";
import { MatBadgeModule } from "@angular/material/badge";
import { TranslateModule } from "@ngx-translate/core";
import { RouterModule } from "@angular/router";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";
import { MatButtonModule } from "@angular/material/button";
import { DecodingTokenService } from "@services/decoding-token.service";
import { AuthServiceService } from "@services/auth-service.service";
import { WishlistService } from "@services/wishlist.service";
import { Property } from "@models/app.models";
import { ChangeDetectorRef } from "@angular/core";

@Component({
  selector: "app-toolbar1",
  standalone: true,
  imports: [
    FlexLayoutModule,
    RouterModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
    TranslateModule,
    ContactsComponent,
    SocialIconsComponent,
    CurrencyComponent,
    LangComponent,
    UserMenuComponent,
    LogoComponent,
    HorizontalMenuComponent,
  ],
  templateUrl: "./toolbar1.component.html",
  styleUrls: ["./toolbar1.css"],
})
export class Toolbar1Component implements OnInit {
  token = localStorage.getItem("token");
  isLoggedIn: boolean = false;
  public customerId: string;
  public servicesIDs: number[] = [];
  wishlistCount: number = 0;
  @Output() onMenuIconClick: EventEmitter<any> = new EventEmitter<any>();
  constructor(
    public appService: AppService,
    private authService: AuthServiceService,
    private wishListService: WishlistService,
    public decodeCustomerID: DecodingTokenService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe((status) => {
      this.isLoggedIn = status;
    });

    this.customerId = this.decodeCustomerID.getUserIdFromToken();

    this.wishListService.wishlistCount$.subscribe((count) => {
      this.wishlistCount = count;
      //console.log("Count = " + this.wishlistCount); // Updated count on UI
    });

    // Fetch wishlist services
    this.wishListService.getWishlistServices(this.customerId).subscribe(
      (data) => {
        const properties: Property[] = JSON.parse(data);
        this.wishListService.addToWishList(properties); // Update wishlist and count
      },
      (error) => {
        console.error("Error loading wishlist", error);
      }
    );
  }

  public sidenavToggle() {
    this.onMenuIconClick.emit();
  }

  //Basma "Retrieve the Ids of services in WishList of Current Customer"
  public getWishListServices() {
    this.wishListService.getWishlistServices(this.customerId).subscribe({
      next: (data) => {
        if (typeof data === "string") {
          try {
            data = JSON.parse(data);
          } catch (error) {
            return;
          }
        }
        if (Array.isArray(data)) {
          data.forEach((service) => {
            this.servicesIDs.push(service.id);
          });
        } else {
          console.error("Expected an array but got:", typeof data);
        }
        console.log(this.servicesIDs.length + "Length After Edit");
      },
      error: (err) => {
        console.error("Error fetching wishlist services:", err); // Handle errors
      },
    });
  }
}
