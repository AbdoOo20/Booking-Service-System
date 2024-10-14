import { Component, OnInit } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatMenuModule } from "@angular/material/menu";
import { MatBadgeModule } from "@angular/material/badge";
import { FlexLayoutModule } from "@ngbracket/ngx-layout";
import { TranslateModule } from "@ngx-translate/core";
import { AppService } from "@services/app.service";
import { Route, Router, RouterModule } from "@angular/router";
import { BidiModule } from "@angular/cdk/bidi";
import { Settings, SettingsService } from "@services/settings.service";
import { AuthServiceService } from "@services/auth-service.service";
import { DecodingTokenService } from "@services/decoding-token.service";
import { ServicesService } from "@services/services.service";

@Component({
    selector: "app-user-menu",
    standalone: true,
    imports: [
        RouterModule,
        MatButtonModule,
        MatIconModule,
        MatMenuModule,
        MatBadgeModule,
        FlexLayoutModule,
        TranslateModule,
        BidiModule,
    ],
    templateUrl: "./user-menu.component.html",
})
export class UserMenuComponent {
    isLoggedIn: boolean = !!localStorage.getItem("token");
    public customerid: string;
    public settings: Settings;
    public customer: any;
    constructor(
        public appService: AppService,
        public settingsService: SettingsService,
        private router: Router,
        private authService: AuthServiceService,
        public decodingCustomerID: DecodingTokenService,
        public myServ: ServicesService
    ) {
        this.settings = this.settingsService.settings;
        this.customerid = this.decodingCustomerID.getUserIdFromToken();
        this.getCustomerByID(this.customerid);
    }

    logout() {
        this.authService.logout();
    }
    // destroyToken(): void {
    //     localStorage.removeItem("token");
    //     this.router.navigate(["/login"]);
    // }
    public getCustomerByID(id: string) {
        this.myServ.getCustomerByID(id).subscribe({
            next: (data) => {
                this.customer = data;
                // console.log(data);
                // console.log(this.customer);
            },
            error: (err) => {
                console.log(err);
            },
        });
    }
}
